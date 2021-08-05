using Discord;
using Discord.Rest;
using Discord.WebSocket;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public class DynamicChannelHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork<IDynamicChannelRepository>> _unitOfWork;
        private readonly ILogger<DynamicChannelHandler> _logger;

        private readonly List<ulong> _channelCache;

        public DynamicChannelHandler(DiscordShardedClient client, Func<IUnitOfWork<IDynamicChannelRepository>> unitOfWork, ILogger<DynamicChannelHandler> logger)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _logger = logger;

            _channelCache = new List<ulong>();
        }

        public Task Ready()
        {
            _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
            _client.ChannelDestroyed += ChannelDestroyed;
            return Task.CompletedTask;
        }

        private async Task ChannelDestroyed(SocketChannel channel)
        {
            if (channel is not SocketVoiceChannel voiceChannel) return;
            _channelCache.Remove(voiceChannel.Id);

            var repository = _unitOfWork();
            var dynamicChannels = await repository.DataSet.ListAsync(voiceChannel.Guild.Id).ConfigureAwait(false);
            await repository.BeginAsync().ConfigureAwait(false);
            foreach (var dynamicChannel in dynamicChannels)
            {
                var update = false;
                if (dynamicChannel.Channels.Contains(voiceChannel.Id))
                {
                    dynamicChannel.Channels.Remove(channel.Id);
                    update = true;
                }

                if (dynamicChannel.GeneratedChannels.Contains(voiceChannel.Id))
                {
                    dynamicChannel.GeneratedChannels.Remove(channel.Id);
                    update = true;
                }

                if (update)
                {
                    repository.DataSet.Update(dynamicChannel);
                }
            }

            await repository.SaveAsync().ConfigureAwait(false);
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
                await LeaveChannelAsync(state1.VoiceChannel, guildUser).ConfigureAwait(false);
                await JoinChannelAsync(state2.VoiceChannel, guildUser).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to handle UserVoiceStateUpdatedAsync for user {User}. From {State1}, to {State2}", user, state1, state2);
            }
        }

        private async Task LeaveChannelAsync(SocketVoiceChannel channel, SocketGuildUser user)
        {
            if (channel == null) return;
            try
            {
                if (channel.Users.Count > 0) return;

                var repository = _unitOfWork();
                var dynamicChannels = await repository.DataSet.ListAsync(channel.Guild.Id, AutomationType.Temporary).ConfigureAwait(false);

                while (_channelCache.Contains(channel.Id)) await Task.Delay(100).ConfigureAwait(false);
                var dynamicChannel = dynamicChannels.FirstOrDefault(x => x.GeneratedChannels.Contains(channel.Id));
                if (dynamicChannel == null || channel.Guild.GetChannel(channel.Id) == null) return;
                try
                {
                    await channel.DeleteAsync().ConfigureAwait(false);
                    dynamicChannel.GeneratedChannels.Remove(channel.Id);

                    await repository.BeginAsync().ConfigureAwait(false);
                    repository.DataSet.Update(dynamicChannel);
                    await repository.SaveAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Deleting a dynamic channel failed unexpectedly. {Channel}, {Server}.", channel.Guild, channel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Leaving a channel failed unexpectedly. {Channel}, {Server}.", channel.Guild, channel);
            }
        }

        private async Task JoinChannelAsync(SocketVoiceChannel channel, SocketGuildUser user)
        {
            if (channel == null) return;

            var repository = _unitOfWork();
            
            RestVoiceChannel newChannel = null;
            DynamicChannel dynamicChannel = null;
            try
            {
                var dynamicChannels = await repository.DataSet.ListAsync(channel.Guild.Id).ConfigureAwait(false);
                dynamicChannel = dynamicChannels.FirstOrDefault(x => x.Channels.Contains(channel.Id));
                if (dynamicChannel == null) return;

                newChannel = await DuplicateChannelAsync(channel, user, dynamicChannel.GenerationName).ConfigureAwait(false);
                if (dynamicChannel.Type == AutomationType.Temporary)
                {
                    dynamicChannel.GeneratedChannels.Add(newChannel.Id);

                    await repository.BeginAsync().ConfigureAwait(false);
                    repository.DataSet.Update(dynamicChannel);
                    await repository.SaveAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to handle user {User} joining the channel {Channel}. Found data: {DynamicChannel}", user, channel, dynamicChannel);

                if (newChannel != null)
                {
                    try
                    {
                        await newChannel.DeleteAsync().ConfigureAwait(false);
                        dynamicChannel.Channels.Remove(newChannel.Id);

                        await repository.BeginAsync().ConfigureAwait(false);
                        repository.DataSet.Update(dynamicChannel);
                        await repository.SaveAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove generated channel {NewChannel} after crash JoinChannelAsync.", newChannel);
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
            name = name.Replace("{user}", user.Nickname().ToPossessive());
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
                _logger.LogError(e, "Failed to move user {User} to newly generated channel {Channel}.", user, newChannel);
            }

            foreach (var p in channel.PermissionOverwrites)
                await newChannel.AddPermissionOverwriteAsync(channel.Guild.GetRole(p.TargetId), p.Permissions).ConfigureAwait(false);

            await newChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow)).ConfigureAwait(false);
            return newChannel;
        }
    }
}