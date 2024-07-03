using UnityEngine;

namespace DungeonGeneration
{
    public enum PrefabType
    {
        Room,
        Hallway
    }

    [System.Serializable]
    public class WeightedPrefab
    {
        public GameObject prefab;
        public float weight;
        public PrefabType type;
    }
}
