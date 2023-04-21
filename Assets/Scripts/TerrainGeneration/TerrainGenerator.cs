using ProjectAldea.Config;
using UnityEngine;
using System.Linq;
using System;

namespace ProjectAldea.Scripts
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugTarget;

        [SerializeField]
        private ComputeShader meshGenrationShader;
        [SerializeField]
        private ComputeShader terrainMapShader;
        [SerializeField]
        private int seed;
        [SerializeField]
        private float scale;
        [SerializeField]
        private Vector2Int chunkDimensions = new Vector2Int(256, 256);

        [SerializeField]
        private MeshFilter chunkPrefab;
        [SerializeField]
        private MapSettings currentSettings;

        [SerializeField]
        private TerrainGeneratorConfig terrainConfig;

        private MapViewMode mapMode = MapViewMode.Default;

        private void OnValidate()
        {
            this.ReloadBiomeConfig();
        }

        public void Generate()
        {
            this.seed = UnityEngine.Random.Range(0, 100000);

            this.Regenerate();
        }

        public void Regenerate()
        {
            this.ClearChunks();

            Vector2Int resolution = this.currentSettings.MapSize;
            Vector2Int chunkCount = new Vector2Int(resolution.x / this.chunkDimensions.x, resolution.y / this.chunkDimensions.y);

            System.Random rng = new System.Random(this.seed + this.currentSettings.GetHashCode() % 10067);
            RenderTexture terrainMap = new RenderTexture(resolution.x, resolution.y, 24);
            terrainMap.enableRandomWrite = true;
            terrainMap.Create();

            this.UpdateTerrainMap(terrainMap, resolution, rng);

            this.debugTarget.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = terrainMap;

            this.SpawnChunks(terrainMap, chunkCount);
        }

        private void ClearChunks()
        {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(this.transform.GetChild(i).gameObject, true);
#else
                    GameObject.Destroy(this.transform.GetChild(i).gameObject);
#endif
            }
        }

        private void SpawnChunks(Texture terrainMap, Vector2Int chunkCount)
        {
            using MeshBuffer meshBuffer = this.GetMeshBuffer(terrainMap);
            using ComputeBuffer biomes = new ComputeBuffer(this.TerrainConfig.Biomes.Length, BufferedBiome.ByteSize);
            biomes.SetData(this.TerrainConfig.Biomes.Select(_biome => new BufferedBiome(_biome)).ToArray());
            Vector2 baseOffset = this.chunkDimensions;

            for (int x = 0; x < chunkCount.x; x++)
            {
                for (int y = 0; y < chunkCount.y; y++)
                {
                    Vector3 offset = new Vector3(baseOffset.x * x - x, 0, baseOffset.y * y - y);
                    this.GenerateMesh(biomes, meshBuffer, new Vector2(offset.x, offset.z));
                    meshBuffer.UpdateArrays();

                    this.InstantiateChunk(offset, meshBuffer);
                }
            }
        }

        private void InstantiateChunk(Vector3 offset, MeshBuffer meshBuffer)
        {
            MeshFilter filter = GameObject.Instantiate(this.chunkPrefab, offset, Quaternion.identity, this.transform);
            filter.gameObject.name = $"Chunk ({offset.x},{offset.z})";
            Mesh mesh = filter.sharedMesh = new Mesh()
            {
                vertices = meshBuffer.VertexArray,
                triangles = meshBuffer.TriangleArray,
                colors = meshBuffer.ColorArray
            };
            mesh.RecalculateNormals();

            MeshCollider collider = filter.GetComponent<MeshCollider>();
            if (collider != null)
            {
                collider.sharedMesh = mesh;
            }
        }

        private void UpdateTerrainMap(Texture map, Vector2Int resolution, System.Random rng)
        {
            this.terrainMapShader.SetFloat("persistance", this.terrainConfig.HeightMap.Persistance);
            this.terrainMapShader.SetFloat("lacunarity", this.terrainConfig.HeightMap.Lacunarity);
            this.terrainMapShader.SetInts("resolution", resolution.x, resolution.y);
            this.terrainMapShader.SetFloats("offset", (float)rng.NextDouble() * 50000, (float)rng.NextDouble() * 50000);
            this.terrainMapShader.SetInt("octaves", this.terrainConfig.HeightMap.Octaves);
            this.terrainMapShader.SetFloat("scale", 1000.0f / this.terrainConfig.HeightMap.Scale);
            this.terrainMapShader.SetTexture(0, "terrainMap", map);

            this.terrainMapShader.Dispatch(0, resolution.x / 8, resolution.y / 8, 1);
        }

        private Mesh GenerateMesh(ComputeBuffer biomes, MeshBuffer buffer, Vector2 offset)
        {
            this.meshGenrationShader.SetInts("dimensions", new int[] { this.chunkDimensions.x, this.chunkDimensions.y });
            this.meshGenrationShader.SetInt("biomeCount", biomes.count);
            this.meshGenrationShader.SetTexture(0, "terrainMap", buffer.TerrainMap);
            this.meshGenrationShader.SetBuffer(0, "triangles", buffer.Triangles);
            this.meshGenrationShader.SetBuffer(0, "vertices", buffer.Vertices);
            this.meshGenrationShader.SetFloats("offset", offset.x, offset.y);
            this.meshGenrationShader.SetBuffer(0, "colors", buffer.Colors);
            this.meshGenrationShader.SetInt("mapMode", (int)this.mapMode);
            this.meshGenrationShader.SetBuffer(0, "biomes", biomes);
            this.meshGenrationShader.SetFloat("scale", this.scale);

            this.meshGenrationShader.Dispatch(0, this.chunkDimensions.x / 8, this.chunkDimensions.y / 8, 1);

            buffer.UpdateArrays();

            Mesh mesh = new Mesh();
            mesh.vertices = buffer.VertexArray;
            mesh.triangles = buffer.TriangleArray;
            mesh.colors = buffer.ColorArray;

            mesh.RecalculateNormals();

            return mesh;
        }

        private MeshBuffer GetMeshBuffer(Texture terrainMap)
        {
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

        public MapViewMode MapMode
        {
            get => this.mapMode;
            set
            {
                this.mapMode = value;
                if (Application.isPlaying)
                {
                    this.Regenerate();
                }
            }
        }

        public TerrainGeneratorConfig TerrainConfig { get => this.terrainConfig; }

        public void ReloadBiomeConfig(bool force = false)
        {
            if (this.terrainConfig is null || force)
            {
                IOptional<TerrainGeneratorConfig> config = ConfigLoader.LoadBiomeConfig();
                if (config.HasMessage)
                {
                    Debug.LogError(config.Message);
                    Debug.LogError(config.VerboseMessage ?? "No Verbose Message");
                }
                else
                {
                    this.terrainConfig = config.Value;
                }
            }
        }

        private class MeshBuffer : IDisposable
        {
            public ComputeBuffer Triangles { get; set; }
            public ComputeBuffer Vertices { get; set; }
            public ComputeBuffer Colors { get; set; }

            public Texture TerrainMap { get; set; }

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

        private struct BufferedBiome
        {
            public BufferedBiome(Biome biome)
            {
                this.minTemperature = biome.MinTemperature;
                this.maxTemperature = biome.MaxTemperature;
                this.minHumidity = biome.MinHumidity;
                this.maxHumidity = biome.MaxHumidity;
                this.minHeight = biome.MinHeight;
                this.maxHeight = biome.MaxHeight;
                this.color = biome.Color;
            }

            public float minTemperature;
            public float maxTemperature;
            public float minHumidity;
            public float maxHumidity;
            public float minHeight;
            public float maxHeight;
            public Color color;

            public const int ByteSize = 10 * sizeof(float);
        }


    }
}
