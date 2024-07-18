using UnityEngine;

public class InteractiveCloth : MonoBehaviour
{
    private Cloth cloth;
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 lastTouchPosition;

    [Header("Cloth Properties")]
    public float stretchingStiffness = 0.5f;
    public float bendingStiffness = 0.5f;
    public float damping = 0.5f;
    public float friction = 0.5f;

    [Header("Interaction")]
    public float interactionRadius = 0.1f;
    public float interactionForce = 10f;

    private Vector3[] clothVertices;
    private Vector3[] clothNormals;

    void Start()
    {
        cloth = GetComponent<Cloth>();
        mainCamera = Camera.main;
        SetupCloth();
    }

    void SetupCloth()
    {
        cloth.stretchingStiffness = stretchingStiffness;
        cloth.bendingStiffness = bendingStiffness;
        cloth.damping = damping;
        cloth.friction = friction;
        cloth.useGravity = true;

        cloth.clothSolverFrequency = 120;
    }

    void Update()
    {
        HandleTouchInteraction();
    }

    void HandleTouchInteraction()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.z - mainCamera.transform.position.z));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit hit;
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        offset = transform.position - hit.point;
                        lastTouchPosition = touchPosition;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                        ApplyLocalDeformation(touchPosition);
                    }
                    lastTouchPosition = touchPosition;
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }
    }

    void ApplyLocalDeformation(Vector3 touchPosition)
    {
        clothVertices = cloth.vertices;
        for (int i = 0; i < clothVertices.Length; i++)
        {
            Vector3 vertexWorldPos = transform.TransformPoint(clothVertices[i]);
            float distance = Vector3.Distance(vertexWorldPos, touchPosition);
            if (distance < interactionRadius)
            {
                float force = 1 - (distance / interactionRadius);
                Vector3 deformationDir = (touchPosition - lastTouchPosition).normalized;
                cloth.externalAcceleration = deformationDir * force * interactionForce;
            }
        }
    }

    void LateUpdate()
    {
        EnhanceFoldingAndWrinkling();
    }

    void EnhanceFoldingAndWrinkling()
    {
        clothVertices = cloth.vertices;
        clothNormals = cloth.normals;

        for (int i = 0; i < clothVertices.Length; i++)
        {
            Vector3 randomDisplacement = Random.insideUnitSphere * 0.0005f;
            clothVertices[i] += Vector3.Scale(randomDisplacement, clothNormals[i]);
        }

        // Apply subtle external forces to enhance wrinkling
        cloth.externalAcceleration += new Vector3(
            Mathf.Sin(Time.time * 10f) * 0.1f,
            Mathf.Cos(Time.time * 10f) * 0.1f,
            Mathf.Sin(Time.time * 15f) * 0.1f
        );
    }
}