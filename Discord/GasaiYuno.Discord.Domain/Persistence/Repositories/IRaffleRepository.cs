using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories;

public interface IRaffleRepository : IRepository<Raffle>
{
    /// <summary>
    /// Gets the raffle object asynchronously.
    /// </summary>
    /// <param name="id">The id of the raffle.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Raffle"/>.</returns>
    Task<Raffle> GetAsync(ulong id);

    /// <summary>
    /// Gets the raffle object asynchronously.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="channelId">The id of the channel.</param>
    /// <param name="messageId">The id of the message.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Raffle"/>.</returns>
    Task<Raffle> GetAsync(ulong serverId, ulong channelId, ulong messageId);

    /// <summary>
    /// Gets all the raffles of a server.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Raffle"/>.</returns>
    Task<List<Raffle>> ListAsync(ulong serverId);

    /// <summary>
    /// Gets all the raffles of a specific channel.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="channelId">The id of the channel.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Raffle"/>.</returns>
    Task<List<Raffle>> ListAsync(ulong serverId, ulong channelId);

    /// <summary>
    /// Gets all the raffles from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="expired">The expired state of the raffle.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Raffle"/>.</returns>
    Task<List<Raffle>> ListAsync(ulong? serverId = null, bool? expired = null);
}