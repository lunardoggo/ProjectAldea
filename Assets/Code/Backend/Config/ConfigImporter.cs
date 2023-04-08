using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace ProjectAldea.Config
{
    public static class ConfigImporter
    {
        public static IOptional<BiomeConfig> LoadBiomeConfig()
        {
            const string relativePath = ConfigPaths.BiomeGeneration;
            try
            {
                JObject obj = JObject.Parse(StreamingAssetHelper.LoadTextFile(relativePath), new JsonLoadSettings()
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                    LineInfoHandling = LineInfoHandling.Ignore,
                    CommentHandling = CommentHandling.Ignore
                });
                return Optional.OfValue(obj).FlatMap(ConfigImporter.ParseBiomeConfig);
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

        private static IOptional<BiomeConfig> ParseBiomeConfig(JObject obj)
        {
            JArray biomes = obj["biomes"] as JArray;
            string[] mountainBiomes = obj["mountainBiomes"].ToObject<string[]>();
            string[] waterBodyBiomes = obj["waterBodyBiomes"].ToObject<string[]>();

            BiomeEntry[] entries = biomes.ToObject<BiomeEntry[]>();

            BiomeConfig config = new BiomeConfig();
            config.Biomes = entries.Select(_entry => Biome.FromBiomeEntry(_entry)).ToArray();
            config.MountainBiomes = config.Biomes.Where(_biome => mountainBiomes.Contains(_biome.Name)).ToArray();
            config.WaterBodyBiomes = config.Biomes.Where(_biome => waterBodyBiomes.Contains(_biome.Name)).ToArray();

            foreach (BiomeEntry entry in entries)
            {
                Biome biome = config.Biomes.Single(_biome => _biome.Name.Equals(entry.Name));
                foreach (string neighbor in entry.ValidNeighbors)
                {
                    Biome adjacent = config.Biomes.SingleOrDefault(_biome => _biome.Name.Equals(neighbor));
                    if (adjacent is null)
                    {
                        throw new Exception($"Biome adjacent to \"{entry.Name}\" with name \"{neighbor}\" does not exist");
                    }
                    config.SetAdjacent(biome, adjacent);
                }
            }

            return Optional.OfValue(config);
        }

        private static IEnumerable<Biome> ParseBiomes(JArray values)
        {
            BiomeEntry[] entries = values.ToObject<BiomeEntry[]>();
            Biome[] biomes = entries.Select(_entry => Biome.FromBiomeEntry(_entry)).ToArray();

            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i] = Biome.FromBiomeEntry(entries[i]);
            }

            return biomes;
        }
    }
}
