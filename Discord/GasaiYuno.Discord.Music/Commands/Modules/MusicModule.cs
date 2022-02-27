using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using GasaiYuno.Discord.Music.Models.Audio;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace GasaiYuno.Discord.Music.Commands.Modules
{
    [Group("music", "Play music in a channel or get the lyrics of a song.")]
    public class MusicModule : BaseInteractionModule<MusicModule>
    {
        private readonly LavaNode _lavaNode;
        private readonly ILyricsService _lyricsService;

        public MusicModule(LavaNode lavaNode, ILyricsService lyricsService)
        {
            _lavaNode = lavaNode;
            _lyricsService = lyricsService;
        }

        [SlashCommand("join", "Connect me to the voice channel you're in.")]
        public async Task JoinMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (user.VoiceChannel == null)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Any"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await RespondAsync(Translation.Message(player.VoiceChannel.Id == user.VoiceChannel.Id ? "Entertainment.Music.Channel.Invalid.Same" : "Entertainment.Music.Channel.Invalid.Different"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                player = await _lavaNode.JoinAsync(user.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to join a voice channel with player {player} per user {user} request.", player, user);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }
        
        [SlashCommand("leave", "Disconnect me from your voice channel.")]
        public async Task LeaveMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Invalid.None"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (player.VoiceChannel.Id != user.VoiceChannel.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(user.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to leave a voice channel with player {player} per user {user} request.", player, user);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }
        

        [SlashCommand("play", "Request a song to be played.")]
        public async Task MusicPlayAsync([Summary("song", "Video-url, playlist-url, or name of a song you want to play.")] string query)
        {
            if (Context.User is not SocketGuildUser user) return;
            if (_lavaNode.TryGetPlayer(Context.Guild, out var player) && player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                if (user.VoiceChannel == null)
                {
                    await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Any"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (player.VoiceChannel.Id != user.VoiceChannel.Id)
                {
                    await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                    return;
                }
            }
            
            await RespondAsync(Translation.Message("Entertainment.Music.Track.Search", query), ephemeral: true).ConfigureAwait(false);
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.YouTubeMusic, query).ConfigureAwait(false);
            if (searchResponse.Status is SearchStatus.NoMatches) searchResponse = await _lavaNode.SearchAsync(SearchType.SoundCloud, query).ConfigureAwait(false);
            
            var tracks = new List<PlayableTrack>();
            switch (searchResponse.Status)
            {
                case SearchStatus.LoadFailed:
                    await ModifyOriginalResponseAsync(x => x.Content = Translation.Message("Entertainment.Music.Exception")).ConfigureAwait(false);
                    return;
                case SearchStatus.SearchResult:
                    var resultTrack = searchResponse.Tracks.FirstOrDefault(t => query.Equals(t.Id));
                    if (resultTrack == null)
                    {
                        var menuBuilder = new SelectMenuBuilder()
                            .WithPlaceholder(Translation.Message("Entertainment.Music.Track.Selection"))
                            .WithCustomId("music_selection")
                            .WithMinValues(1).WithMaxValues(1)
                            .AddOption(Translation.Message("Entertainment.Music.Track.Cancel.Title"), "cancel", Translation.Message("Entertainment.Music.Track.Cancel.Description"));
                        for (var i = 0; i < Math.Min(searchResponse.Tracks.Count, 10); i++)
                        {
                            var responseTrack = searchResponse.Tracks.ElementAt(i);
                            menuBuilder.AddOption(responseTrack.Title, i.ToString(), responseTrack.Author);
                        }

                        var selectionMessage = await ModifyOriginalResponseAsync(x =>
                        {
                            x.Content = Translation.Message("Entertainment.Music.Track.Multiple", query);
                            x.Components = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
                        }).ConfigureAwait(false);
                        var reactionResult = await Interactivity.NextMessageComponentAsync(x => x.User.Id == Context.User.Id && x.Message.Id == selectionMessage.Id, timeout: TimeSpan.FromMinutes(1)).ConfigureAwait(false);
                        if (!reactionResult.IsSuccess || reactionResult.Value == null || reactionResult.Value?.Data.Values.First() == "cancel")
                        {
                            await ModifyOriginalResponseAsync(x =>
                            {
                                x.Content = Translation.Message("Entertainment.Music.Track.Cancel.Performed");
                                x.Components = new ComponentBuilder().Build();
                            }).ConfigureAwait(false);
                            if (reactionResult.Value != null) await reactionResult.Value.DeferAsync(true).ConfigureAwait(false);
                            return;
                        }

                        await reactionResult.Value.DeferAsync(true).ConfigureAwait(false);
                        resultTrack = searchResponse.Tracks.ElementAt(int.Parse(reactionResult.Value.Data.Values.First()));
                    }
                    tracks.Add(new PlayableTrack(resultTrack, user.Nickname(), Context.Channel as ITextChannel));
                    break;
                case SearchStatus.TrackLoaded:
                    tracks.Add(new PlayableTrack(searchResponse.Tracks.First(), user.Nickname(), Context.Channel as ITextChannel));
                    break;
                case SearchStatus.PlaylistLoaded:
                    if (searchResponse.Playlist.SelectedTrack >= 0) tracks.Add(new PlayableTrack(searchResponse.Tracks.ElementAt(searchResponse.Playlist.SelectedTrack), user.Nickname(), Context.Channel as ITextChannel));
                    else tracks.AddRange(searchResponse.Tracks.Select(x => new PlayableTrack(x, user.Nickname(), Context.Channel as ITextChannel)));
                    break;
            }

            if (!tracks.Any())
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = Translation.Message("Entertainment.Music.Track.Invalid");
                    x.Components = new ComponentBuilder().Build();
                }).ConfigureAwait(false);
                return;
            }

            player ??= await _lavaNode.JoinAsync(user.VoiceChannel, Context.Channel as ITextChannel).ConfigureAwait(false);
            var maxQueueItems = 5;
            var embedBuilder = new EmbedBuilder().WithTitle(Translation.Message("Entertainment.Music.Queue.Added"));
            for (var i = 0; i < tracks.Count; i++)
            {
                var track = tracks[i];
                player.Queue.Enqueue(track);
                if (i < maxQueueItems)
                    embedBuilder.AddField(track.Title.Trim(), Translation.Message("Entertainment.Music.Queue.Details", player.Queue.Count, track.Duration));
            }
            if (tracks.Count > maxQueueItems)
                embedBuilder.WithFooter(Translation.Message("Entertainment.Music.Queue.Remaining", tracks.Count - maxQueueItems));
            
            await ModifyOriginalResponseAsync(x =>
            {
                x.Embed = embedBuilder.Build();
                x.Components = new ComponentBuilder().Build();
            }).ConfigureAwait(false);
            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused) return;

            PlayableTrack playableTrack = null;
            while (playableTrack == null)
            {
                if (!player.Queue.TryDequeue(out var queuedTrack))
                {
                    Logger.LogError("Unable to get item from {@player} {@queue}", player, player.Queue);
                    if (player.Queue.Count == 0) return;
                    continue;
                }

                playableTrack = queuedTrack as PlayableTrack;
            }

            await _lavaNode.MoveChannelAsync(playableTrack.TextChannel).ConfigureAwait(false);
            await player.PlayAsync(playableTrack).ConfigureAwait(false);
        }

        [SlashCommand("pause", "Pause the currently playing song.")]
        public async Task PauseMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.PlayerState is PlayerState.Paused)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.IsPaused"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.PauseAsync().ConfigureAwait(false);
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Paused"), ephemeral: true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to pause the player {player}.", player);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }

        [SlashCommand("resume", "Continue playing the previously paused song.")]
        public async Task ResumeMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.PlayerState is PlayerState.Playing)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.IsResumed"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.ResumeAsync().ConfigureAwait(false);
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Resumed"), ephemeral: true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to resume the player {player}.", player);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }

        [SlashCommand("stop", "Stop the current song and don't continue playing.")]
        public async Task StopMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                await player.StopAsync().ConfigureAwait(false);
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Stop"), ephemeral: true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to stop the player {player}.", player);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }


        [SlashCommand("playing", "Show what song is currently playing.")]
        public async Task PlayingMusicCommand()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle(Translation.Message("Entertainment.Music.Track.Current"))
                .WithDescription(Translation.Message("Entertainment.Music.Track.Item", player.Track.Title.Trim(), player.Track.Duration));
            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("queue", "Show the songs in the queue.")]
        public async Task QueueMusicCommand()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.Queue.Count == 0)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var maxQueueItems = 10;
            var embedBuilder = new EmbedBuilder().WithTitle(Translation.Message("Entertainment.Music.Queue.List"));
            for (var i = 1; i <= Math.Min(player.Queue.Count, maxQueueItems); i++)
            {
                var track = player.Queue.ElementAt(i);
                embedBuilder.AddField(track.Title.Trim(), Translation.Message("Entertainment.Music.Queue.Details", i, track.Duration));
            }
            if (player.Queue.Count > maxQueueItems)
                embedBuilder.WithFooter(Translation.Message("Entertainment.Music.Queue.Remaining", player.Queue.Count - maxQueueItems));

            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
        }


        [SlashCommand("shuffle", "Shuffle the songs in the queue.")]
        public async Task ShuffleMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (!player.Queue.Any())
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            await RespondAsync(Translation.Message("Entertainment.Music.Queue.Shuffling"), ephemeral: true).ConfigureAwait(false);
            player.Queue.Shuffle();
            await ModifyOriginalResponseAsync(x => x.Content = Translation.Message("Entertainment.Music.Queue.Shuffled")).ConfigureAwait(false);
        }

        [SlashCommand("skip", "Skip a song and continue playing with the next one.")]
        public async Task SkipMusicCommand([Summary(description: "The amount of songs to skip.")] int amount = 1)
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                if (amount > 1)
                    player.Queue.RemoveRange(0, Math.Min(amount - 1, player.Queue.Count));

                await RespondAsync(Translation.Message("Entertainment.Music.Queue.Skipped", amount), ephemeral: true).ConfigureAwait(false);
                if (player.Queue.Any()) await player.SkipAsync().ConfigureAwait(false);
                else await _lavaNode.LeaveAsync(player.VoiceChannel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to skip tracks of the player {player}.", player);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }

        [SlashCommand("clear", "Remove all the songs from the queue.")]
        public async Task ClearMusicCommand()
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.Queue.Count == 0)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            player.Queue.Clear();
            await RespondAsync(Translation.Message("Entertainment.Music.Queue.Cleared"), ephemeral: true).ConfigureAwait(false);
        }


        [SlashCommand("volume", "Change the volume of the music.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task VolumeMusicCommand([Summary(description: "The new volume.")] ushort volume)
        {
            if (Context.User is not SocketGuildUser user) return;
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player) || player.PlayerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            if (player.VoiceChannel.Id != user.VoiceChannel?.Id)
            {
                await RespondAsync(Translation.Message("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                if (volume > 150) volume = 150;
                await player.UpdateVolumeAsync(volume).ConfigureAwait(false);
                await RespondAsync(Translation.Message("Entertainment.Music.Player.Volume", volume), ephemeral: true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Something went wrong trying to change the volume of the player {player}.", player);
                await RespondAsync(Translation.Message("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
            }
        }

        [SlashCommand("lyrics", "Get the lyrics of a song.")]
        public async Task LyricsMusicCommand([Summary("song", "The name of the song.")] string input)
        {
            SelectMenuBuilder menuBuilder;
            ILyricsOption[] options;
            var typingState = Context.Channel.EnterTypingState();
            try
            {
                options = await _lyricsService.Search(input).ConfigureAwait(false);
                if (options == null || options.Length == 0)
                {
                    await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None"), ephemeral: true).ConfigureAwait(false);
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
                await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.None"), ephemeral: true).ConfigureAwait(false);
                return;
            }
            finally
            {
                typingState.Dispose();
            }

            var selectionMessage = await ReplyAsync(Translation.Message("Entertainment.Music.Lyrics.Multiple", input), components: new ComponentBuilder().WithSelectMenu(menuBuilder).Build()).ConfigureAwait(false);
            var reactionResult = await Interactivity.NextMessageComponentAsync(x => x.User.Id == Context.User.Id && x.Message.Id == selectionMessage.Id, timeout: TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            if (!reactionResult.IsSuccess || reactionResult.Value == null)
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
                    await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(result.FullTitle);
                if (!string.IsNullOrWhiteSpace(lyrics.Content))
                {
                    if (lyrics.Content.Length > 4096)
                    {
                        await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Length"), ephemeral: true).ConfigureAwait(false);
                        return;
                    }

                    embedBuilder.WithDescription(lyrics.Content);
                    await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                    return;
                }

                if (lyrics.Parts == null || lyrics.Parts.Length == 0)
                {
                    await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format"), ephemeral: true).ConfigureAwait(false);
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
                await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process the lyrics for {@lyrics}", result);
                await RespondAsync(Translation.Message("Entertainment.Music.Lyrics.Invalid.Format"), ephemeral: true).ConfigureAwait(false);
            }
            finally
            {
                typingState.Dispose();
            }
        }
    }
}