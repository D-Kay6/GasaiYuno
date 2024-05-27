using Discord;
using Lavalink4NET.Player;

namespace GasaiYuno.Discord.Music.Models.Audio;

public record TrackContext
{
    public LavalinkTrack Track { get; init; }

    public ITextChannel TextChannel { get; init; }

    public string Requester { get; init; }

    public TrackContext(LavalinkTrack track, ITextChannel textChannel, string requester)
    {
        Track = track;
        TextChannel = textChannel;
        Requester = requester;
    }
}