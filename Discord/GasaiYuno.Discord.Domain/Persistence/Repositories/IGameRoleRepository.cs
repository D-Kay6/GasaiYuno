using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories;

public interface IGameRoleRepository : IRepository<GameRole>
{
    /// <summary>
    /// Gets the game role object asynchronously.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="name">The name of the configuration.</param>
    /// <param name="type">The type of automation.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="GameRole"/>.</returns>
    Task<GameRole> GetAsync(ulong serverId, string name, AutomationType type);

    /// <summary>
    /// Gets all the game roles from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="GameRole"/>.</returns>
    Task<List<GameRole>> ListAsync(ulong serverId);

    /// <summary>
    /// Gets all the game roles from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="type">The type of automation.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="GameRole"/>.</returns>
    Task<List<GameRole>> ListAsync(ulong serverId, AutomationType type);
}