using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    public enum DungeonStyle
    {
        Tomb,
        Overgrown,
        Decrepit,
        // Add more styles as needed
    }

    [CreateAssetMenu(fileName = "PrefabWeights", menuName = "Dungeon/PrefabWeights", order = 1)]
    public class PrefabWeights : ScriptableObject
    {
        public List<DungeonStylePrefabs> dungeonStyles = new List<DungeonStylePrefabs>();

    }

    [System.Serializable]
    public class DungeonStylePrefabs
    {
        public DungeonStyle style;
        public List<WeightedPrefab> prefabs = new List<WeightedPrefab>();
        public GameObject closedDoorPrefab;
    }
}
