using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Commands.Autocomplete;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation
{
    [Group("dynamicchannels", "Configure and manage the dynamically generated channels.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class DynamicChannelModule : BaseInteractionModule<DynamicChannelModule>
    {
        [SlashCommand("info", "Display information on the use of dynamic channels.")]
        public Task InfoDynamicChannelCommand() => RespondAsync(Translation.Message("Automation.Channel.Info"), ephemeral: true);

        [SlashCommand("details", "Display detailed information of a specific configuration.")]
        public async Task DetailsDynamicChannelCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))][Summary(description: "The name of the configuration.")] string name)
        {
            var dynamicChannel = await UnitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var assignedChannels = string.Empty;
            if (dynamicChannel.Channels.Any())
                assignedChannels = string.Join(Environment.NewLine, dynamicChannel.Channels.Select(x => Context.Guild.GetChannel(x)?.Name).Where(x => !string.IsNullOrEmpty(x)));

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(dynamicChannel.Name);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Details.Channels"), assignedChannels, true);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Details.Type"), dynamicChannel.Type.ToString(), true);
            embedBuilder.AddField(Translation.Message("Automation.Channel.Details.GenerationName"), dynamicChannel.GenerationName, true);

            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("list", "Display short information of all the configurations.")]
        public async Task ListDynamicChannelCommand()
        {
            var dynamicChannels = await UnitOfWork.DynamicChannels.ListAsync(Context.Guild.Id).ConfigureAwait(false);
            if (!dynamicChannels.Any())
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configurations"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Automation.Channel.Details.Title"));
            foreach (var dynamicChannel in dynamicChannels)
                embedBuilder.AddField(dynamicChannel.Name, Translation.Message("Automation.Channel.Details.List", dynamicChannel.Type.ToString(), dynamicChannel.Channels.Count(x => Context.Guild.GetVoiceChannel(x) != null)), true);

            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("new", "Create a new dynamic channel configuration.")]
        public async Task DynamicChannelAddAsync([Summary(description: "The name of the configuration.")] string name, [Summary(description: "The type of the generated channels.")] AutomationType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Name"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (await UnitOfWork.DynamicChannels.AnyAsync(x => x.Server.Id == Context.Guild.Id && x.Name == name).ConfigureAwait(false))
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Exists", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var dynamicChannel = new DynamicChannel
            {
                Server = Server,
                Type = type,
                Name = name
            };

            UnitOfWork.DynamicChannels.Add(dynamicChannel);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Automation.Channel.Added", name), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("delete", "Delete a dynamic channel configuration.")]
        public async Task DeleteDynamicChannelCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))][Summary(description: "The name of the configuration.")] string name)
        {
            var dynamicChannel = await UnitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            UnitOfWork.DynamicChannels.Remove(dynamicChannel);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Automation.Channel.Deleted", name), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("edit", "Change a dynamic channel configuration.")]
        public async Task EditDynamicChannelCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))][Summary(description: "The name of the configuration.")] string name, [Summary(description: "The new name for generated channels.")] string generatedName)
        {
            var dynamicChannel = await UnitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (string.IsNullOrWhiteSpace(generatedName))
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.GenerationName"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            dynamicChannel.GenerationName = generatedName;

            UnitOfWork.DynamicChannels.Update(dynamicChannel);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Automation.Channel.Renamed", dynamicChannel.Name, dynamicChannel.GenerationName), ephemeral: true);
        }

        [SlashCommand("assign", "Assign a channel to a configuration.")]
        public async Task AssignDynamicChannelCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))][Summary(description: "The name of the configuration.")] string name, [Summary(description: "The channel to add to the configuration.")] IVoiceChannel channel)
        {
            var dynamicChannel = await UnitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var dynamicChannels = await UnitOfWork.DynamicChannels.ListAsync(Context.Guild.Id).ConfigureAwait(false);
            var conflictingConfiguration = dynamicChannels.FirstOrDefault(x => x.Channels.Contains(channel.Id));
            if (conflictingConfiguration != null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Assigned", conflictingConfiguration.Name, channel.Name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            dynamicChannel.Channels.Add(channel.Id);

            UnitOfWork.DynamicChannels.Update(dynamicChannel);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Automation.Channel.Assigned", dynamicChannel.Name, channel.Name), ephemeral: true);
        }

        [SlashCommand("remove", "Remove a channel from a configuration.")]
        public async Task RemoveDynamicChannelCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))][Summary(description: "The name of the configuration.")] string name, [Summary(description: "The channel to remove from the configuration.")] IVoiceChannel channel)
        {
            var dynamicChannel = await UnitOfWork.DynamicChannels.GetAsync(Context.Guild.Id, name).ConfigureAwait(false);
            if (dynamicChannel == null)
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (!dynamicChannel.Channels.Contains(channel.Id))
            {
                await RespondAsync(Translation.Message("Automation.Channel.Invalid.Unassigned", dynamicChannel.Name, channel.Name), ephemeral: true).ConfigureAwait(false);
                return;
            }

            dynamicChannel.Channels.Remove(channel.Id);

            UnitOfWork.DynamicChannels.Update(dynamicChannel);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Automation.Channel.Removed", dynamicChannel.Name, channel.Name), ephemeral: true);
        }
    }
}