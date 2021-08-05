using GasaiYuno.Discord.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.Repositories
{
    public interface IServerRepository : IRepository<Server>
    {
        /// <summary>
        /// Adds the server to the database.
        /// If it already exists, it will be updated.
        /// </summary>
        /// <param name="serverId">The serverId of the server.</param>
        /// <param name="name">The name of the server.</param>
        /// <returns>A<see cref="Task"/></returns>
        Task AddOrUpdateAsync(ulong serverId, string name);

        /// <summary>
        /// Gets the server object asynchronously.
        /// </summary>
        /// <param name="serverId">The serverId of the server.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Server"/>.</returns>
        Task<Server> GetAsync(ulong serverId);

        /// <summary>
        /// Gets the server if it exists in the database.
        /// And it adds the server to the database if it doesn't exist.
        /// </summary>
        /// <param name="serverId">The serverId of the server.</param>
        /// <param name="name">The name of the server.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Server"/>.</returns>
        Task<Server> GetOrAddAsync(ulong serverId, string name);

        /// <summary>
        /// Gets all server objects asynchronously.
        /// </summary>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Server"/>.</returns>
        Task<List<Server>> ListAsync();
    }
}