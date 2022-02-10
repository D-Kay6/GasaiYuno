using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Music.Models.Audio;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace GasaiYuno.Discord.Music.Listeners
{
    internal class MusicListener
    {
        private readonly DiscordShardedClient _client;
        private readonly LavaNode _lavaNode;
        private readonly IMediator _mediator;
        private readonly ILogger<MusicListener> _logger;

        public MusicListener(DiscordShardedClient client, LavaNode lavaNode, IMediator mediator, ILogger<MusicListener> logger)
        {
            _client = client;
            _lavaNode = lavaNode;
            _mediator = mediator;
            _logger = logger;
        
        }

        public async Task Start()
        {
            _client.LoggedOut += OnLoggedOut;
            _client.UserVoiceStateUpdated += ChangeVoiceChannelAsync;
            _lavaNode.OnTrackStarted += TrackStartAsync;
            _lavaNode.OnTrackEnded += TrackEndAsync;
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

                var translation = await _mediator.Send(new GetTranslationRequest(voiceChannel.Guild.Id));
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

        private async Task TrackStartAsync(TrackStartEventArgs e)
        {
            var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild.Id));
            var embedBuilder = new EmbedBuilder()
                .WithTitle(translation.Message("Entertainment.Music.Track.Current"))
                .WithDescription(translation.Message("Entertainment.Music.Track.Item", e.Track.Title.Trim(), e.Track.Duration));
            await e.Player.TextChannel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        private async Task TrackEndAsync(TrackEndedEventArgs e)
        {
            switch (e.Reason)
            {
                case TrackEndReason.Replaced:
                    break;
                case TrackEndReason.Finished:
                    await PlayNextAsync(e.Player).ConfigureAwait(false);
                    break;
                case TrackEndReason.LoadFailed:
                    var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild.Id));
                    if (e.Player.TextChannel != null)
                        await e.Player.TextChannel.SendMessageAsync(translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);

                    await PlayNextAsync(e.Player).ConfigureAwait(false);
                    break;
            }
        }

        private async Task TrackStuckAsync(TrackStuckEventArgs e)
        {
            _logger.LogError("PlayableTrack {PlayableTrack} got stuck. Time: {Duration} Player {Player}", e.Track, e.Threshold, e.Player);
            await PlayNextAsync(e.Player).ConfigureAwait(false);
        }

        private async Task TrackExceptionAsync(TrackExceptionEventArgs e)
        {
            _logger.LogError("Could not play track {PlayableTrack}. Reason: {Message}. Player {Player}", e.Track, e.Exception, e.Player);

            var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild.Id));
            if (e.Player.TextChannel != null)
            {
                var message = translation.Message("Entertainment.Music.Track.Exception.Message", e.Track.Title);
                if (!string.IsNullOrEmpty(e.Exception.Message)) message += Environment.NewLine + translation.Message("Entertainment.Music.Track.Exception.Reason", e.Exception);
                await e.Player.TextChannel.SendMessageAsync(message).ConfigureAwait(false);
            }

            await PlayNextAsync(e.Player).ConfigureAwait(false);
        }

        private async Task PlayNextAsync(LavaPlayer player)
        {
            try
            {
                if (!player.Queue.TryDequeue(out var lavaTrack) || lavaTrack is not PlayableTrack track)
                {
                    if (player.Queue.Count != 0)
                    {
                        _logger.LogError("Unable to get item from {@player} {@queue}", player, player.Queue);
                        return;
                    }

                    await player.StopAsync().ConfigureAwait(false);
                    await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
                    return;
                }

                await _lavaNode.MoveChannelAsync(track.TextChannel).ConfigureAwait(false);
                await player.PlayAsync(track).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong trying to play the next song.");
                await PlayNextAsync(player);
            }
        }
    }
}