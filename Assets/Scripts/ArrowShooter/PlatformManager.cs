using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public float platformLength = 10f;
    public float platformSpacing = 0f;
    public int minPlatformsAhead = 4; // Minimum platforms to keep ahead of player
    public int maxPlatformsAhead = 5; // Maximum platforms to keep ahead

    [Header("Player Reference")]
    public Transform player;
    public float activationDistance = 40f; // Distance ahead to activate platforms

    private Queue<GameObject> activePlatforms = new Queue<GameObject>();
    private List<GameObject> platformPool = new List<GameObject>();
    private float nextZPosition = 0f;
    private float playerFurthestZ = 0f;

    void Start()
    {
        // Initialize pool with enough platforms
        int initialPoolSize = maxPlatformsAhead + 2; // Buffer of 2 extra platforms
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

        // Calculate how many platforms should be ahead
        int platformsNeeded = Mathf.FloorToInt(playerFurthestZ / activationDistance) + minPlatformsAhead;
        platformsNeeded = Mathf.Clamp(platformsNeeded, minPlatformsAhead, maxPlatformsAhead);

        // Manage platforms
        while (activePlatforms.Count < platformsNeeded)
        {
            ActivatePlatformFromPool();
        }

        RemoveDistantPlatforms();
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
        // Find an inactive platform
        foreach (GameObject platform in platformPool)
        {
            if (!platform.activeInHierarchy)
            {
                platform.transform.position = new Vector3(0, 0, nextZPosition);
                platform.SetActive(true);
                activePlatforms.Enqueue(platform);
                nextZPosition += platformLength + platformSpacing;
                return;
            }
        }

        // If no inactive platforms, create new one
        CreatePlatformInPool();
        ActivatePlatformFromPool(); // Try again with the new platform
    }

    void RemoveDistantPlatforms()
    {
        while (activePlatforms.Count > 0 && 
               activePlatforms.Peek().transform.position.z + platformLength < playerFurthestZ)
        {
            GameObject oldPlatform = activePlatforms.Dequeue();
            oldPlatform.SetActive(false);
        }
    }
}