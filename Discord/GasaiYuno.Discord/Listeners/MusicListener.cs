using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Mediator.Requests;
using GasaiYuno.Discord.Models;
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
        private readonly LavaNode<LavaPlayer<PlayableTrack>, PlayableTrack> _lavaNode;
        private readonly IMediator _mediator;
        private readonly ILogger<MusicListener> _logger;

        public MusicListener(DiscordConnectionClient client, LavaNode<LavaPlayer<PlayableTrack>, PlayableTrack> lavaNode, IMediator mediator, ILogger<MusicListener> logger)
        {
            _client = client;
            _lavaNode = lavaNode;
            _mediator = mediator;
            _logger = logger;

            client.Ready += OnReady;
            client.LoggedOut += OnLoggedOut;
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

        private async Task TrackStartAsync(TrackStartEventArg<LavaPlayer<PlayableTrack>, PlayableTrack> e)
        {
            var translation = await _mediator.Send(new GetTranslationRequest(e.Player.VoiceChannel.Guild));
            var embedBuilder = new EmbedBuilder()
                .WithTitle(translation.Message("Entertainment.Music.Track.Current"))
                .WithDescription(translation.Message("Entertainment.Music.Track.Item", e.Track.Title.Trim(), e.Track.Duration));
            await e.Player.TextChannel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        private async Task TrackEndAsync(TrackEndEventArg<LavaPlayer<PlayableTrack>, PlayableTrack> e)
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

        private async Task TrackStuckAsync(TrackStuckEventArg<LavaPlayer<PlayableTrack>, PlayableTrack> e)
        {
            _logger.LogError("PlayableTrack {PlayableTrack} got stuck. Time: {Duration} Player {Player}", e.Track, e.Threshold, e.Player);
            await PlayNextAsync(e.Player).ConfigureAwait(false);
        }

        private async Task TrackExceptionAsync(TrackExceptionEventArg<LavaPlayer<PlayableTrack>, PlayableTrack> e)
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

        private async Task PlayNextAsync(LavaPlayer<PlayableTrack> player)
        {
            try
            {
                if (!player.Vueue.TryDequeue(out var lavaTrack))
                {
                    if (player.Vueue.Count != 0)
                    {
                        _logger.LogError("Unable to get item from {@player} {@queue}", player, player.Vueue);
                        return;
                    }

                    await player.StopAsync().ConfigureAwait(false);
                    await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
                    return;
                }
                
                player.SetTextChannel(lavaTrack.TextChannel);
                await player.PlayAsync(lavaTrack).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong trying to play the next song.");
                await PlayNextAsync(player);
            }
        }
    }
}