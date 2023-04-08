using ProjectAldea.Scripts.TerrainGeneration;
using ProjectAldea.Config;
using UnityEngine;
using System.Linq;
using System;

namespace ProjectAldea.Scripts
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader meshGenrationShader;
        [SerializeField]
        private ComputeShader terrainMapShader;
        [SerializeField]
        private int seed;
        [SerializeField]
        private float scale;
        [SerializeField]
        private Vector2Int chunkDimensions = new Vector2Int(128, 128);
        [SerializeField]
        private Vector2Int chunkCount = new Vector2Int(3, 3);

        [SerializeField]
        private MeshRenderer chunkPrefab;

        [SerializeField]
        private Octave[] octaves;

        [SerializeField]
        private BiomeConfig biomeConfig;

        private void OnValidate()
        {
            this.ReloadBiomeConfig();
        }


        public void Generate()
        {
            this.seed = UnityEngine.Random.Range(byte.MaxValue * 2, byte.MaxValue * 32);

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform t = this.transform.GetChild(i);
                GameObject.Destroy(t.gameObject);
            }

            //TODO: for some stupid reason the shader cannot write to the buffer if ComputeBufferMode.Dynamic is used
            using ComputeBuffer octaves = new ComputeBuffer(this.octaves.Length, 3 * sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            octaves.SetData(this.octaves);
            using ComputeBuffer biomes = new ComputeBuffer(this.biomeConfig.Biomes.Length, 8 * sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.Immutable);
            biomes.SetData(this.biomeConfig.Biomes.Select(_biome => new B
            {
                minTemperature = Mathf.Max(0.0f, _biome.Temperature - 0.1f),
                maxTemperature = Mathf.Min(1.0f, _biome.Temperature + 0.1f),
                minHumidity = Mathf.Max(0.0f, _biome.Humidity - 0.1f),
                maxHumidity = Mathf.Min(1.0f, _biome.Humidity + 0.1f),
                color = _biome.Color
            }).ToArray());
            using MeshBuffer meshBuffer = this.GetMeshBuffer();

            for (int x = 0; x < this.chunkCount.x; x++)
            {
                for (int y = 0; y < this.chunkCount.y; y++)
                {
                    Vector2 noiseOffset = new Vector2(x * this.chunkDimensions.x - x, y * this.chunkDimensions.y - y);
                    Vector3 positionOffset = new Vector3(noiseOffset.x, 0, noiseOffset.y);

                    MeshRenderer target = GameObject.Instantiate(this.chunkPrefab, positionOffset, Quaternion.identity);
                    target.name = $"Chunk_{x}-{y}";

                    this.GenerateTerrainMap(noiseOffset, this.seed, octaves, meshBuffer);
                    target.GetComponent<MeshFilter>().mesh = this.GenerateMesh(biomes, meshBuffer);

                    target.transform.SetParent(this.transform);
                }
            }
        }

        private void GenerateTerrainMap(Vector2 offset, int seed, ComputeBuffer octaves, MeshBuffer meshBuffer)
        {
            RenderTexture terrainMap = meshBuffer.TerrainMap;

            this.terrainMapShader.SetTexture(0, "terrainMap", terrainMap);
            this.terrainMapShader.SetInt("seed", this.seed);
            this.terrainMapShader.SetInt("octaveCount", this.octaves.Length);
            this.terrainMapShader.SetBuffer(0, "octaves", octaves);
            this.terrainMapShader.SetInt("dimensions", terrainMap.width);
            this.terrainMapShader.SetFloats("offset", offset.x, offset.y);

            this.terrainMapShader.Dispatch(0, terrainMap.width / 8, terrainMap.height / 8, 1);
        }

        private Mesh GenerateMesh(ComputeBuffer biomes, MeshBuffer buffer)
        {
            this.meshGenrationShader.SetInts("dimensions", new int[] { this.chunkDimensions.x, this.chunkDimensions.y });
            this.meshGenrationShader.SetTexture(0, "terrainMap", buffer.TerrainMap);
            this.meshGenrationShader.SetBuffer(0, "triangles", buffer.Triangles);
            this.meshGenrationShader.SetBuffer(0, "vertices", buffer.Vertices);
            this.meshGenrationShader.SetBuffer(0, "colors", buffer.Colors);
            this.meshGenrationShader.SetFloat("scale", this.scale);
            this.meshGenrationShader.SetInt("biomeCount", this.biomeConfig.Biomes.Length);
            this.meshGenrationShader.SetBuffer(0, "biomes", biomes);

            this.meshGenrationShader.Dispatch(0, this.chunkDimensions.x / 8, this.chunkDimensions.y / 8, 1);

            buffer.UpdateArrays();

            Mesh mesh = new Mesh();
            mesh.vertices = buffer.VertexArray;
            mesh.triangles = buffer.TriangleArray;
            mesh.colors = buffer.ColorArray;

            mesh.RecalculateNormals();

            return mesh;
        }

        private MeshBuffer GetMeshBuffer()
        {
            RenderTexture terrainMap = new RenderTexture(this.chunkDimensions.x, this.chunkDimensions.y, 24);
            terrainMap.enableRandomWrite = true;
            terrainMap.Create();

            Vector3[] verts = new Vector3[this.chunkDimensions.x * this.chunkDimensions.y];
            int[] tris = new int[6 * (this.chunkDimensions.x) * (this.chunkDimensions.y)];
            Color[] col = new Color[verts.Length];

            return new MeshBuffer()
            {
                //TODO: for some stupid reason the shader cannot write to the buffer if ComputeBufferMode.Dynamic is used
                Vertices = new ComputeBuffer(verts.Length, 3 * sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.Immutable),
                Colors = new ComputeBuffer(col.Length, 4 * sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.Immutable),
                Triangles = new ComputeBuffer(tris.Length, sizeof(int), ComputeBufferType.Structured, ComputeBufferMode.Immutable),
                TerrainMap = terrainMap,
                TriangleArray = tris,
                VertexArray = verts,
                ColorArray = col
            };
        }

        private class MeshBuffer : IDisposable
        {
            public ComputeBuffer Triangles { get; set; }
            public ComputeBuffer Vertices { get; set; }
            public ComputeBuffer Colors { get; set; }

            public RenderTexture TerrainMap { get; set; }

            public Vector3[] VertexArray { get; set; }
            public int[] TriangleArray { get; set; }
            public Color[] ColorArray { get; set; }

            public void UpdateArrays()
            {
                this.Triangles.GetData(this.TriangleArray);
                this.Vertices.GetData(this.VertexArray);
                this.Colors.GetData(this.ColorArray);
            }

            public void Dispose()
            {
                this.Triangles.Dispose();
                this.Vertices.Dispose();
                this.Colors.Dispose();
            }
        }

        private struct B
        {
            public float minTemperature;
            public float maxTemperature;
            public float minHumidity;
            public float maxHumidity;
            public Color color;
        }

        public BiomeConfig BiomeConfig { get => this.biomeConfig; }

        public void ReloadBiomeConfig(bool force = false)
        {
            if (this.biomeConfig is null || force)
            {
                IOptional<BiomeConfig> config = ConfigImporter.LoadBiomeConfig();
                if (config.HasMessage)
                {
                    Debug.LogError(config.Message);
                    Debug.LogError(config.VerboseMessage ?? "No Verbose Message");
                }
                else
                {
                    this.biomeConfig = config.Value;
                }
            }
        }
    }
}
