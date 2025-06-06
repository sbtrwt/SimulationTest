using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float smoothSpeed = 0.125f;

    [Header("Portal Transition")]
    public float portalTransitionSpeed = 5f;
    private bool isTransitioning;
    private Vector3 transitionVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position behind and above the target
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;

        if (isTransitioning)
        {
            // Smoothly move camera during portal transition
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref transitionVelocity,
                1f / portalTransitionSpeed
            );

            // Check if transition is complete
            if (Vector3.Distance(transform.position, desiredPosition) < 0.1f)
            {
                isTransitioning = false;
            }
        }
        else
        {
            // Normal smooth follow
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
            transform.position = smoothedPosition;
        }

        // Always look at the target
        transform.LookAt(target);
    }

    // Call this when player teleports through a portal
    public void OnPlayerTeleported()
    {
        isTransitioning = true;
        transitionVelocity = Vector3.zero;
    }
}