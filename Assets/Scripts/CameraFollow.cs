using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform car;
    public Transform ball;

    [SerializeField] private float smoothSpeed = 5f;  // How fast the camera moves
    [SerializeField] private float minZoom = 5f;      // Closest zoom
    [SerializeField] private float maxZoom = 15f;     // Farthest zoom
    [SerializeField] private float zoomMultiplier = 2f;  // Zoom scaling factor

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - GetMidpoint();
    }

    private void LateUpdate()
    {
        if (car == null || ball == null) return;  // Safety check

        Vector3 midpoint = GetMidpoint();
        float distance = Vector3.Distance(car.position, ball.position);

        // Set camera position
        Vector3 desiredPosition = midpoint + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Adjust zoom
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomMultiplier);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
    }

    private Vector3 GetMidpoint()
    {
        return (car.position + ball.position) / 2;
    }
}
