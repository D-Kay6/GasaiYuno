using Discord;
using System;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicPlayer : LavaPlayer
{
    /// <inheritdoc />
    public MusicPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
    }

    /// <summary>
    ///     Skips the current track after the specified delay.
    /// </summary>
    /// <param name="delay">If set to null, skips instantly otherwise after the specified value.</param>
    /// <returns>
    ///     The next <see cref="LavaTrack" />.
    /// </returns>
    public new async Task<(LavaTrack Skipped, LavaTrack Current)> SkipAsync(TimeSpan? delay = default)
    {
        if (PlayerState == PlayerState.None)
        {
            throw new InvalidOperationException("Player's current state is set to None. Please make sure Player is connected to a voice channel.");
        }

        LavaTrack nextTrack = null;
        if (Queue.Count > 0)
        {
            if (!Queue.TryDequeue(out nextTrack))
            {
                throw new InvalidOperationException("Can't skip to the next item in queue.");
            }

            if (!nextTrack.GetType().IsAssignableTo(typeof(LavaTrack)))
            {
                throw new InvalidCastException($"Couldn't cast {nextTrack.GetType()} to {typeof(LavaTrack)}.");
            }
        }
        
        await Task.Delay(delay ?? TimeSpan.Zero);
        var skippedTrack = Track;
        if (nextTrack != null)
            await PlayAsync(nextTrack).ConfigureAwait(false);
        else
            await StopAsync().ConfigureAwait(false);
        
        return (skippedTrack, nextTrack);
    }
}