using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Music.Models.Audio;
using Lavalink4NET;
using Lavalink4NET.Events;
using Lavalink4NET.Player;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Music.Listeners;

internal class MusicListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IAudioService _audioService;
    private readonly IMediator _mediator;
    private readonly ILogger<MusicListener> _logger;

    public MusicListener(DiscordShardedClient client, IAudioService audioService, IMediator mediator, ILogger<MusicListener> logger)
    {
        _client = client;
        _audioService = audioService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Start()
    {
        _client.LoggedOut += OnLoggedOut;
        _client.UserVoiceStateUpdated += ChangeVoiceChannelAsync;
        _audioService.TrackStarted += TrackStartedAsync;
        _audioService.TrackEnd += TrackEndAsync;
        _audioService.TrackStuck += TrackStuckAsync;
        _audioService.TrackException += TrackExceptionAsync;

        await _audioService.InitializeAsync().ConfigureAwait(false);
    }

    private Task OnLoggedOut()
    {
        _audioService.Dispose();
        return Task.CompletedTask;
    }

    private async Task ChangeVoiceChannelAsync(SocketUser user, SocketVoiceState leaveState, SocketVoiceState joinState)
    {
        var voiceChannel = leaveState.VoiceChannel;
        if (voiceChannel == null) return;

        try
        {
            var player = _audioService.GetPlayer<MusicPlayer>(voiceChannel.Guild.Id);
            if (player == null) return;
            if (voiceChannel.ConnectedUsers.Count != 1) return;
            if (!voiceChannel.ConnectedUsers.First().Id.Equals(_client.CurrentUser.Id)) return;

            var translation = await _mediator.Send(new GetTranslationRequest(voiceChannel.Guild.Id)).ConfigureAwait(false);
            if (player.TextChannel != null)
                await player.TextChannel.SendMessageAsync(translation.Translate("Entertainment.Music.Channel.Stop")).ConfigureAwait(false);

            await player.StopAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to handle voice channel change. {@User}, {@LeaveState}, {@JoinState}", user, leaveState, joinState);
        }
    }

    private async Task TrackStartedAsync(object sender, TrackStartedEventArgs eventArgs)
    {
        if (eventArgs.Player is not MusicPlayer player)
            return;

        var translation = await _mediator.Send(new GetTranslationRequest(eventArgs.Player.GuildId)).ConfigureAwait(false);
        var embedBuilder = new EmbedBuilder()
            .WithTitle(translation.Translate("Entertainment.Music.Track.Current"))
            .WithDescription(translation.Translate("Entertainment.Music.Track.Item", eventArgs.Player.CurrentTrack!.Title.Trim(), eventArgs.Player.CurrentTrack!.Duration));

        await player.TextChannel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
    }

    private async Task TrackEndAsync(object sender, TrackEndEventArgs eventArgs)
    {
        if (eventArgs.Player is not MusicPlayer player)
            return;

        if (eventArgs.Reason == TrackEndReason.LoadFailed)
        {
            var translation = await _mediator.Send(new GetTranslationRequest(player.GuildId)).ConfigureAwait(false);
            if (player.TextChannel != null)
                await player.TextChannel.SendMessageAsync(translation.Translate("Entertainment.Music.Exception")).ConfigureAwait(false);
        }

        if (player.Queue.Count == 0)
            await player.StopAsync().ConfigureAwait(false);
    }

    private async Task TrackStuckAsync(object sender, TrackStuckEventArgs eventArgs)
    {
        _logger.LogError("PlayableTrack {@PlayableTrack} got stuck. Time: {Duration} Player {@Player}", eventArgs.Player.CurrentTrack, eventArgs.Threshold, eventArgs.Player);
        if (eventArgs.Player is not MusicPlayer player)
            return;

        await PlayNextAsync(player).ConfigureAwait(false);
    }

    private async Task TrackExceptionAsync(object sender, TrackExceptionEventArgs eventArgs)
    {
        _logger.LogError("Could not play track {@PlayableTrack}. Error: {@Exception}. Player {@Player}", eventArgs.Player.CurrentTrack, eventArgs.ErrorMessage, eventArgs.Player);
        if (eventArgs.Player is not MusicPlayer player)
            return;

        var translation = await _mediator.Send(new GetTranslationRequest(eventArgs.Player.GuildId)).ConfigureAwait(false);
        if (player.TextChannel != null)
        {
            var message = translation.Translate("Entertainment.Music.Track.Exception.Message", player.CurrentTrack!.Title);
            if (!string.IsNullOrEmpty(eventArgs.ErrorMessage))
                message += Environment.NewLine + translation.Translate("Entertainment.Music.Track.Exception.Reason", eventArgs.ErrorMessage);

            await player.TextChannel.SendMessageAsync(message).ConfigureAwait(false);
        }

        await PlayNextAsync(player).ConfigureAwait(false);
    }

    private async Task PlayNextAsync(MusicPlayer player)
    {
        try
        {
            await player.PlayNextAsync().ConfigureAwait(false);
        }
        catch (ObjectDisposedException e)
        {
            _logger.LogError(e, "Player {@Player} got disposed while trying to play the next song", player);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogError(e, "Player {@Player} got canceled while trying to play the next song", player);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong trying to play the next song");
            await PlayNextAsync(player).ConfigureAwait(false);
        }
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}