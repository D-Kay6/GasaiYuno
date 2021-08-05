using GasaiYuno.Discord.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.Repositories
{
    public interface IDynamicChannelRepository : IRepository<DynamicChannel>
    {
        /// <summary>
        /// Gets the dynamic channel object asynchronously.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="name">The name of the configuration.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="DynamicChannel"/>.</returns>
        Task<DynamicChannel> GetAsync(ulong serverId, string name);

        /// <summary>
        /// Gets all the dynamic channels from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="DynamicChannel"/>.</returns>
        Task<List<DynamicChannel>> ListAsync(ulong serverId);

        /// <summary>
        /// Gets all the dynamic channels from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="type">The type of automation.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="DynamicChannel"/>.</returns>
        Task<List<DynamicChannel>> ListAsync(ulong serverId, AutomationType type);
    }
}