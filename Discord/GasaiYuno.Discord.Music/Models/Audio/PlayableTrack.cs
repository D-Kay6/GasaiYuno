using Discord;
using Victoria.Player;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class PlayableTrack : LavaTrack
{
    public ITextChannel TextChannel { get; }
    public string Requester { get; }

    public PlayableTrack(LavaTrack lavaTrack, string requester, ITextChannel textChannel) : base(lavaTrack)
    {
        Requester = requester;
        TextChannel = textChannel;
    }
}