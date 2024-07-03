using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGeneration;

public class DungeonManager : MonoBehaviour
{

    public static DungeonManager Instance;
    public PrefabWeights prefabWeights;
    public DungeonStyle currentStyle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDungeonGeneration()
    {
        StartCoroutine(SpawnRoomsCoroutine());
    }

    private IEnumerator SpawnRoomsCoroutine()
    {
        while (SpawnPoint.activeSpawnPoints.Count > 0)
        {
            List<SpawnPoint> currentSpawnPoints = new List<SpawnPoint>(SpawnPoint.activeSpawnPoints);

            foreach (var spawnPoint in currentSpawnPoints)
            {
                if (!spawnPoint.isOccupied)
                {
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
        GameObject prefab = ChooseWeightedPrefab(stylePrefabs.prefabs, PrefabType.Room);
        GameObject instance = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

        if (CheckForCollision(instance))
        {
            Destroy(instance);
            spawnPoint.isOccupied = false;
        }
        else
        {
            spawnPoint.isOccupied = true;
            var spawnPoints = instance.GetComponentsInChildren<SpawnPoint>();
            StartCoroutine(CheckHallwayDeadEnd(spawnPoints, instance));
        }
    }

    public void SpawnClosedDoor(SpawnPoint spawnPoint)
    {
        DungeonStylePrefabs stylePrefabs = GetStylePrefabs();
        GameObject closedDoorPrefab = ChooseWeightedPrefab(stylePrefabs.prefabs, PrefabType.Room); // Assuming closed doors are of type Room
        Instantiate(closedDoorPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        spawnPoint.isOccupied = true;
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

        return null; // This should never happen if weights are correctly assigned
    }

    private bool CheckForCollision(GameObject instance)
    {
        Collider[] colliders = instance.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);
            foreach (var hit in hits)
            {
                if (hit.gameObject != instance)
                {
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
            Destroy(instance);
            SpawnClosedDoor(instance.GetComponentInParent<SpawnPoint>()); // Use the originating spawn point
        }
    }
}
