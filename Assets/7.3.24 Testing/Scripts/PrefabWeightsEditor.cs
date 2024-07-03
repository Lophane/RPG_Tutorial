using UnityEditor;
using UnityEngine;
using DungeonGeneration;
using System.Collections.Generic;

public class PrefabWeightsEditor : EditorWindow
{

    private PrefabWeights prefabWeights;
    private int selectedTab = 0;
    private string[] tabNames;

    [MenuItem("Dungeon/Prefab Weights Editor")]
    public static void ShowWindow()
    {
        GetWindow<PrefabWeightsEditor>("Prefab Weights Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Weights Editor", EditorStyles.boldLabel);

        prefabWeights = (PrefabWeights)EditorGUILayout.ObjectField("Prefab Weights Asset", prefabWeights, typeof(PrefabWeights), false);

        if (prefabWeights != null)
        {
            tabNames = new string[prefabWeights.dungeonStyles.Count];
            for (int i = 0; i < prefabWeights.dungeonStyles.Count; i++)
            {
                tabNames[i] = prefabWeights.dungeonStyles[i].style.ToString();
            }

            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

            if (selectedTab < prefabWeights.dungeonStyles.Count)
            {
                DrawDungeonStyle(prefabWeights.dungeonStyles[selectedTab]);
            }

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(prefabWeights);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private void DrawDungeonStyle(DungeonStylePrefabs stylePrefabs)
    {
        EditorGUILayout.LabelField(stylePrefabs.style.ToString() + " Prefabs", EditorStyles.boldLabel);

        stylePrefabs.closedDoorPrefab = (GameObject)EditorGUILayout.ObjectField("Closed Door Prefab", stylePrefabs.closedDoorPrefab, typeof(GameObject), false);

        DrawPrefabList(stylePrefabs.prefabs);
    }

    private void DrawPrefabList(List<WeightedPrefab> prefabs)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabs[i].prefab = (GameObject)EditorGUILayout.ObjectField(prefabs[i].prefab, typeof(GameObject), false);
            prefabs[i].weight = EditorGUILayout.FloatField(prefabs[i].weight);
            prefabs[i].type = (DungeonGeneration.PrefabType)EditorGUILayout.EnumPopup(prefabs[i].type); // Fully qualify the enum
            if (GUILayout.Button("Remove"))
            {
                prefabs.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Prefab"))
        {
            prefabs.Add(new WeightedPrefab());
        }
    }
}
