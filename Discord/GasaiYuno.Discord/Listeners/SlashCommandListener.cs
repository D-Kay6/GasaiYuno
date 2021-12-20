using Discord;
using Discord.Net;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class SlashCommandListener
    {
        private readonly DiscordShardedClient _client;
        private readonly ILogger<SlashCommandListener> _logger;

        public SlashCommandListener(DiscordConnectionClient client, ILogger<SlashCommandListener> logger)
        {
            _client = client;
            _logger = logger;

            //client.ShardReady += OnShardReady;
        }

        private async Task OnShardReady(DiscordSocketClient client)
        {
            var slashCommands = GenerateSlashCommands();
            try
            {
                await client.BulkOverwriteGlobalApplicationCommandsAsync(slashCommands.ToArray());
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                _logger.LogError(exception, json);
            }
        }

        private List<ApplicationCommandProperties> GenerateSlashCommands()
        {
            var result = new List<ApplicationCommandProperties>();

            var globalCommand = new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("A descriptive list of my commands.");
            result.Add(globalCommand.Build());

            globalCommand = new SlashCommandBuilder()
                .WithName("invite")
                .WithDescription("My invite link, so you, or someone else, can add me to a server.");
            result.Add(globalCommand.Build());

            globalCommand = new SlashCommandBuilder()
                .WithName("support")
                .WithDescription("Link to my support discord.");
            result.Add(globalCommand.Build());

            globalCommand = new SlashCommandBuilder()
                .WithName("birthday")
                .WithDescription("Sing a song for a happy fellow.");
            result.Add(globalCommand.Build());

            globalCommand = new SlashCommandBuilder()
                .WithName("chat")
                .WithDescription("Engage in a casual, but probably still weird, conversation with me.");
            result.Add(globalCommand.Build());

            globalCommand = new SlashCommandBuilder()
                .WithName("kill")
                .WithDescription("Have me kill a user.");
            result.Add(globalCommand.Build());

            return result;
        }
    }
}