using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpeedBoosterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform location;
        public bool isOccupied = false;
    }

    [Header("Spawn Settings")]
    public GameObject speedBoosterPrefab;
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    public float respawnTime = 10f;
    public bool randomSpawnOrder = true;

    [Header("Booster Settings")]
    public float minBoostPower = 1.5f;
    public float maxBoostPower = 3f;
    public float minBoostDuration = 2f;
    public float maxBoostDuration = 5f;

    private List<int> availableSpawnIndices = new List<int>();

    private void Start()
    {
        InitializeSpawnPoints();
        SpawnInitialBoosters();
    }

    private void InitializeSpawnPoints()
    {
        availableSpawnIndices.Clear();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            availableSpawnIndices.Add(i);
        }
    }

    private void SpawnInitialBoosters()
    {
        foreach (var point in spawnPoints)
        {
            if (!point.isOccupied)
            {
                SpawnBoosterAtPoint(point);
            }
        }
    }

    public void SpawnBoosterAtPoint(SpawnPoint point)
    {
        // if (point.isOccupied || point.location == null) return;

        // GameObject booster = Instantiate(speedBoosterPrefab, point.location.position, point.location.rotation);
        // SpeedBooster boosterScript = booster.GetComponent<SpeedBooster>();
        
        // if (boosterScript != null)
        // {
        //     // Set random boost values
        //     float randomPower = Random.Range(minBoostPower, maxBoostPower);
        //     float randomDuration = Random.Range(minBoostDuration, maxBoostDuration);
            
        //     boosterScript.boostMultiplier = randomPower;
        //     boosterScript.boostDuration = randomDuration;
            
        //     // Setup respawn callback
        //     boosterScript.onBoosterCollected.AddListener(() => StartCoroutine(RespawnBooster(point)));
        // }

        // point.isOccupied = true;
    }

    private IEnumerator RespawnBooster(SpawnPoint point)
    {
        point.isOccupied = false;
        yield return new WaitForSeconds(respawnTime);
        
        if (randomSpawnOrder)
        {
            SpawnRandomBooster();
        }
        else
        {
            SpawnBoosterAtPoint(point);
        }
    }

    public void SpawnRandomBooster()
    {
        if (availableSpawnIndices.Count == 0) return;

        // Get random available spawn point
        int randomIndex = Random.Range(0, availableSpawnIndices.Count);
        int spawnPointIndex = availableSpawnIndices[randomIndex];
        SpawnPoint selectedPoint = spawnPoints[spawnPointIndex];

        SpawnBoosterAtPoint(selectedPoint);
    }

    // Call this when a booster is collected
    public void BoosterCollected(Transform boosterLocation)
    {
        foreach (var point in spawnPoints)
        {
            if (point.location == boosterLocation)
            {
                point.isOccupied = false;
                StartCoroutine(RespawnBooster(point));
                break;
            }
        }
    }
}