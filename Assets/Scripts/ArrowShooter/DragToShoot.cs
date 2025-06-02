using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToShoot : MonoBehaviour
{
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;

    public float forceMultiplier = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse Down on Arrow");
        if (rb.velocity == Vector3.zero) // Only allow when not moving
        {
            isDragging = true;
            dragStartPos = Input.mousePosition;
        }
    }

    void OnMouseDrag()
    {
        // You can visualize an arrow here if you want
        if (isDragging)
        {
            dragEndPos = Input.mousePosition;
            Vector3 dragDirection = dragStartPos - dragEndPos; // Calculate drag direction
                                                               // Optionally, you can visualize the drag direction here
            Debug.Log("Dragging: " + dragDirection);
            // You can also update a UI element or a line renderer to show the drag direction
            // For example, if you have a LineRenderer, you could set its positions here
            // lineRenderer.SetPosition(0, transform.position);
            // lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(dragEndPos));
            
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            dragEndPos = Input.mousePosition;
            Vector3 force = (dragStartPos - dragEndPos); // direction: drag backward
            force.z = force.y; // For 3D, convert vertical to forward
            force.y = 0; // No upward force, adjust if needed

            rb.AddForce(force * forceMultiplier);
            isDragging = false;
        }
    }
}
