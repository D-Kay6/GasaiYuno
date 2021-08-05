using Discord;
using Victoria.Player;

namespace GasaiYuno.Discord.Models
{
    public interface IPlayable
    {
        IGuild Guild { get; }
        IGuildUser Requester { get; }
        ITextChannel TextChannel { get; }
        int Volume { get; }
    }

    public class PlayableTrack : LavaTrack, IPlayable
    {
        public IGuild Guild { get; }
        public IGuildUser Requester { get; }
        public ITextChannel TextChannel { get; }
        public int Volume { get; }

        public PlayableTrack(LavaTrack lavaTrack, IGuildUser requester, ITextChannel textChannel, int volume = 25) : base(lavaTrack)
        {
            Guild = requester.Guild;
            Requester = requester;
            TextChannel = textChannel;
            Volume = volume;
        }
    }
}