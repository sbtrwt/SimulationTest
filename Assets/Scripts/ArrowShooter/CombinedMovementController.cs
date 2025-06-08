using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class CombinedMovementController : MonoBehaviour
{
    [Header("Drag to Shoot Settings")]
    public float forceMultiplier = 10f;
    public LineRenderer shootPreview;
    public LineRenderer ghostArrow;
    
    [Header("X Movement Settings")]
    public float xSensitivity = 1f;
    public float maxXPosition = 4f;
    public float laneChangeSmoothness = 10f;
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 dragStartPos;
    private bool isDragging = false;
    private float targetXPosition;
    private bool canControlX = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetXPosition = transform.position.x;
        
        // Initialize line renderers
        shootPreview.positionCount = 2;
        shootPreview.material = new Material(Shader.Find("Sprites/Default"));
        shootPreview.startColor = Color.red;
        shootPreview.endColor = Color.yellow;
        shootPreview.startWidth = 0.1f;
        shootPreview.endWidth = 0.05f;
        shootPreview.enabled = false;

        ghostArrow.positionCount = 2;
        ghostArrow.material = new Material(Shader.Find("Sprites/Default"));
        ghostArrow.startColor = new Color(1f, 1f, 1f, 0.3f);
        ghostArrow.endColor = new Color(1f, 1f, 1f, 0.1f);
        ghostArrow.startWidth = 0.5f;
        ghostArrow.endWidth = 0.1f;
        ghostArrow.enabled = false;
    }

    void Update()
    {
        HandleXMovement();
        HandleDragInput();
    }

    void FixedUpdate()
    {
        ApplyXMovement();
    }

    void HandleXMovement()
    {
        if (!canControlX) return;

        float xInput = 0f;
        
        // Get input only when not dragging
        if (!isDragging)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    xInput = touch.deltaPosition.x / Screen.width * 2f;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                xInput = Input.GetAxis("Mouse X");
            }
            else
            {
                xInput = Input.GetAxis("Horizontal");
            }
        }

        if (xInput != 0)
        {
            targetXPosition += xInput * xSensitivity;
            targetXPosition = Mathf.Clamp(targetXPosition, -maxXPosition, maxXPosition);
        }
    }

    void ApplyXMovement()
    {
        if (rb.velocity.magnitude < 0.1f)
        {
            canControlX = false;
            return;
        }
        else
        {
            canControlX = true;
        }

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, targetXPosition, laneChangeSmoothness * Time.fixedDeltaTime);
        transform.position = newPosition;
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0) && rb.velocity.magnitude < 0.1f)
        {
            StartDrag();
        }

        if (isDragging)
        {
            UpdateDrag();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        isDragging = true;
        dragStartPos = Input.mousePosition;
        shootPreview.enabled = true;
        ghostArrow.enabled = true;
    }

    void UpdateDrag()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 dragVector = dragStartPos - currentMousePos;

        // Visualize drag direction
        Vector3 worldDragDir = new Vector3(dragVector.x, 0, dragVector.y) * 0.05f;
        shootPreview.SetPosition(0, transform.position);
        shootPreview.SetPosition(1, transform.position + worldDragDir);

        // Visualize shoot direction (inverse of drag)
        Vector3 shootDirection = new Vector3(-dragVector.x, 0, -dragVector.y) * 0.05f;
        ghostArrow.SetPosition(0, transform.position);
        ghostArrow.SetPosition(1, transform.position + shootDirection);
    }

    void EndDrag()
    {
        Vector3 dragEndPos = Input.mousePosition;
        Vector3 force = new Vector3(
            dragStartPos.x - dragEndPos.x,
            0,
            dragStartPos.y - dragEndPos.y
        ) * forceMultiplier;

        rb.AddForce(force);
        
        shootPreview.enabled = false;
        ghostArrow.enabled = false;
        isDragging = false;
        
        StartCoroutine(EnableXControlAfterDelay(1f));
    }

    IEnumerator EnableXControlAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canControlX = true;
    }
}