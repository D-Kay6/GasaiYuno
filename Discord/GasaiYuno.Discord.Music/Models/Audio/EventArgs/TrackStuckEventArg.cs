using System;

namespace GasaiYuno.Discord.Music.Models.Audio.EventArgs
{
    /// <summary>
    /// Information about track that got stuck.
    /// </summary>
    public readonly struct TrackStuckEventArgs
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
        /// How long track was stuck for.
        /// </summary>
        public TimeSpan Threshold { get; }

        internal TrackStuckEventArgs(MusicPlayer player, PlayableTrack track, TimeSpan threshold)
        {
            Player = player;
            Track = track;
            Threshold = threshold;
        }
    }
}