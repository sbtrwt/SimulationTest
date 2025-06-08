using UnityEngine;

public class XDirectionController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float xSensitivity = 1f;
    public float maxXPosition = 4f; // Left/right boundaries
    public float laneChangeSmoothness = 10f;

    [Header("Physics Settings")]
    public bool usePhysics = false;
    public float physicsForceMultiplier = 50f;

    private float targetXPosition;
    private Rigidbody rb;
    private bool isMoving = false;

    void Start()
    {
        targetXPosition = transform.position.x;
        if (usePhysics) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input (touch/mouse/keyboard)
        float xInput = GetXInput();

        // Calculate target X position
        if (xInput != 0)
        {
            targetXPosition += xInput * xSensitivity;
            targetXPosition = Mathf.Clamp(targetXPosition, -maxXPosition, maxXPosition);
            isMoving = true;
        }

        // Continuous forward movement
        if (usePhysics)
        {
            if (rb != null)
            {
                rb.AddForce(Vector3.forward * moveSpeed * physicsForceMultiplier * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        // Smooth X position movement
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, targetXPosition, laneChangeSmoothness * Time.fixedDeltaTime);

        if (usePhysics && rb != null)
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.x = (newPosition.x - transform.position.x) * laneChangeSmoothness;
            rb.velocity = newVelocity;
        }
        else
        {
            transform.position = newPosition;
        }
    }

    float GetXInput()
    {
        // Mobile touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                return touch.deltaPosition.x / Screen.width * 2f;
            }
        }
        
        // Keyboard input
        if (Input.GetAxis("Horizontal") != 0)
        {
            return Input.GetAxis("Horizontal");
        }
        
        // Mouse drag input
        if (Input.GetMouseButton(0))
        {
            return Input.GetAxis("Mouse X");
        }

        return 0;
    }
}