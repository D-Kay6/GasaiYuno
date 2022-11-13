using Discord;
using Victoria.Player;
using Victoria.WebSocket;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicPlayer : LavaPlayer<PlayableTrack>
{
    /// <summary>
    ///     Voice channel bound to this player.
    /// </summary>
    public new IVoiceChannel VoiceChannel { get; private set; }
    
    /// <inheritdoc />
    public MusicPlayer(WebSocketClient socketClient, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(socketClient, voiceChannel, textChannel)
    {
        VoiceChannel = voiceChannel;
    }

    /// <summary>
    ///     Skips the current track after the specified delay.
    /// </summary>
    /// <param name="delay">If set to null, skips instantly otherwise after the specified value.</param>
    /// <exception cref="InvalidOperationException">Throws when <see cref="PlayerState" /> is invalid.</exception>
    /// <returns>
    ///     The next <see cref="PlayableTrack" />.
    /// </returns>
    public new async Task<(PlayableTrack Skipped, PlayableTrack Current)> SkipAsync(TimeSpan? delay = default)
    {
        if (PlayerState == PlayerState.None)
        {
            throw new InvalidOperationException("Player's current state is set to None. Please make sure Player is connected to a voice channel.");
        }

        PlayableTrack nextTrack = null;
        if (Vueue.Count > 0)
        {
            if (!Vueue.TryDequeue(out nextTrack))
            {
                throw new InvalidOperationException("Can't skip to the next item in queue.");
            }

            if (!nextTrack.GetType().IsAssignableTo(typeof(LavaTrack)))
            {
                throw new InvalidCastException($"Couldn't cast {nextTrack.GetType()} to {typeof(LavaTrack)}.");
            }
        }
        
        await Task.Delay(delay ?? TimeSpan.Zero).ConfigureAwait(false);
        var skippedTrack = Track;
        if (nextTrack != null)
            await PlayAsync(nextTrack).ConfigureAwait(false);
        else
            await StopAsync().ConfigureAwait(false);
        
        return (skippedTrack, nextTrack);
    }

    /// <summary>
    ///     Update the active voice channel.
    /// </summary>
    /// <param name="textChannel"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void SetVoiceChannel(IVoiceChannel textChannel)
    {
        if (textChannel == null)
            throw new ArgumentNullException(nameof(textChannel));
        
        if (VoiceChannel.Id == textChannel.Id)
            return;

        VoiceChannel = textChannel;
    }
}