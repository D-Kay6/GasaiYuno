using Discord.WebSocket;
using GasaiYuno.Discord.Music.Models.Audio.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Victoria;

namespace GasaiYuno.Discord.Music.Models.Audio
{
    public class MusicNode : MusicNode<MusicPlayer>
    {
        /// <inheritdoc />
        public MusicNode(DiscordShardedClient shardedClient, LavaConfig config) : base(shardedClient.Shards.First(), config) { }
    }

    public class MusicNode<TPlayer> : LavaNode<TPlayer> where TPlayer : MusicPlayer
    {
        /// <summary>
        /// Fires when a track playback has started.
        /// </summary>
        public new event Func<TrackStartEventArgs, Task> OnTrackStarted;

        /// <summary>
        /// Fires when a track playback has finished.
        /// </summary>
        public new event Func<TrackEndedEventArgs, Task> OnTrackEnded;

        /// <summary>
        /// Fires when a track has thrown an exception.
        /// </summary>
        public new event Func<TrackExceptionEventArgs, Task> OnTrackException;

        /// <summary>
        /// Fires when a track got stuck.
        /// </summary>
        public new event Func<TrackStuckEventArgs, Task> OnTrackStuck;
    
        /// <inheritdoc />
        public MusicNode(DiscordSocketClient socketClient, LavaConfig config) : base(socketClient, config)
        {
            base.OnTrackStarted += TrackStarted;
            base.OnTrackEnded += TrackEnded;
            base.OnTrackException += TrackException;
            base.OnTrackStuck += TrackStuck;
        }

        private Task TrackStarted(Victoria.EventArgs.TrackStartEventArgs arg)
        {
            if (arg.Player is not TPlayer player) 
                throw new InvalidOperationException("The player is not of the correct type.");

            if (!player.TryGetCachedTrack(arg.Track.Id, out var track))
                throw new InvalidOperationException("The track does not exist in the cache.");

            return OnTrackStarted?.Invoke(new TrackStartEventArgs(player, track));
        }

        private Task TrackEnded(Victoria.EventArgs.TrackEndedEventArgs arg)
        {
            if (arg.Player is not TPlayer player)
                throw new InvalidOperationException("The player is not of the correct type.");

            if (!player.TryGetCachedTrack(arg.Track.Id, out var track))
                throw new InvalidOperationException("The track does not exist in the cache.");

            player.RemoveCachedTrack(track.Id);
            return OnTrackEnded?.Invoke(new TrackEndedEventArgs(player, track, arg.Reason));
        }

        private Task TrackException(Victoria.EventArgs.TrackExceptionEventArgs arg)
        {
            if (arg.Player is not TPlayer player)
                throw new InvalidOperationException("The player is not of the correct type.");

            if (!player.TryGetCachedTrack(arg.Track.Id, out var track))
                throw new InvalidOperationException("The track does not exist in the cache.");

            player.RemoveCachedTrack(track.Id);
            return OnTrackException?.Invoke(new TrackExceptionEventArgs(player, track, arg.Exception));
        }

        private Task TrackStuck(Victoria.EventArgs.TrackStuckEventArgs arg)
        {
            if (arg.Player is not TPlayer player)
                throw new InvalidOperationException("The player is not of the correct type.");

            if (!player.TryGetCachedTrack(arg.Track.Id, out var track))
                throw new InvalidOperationException("The track does not exist in the cache.");

            player.RemoveCachedTrack(track.Id);
            return OnTrackStuck?.Invoke(new TrackStuckEventArgs(player, track, arg.Threshold));
        }
    }
}