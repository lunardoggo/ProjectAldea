using System.Collections.Generic;
using ProjectAldea.Scripts;
using ProjectAldea.Config;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private static readonly List<MapViewMode> mapModes = Enum.GetValues(typeof(MapViewMode)).Cast<MapViewMode>().ToList();

    public override void OnInspectorGUI()
    {
        TerrainGenerator generator = this.target as TerrainGenerator;

        int selected = EditorGUILayout.Popup("Map modes", mapModes.IndexOf(generator.MapMode), mapModes.Select(_option => _option.ToString()).ToArray());
        if (selected >= 0 && generator.MapMode != mapModes[selected])
        {
            generator.MapMode = mapModes[selected];
        }

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
        if (GUILayout.Button("Reload Config"))
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
        this.serializedObject.Update();
    }
}