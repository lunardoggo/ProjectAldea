using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace ProjectAldea.Config
{
    public static class ConfigSaver
    {
        public static void SaveBiomeConfig(TerrainGeneratorConfig config)
        {
            JObject json = new JObject();
            json.Add(new JProperty("heightMap", JObject.FromObject(config.HeightMap)));
            json.Add("biomes", new JArray(config.Biomes.Select(_biome => JObject.FromObject(_biome))));
            json.Add(new JProperty("mountainBiomes", config.MountainBiomes.Select(_biome => _biome.Name)));
            json.Add(new JProperty("waterBodyBiomes", config.WaterBodyBiomes.Select(_biome => _biome.Name)));

            ConfigSaver.SaveJson(ConfigPaths.TerrainGeneration, json);
        }

        private static void SaveJson(string relativePath, JObject json)
        {
            StreamingAssetHelper.SaveTextFile(relativePath, json.ToString(Formatting.Indented));
        }
    }
}