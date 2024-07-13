using UnityEngine;
using System.Collections.Generic;

public class WaterContainer : MonoBehaviour
{
    public float waterAmount = 1f;
    public float maxWaterAmount = 1f;
    public float pourThreshold = 45f;
    public float pourRate = 0.1f;
    public Transform pourPoint;

    public float liftHeight = 1f;
    public float tiltAngle = 45f;
    public float liftSpeed = 2f;
    public float tiltSpeed = 90f;

    public GameObject waterDropPrefab;
    public int maxDrops = 10;
    public float dropSpawnRate = 0.1f;
    public float waterForceMultiplier = 1f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isLifted = false;
    private bool isTilted = false;
    private bool isPouring = false;
    private float lastDropTime;
    private List<GameObject> activeDrops = new List<GameObject>();
    public GameObject rippleEffectPrefab;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (waterDropPrefab == null)
        {
            Debug.LogError("Water drop prefab is not assigned!");
        }
        if (pourPoint == null)
        {
            Debug.LogError("Pour point is not assigned!");
        }
    }

    private void Update()
    {
        HandleInput();
        UpdatePosition();
        UpdateRotation();
        HandlePouring();
        UpdateWaterDrops();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                if (!isLifted)
                {
                    isLifted = true;
                }
                else if (!isTilted)
                {
                    isTilted = true;
                }
                else
                {
                    isLifted = false;
                    isTilted = false;
                }
            }
        }
    }

    private void UpdatePosition()
    {
        Vector3 targetPosition = isLifted ? initialPosition + Vector3.up * liftHeight : initialPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * liftSpeed);
    }

    private void UpdateRotation()
    {
        Quaternion targetRotation = isTilted ? initialRotation * Quaternion.Euler(0, 0, -tiltAngle) : initialRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);

        isPouring = isTilted && Vector3.Angle(transform.up, Vector3.up) > pourThreshold;
    }

    private void HandlePouring()
    {
        if (isPouring && waterAmount > 0)
        {
            Pour();
        }
       
    }

    private void Pour()
    {
        waterAmount -= pourRate * Time.deltaTime;
        waterAmount = Mathf.Clamp(waterAmount, 0f, maxWaterAmount);

       

        if (Time.time - lastDropTime > dropSpawnRate)
        {
            SpawnWaterDrop();
            lastDropTime = Time.time;
        }
    }

    private void SpawnWaterDrop()
    {
        if (activeDrops.Count >= maxDrops || pourPoint == null)
        {
            Debug.Log("Max drops reached or pour point not set");
            return;
        }

        GameObject waterDrop = Instantiate(waterDropPrefab, pourPoint.position, Quaternion.identity);
        Debug.Log("Water drop spawned at: " + pourPoint.position);

        Rigidbody rb = waterDrop.GetComponent<Rigidbody>();
        if (rb == null) rb = waterDrop.AddComponent<Rigidbody>();

        rb.useGravity = true;

        // Calculate pour direction based on gravity and container's orientation
        Vector3 pourDirection = (Physics.gravity - transform.right).normalized;

        rb.AddForce(pourDirection * waterForceMultiplier, ForceMode.Impulse);

        TrailRenderer trail = waterDrop.GetComponent<TrailRenderer>();
        if (trail == null) trail = waterDrop.AddComponent<TrailRenderer>();

        trail.startWidth = 0.05f;
        trail.endWidth = 0.01f;
        trail.time = 0.5f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = Color.blue;
        trail.endColor = new Color(0, 0, 1, 0);

        activeDrops.Add(waterDrop);
    }

    private void UpdateWaterDrops()
    {
        for (int i = activeDrops.Count - 1; i >= 0; i--)
        {
            GameObject drop = activeDrops[i];
            if (drop == null)
            {
                activeDrops.RemoveAt(i);
                continue;
            }

            RaycastHit hit;
            if (Physics.Raycast(drop.transform.position, Vector3.down, out hit, 0.1f))
            {
                WaterContainer targetContainer = hit.collider.GetComponent<WaterContainer>();
                if (targetContainer != null && targetContainer != this)
                {
                    targetContainer.FillWater(pourRate * Time.deltaTime / maxDrops);
                    Destroy(drop);
                    activeDrops.RemoveAt(i);
                }
            }
        }
    }


    public void FillWater(float amount)
    {
        waterAmount += amount;
        waterAmount = Mathf.Clamp(waterAmount, 0f, maxWaterAmount);
    }
}