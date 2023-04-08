using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System;

namespace ProjectAldea.Config
{
    [Serializable]
    public class BiomeConfig
    {
        [SerializeField]
        private Dictionary<Biome, List<Biome>> validNeighbors = new Dictionary<Biome, List<Biome>>();
        [SerializeField]
        private Biome[] biomes;
        [SerializeField]
        private Biome[] mountainBiomes;
        [SerializeField]
        private Biome[] waterBodyBiomes;

        public Biome[] Biomes { get => this.biomes; internal set => this.biomes = value; }
        public Biome[] MountainBiomes { get => this.mountainBiomes; internal set => this.mountainBiomes = value; }
        public Biome[] WaterBodyBiomes { get => this.waterBodyBiomes; internal set => this.waterBodyBiomes = value; }
        internal Dictionary<Biome, List<Biome>> ValidNeighbors { get => this.validNeighbors; }

        internal void SetAdjacent(Biome first, Biome second)
        {
            if (!this.validNeighbors.ContainsKey(first))
            {
                this.validNeighbors.Add(first, new List<Biome>());
            }
            if (!this.validNeighbors.ContainsKey(second))
            {
                this.validNeighbors.Add(second, new List<Biome>());
            }

            if (!this.validNeighbors[first].Contains(second))
            {
                this.validNeighbors[first].Add(second);
            }
            if (!this.validNeighbors[second].Contains(first))
            {
                this.validNeighbors[second].Add(first);
            }
        }
    }

    [Serializable]
    public class Biome
    {
        //TODO: Resources

        [SerializeField, Range(0.0f, 1.0f)]
        private float temperature;
        [SerializeField, Range(0.0f, 1.0f)]
        private float humidity;
        [SerializeField, Range(0.0f, 1.0f)]
        private float maxHeight;
        [SerializeField, Range(0.0f, 1.0f)]
        private float minHeight;
        [SerializeField]
        private Color color;
        [SerializeField]
        private string name;

        public string ColorHex { get => "#" + ColorUtility.ToHtmlStringRGB(this.color); set => this.SetColor(value); }
        public float Temperature { get => this.temperature; set => this.temperature = value; }
        public float MaxHeight { get => this.maxHeight; set => this.maxHeight = value; }
        public float MinHeight { get => this.minHeight; set => this.minHeight = value; }
        public float Humidity { get => this.humidity; set => this.humidity = value; }
        public Color Color { get => this.color; set => this.color = value; }
        public string Name { get => this.name; set => this.name = value; }

        private void SetColor(string colorHex)
        {
            if (!String.IsNullOrEmpty(colorHex) && Regex.IsMatch(colorHex, "#[0-9a-fA-F]{6}"))
            {
                ColorUtility.TryParseHtmlString(colorHex, out this.color);
            }
        }

        internal static Biome FromBiomeEntry(BiomeEntry entry)
        {
            return new Biome()
            {
                Temperature = Mathf.Clamp01(entry.Temperature),
                MaxHeight = Mathf.Clamp01(entry.MaxHeight),
                MinHeight = Mathf.Clamp01(entry.MinHeight),
                Humidity = Mathf.Clamp01(entry.Humidity),
                ColorHex = entry.Color,
                Name = entry.Name
            };
        }
    }

    [Serializable]
    public class BiomeEntry
    {
        [JsonProperty("validNeighbors")]
        public List<string> ValidNeighbors { get; set; }
        [JsonProperty("temperature")]
        public float Temperature { get; set; }
        [JsonProperty("maxHeight")]
        public float MaxHeight { get; set; }
        [JsonProperty("minHeight")]
        public float MinHeight { get; set; }
        [JsonProperty("humidity")]
        public float Humidity { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
