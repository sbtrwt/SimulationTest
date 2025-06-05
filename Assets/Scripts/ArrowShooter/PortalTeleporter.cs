using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform destinationPortal;
    public bool preserveVelocity = true; // Toggle if you want speed maintained

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null) // Check if object has physics
        {
            TeleportObject(other.transform, other.attachedRigidbody);
        }
    }

    private void TeleportObject(Transform obj, Rigidbody rb)
    {
        // Get portal transforms in world space
        Transform entryPortal = this.transform;
        Transform exitPortal = destinationPortal;

        // Calculate relative position (from entry portal's perspective)
        Vector3 localPos = entryPortal.InverseTransformPoint(obj.position);

        // Mirror the Z-position (so object comes out the front of exit portal)
        localPos.z = -localPos.z;

        // Convert to exit portal's world space
        Vector3 exitWorldPos = exitPortal.position;

        // Calculate rotation difference (accounting for portal orientations)
        Quaternion rotationDifference = exitPortal.rotation;
        Quaternion exitRotation = rotationDifference * obj.rotation;

        // Apply new position and rotation
        obj.position = exitWorldPos;
        obj.rotation = exitRotation;

        // Handle velocity preservation
        if (preserveVelocity && rb != null)
        {
            Vector3 localVelocity = entryPortal.InverseTransformDirection(rb.velocity);
            localVelocity.z = -localVelocity.z; // Mirror forward direction
            rb.velocity = exitPortal.TransformDirection(localVelocity);

            // Preserve angular velocity if needed
            Vector3 localAngular = entryPortal.InverseTransformDirection(rb.angularVelocity);
            rb.angularVelocity = exitPortal.TransformDirection(localAngular);
        }
    }
}