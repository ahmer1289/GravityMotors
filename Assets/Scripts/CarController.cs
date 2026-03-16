using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private bool isBreaking, isBoosting;

    [SerializeField] private float forwardSpeed = 15f;
    [SerializeField] private float reverseSpeed = 10f;
    [SerializeField] private float turnSpeed = 80f;
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private ParticleSystem boostEffect;

    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private Rigidbody rb;
    public AudioSource kick;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (boostEffect != null)
        {
            boostEffect.Stop();
        }
    }

    private void FixedUpdate()
    {
        GetInput();
        MoveCar();
        ApplyGravity();
        HandleSteering();
        UpdateWheels();
        UpdateBoostEffect();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
        isBoosting = Input.GetKey(KeyCode.LeftShift);
    }

    private void MoveCar()
    {
        float speedMultiplier = isBoosting ? boostMultiplier : 1f;

        if (isBreaking)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else if (verticalInput > 0.1f)
        {
            rb.velocity = transform.forward * forwardSpeed * speedMultiplier + new Vector3(0, rb.velocity.y, 0);
        }
        else if (verticalInput < -0.1f)
        {
            rb.velocity = transform.forward * -reverseSpeed + new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void ApplyGravity()
    {
        rb.velocity += Vector3.down * gravityMultiplier * Time.fixedDeltaTime;
    }

    private void HandleSteering()
    {
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            transform.Rotate(Vector3.up * horizontalInput * turnSpeed * Time.fixedDeltaTime);
        }
    }

    private void UpdateBoostEffect()
    {
        if (boostEffect == null) return;

        if (isBoosting)
        {
            if (!boostEffect.isPlaying)
                boostEffect.Play();
        }
        else
        {
            if (boostEffect.isPlaying)
                boostEffect.Stop();
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            kick.Play();
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

            ContactPoint contact = collision.contacts[0];
            Vector3 forceDirection = Vector3.Reflect(transform.forward, contact.normal);

            float forceMagnitude = 15f;
            ballRb.AddForce(forceDirection * forceMagnitude + Vector3.up * 8f, ForceMode.Impulse);

            rb.AddForce(-rb.velocity * 0.3f, ForceMode.Impulse);
        }
    }
}
