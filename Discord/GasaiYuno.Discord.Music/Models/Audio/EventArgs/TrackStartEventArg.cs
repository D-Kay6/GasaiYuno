namespace GasaiYuno.Discord.Music.Models.Audio.EventArgs
{
    /// <summary>
    /// Information about the track that started.
    /// </summary>
    public readonly struct TrackStartEventArgs
    {
        /// <summary>
        /// Player for which this event fired.
        /// </summary>
        public MusicPlayer Player { get; }

        /// <summary>
        /// Track sent by Lavalink.
        /// </summary>
        public PlayableTrack Track { get; }

        internal TrackStartEventArgs(MusicPlayer player, PlayableTrack track)
        {
            Player = player;
            Track = track;
        }
    }
}