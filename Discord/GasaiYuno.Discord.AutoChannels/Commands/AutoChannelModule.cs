using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.AutoChannels.Mediator.Commands;
using GasaiYuno.Discord.AutoChannels.Mediator.Requests;
using GasaiYuno.Discord.AutoChannels.Models;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.AutoChannels.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageChannels)]
[RequireUserPermission(GuildPermission.ManageChannels)]
[Group("auto-channels", "Configure and manage the auto-channels.")]
public class AutoChannelModule : BaseInteractionModule<AutoChannelModule>
{
    [SlashCommand("info", "Display information on the use of auto-channels.")]
    public Task InfoCommand() => RespondAsync(Localization.Translate("Automation.Channel.Info"), ephemeral: true);

    [SlashCommand("details", "Display information of a specific auto-channel.")]
    public async Task DetailsCommand([Summary(description: "The voice-channel used as an auto-channel.")] IVoiceChannel voiceChannel)
    {
        var autoChannel = await Mediator.Send(new GetAutoChannelRequest(Context.Guild.Id, voiceChannel.Id)).ConfigureAwait(false);
        if (autoChannel == null)
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.Channel"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithTitle(voiceChannel.Name);
        embedBuilder.AddField(Localization.Translate("Automation.Channel.Details.Type"), autoChannel.Type.ToString(), true);
        embedBuilder.AddField(Localization.Translate("Automation.Channel.Details.GenerationName"), autoChannel.GenerationName, true);
        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("list", "Display short information of all the configurations.")]
    public async Task ListCommand()
    {
        var autoChannels = await Mediator.Send(new ListAutoChannelsRequest(Context.Guild.Id)).ConfigureAwait(false);
        if (!autoChannels.Any())
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.Channels"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithTitle(Localization.Translate("Automation.Channel.Details.Title"));
        var groupedAutoChannels = autoChannels.GroupBy(x => x.Type);
        foreach (var groupedAutoChannel in groupedAutoChannels)
        {
            var channels = groupedAutoChannel.Select(x => Context.Guild.GetVoiceChannel(x.Channel))
                .Where(x => x != null)
                .Select(x => x.Mention);
            embedBuilder.AddField(groupedAutoChannel.Key.ToString(), string.Join(Environment.NewLine, channels), true);
        }
        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("assign", "Assign a voice-channel as an auto-channel.")]
    public async Task NewCommand(
        [Summary(description: "The channel to assign.")] IVoiceChannel channel,
        [Summary(description: "What kind of channels are generated.")] AutomationType type,
        [Summary(description: "The name of the generated channels.")] string generationName = null)
    {
        var autoChannel = await Mediator.Send(new GetAutoChannelRequest(Context.Guild.Id, channel.Id)).ConfigureAwait(false);
        if (autoChannel != null)
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.Exists", channel.Name), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await Mediator.Publish(new AddAutoChannelCommand(Context.Guild.Id, channel.Id, type, generationName)).ConfigureAwait(false);
        await RespondAsync(Localization.Translate("Automation.Channel.Created", channel.Mention), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("remove", "Undo the assignment of an auto-channel.")]
    public async Task DeleteCommand([Summary(description: "The channel to unassign.")] IVoiceChannel voiceChannel)
    {
        var autoChannel = await Mediator.Send(new GetAutoChannelRequest(Context.Guild.Id, voiceChannel.Id)).ConfigureAwait(false);
        if (autoChannel == null)
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.Channel"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await Mediator.Publish(new RemoveAutoChannelCommand(Context.Guild.Id, voiceChannel.Id)).ConfigureAwait(false);
        await RespondAsync(Localization.Translate("Automation.Channel.Removed", voiceChannel.Mention), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("edit", "Change a dynamic channel configuration.")]
    public async Task EditCommand(
        [Summary(description: "The channel to modify.")] IVoiceChannel voiceChannel,
        [Summary(description: "The new name for generated channels.")] string generatedName)
    {
        var autoChannel = await Mediator.Send(new GetAutoChannelRequest(Context.Guild.Id, voiceChannel.Id)).ConfigureAwait(false);
        if (autoChannel == null)
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.Channel"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (string.IsNullOrWhiteSpace(generatedName))
        {
            await RespondAsync(Localization.Translate("Automation.Channel.Invalid.GenerationName"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        autoChannel.GenerationName = generatedName;
        await Mediator.Publish(new UpdateAutoChannelCommand(autoChannel)).ConfigureAwait(false);
        await RespondAsync(Localization.Translate("Automation.Channel.Renamed", voiceChannel.Mention, autoChannel.GenerationName), ephemeral: true).ConfigureAwait(false);
    }
}