using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject platformPrefab;
    public int initialPlatformCount = 5;
    public float platformLength = 10f; // Z-axis length of each platform
    public float platformSpacing = 0f; // Extra space between platforms

    [Header("Player Reference")]
    public Transform player;
    public float repositionDistance = 40f; // Distance ahead to maintain platforms

    private Queue<GameObject> platformQueue = new Queue<GameObject>();
    private float nextZPosition = 0f;
    private float playerFurthestZ = 0f;

    void Start()
    {
        // Initial platform generation
        for (int i = 0; i < initialPlatformCount; i++)
        {
            CreatePlatform();
        }
    }

    void Update()
    {
        // Track player's furthest Z position
        if (player.position.z > playerFurthestZ)
        {
            playerFurthestZ = player.position.z;
        }

        // Check if we need new platforms ahead
        if (playerFurthestZ + repositionDistance > nextZPosition - platformLength)
        {
            CreatePlatform();
            RemoveDistantPlatforms();
        }
    }

    void CreatePlatform()
    {
        Vector3 position = new Vector3(0, 0, nextZPosition);
        GameObject platform = Instantiate(platformPrefab, position, Quaternion.identity);
        platform.transform.localScale = new Vector3(
            platform.transform.localScale.x,
            platform.transform.localScale.y,
            platformLength
        );

        platformQueue.Enqueue(platform);
        nextZPosition += platformLength + platformSpacing;
    }

    void RemoveDistantPlatforms()
    {
        // Remove platforms far behind player
        while (platformQueue.Count > 0 && 
               platformQueue.Peek().transform.position.z + platformLength < playerFurthestZ - repositionDistance)
        {
            GameObject oldPlatform = platformQueue.Dequeue();
            Destroy(oldPlatform);
        }
    }
}