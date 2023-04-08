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
        if(GUILayout.Button("Reload Biomes"))
        {
            (this.target as TerrainGenerator).ReloadBiomeConfig(true);
        }
        if (GUILayout.Button("Save Biomes"))
        {
            ConfigSaver.SaveBiomeConfig((this.target as TerrainGenerator).BiomeConfig);
            (this.target as TerrainGenerator).ReloadBiomeConfig(true);
        }
        GUILayout.EndHorizontal();
    }
}