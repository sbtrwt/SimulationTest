using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;  // Adjustable speed
    [SerializeField] private float rotationSpeed = 10f;  // Smooth rotation

    private Rigidbody rb;
    private Vector3 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get input (WASD or Arrow Keys)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Store movement direction (normalized to prevent faster diagonal movement)
        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void FixedUpdate()
    {
        if (movementInput.magnitude > 0.1f)  // Only move if input is significant
        {
            // Calculate movement direction
            Vector3 moveDirection = movementInput * moveSpeed * Time.fixedDeltaTime;

            // Apply movement (using Rigidbody for physics)
            rb.MovePosition(rb.position + moveDirection);

            // Smoothly rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}