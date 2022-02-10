using Discord;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Victoria;

namespace GasaiYuno.Discord.Music.Models.Audio
{
    public class MusicPlayer : MusicPlayer<PlayableTrack>
    {
        /// <inheritdoc />
        public MusicPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel) { }
    }

    public class MusicPlayer<TTrack> : LavaPlayer where TTrack : LavaTrack
    {
        private readonly ConcurrentDictionary<string, TTrack> _trackCache;

        /// <inheritdoc />
        public MusicPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
        {
            _trackCache = new ConcurrentDictionary<string, TTrack>();
        }

        /// <summary>
        /// Get a track from the player's cache.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="track"></param>
        /// <returns></returns>
        internal bool TryGetCachedTrack(string id, out TTrack track)
        {
            return _trackCache.TryGetValue(id, out track);
        }

        /// <summary>
        /// Remove a track from the player's cache if its not in use.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal void RemoveCachedTrack(string id)
        {
            if (Queue.Any(x => x.Id == id)) return;
            _trackCache.TryRemove(id, out _);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Throws when <paramref name="track"/> is null.</exception>
        public async Task PlayAsync(TTrack track)
        {
            await base.PlayAsync(track);
            _trackCache.TryAdd(track.Id, track);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public new async ValueTask DisposeAsync()
        {
            _trackCache.Clear();
            await base.DisposeAsync();
        }
    }
}