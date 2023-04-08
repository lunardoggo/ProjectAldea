using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace ProjectAldea.Config
{
    public static class ConfigSaver
    {
        public static void SaveBiomeConfig(BiomeConfig config)
        {
            JObject json = new JObject();
            json.Add("biomes", ConfigSaver.GetBiomes(config));
            json.Add(new JProperty("mountainBiomes", config.MountainBiomes.Select(_biome => _biome.Name)));
            json.Add(new JProperty("waterBodyBiomes", config.WaterBodyBiomes.Select(_biome => _biome.Name)));

            ConfigSaver.SaveJson(ConfigPaths.BiomeGeneration, json);
        }

        private static JArray GetBiomes(BiomeConfig config)
        {
            JArray array = new JArray();
            foreach(Biome biome in config.Biomes)
            {
                array.Add(ConfigSaver.ToJObject(biome, config.ValidNeighbors));
            }
            return array;
        }

        private static JObject ToJObject(Biome biome, Dictionary<Biome, List<Biome>> neighbors)
        {
            JArray validNeighbors = new JArray();
            if(neighbors.ContainsKey(biome))
            {
                JArray.FromObject(neighbors[biome].Select(_biome => _biome.Name).ToArray());
            }

            JObject json = new JObject();
            json.Add(new JProperty("name", biome.Name));
            json.Add(new JProperty("validNeighbors", validNeighbors));
            json.Add(new JProperty("resources", new JArray()));
            json.Add(new JProperty("temperature", biome.Temperature));
            json.Add(new JProperty("humidity", biome.Humidity));
            json.Add(new JProperty("minHeight", biome.MinHeight));
            json.Add(new JProperty("maxHeight", biome.MaxHeight));
            json.Add(new JProperty("color", biome.ColorHex));

            return json;
        }

        private static void SaveJson(string relativePath, JObject json)
        {
            StreamingAssetHelper.SaveTextFile(relativePath, json.ToString(Formatting.Indented));
        }
    }
}