using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Discord.Models;
using Interactivity;
using Interactivity.Selection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria.Node;
using Victoria.Player;
using Victoria.Responses.Search;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Music")]
    [Alias("m")]
    public class MusicModule : BaseModule<MusicModule>
    {
        private readonly LavaNode _lavaNode;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Priority(-1)]
        [Command]
        public async Task MusicDefaultAsync()
        {
            await MusicPlayingAsync();
        }

        [Command("Join")]
        public async Task MusicJoinAsync()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (user.VoiceChannel == null)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Required.Any")).ConfigureAwait(false);
                return;
            }

            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync(Translation.Message(player.VoiceChannel.Id == user.VoiceChannel.Id ? "Entertainment.Music.Channel.Invalid.Same" : "Entertainment.Music.Channel.Invalid.Different")).ConfigureAwait(false);
                return;
            }

            try
            {
                player = await _lavaNode.JoinAsync(user.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to join a voice channel with player {player} per user {user} request.", player, user);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Move")]
        public async Task MusicMoveAsync()
        {
            if (Context.User is not SocketGuildUser user) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is PlayerState.None)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Invalid.None")).ConfigureAwait(false);
                return;
            }

            if (user.VoiceChannel == null)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Required.Any")).ConfigureAwait(false);
                return;
            }

            if (player.VoiceChannel.Id == user.VoiceChannel.Id)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Invalid.Same")).ConfigureAwait(false);
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(user.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to move to another voice channel with player {player} per user {user} request.", player, user);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Leave")]
        public async Task MusicLeaveAsync()
        {
            if (Context.User is not SocketGuildUser user) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is PlayerState.None)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Invalid.None")).ConfigureAwait(false);
                return;
            }

            if (player.VoiceChannel.Id != user.VoiceChannel.Id)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Required.Same")).ConfigureAwait(false);
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(user.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to leave a voice channel with player {player} per user {user} request.", player, user);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }


        [Command("Play")]
        [Alias("Request")]
        [Summary("Requests a song to be played")]
        public async Task MusicPlayAsync([Remainder] string query)
        {
            if (!await CanPerformAsync()) return;

            var message = await ReplyAsync(Translation.Message("Entertainment.Music.Track.Search", query)).ConfigureAwait(false);
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTubeMusic, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.SoundCloud, query).ConfigureAwait(false);

            await message.DeleteAsync().ConfigureAwait(false);
            var user = Context.User as SocketGuildUser;
            var tracks = new List<PlayableTrack>();
            switch (searchResponse.Status)
            {
                case SearchStatus.LoadFailed:
                    await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
                    return;
                case SearchStatus.SearchResult:
                    var resultTrack = searchResponse.Tracks.FirstOrDefault(t => query.Equals(t.Id));
                    if (resultTrack == null)
                    {
                        var reactions = new Dictionary<IEmote, LavaTrack>();
                        for (var i = 0; i < Math.Min(searchResponse.Tracks.Count, 5); i++)
                            reactions.Add(EmojiExtensions.GetNumber(i + 1), searchResponse.Tracks.ElementAt(i));
                        var reactionBuilder = new ReactionSelectionBuilder<LavaTrack>()
                            .WithTitle(Translation.Message("Entertainment.Music.Track.Multiple", query))
                            .WithStringConverter(x => x.Title)
                            .WithSelectables(reactions)
                            .WithUsers(Context.User)
                            .WithDeletion(DeletionOptions.AfterCapturedContext | DeletionOptions.Invalids);
                        var reactionResult = await Interactivity.SendSelectionAsync(reactionBuilder.Build(), Context.Channel);
                        if (!reactionResult.IsSuccess) return;

                        resultTrack = reactionResult.Value;
                    }
                    tracks.Add(new PlayableTrack(resultTrack ?? searchResponse.Tracks.First(), user, Context.Channel as ITextChannel));
                    break;
                case SearchStatus.TrackLoaded:
                    tracks.Add(new PlayableTrack(searchResponse.Tracks.First(), user, Context.Channel as ITextChannel));
                    break;
                case SearchStatus.PlaylistLoaded:
                    if (searchResponse.Playlist.SelectedTrack >= 0) tracks.Add(new PlayableTrack(searchResponse.Tracks.ElementAt(searchResponse.Playlist.SelectedTrack), user, Context.Channel as ITextChannel));
                    else tracks.AddRange(searchResponse.Tracks.Select(x => new PlayableTrack(x, user, Context.Channel as ITextChannel)));
                    break;
            }

            if (!tracks.Any())
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Track.Invalid")).ConfigureAwait(false);
                return;
            }

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
                player = await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel).ConfigureAwait(false);

            tracks.ForEach(player.Vueue.Enqueue);
            if (tracks.Count == 1 && player.PlayerState is PlayerState.Playing or PlayerState.Paused)
                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Song", player.Vueue.Count, tracks[0].Title, tracks[0].Duration)).ConfigureAwait(false);
            else if (tracks.Count > 1)
                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Playlist", searchResponse.Tracks.Count)).ConfigureAwait(false);

            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused) return;

            LavaTrack lavaTrack;
            while (!player.Vueue.TryDequeue(out lavaTrack)) Logger.LogError("Unable to get item from {@player} {@queue}", player, player.Vueue);
            if (lavaTrack is not PlayableTrack track)
            {
                await player.StopAsync().ConfigureAwait(false);
                await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
                return;
            }

            player.SetTextChannel(track.TextChannel);
            await player.PlayAsync(track).ConfigureAwait(false);
        }

        [Command("Pause")]
        public async Task MusicPauseAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            if (player.PlayerState is PlayerState.Paused)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.IsPaused")).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.PauseAsync().ConfigureAwait(false);
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Paused")).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to pause the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Resume")]
        public async Task MusicResumeAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            if (player.PlayerState is PlayerState.Playing)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.IsResumed")).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.ResumeAsync().ConfigureAwait(false);
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Resumed")).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to resume the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Stop")]
        public async Task MusicStopAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.StopAsync().ConfigureAwait(false);
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Stop")).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to stop the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }


        [Alias("Now")]
        [Command("Playing")]
        public async Task MusicPlayingAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            await ReplyAsync(Translation.Message("Entertainment.Music.Track.Current", player.Track.Title)).ConfigureAwait(false);
        }

        [Command("Queue")]
        public async Task MusicQueueAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            if (player.Vueue.Count == 0)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Empty")).ConfigureAwait(false);
                return;
            }

            int queueSize = 15;
            var items = new List<string>();
            for (var i = 0; i < player.Vueue.Count; i++)
            {
                if (i == queueSize)
                {
                    items.Add(Translation.Message("Entertainment.Music.Queue.Remaining", player.Vueue.Count - queueSize));
                    break;
                }
                var track = player.Vueue.ElementAt(i);
                items.Add(Translation.Message("Entertainment.Music.Queue.Item", i + 1, track.Title, track.Duration));
            }

            await ReplyAsync(string.Join(Environment.NewLine, items)).ConfigureAwait(false);
        }


        [Command("Shuffle")]
        public async Task MusicShuffleAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || !player.Vueue.Any())
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Empty")).ConfigureAwait(false);
                return;
            }

            var notice = await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Shuffling")).ConfigureAwait(false);
            player.Vueue.Shuffle();
            await notice.DeleteAsync().ConfigureAwait(false);
            await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Shuffled")).ConfigureAwait(false);
        }

        [Command("Skip")]
        public async Task MusicSkipAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            try
            {
                if (player.Vueue.Any()) await player.SkipAsync().ConfigureAwait(false);
                else await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to skip a track of the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Skip")]
        public async Task MusicSkipAsync(int amount)
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            try
            {
                player.Vueue.RemoveRange(0, amount - 1);
                if (player.Vueue.Any()) await player.SkipAsync().ConfigureAwait(false);
                else await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Skipped", amount)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to skip multiple tracks of the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }

        [Command("Clear")]
        public async Task MusicClearAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            if (player.Vueue.Count == 0)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Empty")).ConfigureAwait(false);
                return;
            }

            player.Vueue.Clear();
            await ReplyAsync(Translation.Message("Entertainment.Music.Queue.Cleared")).ConfigureAwait(false);
        }


        [Command("Volume")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MusicVolumeAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            await ReplyAsync(Translation.Message("Entertainment.Music.Player.Volume.current", player.Volume)).ConfigureAwait(false);
        }

        [Command("Volume")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MusicVolumeAsync(ushort volume)
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            try
            {
                if (volume > 150) volume = 150;
                await player.SetVolumeAsync(volume).ConfigureAwait(false);
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Volume.New", volume)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to change the volume of the player {player}.", player);
                await ReplyAsync(Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
            }
        }


        private async Task<bool> CanPerformAsync()
        {
            if (Context.User is not SocketGuildUser user) return false;

            if (_lavaNode.TryGetPlayer(Context.Guild, out var player) && player.PlayerState is PlayerState.Playing or PlayerState.Paused && player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Required.Same")).ConfigureAwait(false);
                return false;
            }
            if (user.VoiceChannel == null)
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Channel.Required.Any")).ConfigureAwait(false);
                return false;
            }
            return true;
        }
    }
}