using UnityEngine;

public class RacingFollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float distance = 6f; // Distance behind target
    public float height = 2f; // Height above target
    public float positionDamping = 10f; // Position follow speed
    public float rotationDamping = 5f; // Rotation follow speed
    public float maxSpeedEffect = 30f; // Speed at which max effects occur
    public float fovSpeedFactor = 0.2f; // How much speed affects FOV

    private Vector3 offset;
    private Camera cam;
    private float defaultFOV;
    private Rigidbody targetRigidbody;

    void Start()
    {
        offset = new Vector3(0, height, -distance);
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
        
        if (target != null)
            targetRigidbody = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + 
                                target.forward * offset.z + 
                                target.up * offset.y;

        // Smooth position transition
        transform.position = Vector3.Lerp(transform.position, 
                                        desiredPosition, 
                                        positionDamping * Time.deltaTime);

        // Smooth rotation to look slightly ahead of target
        Quaternion desiredRotation = Quaternion.LookRotation(
            target.position + target.forward * 5f - transform.position, 
            target.up
        );

        transform.rotation = Quaternion.Slerp(transform.rotation, 
                                            desiredRotation, 
                                            rotationDamping * Time.deltaTime);

        // Speed effects (optional)
        if (targetRigidbody != null)
        {
            float speedFactor = Mathf.Clamp01(targetRigidbody.velocity.magnitude / maxSpeedEffect);
            cam.fieldOfView = defaultFOV + (defaultFOV * fovSpeedFactor * speedFactor);
        }
    }
}