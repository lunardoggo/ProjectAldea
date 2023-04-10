using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace ProjectAldea.Config
{
    public static class ConfigLoader
    {
        public static IOptional<TerrainGeneratorConfig> LoadBiomeConfig()
        {
            const string relativePath = ConfigPaths.TerrainGeneration;
            try
            {
                JObject json = JObject.Parse(StreamingAssetHelper.LoadTextFile(relativePath), new JsonLoadSettings()
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                    LineInfoHandling = LineInfoHandling.Ignore,
                    CommentHandling = CommentHandling.Ignore
                });
                return Optional.OfValue(json).FlatMap(ConfigLoader.ParseBiomeConfig);
            }
#if UNITY_EDITOR
            catch (Exception)
            {
                throw;
#else
            catch (Exception ex)
            {
                return Optional.OfMessage<BiomeConfig>($"Error while loading biome config: {ex.Message}", ex.ToString());
#endif
            }
        }

        private static IOptional<TerrainGeneratorConfig> ParseBiomeConfig(JObject json)
        {
            string[] mountainBiomes = json["mountainBiomes"].ToObject<string[]>();
            string[] waterBodyBiomes = json["waterBodyBiomes"].ToObject<string[]>();


            TerrainGeneratorConfig config = new TerrainGeneratorConfig();
            config.HeightMap = json["heightMap"].ToObject<HeightMapConfig>();
            config.Biomes = json["biomes"] .ToObject<Biome[]>();
            config.MountainBiomes = config.Biomes.Where(_biome => mountainBiomes.Contains(_biome.Name)).ToArray();
            config.WaterBodyBiomes = config.Biomes.Where(_biome => waterBodyBiomes.Contains(_biome.Name)).ToArray();

            return Optional.OfValue(config);
        }
    }
}
