using UnityEngine;

public class ThirdPersonCarCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform car; // Assign your car's transform
    public Rigidbody carRigidbody; // For speed effects

    [Header("Positioning")]
    public float distance = 5f; // How far behind the car
    public float height = 2f; // How high above the car
    public float positionDamping = 10f; // Smooth follow speed

    [Header("Rotation")]
    public float rotationDamping = 5f; // Smooth rotation speed
    public bool lookAtTarget = true; // Always point at car
    public float pitchAngle = 10f; // Downward tilt angle

    [Header("Speed Effects")]
    public float maxDistance = 8f; // Zoom out at speed
    public float maxFOV = 75f; // Field of view at speed
    private float initFOV;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        initFOV = cam.fieldOfView;
    }

    private void LateUpdate()
    {
        if (car == null) return;

        // Calculate desired position behind the car
        Vector3 desiredPosition = car.position - (car.forward * distance) + (Vector3.up * height);
        
        // Smoothly move to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * Time.deltaTime);

        // Handle rotation
        if (lookAtTarget)
        {
            Quaternion lookRotation = Quaternion.LookRotation(car.position - transform.position);
            lookRotation *= Quaternion.Euler(pitchAngle, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationDamping * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, car.rotation, rotationDamping * Time.deltaTime);
        }

        // Speed effects (optional)
        if (carRigidbody != null)
        {
            float speedFactor = Mathf.Clamp01(carRigidbody.velocity.magnitude / 50f); // Adjust 50 to your car's max speed
            distance = Mathf.Lerp(5f, maxDistance, speedFactor);
            cam.fieldOfView = Mathf.Lerp(initFOV, maxFOV, speedFactor);
        }
    }
}