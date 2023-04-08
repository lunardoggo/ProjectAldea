using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using ProjectAldea;
using UnityEngine;
using System.Linq;
using System;

namespace ProjectAldea.Config
{
    public static class ConfigImporter
    {
        public static IOptional<BiomeConfig> LoadBiomeConfig(TextAsset asset)
        {
            return ConfigImporter.LoadFromAsset(asset, "Biome Config")
                .FlatMap(ConfigImporter.ParseBiomeConfig);
        }


        private static IOptional<JObject> LoadFromAsset(TextAsset asset, string name)
        {
            try
            {
                JObject obj = JObject.Parse(asset.text, new JsonLoadSettings()
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                    LineInfoHandling = LineInfoHandling.Ignore,
                    CommentHandling = CommentHandling.Ignore
                });
                return Optional.OfValue(obj);
            }
            catch (Exception ex)
            {
                return Optional.OfMessage<JObject>($"Error while loading config \"{name}\": {ex.Message}", ex.ToString());
            }
        }

        private static IOptional<BiomeConfig> ParseBiomeConfig(JObject obj)
        {
            try
            {
                JArray biomes = obj["biomes"] as JArray;

                BiomeConfig config = ScriptableObject.CreateInstance<BiomeConfig>();
                config.Biomes.AddRange(ConfigImporter.ParseBiomes(biomes));

                return Optional.OfValue(config);
            }
            catch (Exception ex)
            {
                return Optional.OfMessage<BiomeConfig>("Error while parsing biome config: " + ex.Message);
            }
        }

        private static IEnumerable<Biome> ParseBiomes(JArray values)
        {
            BiomeEntry[] entries = values.ToObject<BiomeEntry[]>();
            Biome[] biomes = entries.Select(_entry => Biome.FromBiomeEntry(_entry)).ToArray();

            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i] = Biome.FromBiomeEntry(entries[i]);
            }

            foreach(BiomeEntry entry in entries)
            {
                Biome entryBiome = biomes.Single(_biome => _biome.Name.Equals(entry.Name));
                foreach(string name in entry.ValidNeighbors)
                {
                    Biome biome = biomes.SingleOrDefault(_biome => _biome.Name.Equals(name));
                    if(biome is null)
                    {
                        throw new NullReferenceException($"Biome with name \"{name}\" linked to biome \"{entry.Name}\" could not be found");
                    }
                    entryBiome.SetAdjacent(biome);
                }
            }

            return biomes;
        }
    }
}
