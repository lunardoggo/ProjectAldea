using ProjectAldea.Scripts;
using ProjectAldea.Config;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            (this.target as TerrainGenerator).Generate();
        }
        if (GUILayout.Button("Regenerate"))
        {
            (this.target as TerrainGenerator).Regenerate();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Reload Config"))
        {
            (this.target as TerrainGenerator).ReloadBiomeConfig(true);
        }
        if (GUILayout.Button("Save Config"))
        {
            ConfigSaver.SaveBiomeConfig((this.target as TerrainGenerator).TerrainConfig);
            (this.target as TerrainGenerator).ReloadBiomeConfig(true);
        }
        GUILayout.EndHorizontal();

        this.serializedObject.ApplyModifiedProperties();
    }
}