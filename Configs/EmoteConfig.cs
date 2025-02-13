﻿using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Levante.Configs
{
    public class EmoteConfig
    {
        public static string FilePath { get; } = @"Configs/emoteConfig.json";

        [JsonProperty("EmoteServers")]
        public List<ulong> EmoteServers { get; internal set; } = new List<ulong>();

        [JsonProperty("Emotes")]
        public Dictionary<string, string> Emotes { get; internal set; } = new Dictionary<string, string>();

        public async Task<bool> AddEmote(string name, Image image)
        {
            bool success = false;
            foreach (var guildId in EmoteServers)
            {
                var guild = LevanteCordInstance.Client.GetGuild(guildId);
                try
                {
                    if (guild.Emotes.Count(x => !x.Animated) < 50)
                    {
                        var emote = await guild.CreateEmoteAsync(name, image);
                        Emotes.Add(name, $"<:{emote.Name}:{emote.Id}>");
                        success = true;
                        Console.WriteLine($"Added emote for {name}.");
                        break;
                    }
                }
                catch (Discord.Net.HttpException x)
                {
                    var json = JsonConvert.SerializeObject(x.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                    break;
                }
            }
            image.Dispose();
            return success;
        }

        public void UpdateJSON()
        {
            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }
    }
}
