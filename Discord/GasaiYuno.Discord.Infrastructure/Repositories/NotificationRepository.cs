using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        /// <summary>
        /// Creates a new <see cref="NotificationRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public NotificationRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Notification> GetAsync(ulong serverId, NotificationType type)
        {
            return await Context.Notifications
                .Include(x => x.Server)
                .FirstOrDefaultAsync(x => x.Server.Id == serverId && x.Type == type)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<Notification> GetOrAddAsync(ulong serverId, NotificationType type, ulong? channel = null, string message = null, string image = null)
        {
            var entity = await GetAsync(serverId, type).ConfigureAwait(false);
            if (entity != null) return entity;

            var server = await Context.Servers.FindAsync(serverId).ConfigureAwait(false);
            var notification = Context.Notifications.Add(new Notification
            {
                Server = server,
                Type = type,
                Channel = channel,
                Message = message,
                Image = image
            });
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return notification.Entity;
        }

        /// <inheritdoc/>
        public async Task<List<Notification>> ListAsync(ulong serverId)
        {
            return await Context.Notifications
                .Where(x => x.Server.Id == serverId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}