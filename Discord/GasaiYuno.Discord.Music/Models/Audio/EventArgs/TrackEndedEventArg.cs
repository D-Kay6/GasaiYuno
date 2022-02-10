using Victoria.Enums;

namespace GasaiYuno.Discord.Music.Models.Audio.EventArgs
{
    /// <summary>
    /// Information about track that ended.
    /// </summary>
    public readonly struct TrackEndedEventArgs
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
        /// Reason for track ending.
        /// </summary>
        public TrackEndReason Reason { get; }

        internal TrackEndedEventArgs(MusicPlayer player, PlayableTrack track, TrackEndReason reason)
        {
            Player = player;
            Track = track;
            Reason = reason;
        }
    }
}