using Discord.WebSocket;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Services;
using GasaiYuno.Interface.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace GasaiYuno.Discord.Handlers
{
    public class MusicHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly LavaNode _lavaNode;
        private readonly ServerService _serverService;
        private readonly ILocalization _localization;
        private readonly ILogger<MusicHandler> _logger;

        public MusicHandler(DiscordShardedClient client, LavaNode lavaNode, ServerService serverService, ILocalization localization, ILogger<MusicHandler> logger)
        {
            _client = client;
            _lavaNode = lavaNode;
            _serverService = serverService;
            _localization = localization;
            _logger = logger;
        }

        public async Task Ready()
        {
            _client.UserVoiceStateUpdated += ChangeVoiceChannelAsync;
            _lavaNode.OnTrackStart += TrackStartAsync;
            _lavaNode.OnTrackEnd += TrackEndAsync;
            _lavaNode.OnTrackStuck += TrackStuckAsync;
            _lavaNode.OnTrackException += TrackExceptionAsync;
            
            await _lavaNode.ConnectAsync().ConfigureAwait(false);
        }

        private async Task ChangeVoiceChannelAsync(SocketUser user, SocketVoiceState leaveState, SocketVoiceState joinState)
        {
            var voiceChannel = leaveState.VoiceChannel;
            if (voiceChannel == null) return;

            try
            {
                if (!_lavaNode.TryGetPlayer(voiceChannel.Guild, out var player)) return;
                if (voiceChannel.Users.Count != 1) return;
                if (!voiceChannel.Users.First().Id.Equals(_client.CurrentUser.Id)) return;

                var server = await _serverService.Load(voiceChannel.Guild).ConfigureAwait(false);
                var translation = _localization.GetTranslation(server.Language?.Name);
                if (player.TextChannel != null)
                    await player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Channel.Stop")).ConfigureAwait(false);

                await player.StopAsync().ConfigureAwait(false);
                await _lavaNode.LeaveAsync(voiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to handle voice channel change for music handler.");
            }
        }

        private async Task TrackStartAsync(TrackStartEventArg<LavaPlayer> e)
        {
            var server = await _serverService.Load(e.Player.VoiceChannel.Guild).ConfigureAwait(false);
            var translation = _localization.GetTranslation(server.Language?.Name);

            if (e.Player.Track is not PlayableTrack track) return;
            await track.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Player.Playing", track.Title, track.Requester.Nickname())).ConfigureAwait(false);
        }

        private async Task TrackEndAsync(TrackEndEventArg<LavaPlayer> e)
        {
            switch (e.Reason)
            {
                case TrackEndReason.Finished:
                    await PlayNextAsync(e.Player).ConfigureAwait(false);
                    break;
                case TrackEndReason.LoadFailed:
                    var server = await _serverService.Load(e.Player.VoiceChannel.Guild).ConfigureAwait(false);
                    var translation = _localization.GetTranslation(server.Language?.Name);
                    if (e.Player.TextChannel != null)
                        await e.Player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);

                    await PlayNextAsync(e.Player).ConfigureAwait(false);
                    break;
            }
        }

        private async Task TrackStuckAsync(TrackStuckEventArg<LavaPlayer> e)
        {
            _logger.LogError("PlayableTrack {PlayableTrack} got stuck. Time: {Duration} Player {Player}", e.Track, e.Threshold, e.Player);
            await PlayNextAsync(e.Player).ConfigureAwait(false);
        }

        private async Task TrackExceptionAsync(TrackExceptionEventArg<LavaPlayer> e)
        {
            _logger.LogError("Could not play track {PlayableTrack}. Reason: {Message}. Player {Player}", e.Track, e.Exception, e.Player);

            var server = await _serverService.Load(e.Player.VoiceChannel.Guild).ConfigureAwait(false);
            var translation = _localization.GetTranslation(server.Language?.Name);
            if (e.Player.TextChannel != null)
                await e.Player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Song.Exception", e.Track.Title)).ConfigureAwait(false);

            await PlayNextAsync(e.Player).ConfigureAwait(false);
        }

        private async Task PlayNextAsync(LavaPlayer player)
        {
            if (!player.Vueue.TryDequeue(out var lavaTrack) || lavaTrack is not PlayableTrack track)
            {
                if (player.Vueue.Count != 0)
                {
                    _logger.LogError("Unable to get item from player {player} queue {queue}", player, player.Vueue);
                    return;
                }

                await player.StopAsync().ConfigureAwait(false);
                await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
                return;
            }

            await player.PlayAsync(track).ConfigureAwait(false);
        }
    }
}