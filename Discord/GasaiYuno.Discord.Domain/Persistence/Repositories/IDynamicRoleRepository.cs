using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories
{
    public interface IDynamicRoleRepository : IRepository<DynamicRole>
    {
        /// <summary>
        /// Gets the dynamic role object asynchronously.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="type">The type of automation.</param>
        /// <param name="status">The status assigned to the role.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="DynamicRole"/>.</returns>
        Task<DynamicRole> GetAsync(ulong serverId, AutomationType type, string status);

        /// <summary>
        /// Gets all the dynamic roles from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="DynamicRole"/>.</returns>
        Task<List<DynamicRole>> ListAsync(ulong serverId);

        /// <summary>
        /// Gets all the dynamic roles from the database.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="type">The type of automation.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="DynamicRole"/>.</returns>
        Task<List<DynamicRole>> ListAsync(ulong serverId, AutomationType type);
    }
}