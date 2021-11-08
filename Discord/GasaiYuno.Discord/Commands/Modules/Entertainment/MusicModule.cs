using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using GasaiYuno.Interface.Music;
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
        private readonly ILyricsService _lyricsService;

        public MusicModule(LavaNode lavaNode, ILyricsService lyricsService)
        {
            _lavaNode = lavaNode;
            _lyricsService = lyricsService;
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


        [Command("Status")]
        [RequireOwner]
        public async Task MusicStatusAsync()
        {
            await Context.Message.DeleteAsync().ConfigureAwait(false);
            var dm = await Context.User.CreateDMChannelAsync().ConfigureAwait(false);
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await dm.SendMessageAsync("No player for that server.").ConfigureAwait(false);
                return;
            }

            var msg = $"State: {player.PlayerState} \nVoice: {player.VoiceChannel.Name} \nText: {player.TextChannel?.Name ?? "none"} \nTrack: {player.Track?.Title ?? "none"}";
            await dm.SendMessageAsync(msg).ConfigureAwait(false);
        }


        [Command("Play")]
        [Alias("Request")]
        [Summary("Requests a song to be played")]
        public async Task MusicPlayAsync([Remainder] string query)
        {
            if (!await CanPerformAsync()) return;

            var message = await ReplyAsync(Translation.Message("Entertainment.Music.Track.Search", query)).ConfigureAwait(false);
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTubeMusic, query).ConfigureAwait(false);
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
                        var menuBuilder = new SelectMenuBuilder().WithPlaceholder(Translation.Message("Entertainment.Music.Track.Selection")).WithCustomId(query).WithMinValues(1).WithMaxValues(1);
                        for (var i = 0; i < Math.Min(searchResponse.Tracks.Count, 10); i++)
                        {
                            var responseTrack = searchResponse.Tracks.ElementAt(i);
                            menuBuilder.AddOption(responseTrack.Title, responseTrack.Id, responseTrack.Author);
                        }

                        var selectionMessage = await ReplyAsync(Translation.Message("Entertainment.Music.Track.Multiple", query), component: new ComponentBuilder().WithSelectMenu(menuBuilder).Build());
                        var reactionResult = await Interactivity.NextMessageComponentAsync(x => x.User.Id == Context.User.Id && x.Message.Id == selectionMessage.Id, timeout: TimeSpan.FromMinutes(1));
                        if (!reactionResult.IsSuccess)
                        {
                            await selectionMessage.DeleteAsync().ConfigureAwait(false);
                            return;
                        }

                        await reactionResult.Value.DeferAsync(true).ConfigureAwait(false);
                        resultTrack = searchResponse.Tracks.First(x => x.Id == reactionResult.Value.Data.Values.First());
                        await selectionMessage.DeleteAsync().ConfigureAwait(false);
                    }
                    tracks.Add(new PlayableTrack(resultTrack, user, Context.Channel as ITextChannel));
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
            
            var maxQueueItems = 5;
            var embedBuilder = new EmbedBuilder().WithTitle(Translation.Message("Entertainment.Music.Queue.Added"));
            for (var i = 0; i < tracks.Count; i++)
            {
                var track = tracks[i];
                player.Vueue.Enqueue(track);
                if (i < maxQueueItems)
                    embedBuilder.AddField(track.Title.Trim(), Translation.Message("Entertainment.Music.Queue.Details", player.Vueue.Count, track.Duration));
            }
            if (tracks.Count > maxQueueItems)
                embedBuilder.WithFooter(Translation.Message("Entertainment.Music.Queue.Remaining", tracks.Count - maxQueueItems));
            if (tracks.Count > 1 || player.PlayerState is PlayerState.Playing or PlayerState.Paused)
                await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused) return;

            LavaTrack lavaTrack;
            while (!player.Vueue.TryDequeue(out lavaTrack)) Logger.LogError("Unable to get item from {@player} {@queue}", player, player.Vueue);
            if (lavaTrack is not PlayableTrack playableTrack)
            {
                await player.StopAsync().ConfigureAwait(false);
                await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
                return;
            }

            player.SetTextChannel(playableTrack.TextChannel);
            await player.PlayAsync(playableTrack).ConfigureAwait(false);
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

            var embedBuilder = new EmbedBuilder()
                .WithTitle(Translation.Message("Entertainment.Music.Track.Current"))
                .WithDescription(Translation.Message("Entertainment.Music.Track.Item", player.Track.Title.Trim(), player.Track.Duration));
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
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

            var maxQueueItems = 10;
            var embedBuilder = new EmbedBuilder().WithTitle(Translation.Message("Entertainment.Music.Queue.List"));
            for (var i = 1; i <= Math.Min(player.Vueue.Count, maxQueueItems); i++)
            {
                var track = player.Vueue.ElementAt(i);
                embedBuilder.AddField(track.Title.Trim(), Translation.Message("Entertainment.Music.Queue.Details", i, track.Duration));
            }
            if (player.Vueue.Count > maxQueueItems)
                embedBuilder.WithFooter(Translation.Message("Entertainment.Music.Queue.Remaining", player.Vueue.Count - maxQueueItems));

            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
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

        [Command("Lyrics")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MusicLyricsAsync()
        {
            if (!await CanPerformAsync()) return;

            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Player.Inactive")).ConfigureAwait(false);
                return;
            }

            ILyricsOption selectedOption = null;
            var typingState = Context.Channel.EnterTypingState();
            try
            {
                async Task<ILyricsOption> FindMatch(string input)
                {
                    input = input.Replace('’', '\'');
                    var options = await _lyricsService.Search(input, 50).ConfigureAwait(false);
                    if (options == null || options.Length == 0) return null;
                    return (from lyricsOption in options
                        let artist = lyricsOption.Artist.Replace('’', '\'')
                        let title = lyricsOption.Title.Replace('’', '\'')
                        let fullTitle = lyricsOption.FullTitle.Replace('’', '\'')
                        let possibleTitles = new[] { $"{artist} - {title}", $"{title} - {artist}", fullTitle, title }
                        from possibleTitle in possibleTitles
                        where input.Contains(possibleTitle, StringComparison.OrdinalIgnoreCase)
                        select lyricsOption).FirstOrDefault();
                }

                var inputParts = player.Track.Title.Contains(" - ")
                    ? player.Track.Title.Split("-", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    : new[] { player.Track.Title };
                for (var i = 0; i < inputParts.Length && (selectedOption == null); i++) selectedOption = await FindMatch(inputParts[i]);
                if (selectedOption == null)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
                    return;
                }
            }
            catch
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
                return;
            }
            finally
            {
                typingState.Dispose();
            }

            typingState = Context.Channel.EnterTypingState();
            try
            {
                var lyrics = await _lyricsService.Get(selectedOption).ConfigureAwait(false);
                if (lyrics == null)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
                    return;
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(selectedOption.FullTitle);
                if (!string.IsNullOrWhiteSpace(lyrics.Content))
                {
                    if (lyrics.Content.Length > 4096)
                    {
                        await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Length")).ConfigureAwait(false);
                        return;
                    }

                    embedBuilder.WithDescription(lyrics.Content);
                    await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                    return;
                }

                if (lyrics.Parts == null || lyrics.Parts.Length == 0)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
                    return;
                }

                foreach (var lyricsPart in lyrics.Parts)
                {
                    var title = lyricsPart.Title;
                    var content = lyricsPart.Content;
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        if (string.IsNullOrWhiteSpace(lyricsPart.Content)) continue;
                        title = "-";
                    }
                    if (string.IsNullOrWhiteSpace(content)) content = "-";
                    embedBuilder.AddField(title, content);
                }
                await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process the lyrics for {@lyrics}", selectedOption);
                await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
            }
            finally
            {
                typingState.Dispose();
            }
        }

        [Command("Lyrics")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MusicLyricsAsync([Remainder] string input)
        {
            SelectMenuBuilder menuBuilder;
            ILyricsOption[] options;
            var typingState = Context.Channel.EnterTypingState();
            try
            {
                options = await _lyricsService.Search(input).ConfigureAwait(false);
                if (options == null || options.Length == 0)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
                    return;
                }

                menuBuilder = new SelectMenuBuilder().WithPlaceholder(Translation.Message("Entertainment.Music.Lyrics.Selection")).WithCustomId(input).WithMinValues(1).WithMaxValues(1);
                for (var i = 0; i < Math.Min(options.Length, 10); i++)
                {
                    var option = options[i];
                    menuBuilder.AddOption(option.FullTitle, option.Id);
                }
            }
            catch
            {
                await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
                return;
            }
            finally
            {
                typingState.Dispose();
            }

            var selectionMessage = await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Multiple", input), component: new ComponentBuilder().WithSelectMenu(menuBuilder).Build());
            var reactionResult = await Interactivity.NextMessageComponentAsync(x => x.User.Id == Context.User.Id && x.Message.Id == selectionMessage.Id, timeout: TimeSpan.FromMinutes(1));
            if (!reactionResult.IsSuccess)
            {
                await selectionMessage.DeleteAsync().ConfigureAwait(false);
                return;
            }

            await reactionResult.Value.DeferAsync(true).ConfigureAwait(false);
            var result = options.First(x => x.Id == reactionResult.Value.Data.Values.First());
            await selectionMessage.DeleteAsync().ConfigureAwait(false);
            typingState = Context.Channel.EnterTypingState();
            try
            {
                var lyrics = await _lyricsService.Get(result).ConfigureAwait(false);
                if (lyrics == null)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
                    return;
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(result.FullTitle);
                if (!string.IsNullOrWhiteSpace(lyrics.Content))
                {
                    if (lyrics.Content.Length > 4096)
                    {
                        await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Length")).ConfigureAwait(false);
                        return;
                    }

                    embedBuilder.WithDescription(lyrics.Content);
                    await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                    return;
                }

                if (lyrics.Parts == null || lyrics.Parts.Length == 0)
                {
                    await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
                    return;
                }

                foreach (var lyricsPart in lyrics.Parts)
                {
                    var title = lyricsPart.Title;
                    var content = lyricsPart.Content;
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        if (string.IsNullOrWhiteSpace(lyricsPart.Content)) continue;
                        title = "-";
                    }
                    if (string.IsNullOrWhiteSpace(content)) content = "-";
                    embedBuilder.AddField(title, content);
                }
                await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process the lyrics for {@lyrics}", result);
                await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format")).ConfigureAwait(false);
            }
            finally
            {
                typingState.Dispose();
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