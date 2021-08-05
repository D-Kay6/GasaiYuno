using GasaiYuno.Discord.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        /// <summary>
        /// Gets the notification object asynchronously.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="type">The type of the notification.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Notification"/>.</returns>
        Task<Notification> GetAsync(ulong serverId, NotificationType type);

        /// <summary>
        /// Gets the notification object asynchronously.
        /// And it adds the notification to the database if it doesn't exist.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <param name="type">The type of the notification.</param>
        /// <param name="channel">The channel id of the notification.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="image">The image of the notification.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Notification"/>.</returns>
        Task<Notification> GetOrAddAsync(ulong serverId, NotificationType type, ulong? channel = null, string message = null, string image = null);

        /// <summary>
        /// Gets all the notifications of a server.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Notification"/>.</returns>
        Task<List<Notification>> ListAsync(ulong serverId);
    }
}