using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    public PrefabWeights prefabWeights;
    public DungeonStyle currentStyle;
    public LayerMask dungeonLayerMask;  // Add a LayerMask for dungeon elements

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("DungeonManager instance created.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartDungeonGeneration();
    }

    public void StartDungeonGeneration()
    {
        Debug.Log("Starting dungeon generation...");
        StartCoroutine(SpawnRoomsCoroutine());
    }

    private IEnumerator SpawnRoomsCoroutine()
    {
        Debug.Log("SpawnRoomsCoroutine started.");
        while (SpawnPoint.activeSpawnPoints.Count > 0)
        {
            Debug.Log($"Active spawn points count: {SpawnPoint.activeSpawnPoints.Count}");
            List<SpawnPoint> currentSpawnPoints = new List<SpawnPoint>(SpawnPoint.activeSpawnPoints);

            foreach (var spawnPoint in currentSpawnPoints)
            {
                if (!spawnPoint.isOccupied)
                {
                    Debug.Log($"Trying to spawn at {spawnPoint.transform.position}");
                    spawnPoint.TrySpawn();
                    yield return new WaitForSeconds(0.1f); // Small delay to allow for room placement
                }
            }

            yield return null;
        }

        Debug.Log("Dungeon generation complete.");
    }

    public void SpawnRoomOrHallway(SpawnPoint spawnPoint)
    {
        DungeonStylePrefabs stylePrefabs = GetStylePrefabs();
        bool spawnedSuccessfully = false;

        for (int i = 0; i < 3; i++) // Try up to 3 times
        {
            GameObject prefab = ChooseWeightedPrefab(stylePrefabs.prefabs, PrefabType.Room);
            GameObject instance = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            if (CheckForCollision(instance))
            {
                Debug.Log($"Collision detected on attempt {i + 1}, destroying instance.");
                Destroy(instance);
            }
            else
            {
                Debug.Log("Instance spawned successfully.");
                spawnPoint.isOccupied = true;
                var spawnPoints = instance.GetComponentsInChildren<SpawnPoint>();
                StartCoroutine(CheckHallwayDeadEnd(spawnPoints, instance));
                spawnedSuccessfully = true;
                break;
            }
        }

        if (!spawnedSuccessfully)
        {
            Debug.Log("Failed to spawn room after 3 attempts, spawning closed door.");
            SpawnClosedDoor(spawnPoint);
        }
    }

    public void SpawnClosedDoor(SpawnPoint spawnPoint)
    {
        Debug.Log("Spawning closed door.");
        DungeonStylePrefabs stylePrefabs = GetStylePrefabs();
        GameObject closedDoorPrefab = stylePrefabs.closedDoorPrefab;  // Use the closed door prefab from the current style
        Instantiate(closedDoorPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        DeactivateSpawnPoint(spawnPoint);
    }

    private void DeactivateSpawnPoint(SpawnPoint spawnPoint)
    {
        spawnPoint.isOccupied = true;
        spawnPoint.gameObject.SetActive(false);
        SpawnPoint.activeSpawnPoints.Remove(spawnPoint);
        Debug.Log($"Deactivated spawn point at {spawnPoint.transform.position}");
    }

    private DungeonStylePrefabs GetStylePrefabs()
    {
        foreach (var stylePrefabs in prefabWeights.dungeonStyles)
        {
            if (stylePrefabs.style == currentStyle)
            {
                return stylePrefabs;
            }
        }
        Debug.LogWarning("No matching style found.");
        return null; // This should never happen if styles are correctly assigned
    }

    private GameObject ChooseWeightedPrefab(List<WeightedPrefab> prefabs, PrefabType type)
    {
        List<WeightedPrefab> filteredPrefabs = prefabs.FindAll(p => p.type == type);
        float totalWeight = 0f;
        foreach (var prefab in filteredPrefabs)
        {
            totalWeight += prefab.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        foreach (var prefab in filteredPrefabs)
        {
            if (randomValue < prefab.weight)
            {
                return prefab.prefab;
            }
            randomValue -= prefab.weight;
        }

        Debug.LogWarning("No prefab chosen, this should never happen.");
        return null; // This should never happen if weights are correctly assigned
    }

    private bool CheckForCollision(GameObject instance)
    {
        Collider[] colliders = instance.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation, dungeonLayerMask);
            foreach (var hit in hits)
            {
                if (hit.gameObject != instance)
                {
                    Debug.Log($"Collision detected with {hit.gameObject.name} at {hit.transform.position}");
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator CheckHallwayDeadEnd(SpawnPoint[] spawnPoints, GameObject instance)
    {
        yield return new WaitForSeconds(0.5f); // Wait for a short period to ensure all potential spawns are processed

        bool allSpawnPointsClosed = true;
        foreach (var spawnPoint in spawnPoints)
        {
            if (!spawnPoint.isOccupied)
            {
                allSpawnPointsClosed = false;
                break;
            }
        }

        if (allSpawnPointsClosed)
        {
            Debug.Log("Dead end detected, replacing with closed door.");
            Destroy(instance);
            SpawnClosedDoor(instance.GetComponentInParent<SpawnPoint>()); // Use the originating spawn point
        }
    }
}
