using System.Collections;
using TMPro;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public ParticleSystem celebrationEffect;
    private int goalCount = 0;
    public Crowd[] crowds;
    public Transform resetPoint;
    public GameObject ball;
    private bool goalScored = false;
    public TextMeshProUGUI goalText;
    public AudioSource crowdCheerSound; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !goalScored)
        {
            goalScored = true;
            goalCount++;
            Debug.Log("Goal Scored! Total Goals: " + goalCount);

            if (celebrationEffect != null)
            {
                celebrationEffect.Play();
            }

            if (crowdCheerSound != null)
            {
                crowdCheerSound.Play();
                StartCoroutine(StopCheeringAfterDelay(4f));
            }

            foreach (Crowd crowd in crowds)
            {
                if (crowd != null)
                {
                    crowd.Cheer();
                }
            }

            StartCoroutine(ShowGoalText());
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    private IEnumerator ShowGoalText()
    {
        if (goalText != null)
        {
            goalText.text = "GOAL Number " + goalCount + " Scored!";
            goalText.alpha = 1;
            yield return new WaitForSeconds(3f);
            goalText.alpha = 0;
        }
    }

    private IEnumerator ResetBallAfterDelay()
    {
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(3);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (ball != null && resetPoint != null)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            ballRb.isKinematic = true;
            ball.transform.position = resetPoint.position;

            yield return new WaitForSeconds(0.1f);
            ballRb.isKinematic = false;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }

        foreach (Crowd crowd in crowds)
        {
            if (crowd != null)
            {
                crowd.Idle();
            }
        }
        goalScored = false;
    }

    private IEnumerator StopCheeringAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (crowdCheerSound != null && crowdCheerSound.isPlaying)
        {
            crowdCheerSound.Stop();
        }
    }
}
