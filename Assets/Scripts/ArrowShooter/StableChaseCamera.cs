using UnityEngine;

public class StableChaseCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Rigidbody targetRigidbody; // Optional for velocity-based smoothing

    [Header("Camera Position")]
    public float height = 5f;
    public float distance = 10f;
    public Vector3 fixedRotation = new Vector3(35f, 0f, 0f);

    [Header("Stabilization Settings")]
    public float positionSmoothTime = 0.3f;
    public float rotationSmoothTime = 0.1f;
    public float maxSpeed = 50f;
    public float collisionRecoverySpeed = 5f;
    public float shakeReduction = 0.5f;

    [Header("Rotation Settings")]
    public float followRotationSpeed = 3f;
    public float returnRotationSpeed = 1f;
    private Vector3 positionVelocity;
    private Vector3 lastStablePosition;
    private Vector3 offset;
    private bool isRecoveringFromCollision;
    private Camera cam;
    private bool targetIsVisible;
    public float viewMargin = 0.1f;
    private Quaternion targetRotation;
    public float maxRotationAngle = 45f;
    void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null && TryGetComponent(out targetRigidbody))
            target = targetRigidbody.transform;

        transform.rotation = Quaternion.Euler(fixedRotation);
        offset = Quaternion.Euler(fixedRotation) * new Vector3(0, 0, -distance);
        lastStablePosition = target.position + offset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = targetPosition + offset;
        desiredPosition.y = targetPosition.y + height;
        // Check if target is in camera view (with margin)
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);
        targetIsVisible = viewportPos.x > viewMargin &&
                         viewportPos.x < 1 - viewMargin &&
                         viewportPos.y > viewMargin &&
                         viewportPos.y < 1 - viewMargin &&
                         viewportPos.z > 0;
        // Detect sudden position changes (collisions)
        float positionChange = Vector3.Distance(target.position, lastStablePosition);
        if (positionChange > 1f) // Threshold for collision detection
        {
            isRecoveringFromCollision = true;
        }

        if (isRecoveringFromCollision)
        {
            // Slower, more stable movement after collisions
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                collisionRecoverySpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, desiredPosition) < 0.1f)
            {
                isRecoveringFromCollision = false;
            }
        }
        else
        {
            // Normal smooth follow with velocity damping
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref positionVelocity,
                positionSmoothTime,
                maxSpeed
            );
        }

        // Calculate desired rotation
        if (!targetIsVisible)
        {
            // Rotate toward target when out of frame
            Vector3 directionToTarget = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            // Clamp the rotation to avoid extreme angles
            targetRotation = Quaternion.RotateTowards(
                Quaternion.Euler(fixedRotation),
                lookRotation,
                maxRotationAngle
            );
        }
        else
        {
            // Return to fixed rotation when target is visible
            targetRotation = Quaternion.Euler(fixedRotation);
        }
        // Apply slight rotation stabilization
        //Quaternion targetRotation = Quaternion.Euler(fixedRotation);
        // Quaternion targetRotation = Quaternion.LookRotation(
        //     target.forward, 
        //     Vector3.up
        // );
        float currentSpeed = targetIsVisible ? returnRotationSpeed : followRotationSpeed;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            currentSpeed * Time.deltaTime
        );
        // transform.rotation = Quaternion.RotateTowards(
        //             transform.rotation,
        //             targetRotation,
        //             rotationSmoothTime * Time.deltaTime * 360f // Convert to degrees
        //         );
        // Store stable position for next frame comparison
        if (!isRecoveringFromCollision)
        {
            lastStablePosition = target.position;
        }
    }
}