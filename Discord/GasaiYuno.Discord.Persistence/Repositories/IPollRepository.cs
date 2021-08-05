using GasaiYuno.Discord.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.Repositories
{
    public interface IPollRepository : IRepository<Poll>
    {
        /// <summary>
        /// Gets the poll object asynchronously.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="channelId">The id of the channel.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Poll"/>.</returns>
        Task<Poll> GetAsync(ulong serverId, ulong channelId, ulong messageId);

        /// <summary>
        /// Gets all the polls of a server.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Poll"/>.</returns>
        Task<List<Poll>> ListAsync(ulong serverId);

        /// <summary>
        /// Gets all the polls of a specific channel.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="channelId">The id of the channel.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Poll"/>.</returns>
        Task<List<Poll>> ListAsync(ulong serverId, ulong channelId);

        /// <summary>
        /// Gets all the polls from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="expired">The expired state of the poll.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Poll"/>.</returns>
        Task<List<Poll>> ListAsync(ulong? serverId = null, bool? expired = null);
    }
}