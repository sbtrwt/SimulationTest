using UnityEngine;

public class FixedAngleChaseCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float followSpeed = 5f;
    public float height = 5f;
    public float distance = 10f;
    public Vector3 fixedRotation = new Vector3(35f, 0f, 0f); // Set your desired fixed angle

    [Header("Edge Protection")]
    public bool useBoundaries = true;
    public float xBoundary = 50f;
    public float zBoundary = 50f;

    private Vector3 offset;

    void Start()
    {
        // Set initial rotation
        transform.rotation = Quaternion.Euler(fixedRotation);
        
        // Calculate initial offset based on rotation
        offset = Quaternion.Euler(fixedRotation) * new Vector3(0, 0, -distance);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 targetPosition = target.position;
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, -xBoundary, xBoundary);
            targetPosition.z = Mathf.Clamp(targetPosition.z, -zBoundary, zBoundary);
        }

        Vector3 desiredPosition = targetPosition + offset;
        desiredPosition.y = targetPosition.y + height;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Maintain fixed rotation
        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}