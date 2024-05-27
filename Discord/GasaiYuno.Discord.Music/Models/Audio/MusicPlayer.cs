using Discord;
using Lavalink4NET.Player;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicPlayer : LavalinkPlayer
{
    private TrackContext _currentTrackContext;

    /// <summary>Gets or sets a value indicating whether the current playing track should be looped.</summary>
    public bool IsLooping { get; set; }

    /// <summary>Gets the text channel the player is sending messages to.</summary>
    public ITextChannel TextChannel { get; private set; }

    /// <summary>Gets the requester of the current playing track.</summary>
    public string Requester { get; private set; }

    /// <summary>Gets the track queue.</summary>
    public Queue<TrackContext> Queue { get; }

    /// <summary>Initializes a new instance of the <see cref="T:GasaiYuno.Discord.Music.Models.Audio.MusicPlayer" /> class.</summary>
    public MusicPlayer()
    {
        Queue = new Queue<TrackContext>();
    }

    /// <summary>Plays the specified <paramref name="context" /> asynchronously.</summary>
    /// <param name="context">The context of the track to play</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation <para>the position in the track queue ( <c>0</c> = now playing)</para></returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    /// <exception cref="T:System.ArgumentNullException">Thrown if the specified <paramref name="context" /> is <see langword="null" /></exception>
    public virtual async ValueTask<int> PlayAsync(TrackContext context, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context?.Track, nameof(context));

        Queue.Add(context);
        if (Queue.Count <= 0 && State is not (PlayerState.Playing or PlayerState.Paused))
            await PlayNextAsync(cancellationToken).ConfigureAwait(false);

        return Queue.Count;
    }

    /// <summary>
    ///     Plays the next item in the queue asynchronously.
    ///     If the player is looping, the current track will be played again.
    ///     If the queue is empty, the player will be stopped.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    public virtual async ValueTask PlayNextAsync(CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        if (IsLooping && _currentTrackContext != null)
        {
            await PlayAsync(_currentTrackContext.Track).ConfigureAwait(false);
            return;
        }

        if (Queue.IsEmpty)
        {
            await StopAsync().ConfigureAwait(false);
            return;
        }

        _currentTrackContext = Queue.Pop();
        TextChannel = _currentTrackContext.TextChannel;
        Requester = _currentTrackContext.Requester;
        await PlayAsync(_currentTrackContext.Track).ConfigureAwait(false);
    }

    /// <summary>Plays the specified <paramref name="context" /> at the top of the queue asynchronously.</summary>
    /// <param name="context">The context of the track to play</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    /// <exception cref="T:System.ArgumentNullException">Thrown if the specified <paramref name="context" /> is <see langword="null" /></exception>
    public virtual async ValueTask PlayTopAsync(TrackContext context, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context?.Track, nameof(context));
        Queue.AddStart(context);

        if (State == PlayerState.NotPlaying)
            await PlayNextAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Pushes a track between the current asynchronously.</summary>
    /// <param name="track">The track to push between the current</param>
    /// <param name="push">A value indicating whether the track should only played when a track is playing currently.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <remarks>
    ///     This will stop playing the current track and start playing the specified <paramref name="track" />.
    ///     After the track is finished the track will restart at the stopped position.
    ///     This can be useful for example soundboards (playing an air-horn or something).
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is a value indicating whether the track was pushed between the current
    ///     (<see langword="true" />) or the specified track was simply started (<see langword="false" />), because there is no track playing.
    /// </returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    /// <exception cref="T:System.ArgumentNullException">Thrown if the specified <paramref name="track" /> is <see langword="null" /></exception>
    public virtual async ValueTask<bool> PushTrackAsync(LavalinkTrack track, bool push = false, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(track, nameof(track));
        if (State == PlayerState.NotPlaying)
        {
            if (push)
                return false;

            await PlayAsync(track).ConfigureAwait(false);
            return false;
        }

        var currentTrack = CurrentTrack!.WithPosition(Position.Position);
        Queue.Add(_currentTrackContext with {Track = currentTrack});
        await PlayAsync(track).ConfigureAwait(false);
        return true;
    }

    /// <summary>Skips the current track asynchronously.</summary>
    /// <param name="count">The number of tracks to skip</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    public virtual ValueTask SkipAsync(int count = 1, CancellationToken cancellationToken = default)
    {
        EnsureNotDestroyed();
        if (count <= 0)
            return ValueTask.CompletedTask;

        IsLooping = false;
        if (count > Queue.Count)
            count = Queue.Count;

        Queue.RemoveRange(0, count - 1);
        return PlayNextAsync(cancellationToken);
    }

    /// <summary>Stops playing the current track asynchronously.</summary>
    /// <param name="disconnect">A value indicating whether the connection to the voice server should be closed</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="T:System.ObjectDisposedException">Thrown if the player is destroyed</exception>
    /// <exception cref="T:System.OperationCanceledException">Thrown if the operation was cancelled</exception>
    public override Task StopAsync(bool disconnect = false)
    {
        Requester = null;
        Queue.Clear();
        return base.StopAsync(disconnect);
    }
}