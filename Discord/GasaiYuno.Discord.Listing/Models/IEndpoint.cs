using RestSharp;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Models
{
    public interface IEndpoint
    {
        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task<RestResponse> SendUpdateAsync(ulong botId, int guildCount);

        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="shardCount">The total amount of shards the client has.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task<RestResponse> SendUpdateAsync(ulong botId, int shardCount, int guildCount);

        /// <summary>
        /// Updates the stats for a bot listing.
        /// </summary>
        /// <param name="botId">The Id of the client.</param>
        /// <param name="shardCount">The total amount of shards the client has.</param>
        /// <param name="shardId">The shard id.</param>
        /// <param name="guildCount">The amount of servers the bot is in.</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        Task<RestResponse> SendUpdateAsync(ulong botId, int shardCount, int shardId, int guildCount);
    }
}