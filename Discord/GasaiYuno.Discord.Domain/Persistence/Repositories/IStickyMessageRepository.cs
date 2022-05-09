using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.Repositories;

public interface IStickyMessageRepository : IRepository<StickyMessage>
{
    /// <summary>
    /// Gets the sticky message object asynchronously.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="channelId">The id of the channel.</param>
    /// <param name="messageId">The id of the message.</param>
    /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="StickyMessage"/>.</returns>
    Task<StickyMessage> GetAsync(ulong serverId, ulong channelId, ulong messageId);

    /// <summary>
    /// Gets all the sticky messages from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="StickyMessage"/>.</returns>
    Task<List<StickyMessage>> ListAsync(ulong serverId);

    /// <summary>
    /// Gets all the sticky messages from the database.
    /// </summary>
    /// <param name="serverId">The id of the server.</param>
    /// <param name="channelId">The id of the channel.</param>
    /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="StickyMessage"/>.</returns>
    Task<List<StickyMessage>> ListAsync(ulong serverId, ulong channelId);
}