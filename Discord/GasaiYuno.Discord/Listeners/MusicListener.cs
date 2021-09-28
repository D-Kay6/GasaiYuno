using Discord.WebSocket;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Discord.Mediator.Requests;
using GasaiYuno.Discord.Models;
using GasaiYuno.Interface.Localization;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace GasaiYuno.Discord.Listeners
{
    internal class MusicListener
    {
        private readonly DiscordShardedClient _client;
        private readonly LavaNode _lavaNode;
        private readonly IMediator _mediator;
        private readonly ILocalization _localization;
        private readonly ILogger<MusicListener> _logger;

        public MusicListener(Connection connection, LavaNode lavaNode, IMediator mediator, ILocalization localization, ILogger<MusicListener> logger)
        {
            _client = connection.Client;
            _lavaNode = lavaNode;
            _mediator = mediator;
            _localization = localization;
            _logger = logger;

            connection.Ready += OnReady;
            _client.LoggedOut += OnLoggedOut;
        }

        private async Task OnReady()
        {
            _client.UserVoiceStateUpdated += ChangeVoiceChannelAsync;
            _lavaNode.OnTrackStart += TrackStartAsync;
            _lavaNode.OnTrackEnd += TrackEndAsync;
            _lavaNode.OnTrackStuck += TrackStuckAsync;
            _lavaNode.OnTrackException += TrackExceptionAsync;
            
            await _lavaNode.ConnectAsync().ConfigureAwait(false);
        }

        private async Task OnLoggedOut()
        {
            await _lavaNode.DisconnectAsync().ConfigureAwait(false);
            await _lavaNode.DisposeAsync().ConfigureAwait(false);
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
                
                var translation = await _mediator.Send(new GetTranslationRequest(voiceChannel.Guild));
                if (player.TextChannel != null)
                    await player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Channel.Stop")).ConfigureAwait(false);

                await player.StopAsync().ConfigureAwait(false);
                await _lavaNode.LeaveAsync(voiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to handle voice channel change. {@user}, {@leaveState}, {@joinState}", user, leaveState, joinState);
            }
        }

        private async Task TrackStartAsync(TrackStartEventArg<LavaPlayer> e)
        {
            var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild));
            await e.Player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Track.Current", e.Track.Title, e.Track.Duration)).ConfigureAwait(false);
        }

        private async Task TrackEndAsync(TrackEndEventArg<LavaPlayer> e)
        {
            switch (e.Reason)
            {
                case TrackEndReason.Replaced:
                    break;
                case TrackEndReason.Finished:
                    await PlayNextAsync(e.Player).ConfigureAwait(false);
                    break;
                case TrackEndReason.LoadFailed:
                    var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild));
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
            
            var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild));
            if (e.Player.TextChannel != null)
            {
                var message = translation.Message("Entertainment.Music.Track.Exception.Message", e.Track.Title);
                if (!string.IsNullOrEmpty(e.Exception)) message += Environment.NewLine + translation.Message("Entertainment.Music.Track.Exception.Reason", e.Exception);
                await e.Player.TextChannel.SendMessageAsync(message).ConfigureAwait(false);
            }

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
            
            player.SetTextChannel(track.TextChannel);
            await player.PlayAsync(track).ConfigureAwait(false);
        }
    }
}