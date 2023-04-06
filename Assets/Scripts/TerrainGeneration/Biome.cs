using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace ProjectAldea.Scripts.TerrainGeneration
{
    [Serializable]
    public class BiomePreset
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private Color color;
        [SerializeField]
        private float maxTemperature;
        [SerializeField]
        private float minTemperature;
        [SerializeField]
        private float maxHumidity;
        [SerializeField]
        private float minHumidity;

        public float MaxTemperature { get => this.maxTemperature; set => this.maxTemperature = value; }
        public float MinTemperature { get => this.minTemperature; set => this.minTemperature = value; }
        public float MaxHumidity { get => this.maxHumidity; set => this.maxHumidity = value; }
        public float MinHumidity { get => this.minHumidity; set => this.minHumidity = value; }
        public Color Color { get => this.color; set => this.color = value; }
        public string Name { get => this.name; set => this.name = value; }

        public Biome ToBiome()
        {
            return new Biome()
            {
                MinTemperature = this.minTemperature,
                MaxTemperature = this.maxTemperature,
                MinHumidity = this.minHumidity,
                MaxHumidity = this.maxHumidity,
                Color = this.color
            };
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Biome
    {
        public const int ByteSize = 8 * sizeof(float);

        [FieldOffset(0)]
        private float minTemperature;
        [FieldOffset(sizeof(float))]
        private float maxTemperature;
        [FieldOffset(2 * sizeof(float))]
        private float minHumidity;
        [FieldOffset(3 * sizeof(float))]
        private float maxHumidity;
        [FieldOffset(4 * sizeof(float))]
        private Color color;

        public float MinTemperature { get => this.minTemperature; set => this.minTemperature = value; }
        public float MaxTemperature { get => this.maxTemperature; set => this.maxTemperature = value; }
        public float MinHumidity { get => this.minHumidity; set => this.minHumidity = value; }
        public float MaxHumidity { get => this.maxHumidity; set => this.maxHumidity = value; }
        public Color Color { get => this.color; set => this.color = value; }
    }
}
