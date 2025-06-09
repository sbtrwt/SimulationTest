using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public float platformLength = 10f;
    public float platformSpacing = 0f;
    public int minPlatformsAhead = 4;
    public int maxPlatformsAhead = 5;

    [Header("Booster Settings")]
    public GameObject speedBoosterPrefab;
    public float boosterSpawnChance = 0.3f; // 30% chance per platform
    public float minBoosterDistance = 3f; // Min distance between boosters on same platform
    public int maxBoostersPerPlatform = 2;
    public Vector3 boosterScale = Vector3.one; // Add this to control booster scale independently

    [Header("Player Reference")]
    public Transform player;
    public float activationDistance = 40f;

    private Queue<GameObject> activePlatforms = new Queue<GameObject>();
    private List<GameObject> platformPool = new List<GameObject>();
    private List<GameObject> activeBoosters = new List<GameObject>();
    private float nextZPosition = 0f;
    private float playerFurthestZ = 0f;

    void Start()
    {
        // Initialize pool
        int initialPoolSize = maxPlatformsAhead + 2;
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePlatformInPool();
        }

        // Activate initial platforms
        for (int i = 0; i < minPlatformsAhead; i++)
        {
            ActivatePlatformFromPool();
        }
    }

    void Update()
    {
        if (player.position.z > playerFurthestZ)
        {
            playerFurthestZ = player.position.z;
        }

        // Manage platforms
        int platformsNeeded = Mathf.FloorToInt(playerFurthestZ / activationDistance) + minPlatformsAhead;
        platformsNeeded = Mathf.Clamp(platformsNeeded, minPlatformsAhead, maxPlatformsAhead);

        while (activePlatforms.Count < platformsNeeded)
        {
            ActivatePlatformFromPool();
        }

        RemoveDistantObjects();
    }

    void CreatePlatformInPool()
    {
        GameObject platform = Instantiate(platformPrefab);
        platform.transform.localScale = new Vector3(
            platform.transform.localScale.x,
            platform.transform.localScale.y,
            platformLength
        );
        platform.SetActive(false);
        platformPool.Add(platform);
    }

    void ActivatePlatformFromPool()
    {
        foreach (GameObject platform in platformPool)
        {
            if (!platform.activeInHierarchy)
            {
                platform.transform.position = new Vector3(0, 0, nextZPosition);
                platform.SetActive(true);
                activePlatforms.Enqueue(platform);
                
                // Spawn boosters on this platform
                TrySpawnBoosters(platform);
                
                nextZPosition += platformLength + platformSpacing;
                return;
            }
        }

        CreatePlatformInPool();
        ActivatePlatformFromPool();
    }

    void TrySpawnBoosters(GameObject platform)
    {
        // Only spawn boosters some of the time
        if (Random.value > boosterSpawnChance) return;

        // Get platform bounds
        Renderer platformRenderer = platform.GetComponent<Renderer>();
        Bounds platformBounds = platformRenderer != null ? 
            platformRenderer.bounds : 
            new Bounds(platform.transform.position, new Vector3(10f, 1f, platformLength));

        // Determine how many boosters to spawn (1 to maxBoostersPerPlatform)
        int boosterCount = Random.Range(1, maxBoostersPerPlatform + 1);
        
        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < boosterCount; i++)
        {
            Vector3 spawnPos = GetValidBoosterPosition(platformBounds, usedPositions);
            if (spawnPos != Vector3.zero)
            {
                GameObject booster = Instantiate(
                    speedBoosterPrefab, 
                    spawnPos, 
                    Quaternion.identity
                );
                
                // Set the booster's scale independently of the platform
                booster.transform.localScale = boosterScale;
                
                // Optionally parent to platform for organization (scale is already set)
                booster.transform.SetParent(platform.transform, true);
                
                activeBoosters.Add(booster);
                usedPositions.Add(spawnPos);
            }
        }
    }

    Vector3 GetValidBoosterPosition(Bounds platformBounds, List<Vector3> usedPositions)
    {
        // Try several times to find valid position
        for (int i = 0; i < 10; i++)
        {
            // Random position within platform bounds
            Vector3 randomPos = new Vector3(
                Random.Range(platformBounds.min.x, platformBounds.max.x),
                platformBounds.max.y + 0.5f, // Slightly above platform
                Random.Range(platformBounds.min.z, platformBounds.max.z)
            );

            // Check if too close to other boosters
            bool positionValid = true;
            foreach (Vector3 usedPos in usedPositions)
            {
                if (Vector3.Distance(randomPos, usedPos) < minBoosterDistance)
                {
                    positionValid = false;
                    break;
                }
            }

            if (positionValid)
            {
                return randomPos;
            }
        }

        return Vector3.zero; // No valid position found
    }

    void RemoveDistantObjects()
    {
        // Remove old platforms
        while (activePlatforms.Count > 0 && 
               activePlatforms.Peek().transform.position.z + platformLength < playerFurthestZ)
        {
            GameObject oldPlatform = activePlatforms.Dequeue();
            oldPlatform.SetActive(false);
        }

        // Remove boosters behind player
        for (int i = activeBoosters.Count - 1; i >= 0; i--)
        {
            if (activeBoosters[i] == null || 
                activeBoosters[i].transform.position.z + 5f < playerFurthestZ)
            {
                if (activeBoosters[i] != null)
                {
                    Destroy(activeBoosters[i]);
                }
                activeBoosters.RemoveAt(i);
            }
        }
    }
}