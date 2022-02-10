using Victoria;

namespace GasaiYuno.Discord.Music.Models.Audio.EventArgs
{
    /// <summary>
    /// Information about track that threw an exception.
    /// </summary>
    public readonly struct TrackExceptionEventArgs
    {
        /// <summary>
        /// Player for which this event fired.
        /// </summary>
        public MusicPlayer Player { get; }

        /// <summary>
        /// Track sent by Lavalink.
        /// </summary>
        public PlayableTrack Track { get; }

        /// <summary>
        /// Reason for why track threw an exception.
        /// </summary>
        public LavaException Exception { get; }

        internal TrackExceptionEventArgs(MusicPlayer player, PlayableTrack track, LavaException exception)
        {
            Player = player;
            Track = track;
            Exception = exception;
        }
    }
}