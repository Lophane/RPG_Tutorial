using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    public static List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();
    public bool isOccupied = false;

    public float minY = -10f;
    public float maxY = 10f;
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;

    private void Awake()
    {
        activeSpawnPoints.Add(this);
    }

    public void TrySpawn()
    {
        if (IsValidPosition() && !isOccupied)
        {
            DungeonManager.Instance.SpawnRoomOrHallway(this);
        }
        else
        {
            DungeonManager.Instance.SpawnClosedDoor(this);
        }
    }

    private bool IsValidPosition()
    {
        Vector3 pos = transform.position;
        return pos.y >= minY && pos.y <= maxY && pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ;
    }

    private void OnDestroy()
    {
        activeSpawnPoints.Remove(this);
    }
}
