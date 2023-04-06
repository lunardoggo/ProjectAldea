using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace ProjectAldea.Scripts.TerrainGeneration
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Octave
    {
        [SerializeField]
        [FieldOffset(0)]
        private float scale;
        [SerializeField]
        [FieldOffset(sizeof(float))]
        private float weight;
        [SerializeField]
        [FieldOffset(2 * sizeof(float))]
        private float power;

        public float Weight { get => this.weight; set => this.weight = value; }
        public float Scale { get => this.scale; set => this.scale = value; }
        public float Power { get => this.power; set => this.power = value; }
    }
}
