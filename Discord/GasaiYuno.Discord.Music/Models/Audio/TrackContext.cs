using Discord;

namespace GasaiYuno.Discord.Music.Models.Audio;

public sealed class TrackContext
{
    public ITextChannel TextChannel { get; }

    public string Requester { get; }

    public TrackContext(ITextChannel textChannel, string requester)
    {
        TextChannel = textChannel;
        Requester = requester;
    }
}