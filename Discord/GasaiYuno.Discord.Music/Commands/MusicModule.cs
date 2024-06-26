﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using GasaiYuno.Discord.Music.Models.Audio;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Music.Commands;

[EnabledInDm(false)]
[Group("music", "Play music in a channel or get the lyrics of a song.")]
public class MusicModule : BaseInteractionModule<MusicModule>
{
    private readonly IAudioService _audioService;
    private readonly ILyricsService _lyricsService;
    private readonly ICachingService _cachingService;

    public MusicModule(IAudioService audioService, ILyricsService lyricsService, ICachingService cachingService)
    {
        _audioService = audioService;
        _lyricsService = lyricsService;
        _cachingService = cachingService;
    }

    [SlashCommand("join", "Connect me to the voice channel you're in.")]
    public async Task JoinMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        if (user.VoiceChannel == null)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Any"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild.Id);
        if (player != null)
        {
            await RespondAsync(Localization.Translate(player.VoiceChannelId == user.VoiceChannel.Id ? "Entertainment.Music.Channel.Invalid.Same" : "Entertainment.Music.Channel.Invalid.Different"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            player = await _audioService.JoinAsync<MusicPlayer>(user.VoiceChannel).ConfigureAwait(false);
            await ConfirmAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to join a voice channel with player {@Player} per user {@User} request", player, user);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }

    [SlashCommand("leave", "Disconnect me from your voice channel.")]
    public async Task LeaveMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player == null)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Invalid.None"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            await player.StopAsync(true).ConfigureAwait(false);
            await ConfirmAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to leave a voice channel with player {@Player} per user {@User} request", player, user);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }


    [SlashCommand("play", "Request a song to be played.")]
    public async Task PlayMusicCommand([Summary("song", "Video-url, playlist-url, or name of a song you want to play.")] string query)
    {
        // if (Regex.IsMatch(query, @"youtu(?:\.be|be\.com)"))
        // {
        //     await RespondAsync(Translation.Translate("Entertainment.Music.Youtube"), ephemeral: true).ConfigureAwait(false);
        //     return;
        // }

        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is PlayerState.Playing or PlayerState.Paused)
        {
            if (user.VoiceChannel == null)
            {
                await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Any"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (player.VoiceChannelId != user.VoiceChannel.Id)
            {
                await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                return;
            }
        }

        await RespondAsync(Localization.Translate("Entertainment.Music.Track.Search", query), ephemeral: true).ConfigureAwait(false);
        var searchResponse = await _audioService.LoadTracksAsync(query!).ConfigureAwait(false);
        if (searchResponse.LoadType == TrackLoadType.NoMatches)
            searchResponse = await _audioService.LoadTracksAsync(query!, SearchMode.YouTube).ConfigureAwait(false);
        if (searchResponse.LoadType == TrackLoadType.NoMatches)
            searchResponse = await _audioService.LoadTracksAsync(query!, SearchMode.SoundCloud).ConfigureAwait(false);

        var tracks = new List<LavalinkTrack>();
        switch (searchResponse.LoadType)
        {
            case TrackLoadType.LoadFailed:
                await ModifyOriginalResponseAsync(x => x.Content = Localization.Translate("Entertainment.Music.Exception")).ConfigureAwait(false);
                return;
            case TrackLoadType.SearchResult:
                var id = await _cachingService.AddAsync(searchResponse.Tracks).ConfigureAwait(false);
                var menuBuilder = new SelectMenuBuilder()
                    .WithPlaceholder(Localization.Translate("Entertainment.Music.Track.Selection"))
                    .WithCustomId("music track_selection:" + id)
                    .WithMinValues(1).WithMaxValues(1)
                    .AddOption(Localization.Translate("Entertainment.Music.Track.Cancel.Title"), "cancel", Localization.Translate("Entertainment.Music.Track.Cancel.Description"), new Emoji("❌"));
                for (var i = 0; i < Math.Min(searchResponse.Tracks!.Length, 10); i++)
                {
                    var responseTrack = searchResponse.Tracks.ElementAt(i);
                    var title = responseTrack.Title;
                    if (responseTrack.TrackIdentifier.StartsWith("U") && responseTrack.Provider == StreamProvider.SoundCloud)
                    {
                        var preview = " " + Localization.Translate("Entertainment.Music.Track.Preview");
                        if (title.Length + preview.Length > 100)
                            title = title[..(97 - preview.Length)] + "..." + preview;
                        else
                            title += preview;
                    }
                    else if (title.Length > 100)
                    {
                        title = responseTrack.Title[..97] + "...";
                    }
                    menuBuilder.AddOption(title, responseTrack.Title.ToHash(responseTrack.Author) + i, responseTrack.Author, new Emoji("🎶"));
                }

                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Track.Multiple", query);
                    x.Components = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
                }).ConfigureAwait(false);
                return;
            case TrackLoadType.TrackLoaded:
                tracks.Add(searchResponse.Tracks!.First());
                break;
            case TrackLoadType.PlaylistLoaded:
                if (searchResponse.PlaylistInfo!.SelectedTrack >= 0)
                    tracks.Add(searchResponse.Tracks!.ElementAt(searchResponse.PlaylistInfo!.SelectedTrack));
                else
                    tracks.AddRange(searchResponse.Tracks!);
                break;
        }

        if (!tracks.Any())
        {
            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = Localization.Translate("Entertainment.Music.Track.Invalid");
                x.Components = new ComponentBuilder().Build();
            }).ConfigureAwait(false);
            return;
        }

        player ??= await _audioService.JoinAsync<MusicPlayer>(user.VoiceChannel, true).ConfigureAwait(false);
        var embedBuilder = new EmbedBuilder().WithTitle(Localization.Translate("Entertainment.Music.Queue.Added"));
        var messageCount = 0;
        foreach (var track in tracks)
        {
            var context = new TrackContext(track, Context.Channel as ITextChannel, user.Nickname());
            track.Context = context;
            var position = await player.PlayAsync(context).ConfigureAwait(false);
            if (messageCount >= 5)
                continue;

            embedBuilder.AddField(track.Title.Trim(), Localization.Translate("Entertainment.Music.Queue.Details", position, track.Duration));
            messageCount++;
        }

        if (tracks.Count > 5)
            embedBuilder.WithFooter(Localization.Translate("Entertainment.Music.Queue.Remaining", tracks.Count - 5));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = null;
            x.Embed = embedBuilder.Build();
            x.Components = new ComponentBuilder().Build();
        }).ConfigureAwait(false);
    }

    [SlashCommand("pause", "Pause the currently playing song.")]
    public async Task PauseMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.State is PlayerState.Paused)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.IsPaused"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            await player.PauseAsync().ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Paused"), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to pause the player {@Player}", player);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }

    [SlashCommand("resume", "Continue playing the previously paused song.")]
    public async Task ResumeMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.State is PlayerState.Playing)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.IsResumed"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            await player.ResumeAsync().ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Resumed"), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to resume the player {@Player}", player);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }

    [SlashCommand("stop", "Stop the current song and don't continue playing.")]
    public async Task StopMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            await player.StopAsync(true).ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Stop"), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to stop the player {@Player}", player);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }


    [SlashCommand("playing", "Show what song is currently playing.")]
    public async Task PlayingMusicCommand()
    {
        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var embedBuilder = new EmbedBuilder()
            .WithTitle(Localization.Translate("Entertainment.Music.Track.Current"))
            .WithDescription(Localization.Translate("Entertainment.Music.Track.Item", player.CurrentTrack!.Title.Trim(), player.CurrentTrack.Duration));
        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("queue", "Show the songs in the queue.")]
    public async Task QueueMusicCommand()
    {
        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.Queue.IsEmpty)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var maxQueueItems = Math.Min(player.Queue.Count, 10);
        var embedBuilder = new EmbedBuilder().WithTitle(Localization.Translate("Entertainment.Music.Queue.List"));
        for (var i = 0; i < maxQueueItems; i++)
        {
            var trackContext = player.Queue.ElementAt(i);
            embedBuilder.AddField(trackContext.Track.Title.Trim(), Localization.Translate("Entertainment.Music.Queue.Details", i + 1, trackContext.Track.Duration));
        }

        if (player.Queue.Count > maxQueueItems)
            embedBuilder.WithFooter(Localization.Translate("Entertainment.Music.Queue.Remaining", player.Queue.Count - maxQueueItems));

        await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
    }


    [SlashCommand("shuffle", "Shuffle the songs in the queue.")]
    public async Task ShuffleMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.Queue.IsEmpty)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Shuffling"), ephemeral: true).ConfigureAwait(false);
        player.Queue.Shuffle();
        await ModifyOriginalResponseAsync(x => x.Content = Localization.Translate("Entertainment.Music.Queue.Shuffled")).ConfigureAwait(false);
    }

    [SlashCommand("skip", "Skip a song and continue playing with the next one.")]
    public async Task SkipMusicCommand([Summary(description: "The amount of songs to skip.")] int amount = 1)
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            await player.SkipAsync(amount).ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Skipped", amount), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to skip tracks of the player {@Player}", player);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }

    [SlashCommand("clear", "Remove all the songs from the queue.")]
    public async Task ClearMusicCommand()
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.Queue.IsEmpty)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Empty"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        player.Queue.Clear();
        await RespondAsync(Localization.Translate("Entertainment.Music.Queue.Cleared"), ephemeral: true).ConfigureAwait(false);
    }


    [RequireUserPermission(GuildPermission.MuteMembers)]
    [SlashCommand("volume", "Change the volume of the music.")]
    public async Task VolumeMusicCommand([Summary(description: "The new volume.")] ushort volume)
    {
        if (Context.User is not SocketGuildUser user)
            return;

        var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
        if (player?.State is not (PlayerState.Playing or PlayerState.Paused))
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Inactive"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        if (player.VoiceChannelId != user.VoiceChannel?.Id)
        {
            await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            if (volume > 150)
                volume = 150;

            await player.SetVolumeAsync(volume).ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Entertainment.Music.Player.Volume", volume), ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Something went wrong trying to change the volume of the player {@Player}", player);
            await RespondAsync(Localization.Translate("Entertainment.Music.Exception"), ephemeral: true).ConfigureAwait(false);
        }
    }

    [SlashCommand("lyrics", "Get the lyrics of a song.")]
    public async Task LyricsMusicCommand([Summary("song", "The name of the song.")] string input)
    {
        SelectMenuBuilder menuBuilder;
        await DeferAsync(true).ConfigureAwait(false);
        try
        {
            var options = await _lyricsService.Search(input).ConfigureAwait(false);
            if (options == null || options.Length == 0)
            {
                await ModifyOriginalResponseAsync(x => x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
                return;
            }

            var id = input.Replace(" ", ";;");
            menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder(Localization.Translate("Entertainment.Music.Lyrics.Selection"))
                .WithCustomId("music lyrics_selection:" + id)
                .WithMinValues(1).WithMaxValues(1)
                .AddOption(Localization.Translate("Entertainment.Music.Lyrics.Cancel.Title"), "cancel", Localization.Translate("Entertainment.Music.Lyrics.Cancel.Description"), new Emoji("❌"));
            ;
            for (var i = 0; i < Math.Min(options.Length, 10); i++)
            {
                var option = options[i];
                var title = option.FullTitle.Length > 100 ? option.FullTitle[..97] + "..." : option.FullTitle;
                menuBuilder.AddOption(title, option.Id, option.Artist, new Emoji("🎼"));
            }
        }
        catch
        {
            await ModifyOriginalResponseAsync(x => x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.None")).ConfigureAwait(false);
            return;
        }

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = Localization.Translate("Entertainment.Music.Lyrics.Multiple", input);
            x.Components = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
        }).ConfigureAwait(false);
    }


    public class MusicComponentModule : BaseInteractionModule<MusicComponentModule, SocketMessageComponent>
    {
        private readonly IAudioService _audioService;
        private readonly ILyricsService _lyricsService;
        private readonly ICachingService _cachingService;

        public MusicComponentModule(IAudioService audioService, ILyricsService lyricsService, ICachingService cachingService)
        {
            _audioService = audioService;
            _lyricsService = lyricsService;
            _cachingService = cachingService;
        }

        [ComponentInteraction("track_selection:*")]
        public async Task MusicTrackInteraction(string id, string[] selectedTracks)
        {
            if (selectedTracks.Length == 0 || id.IsNullOrWhiteSpace())
                return;

            if (selectedTracks[0].Equals("cancel"))
            {
                await Context.Interaction.UpdateAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Track.Cancel.Performed");
                    x.Components = new ComponentBuilder().Build();
                });
                return;
            }

            if (Context.User is not SocketGuildUser user)
                return;

            var player = _audioService.GetPlayer<MusicPlayer>(Context.Guild);
            if (player?.State is PlayerState.Playing or PlayerState.Paused)
            {
                if (user.VoiceChannel == null)
                {
                    await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Any"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (player.VoiceChannelId != user.VoiceChannel.Id)
                {
                    await RespondAsync(Localization.Translate("Entertainment.Music.Channel.Required.Same"), ephemeral: true).ConfigureAwait(false);
                    return;
                }
            }

            await DeferAsync(true).ConfigureAwait(false);
            var tracks = await _cachingService.GetAsync<LavalinkTrack[]>(id).ConfigureAwait(false);
            if (tracks == null || tracks.Length == 0)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Track.Exception.Expired");
                    x.Components = new ComponentBuilder().Build();
                }).ConfigureAwait(false);
                return;
            }

            var selectedTrack = selectedTracks[0][..^1];
            var track = tracks?.FirstOrDefault(x => x.Title.ToHash(x.Author) == selectedTrack);
            if (track == null)
            {
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Track.Exception.Expired");
                    x.Components = new ComponentBuilder().Build();
                }).ConfigureAwait(false);
                return;
            }

            player ??= await _audioService.JoinAsync<MusicPlayer>(user.VoiceChannel, true).ConfigureAwait(false);
            var embedBuilder = new EmbedBuilder().WithTitle(Localization.Translate("Entertainment.Music.Queue.Added"));
            var context = new TrackContext(track, Context.Channel as ITextChannel, user.Nickname());
            track.Context = context;
            var position = await player.PlayAsync(context).ConfigureAwait(false);
            embedBuilder.AddField(track.Title.Trim(), Localization.Translate("Entertainment.Music.Queue.Details", position, track.Duration));

            await ModifyOriginalResponseAsync(x =>
            {
                x.Content = null;
                x.Embed = embedBuilder.Build();
                x.Components = new ComponentBuilder().Build();
            }).ConfigureAwait(false);
        }

        [ComponentInteraction("lyrics_selection:*")]
        public async Task MusicLyricsInteraction(string id, string[] selectedLyricses)
        {
            if (selectedLyricses.Length == 0 || id.IsNullOrWhiteSpace())
                return;

            if (selectedLyricses[0].Equals("cancel"))
            {
                await Context.Interaction.UpdateAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Lyrics.Cancel.Performed");
                    x.Components = new ComponentBuilder().Build();
                });
                return;
            }

            var query = id.Replace(";;", " ");
            var options = await _lyricsService.Search(query).ConfigureAwait(false);
            if (options == null || options.Length == 0)
                return;

            await DeferAsync(true).ConfigureAwait(false);
            var selectedLyrics = selectedLyricses[0];
            var result = options.FirstOrDefault(x => x.Id == selectedLyrics);
            if (result == null)
                return;

            try
            {
                var lyrics = await _lyricsService.Get(result).ConfigureAwait(false);
                if (lyrics == null)
                {
                    await Context.Interaction.UpdateAsync(x =>
                    {
                        x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.Format");
                        x.Components = new ComponentBuilder().Build();
                    }).ConfigureAwait(false);
                    return;
                }

                var embedBuilder = new EmbedBuilder().WithTitle(result.FullTitle);
                if (!string.IsNullOrWhiteSpace(lyrics.Content))
                {
                    if (lyrics.Content.Length > 4096)
                    {
                        await Context.Interaction.UpdateAsync(x =>
                        {
                            x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.Length");
                            x.Components = new ComponentBuilder().Build();
                        }).ConfigureAwait(false);
                        return;
                    }

                    embedBuilder.WithDescription(lyrics.Content);
                    await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (lyrics.Parts == null || lyrics.Parts.Length == 0)
                {
                    await Context.Interaction.UpdateAsync(x =>
                    {
                        x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.Format");
                        x.Components = new ComponentBuilder().Build();
                    }).ConfigureAwait(false);
                    return;
                }

                foreach (var lyricsPart in lyrics.Parts)
                {
                    var title = lyricsPart.Title;
                    var content = lyricsPart.Content;
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        if (string.IsNullOrWhiteSpace(lyricsPart.Content))
                            continue;

                        title = "-";
                    }

                    if (string.IsNullOrWhiteSpace(content))
                        content = "-";

                    embedBuilder.AddField(title, content);
                }

                await ReplyAsync(embed: embedBuilder.Build(), messageReference: Context.Interaction.Message.Reference).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unable to process the lyrics for {@Lyrics}", result);
                await Context.Interaction.UpdateAsync(x =>
                {
                    x.Content = Localization.Translate("Entertainment.Music.Lyrics.Invalid.Format");
                    x.Components = new ComponentBuilder().Build();
                }).ConfigureAwait(false);
            }
        }
    }
}