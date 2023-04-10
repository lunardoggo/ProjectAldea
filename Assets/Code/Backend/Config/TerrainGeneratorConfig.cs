using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using System;

namespace ProjectAldea.Config
{
    [Serializable]
    public class TerrainGeneratorConfig
    {
        [SerializeField]
        private HeightMapConfig heightMap;
        [SerializeReference]
        private Biome[] biomes;
        [SerializeReference]
        private Biome[] mountainBiomes;
        [SerializeReference]
        private Biome[] waterBodyBiomes;

        public Biome[] Biomes { get => this.biomes; internal set => this.biomes = value; }
        public Biome[] WaterBodyBiomes { get => this.waterBodyBiomes; internal set => this.waterBodyBiomes = value; }
        public Biome[] MountainBiomes { get => this.mountainBiomes; internal set => this.mountainBiomes = value; }
        public HeightMapConfig HeightMap { get => this.heightMap; internal set => this.heightMap = value; }
    }

    [Serializable]
    public class HeightMapConfig
    {
        [SerializeField]
        private float scale;
        [SerializeField]
        private int octaves;
        [SerializeField]
        private float persistance;
        [SerializeField]
        private float lacunarity;

        [JsonProperty("persistance")]
        public float Persistance { get => this.persistance; internal set => this.persistance = value; }
        [JsonProperty("lacunarity")]
        public float Lacunarity { get => this.lacunarity; internal set => this.lacunarity = value; }
        [JsonProperty("octaves")]
        public int Octaves { get => this.octaves; internal set => this.octaves = value; }
        [JsonProperty("scale")]
        public float Scale { get => this.scale; internal set => this.scale = value; }
    }

    [Serializable]
    public class Biome
    {
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxTemperature;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minTemperature;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxHumidity;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minHumidity;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxHeight;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minHeight;
        [SerializeField]
        private Color color;
        [SerializeField]
        private string name;

        [JsonProperty("color")]
        public string ColorHex { get => "#" + ColorUtility.ToHtmlStringRGB(this.color); set => this.SetColor(value); }
        [JsonProperty("minTemperature")]
        public float MinTemperature { get => this.minTemperature; set => this.minTemperature = value; }
        [JsonProperty("maxTemperature")]
        public float MaxTemperature { get => this.maxTemperature; set => this.maxTemperature = value; }
        [JsonProperty("maxHeight")]
        public float MaxHeight { get => this.maxHeight; set => this.maxHeight = value; }
        [JsonProperty("minHeight")]
        public float MinHeight { get => this.minHeight; set => this.minHeight = value; }
        [JsonProperty("minHumidity")]
        public float MinHumidity { get => this.minHumidity; set => this.minHumidity = value; }
        [JsonProperty("maxHumidity")]
        public float MaxHumidity { get => this.maxHumidity; set => this.maxHumidity = value; }
        [JsonIgnore]
        public Color Color { get => this.color; set => this.color = value; }
        [JsonProperty("name")]
        public string Name { get => this.name; set => this.name = value; }

        private void SetColor(string colorHex)
        {
            if (!String.IsNullOrEmpty(colorHex) && Regex.IsMatch(colorHex, "#[0-9a-fA-F]{6}"))
            {
                ColorUtility.TryParseHtmlString(colorHex, out this.color);
            }
        }
    }
}
