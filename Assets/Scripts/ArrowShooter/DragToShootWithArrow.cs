using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class DragToShootWithArrow : MonoBehaviour
{
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;

    public float forceMultiplier = 10f;
    private Rigidbody rb;
    private LineRenderer shootPreview;
    public LineRenderer ghostArrow;
    private Vector3 smoothedDirection;
    private Vector3 ghostArrowTargetPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shootPreview = GetComponent<LineRenderer>();

        shootPreview.positionCount = 2;

        shootPreview.material = new Material(Shader.Find("Sprites/Default"));
        shootPreview.startColor = Color.red;
        shootPreview.endColor = Color.yellow;
        shootPreview.startWidth = 0.1f;
        shootPreview.endWidth = 0.05f;
        shootPreview.enabled = false;

        ghostArrow.positionCount = 2;
        ghostArrow.enabled = false;
        ghostArrow.material = new Material(Shader.Find("Sprites/Default"));
        ghostArrow.startColor = new Color(1f, 1f, 1f, 0.3f); // White, transparent
        ghostArrow.endColor = new Color(1f, 1f, 1f, 0.1f);
        ghostArrow.startWidth = 0.5f;
        ghostArrow.endWidth = 0.1f;
    }

    void OnMouseDown()
    {
        if (rb.velocity == Vector3.zero)
        {
            isDragging = true;
            dragStartPos = Input.mousePosition;
            shootPreview.enabled = true;

            ghostArrow.enabled = true;
            smoothedDirection = Vector3.zero;

        }
    }

    void OnMouseDrag()
{
    if (isDragging)
    {
        Vector3 dragVector = dragStartPos - Input.mousePosition;

        // The direction the object will shoot (opposite of drag)
        Vector3 shootDirection = new Vector3(-dragVector.x, 0, -dragVector.y) * 0.05f;
        Vector3 shootPoint = transform.position + shootDirection;

        // The direction you're dragging (for visual)
        Vector3 dragDirection = new Vector3(dragVector.x, 0, dragVector.y) * 0.05f;
        Vector3 dragPoint = transform.position + dragDirection;

        // 🔴 Main arrow: now shows drag direction
        shootPreview.SetPosition(0, transform.position);
        shootPreview.SetPosition(1, dragPoint);

        // ⚪ Ghost arrow: now shows smoothed shoot direction
        ghostArrowTargetPos = Vector3.Lerp(ghostArrowTargetPos, shootPoint, Time.deltaTime * 5f);
        ghostArrow.SetPosition(0, transform.position);
        ghostArrow.SetPosition(1, ghostArrowTargetPos);
    }
}


    void OnMouseUp()
    {
        if (isDragging)
        {
            dragEndPos = Input.mousePosition;
            Vector3 force = dragStartPos - dragEndPos;
            force.z = force.y;
            force.y = 0;

            rb.AddForce(force * forceMultiplier);
            isDragging = false;
            shootPreview.enabled = false;

            ghostArrow.enabled = false;
            ghostArrowTargetPos = transform.position;
        }
    }
}
