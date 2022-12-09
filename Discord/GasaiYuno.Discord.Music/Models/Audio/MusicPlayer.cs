using Discord;
using Lavalink4NET.Events;
using Lavalink4NET.Player;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicPlayer : LavalinkPlayer
{
    private readonly bool _disconnectOnStop;

    /// <summary>Gets or sets a value indicating whether the current playing track should be looped.</summary>
    public bool IsLooping { get; set; }

    /// <summary>Gets the text channel the player is sending messages to.</summary>
    public ITextChannel TextChannel { get; private set; }

    /// <summary>Gets the track queue.</summary>
    public Queue<LavalinkTrack> Queue { get; }

    /// <summary>Initializes a new instance of the <see cref="T:GasaiYuno.Discord.Music.Models.Audio.MusicPlayer" /> class.</summary>
    public MusicPlayer()
    {
        Queue = new Queue<LavalinkTrack>();
        _disconnectOnStop = DisconnectOnStop;
        DisconnectOnStop = false;
    }

    /// <summary>Asynchronously triggered when a track ends.</summary>
    /// <param name="eventArgs">The track event arguments</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task OnTrackEndAsync(TrackEndEventArgs eventArgs)
    {
        if (eventArgs.MayStartNext)
            return SkipAsync();

        return _disconnectOnStop ? DisconnectAsync() : Task.CompletedTask;
    }

    /// <summary>Plays the specified <paramref name="track" /> asynchronously.</summary>
    /// <param name="track">the track to play</param>
    /// <param name="startTime">The track start position</param>
    /// <param name="endTime">The track end position</param>
    /// <param name="noReplace">A value indicating whether the track play should be ignored if the same track is currently playing</param>
    /// <returns>A task that represents the asynchronous operation <para>the position in the track queue ( <c>0</c> = now playing)</para></returns>
    public override Task<int> PlayAsync(LavalinkTrack track, TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
    {
        if (track.Context is TrackContext context)
            TextChannel = context.TextChannel;

        return PlayAsync(track, true, startTime, endTime, noReplace);
    }

    /// <summary>Plays the specified <paramref name="track" /> asynchronously.</summary>
    /// <param name="track">The track to play</param>
    /// <param name="enqueue">A value indicating whether the track should be enqueued in the track queue</param>
    /// <param name="startTime">The track start position</param>
    /// <param name="endTime">The track end position</param>
    /// <param name="noReplace">A value indicating whether the track play should be ignored if the same track is currently playing</param>
    /// <returns>A task that represents the asynchronous operation <para>the position in the track queue (<c>0</c> = now playing)</para></returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is destroyed</exception>
    public virtual async Task<int> PlayAsync(LavalinkTrack track, bool enqueue, TimeSpan? startTime = null, TimeSpan? endTime = null, bool noReplace = false)
    {
        EnsureNotDestroyed();
        EnsureConnected();
        if (enqueue && (Queue.Count > 0 || State is PlayerState.Playing or PlayerState.Paused))
        {
            Queue.Add(track);
            return Queue.Count;
        }

        if (track.Context is TrackContext context)
            TextChannel = context.TextChannel;

        await base.PlayAsync(track, startTime, endTime, noReplace).ConfigureAwait(false);
        return 0;
    }

    /// <summary>Plays the specified <paramref name="track" /> at the top of the queue asynchronously.</summary>
    /// <param name="track">The track to play</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is destroyed</exception>
    public virtual async Task PlayTopAsync(LavalinkTrack track)
    {
        EnsureNotDestroyed();
        if (track == null)
            throw new ArgumentNullException(nameof (track));

        if (State == PlayerState.NotPlaying)
        {
            await PlayAsync(track, false);
        }
        else
            Queue.AddStart(track);
    }

    /// <summary>Pushes a track between the current asynchronously.</summary>
    /// <param name="track">The track to push between the current</param>
    /// <param name="push">A value indicating whether the track should only played when a track is playing currently.</param>
    /// <remarks>
    ///     This will stop playing the current track and start playing the specified <paramref name="track" />.
    ///     After the track is finished the track will restart at the stopped position.
    ///     This can be useful for example soundboards (playing an air-horn or something).
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is a value indicating whether the track was pushed between the current
    ///     (<see langword="true" />) or the specified track was simply started (<see langword="false" />), because there is no track playing.
    /// </returns>
    public virtual async Task<bool> PushTrackAsync(LavalinkTrack track, bool push = false)
    {
        if (State == PlayerState.NotPlaying)
        {
            if (push)
                return false;

            await PlayAsync(track, false);
            return false;
        }

        var currentTrack = CurrentTrack!.WithPosition(Position.Position);
        Queue.Add(currentTrack);
        await PlayAsync(track, false);
        return true;
    }

    /// <summary>Skips the current track asynchronously.</summary>
    /// <param name="count">The number of tracks to skip</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is destroyed</exception>
    public virtual Task SkipAsync(int count = 1)
    {
        if (count <= 0)
            return Task.CompletedTask;

        EnsureNotDestroyed();
        EnsureConnected();
        if (IsLooping && CurrentTrack != null)
            return PlayAsync(CurrentTrack, false);

        if (Queue.IsEmpty)
            return StopAsync(_disconnectOnStop);

        LavalinkTrack track = null;
        while (count-- > 0 || track == null)
        {
            if (Queue.Count < 1)
                return DisconnectAsync();

            track = Queue.Pop();
        }

        return PlayAsync(track, false);
    }

    /// <summary>Stops playing the current track asynchronously.</summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is not connected to a voice channel</exception>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is destroyed</exception>
    public virtual Task StopAsync()
    {
        return StopAsync(_disconnectOnStop);
    }

    /// <summary>Stops playing the current track asynchronously.</summary>
    /// <param name="disconnect">A value indicating whether the connection to the voice server should be closed</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.InvalidOperationException">Thrown if the player is destroyed</exception>
    public override Task StopAsync(bool disconnect = false)
    {
        Queue.Clear();
        return base.StopAsync(disconnect);
    }
}