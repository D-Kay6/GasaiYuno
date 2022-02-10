using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Ínterfaces
{
    public interface IListingUpdater
    {
        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task UpdateStatsAsync(ulong botId, int guildCount);

        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="shardCount">The total amount of shards the client has.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task UpdateStatsAsync(ulong botId, int shardCount, int guildCount);

        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="shardCount">The total amount of shards the client has.</param>
        /// <param name="shardId">The shard id.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task UpdateStatsAsync(ulong botId, int shardCount, int shardId, int guildCount);

        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="shardCount">The total amount of shards the client has.</param>
        /// <param name="guildCounts">The amount of servers the bot is in per shard.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task UpdateStatsAsync(ulong botId, int shardCount, int[] guildCounts);
    }
}