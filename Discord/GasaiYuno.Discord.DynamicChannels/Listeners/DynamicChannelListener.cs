using Discord;
using Discord.Rest;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Mediator.Commands;
using GasaiYuno.Discord.DynamicChannels.Mediator.Requests;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.DynamicChannels.Listeners;

internal class DynamicChannelListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly ILogger<DynamicChannelListener> _logger;
    private readonly List<ulong> _channelCache;

    public DynamicChannelListener(DiscordShardedClient client, IMediator mediator, ILogger<DynamicChannelListener> logger)
    {
        _client = client;
        _mediator = mediator;
        _logger = logger;
        _channelCache = new List<ulong>();
    }

    public Task Start()
    {
        _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
        _client.ChannelDestroyed += ChannelDestroyed;
        return Task.CompletedTask;
    }

    private async Task ChannelDestroyed(SocketChannel channel)
    {
        if (channel is not SocketVoiceChannel voiceChannel) return;
        _channelCache.Remove(voiceChannel.Id);

        var dynamicChannels = await _mediator.Send(new ListDynamicChannelsRequest(voiceChannel.Guild.Id)).ConfigureAwait(false);
        foreach (var dynamicChannel in dynamicChannels)
        {
            var update = dynamicChannel.Channels.Remove(channel.Id) || dynamicChannel.GeneratedChannels.Remove(channel.Id);
            if (update)
                await _mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
        }
    }

    private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
    {
#if DEBUG
        if (!user.Id.Equals(255453041531158538)) return;
#endif
        if (state1.VoiceChannel == state2.VoiceChannel) return;

        try
        {
            var guildUser = user as SocketGuildUser;
            await LeaveChannelAsync(state1.VoiceChannel).ConfigureAwait(false);
            await JoinChannelAsync(state2.VoiceChannel, guildUser).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to handle UserVoiceStateUpdatedAsync for user {@User}. From {@State1}, to {@State2}", user, state1, state2);
        }
    }

    private async Task LeaveChannelAsync(SocketVoiceChannel channel)
    {
        if (channel == null) return;
        try
        {
            if (channel.Users.Count > 0) return;
            while (_channelCache.Contains(channel.Id)) 
                await Task.Delay(100).ConfigureAwait(false);

            var dynamicChannels = await _mediator.Send(new ListDynamicChannelsRequest(channel.Guild.Id, AutomationType.Temporary)).ConfigureAwait(false);
            var dynamicChannel = dynamicChannels.FirstOrDefault(x => x.GeneratedChannels.Contains(channel.Id));
            if (dynamicChannel == null || channel.Guild.GetChannel(channel.Id) == null) return;
            try
            {
                await channel.DeleteAsync().ConfigureAwait(false);
                dynamicChannel.GeneratedChannels.Remove(channel.Id);
                await _mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Deleting a dynamic channel failed unexpectedly. {@Channel}, {@Server}", channel.Guild, channel);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Leaving a channel failed unexpectedly. {@Channel}, {@Server}", channel.Guild, channel);
        }
    }

    private async Task JoinChannelAsync(SocketVoiceChannel channel, SocketGuildUser user)
    {
        if (channel == null) return;

        RestVoiceChannel newChannel = null;
        DynamicChannel dynamicChannel = null;
        try
        {
            var dynamicChannels = await _mediator.Send(new ListDynamicChannelsRequest(channel.Guild.Id)).ConfigureAwait(false);
            dynamicChannel = dynamicChannels.FirstOrDefault(x => x.Channels.Contains(channel.Id));
            if (dynamicChannel == null) return;

            newChannel = await DuplicateChannelAsync(channel, user, dynamicChannel.GenerationName).ConfigureAwait(false);
            if (dynamicChannel.Type == AutomationType.Temporary)
            {
                dynamicChannel.GeneratedChannels.Add(newChannel.Id);
                await _mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to handle user {@User} joining the channel {@Channel}. Found data: {@DynamicChannel}", user, channel, dynamicChannel);

            if (newChannel != null)
            {
                try
                {
                    await newChannel.DeleteAsync().ConfigureAwait(false);
                    dynamicChannel.Channels.Remove(newChannel.Id);
                    await _mediator.Publish(new UpdateDynamicChannelCommand(dynamicChannel)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to remove generated channel {@NewChannel} after crash JoinChannelAsync", newChannel);
                }
            }
        }
        finally
        {
            if (newChannel != null) _channelCache.Remove(newChannel.Id);
        }
    }

    private async Task<RestVoiceChannel> DuplicateChannelAsync(SocketVoiceChannel channel, SocketGuildUser user, string name)
    {
        name = name.Replace("[user]", user.ToPossessive());
        var newChannel = await channel.Guild.CreateVoiceChannelAsync(name, p =>
        {
            p.Bitrate = channel.Bitrate;
            p.CategoryId = channel.CategoryId;
            p.UserLimit = channel.UserLimit;
        }).ConfigureAwait(false);
        _channelCache.Add(newChannel.Id);

        try
        {
            if (user.VoiceChannel != null)
                await user.ModifyAsync(u => u.Channel = newChannel).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to move user {@User} to newly generated channel {@Channel}", user, newChannel);
        }
            
        foreach (var permissionOverwrite in channel.PermissionOverwrites)
        {
            switch (permissionOverwrite.TargetType)
            {
                case PermissionTarget.Role:
                    var permissionRole = channel.Guild.GetRole(permissionOverwrite.TargetId);
                    if (permissionRole != null)
                        await newChannel.AddPermissionOverwriteAsync(permissionRole, permissionOverwrite.Permissions).ConfigureAwait(false);
                    break;
                case PermissionTarget.User:
                    var permissionUser = channel.Guild.GetUser(permissionOverwrite.TargetId);
                    if (permissionUser != null)
                        await newChannel.AddPermissionOverwriteAsync(permissionUser, permissionOverwrite.Permissions).ConfigureAwait(false);
                    break;
            }
        }

        await newChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow)).ConfigureAwait(false);
        return newChannel;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}