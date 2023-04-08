using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ProjectAldea.Config
{
    [CreateAssetMenu(fileName = "BiomeConfig", menuName = "ProjectAldea/BiomeConfig", order = 1)]
    public class BiomeConfig : ScriptableObject
    {
        [SerializeField]
        private List<Biome> biomes = new List<Biome>();

        public List<Biome> Biomes { get => this.biomes; }
    }

    [Serializable]
    public class Biome
    {
        //TODO: Resources

        [SerializeField]
        private List<Biome> validNeighbors = new List<Biome>();

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

        public string ColorHex { get => ColorUtility.ToHtmlStringRGB(this.color); internal set => this.SetColor(value); }
        public float Temperature { get => this.temperature; internal set => this.temperature = value; }
        public float MaxHeight { get => this.maxHeight; internal set => this.maxHeight = value; }
        public float MinHeight { get => this.minHeight; internal set => this.minHeight = value; }
        public float Humidity { get => this.humidity; internal set => this.humidity = value; }
        public Color Color { get => this.color; internal set => this.color = value; }
        public string Name { get => this.name; internal set => this.name = value; }

        private void SetColor(string colorHex)
        {
            if (!String.IsNullOrEmpty(colorHex) && Regex.IsMatch(colorHex, "#\\d{6}"))
            {
                ColorUtility.TryParseHtmlString(colorHex, out this.color);
                //throw new ArgumentException($"Invalid color hex value \"{colorHex}\" for biome \"{this.name}\"");
            }
        }

        internal void SetAdjacent(Biome other)
        {
            if (!this.validNeighbors.Contains(other))
            {
                this.validNeighbors.Add(other);
            }
        }

        internal static Biome FromBiomeEntry(BiomeEntry entry)
        {
            return new Biome()
            {
                Temperature = entry.Temperature,
                MaxHeight = entry.MaxHeight,
                MinHeight = entry.MinHeight,
                Humidity = entry.Humidity,
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
