using UnityEngine;

public class SmoothFixedAngleChaseCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float height = 5f;
    public float distance = 10f;
    public Vector3 fixedRotation = new Vector3(35f, 0f, 0f);
    
    [Header("Smoothing")]
    public float positionSmoothTime = 0.3f;
    public float maxSpeed = 50f;
    private Vector3 positionVelocity;

    [Header("Edge Protection")]
    public bool useBoundaries = true;
    public float xBoundary = 50f;
    public float zBoundary = 50f;

    private Vector3 offset;

    void Start()
    {
        // Set initial rotation and calculate offset
        transform.rotation = Quaternion.Euler(fixedRotation);
        offset = Quaternion.Euler(fixedRotation) * new Vector3(0, 0, -distance);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate target position with boundaries
        Vector3 targetPosition = GetBoundedPosition(target.position);

        // Calculate desired camera position
        Vector3 desiredPosition = targetPosition + offset;
        desiredPosition.y = targetPosition.y + height;

        // Smooth damp movement (better than Lerp for high speed)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime,
            maxSpeed
        );

        // Maintain fixed rotation
        transform.rotation = Quaternion.Euler(fixedRotation);
    }

    Vector3 GetBoundedPosition(Vector3 position)
    {
        if (!useBoundaries) return position;
        
        return new Vector3(
            Mathf.Clamp(position.x, -xBoundary, xBoundary),
            position.y,
            Mathf.Clamp(position.z, -zBoundary, zBoundary)
        );
    }
}