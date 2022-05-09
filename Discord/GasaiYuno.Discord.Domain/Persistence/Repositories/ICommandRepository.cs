using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories;

public interface ICommandRepository : IRepository<CustomCommand>
{
    /// <summary>
    /// Gets the custom command object asynchronously.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="command">The name of the command.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="CustomCommand"/>.</returns>
    Task<CustomCommand> GetAsync(ulong serverId, string command);

    /// <summary>
    /// Search for a custom command object asynchronously.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="command">The (partial) name of the command.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="CustomCommand"/>.</returns>
    Task<List<CustomCommand>> SearchAsync(ulong serverId, string command);

    /// <summary>
    /// Gets all the custom commands from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="CustomCommand"/>.</returns>
    Task<List<CustomCommand>> ListAsync(ulong serverId);
}