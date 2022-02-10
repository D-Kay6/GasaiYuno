using MediatR;

namespace GasaiYuno.Discord.Listing.Mediator.Events
{
    public sealed record GuildCountChangedEvent : INotification
    {
        public ulong BotId { get; init; }
        public int ShardCount { get; init; }
        public int ShardId { get; init; }
        public int GuildCount { get; init; }

        public GuildCountChangedEvent(ulong botId, int guildCount) : this(botId, -1, -1, guildCount) { }
        public GuildCountChangedEvent(ulong botId, int shardCount, int guildCount) : this(botId, shardCount, -1, guildCount) { }
        public GuildCountChangedEvent(ulong botId, int shardCount, int shardId, int guildCount)
        {
            BotId = botId;
            ShardCount = shardCount;
            ShardId = shardId;
            GuildCount = guildCount;
        }
    }
}