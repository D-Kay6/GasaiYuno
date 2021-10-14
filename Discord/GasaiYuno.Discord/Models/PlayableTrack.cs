using Discord;
using GasaiYuno.Discord.Extensions;
using Victoria.Player;

namespace GasaiYuno.Discord.Models
{
    public class PlayableTrack : LavaTrack
    {
        public ITextChannel TextChannel { get; }
        public string Requester { get; }

        public PlayableTrack(LavaTrack lavaTrack, IGuildUser requester, ITextChannel textChannel) : this(lavaTrack, requester.Nickname(), textChannel) { }

        public PlayableTrack(LavaTrack lavaTrack, string requester, ITextChannel textChannel) : base(lavaTrack)
        {
            Requester = requester;
            TextChannel = textChannel;
        }
    }
}