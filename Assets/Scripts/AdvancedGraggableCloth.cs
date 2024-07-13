using UnityEngine;
using System.Collections.Generic;

public class AdvancedDraggableCloth : MonoBehaviour
{
    private Cloth cloth;
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    [Header("Cloth Properties")]
    public float stretchingStiffness = 0.5f;
    public float bendingStiffness = 0.5f;
    public float damping = 0.5f;
    public float friction = 0.5f;

    [Header("Wind")]
    public bool useWind = true;
    public float windStrength = 100f;
    public Vector3 windDirection = Vector3.right;

    [Header("Tearing")]
    public bool allowTearing = false;
    public float tearForce = 100f;
    private Mesh clothMesh;
    private ClothSkinningCoefficient[] coefficients;

    [Header("Pinning")]
    public List<int> pinnedVertices = new List<int>();

    [Header("Optimization")]
    public bool optimizeWhenStatic = true;
    public float staticThreshold = 0.01f;

    private Vector3 lastPosition;
    private float timeSinceLastMovement = 0f;

    void Start()
    {
        cloth = GetComponent<Cloth>();
        mainCamera = Camera.main;
        clothMesh = GetComponent<MeshFilter>().mesh;
        SetupCloth();
        SetupWind();
        PinVertices();
        InitializeClothCoefficients();
    }
    void InitializeClothCoefficients()
    {
        coefficients = new ClothSkinningCoefficient[cloth.vertices.Length];
        for (int i = 0; i < coefficients.Length; i++)
        {
            coefficients[i].maxDistance = 1f;
        }
        cloth.coefficients = coefficients;
    }
    void SetupCloth()
    {
        cloth.stretchingStiffness = stretchingStiffness;
        cloth.bendingStiffness = bendingStiffness;
        cloth.damping = damping;
        cloth.friction = friction;
        cloth.useGravity = true;

        
    }

    void SetupWind()
    {
        if (useWind)
        {
            cloth.externalAcceleration = windDirection.normalized * windStrength;
        }
    }

    void PinVertices()
    {
        ClothSkinningCoefficient[] coefficients = new ClothSkinningCoefficient[cloth.vertices.Length];
        for (int i = 0; i < coefficients.Length; i++)
        {
            coefficients[i].maxDistance = 0;
        }

        foreach (int index in pinnedVertices)
        {
            coefficients[index].maxDistance = 0;
            coefficients[index].collisionSphereDistance = 0;
        }

        cloth.coefficients = coefficients;
    }

    void Update()
    {
        HandleDragging();
        UpdateWind();
        OptimizePerformance();

        if (allowTearing)
        {
            UpdateClothMesh();
        }
    }

    void UpdateClothMesh()
    {
        clothMesh.vertices = cloth.vertices;
        clothMesh.RecalculateNormals();
        clothMesh.RecalculateBounds();
    }
    void HandleDragging()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit hit;
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        offset = transform.position - hit.point;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector3 newPosition = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
                        transform.position = newPosition + offset;
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }
    }

    void UpdateWind()
    {
        if (useWind)
        {
            // Simulate changing wind
            windDirection = Quaternion.Euler(0, Time.time * 10, 0) * Vector3.right;
            cloth.externalAcceleration = windDirection.normalized * windStrength;
        }
    }

    void OptimizePerformance()
    {
        if (optimizeWhenStatic)
        {
            if ((transform.position - lastPosition).magnitude < staticThreshold)
            {
                timeSinceLastMovement += Time.deltaTime;
                if (timeSinceLastMovement > 1f)
                {
                    cloth.enabled = false;
                }
            }
            else
            {
                timeSinceLastMovement = 0f;
                cloth.enabled = true;
            }
            lastPosition = transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Cloth collided with: " + collision.gameObject.name);
        // Add any specific collision behavior here
    }

    public void ToggleWind(bool enable)
    {
        useWind = enable;
        if (!useWind)
        {
            cloth.externalAcceleration = Vector3.zero;
        }
    }

    public void SetWindStrength(float strength)
    {
        windStrength = strength;
    }

    public void TearCloth(Vector3 position, float radius)
    {
        if (allowTearing)
        {
            Vector3[] vertices = cloth.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (Vector3.Distance(vertices[i], position) < radius)
                {
                    // Simulate tearing by increasing the max distance
                    coefficients[i].maxDistance = 2f;

                }
            }
            cloth.coefficients = coefficients;
        }
    }
}