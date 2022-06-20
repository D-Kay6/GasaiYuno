using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.DynamicChannels.Models;
using GasaiYuno.Discord.DynamicChannels.Commands.Autocomplete;
using GasaiYuno.Discord.DynamicChannels.Mediator.Commands;
using GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

namespace GasaiYuno.Discord.DynamicChannels.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.ManageChannels)]
[Group("dynamic-channels", "Configure and manage the dynamically generated channels.")]
public class DynamicChannelModule : BaseInteractionModule<DynamicChannelModule>
{
    [SlashCommand("info", "Display information on the use of dynamic channels.")]
    public Task InfoCommand() => RespondAsync(Translation.Message("Automation.Channel.Info"), ephemeral: true);

    [SlashCommand("details", "Display detailed information of a specific configuration.")]
    public async Task DetailsCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))] [Summary(description: "The name of the configuration.")] string name)
    {
        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
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
    public async Task ListCommand()
    {
        var dynamicChannels = await Mediator.Send(new ListDynamicChannelsRequest(Context.Guild.Id)).ConfigureAwait(false);
        if (!dynamicChannels.Any())
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configurations"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithTitle(Translation.Message("Automation.Channel.Details.Title"));
        foreach (var dynamicChannel in dynamicChannels)
            embedBuilder.AddField(dynamicChannel.Name,
                Translation.Message("Automation.Channel.Details.List", dynamicChannel.Type.ToString(), dynamicChannel.Channels.Count(x => Context.Guild.GetVoiceChannel(x) != null)), true);

        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("new", "Create a new dynamic channel configuration.")]
    public async Task NewCommand(
        [Summary(description: "The name of the configuration.")] string name,
        [Summary(description: "The type of the generated channels.")] AutomationType type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Name"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
        if (dynamicChannel != null)
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Exists", name), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await Mediator.Publish(new AddDynamicChannelCommand(Context.Guild.Id, name, type)).ConfigureAwait(false);
        await RespondAsync(Translation.Message("Automation.Channel.Added", name), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("delete", "Delete a dynamic channel configuration.")]
    public async Task DeleteCommand([Autocomplete(typeof(DynamicChannelAutocompleteHandler))] [Summary(description: "The name of the configuration.")] string name)
    {
        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
        if (dynamicChannel == null)
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
            return;
        }
        
        await Mediator.Publish(new RemoveDynamicChannelCommand(Context.Guild.Id, name)).ConfigureAwait(false);
        await RespondAsync(Translation.Message("Automation.Channel.Deleted", name), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("edit", "Change a dynamic channel configuration.")]
    public async Task EditCommand(
        [Autocomplete(typeof(DynamicChannelAutocompleteHandler))] [Summary(description: "The name of the configuration.")] string name,
        [Summary(description: "The new name for generated channels.")] string generatedName)
    {
        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
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
        await Mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
        await RespondAsync(Translation.Message("Automation.Channel.Renamed", dynamicChannel.Name, dynamicChannel.GenerationName), ephemeral: true);
    }

    [SlashCommand("assign", "Assign a channel to a configuration.")]
    public async Task AssignCommand(
        [Autocomplete(typeof(DynamicChannelAutocompleteHandler))] [Summary(description: "The name of the configuration.")] string name,
        [Summary(description: "The channel to add to the configuration.")] IVoiceChannel channel)
    {
        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
        if (dynamicChannel == null)
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Configuration", name), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var dynamicChannels = await Mediator.Send(new ListDynamicChannelsRequest(Context.Guild.Id)).ConfigureAwait(false);
        var conflictingConfiguration = dynamicChannels.FirstOrDefault(x => x.Channels.Contains(channel.Id));
        if (conflictingConfiguration != null)
        {
            await RespondAsync(Translation.Message("Automation.Channel.Invalid.Assigned", conflictingConfiguration.Name, channel.Name), ephemeral: true).ConfigureAwait(false);
            return;
        }

        dynamicChannel.Channels.Add(channel.Id);
        await Mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
        await RespondAsync(Translation.Message("Automation.Channel.Assigned", dynamicChannel.Name, channel.Name), ephemeral: true);
    }

    [SlashCommand("remove", "Remove a channel from a configuration.")]
    public async Task RemoveCommand(
        [Autocomplete(typeof(DynamicChannelAutocompleteHandler))] [Summary(description: "The name of the configuration.")] string name,
        [Summary(description: "The channel to remove from the configuration.")] IVoiceChannel channel)
    {
        var dynamicChannel = await Mediator.Send(new GetDynamicChannelRequest(Context.Guild.Id, name)).ConfigureAwait(false);
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
        await Mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
        await RespondAsync(Translation.Message("Automation.Channel.Removed", dynamicChannel.Name, channel.Name), ephemeral: true);
    }
}