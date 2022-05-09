using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    /// <inheritdoc />
    public NotificationRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<Notification> GetAsync(ulong serverId, NotificationType type)
    {
        return Session.Query<Notification>().SingleOrDefaultAsync(x => x.Server == serverId && x.Type == type);
    }

    /// <inheritdoc />
    public async Task<Notification> GetOrAddAsync(ulong serverId, NotificationType type, ulong? channel = null, string message = null, string image = null)
    {
        var notification = await GetAsync(serverId, type).ConfigureAwait(false);
        if (notification == null)
        {
            notification = new Notification
            {
                Server = serverId,
                Type = type,
                Channel = channel,
                Message = message ?? "Welcome to the party [user]. Hope you will have a good time with us.",
                Image = image
            };
            await Session.StoreAsync(notification).ConfigureAwait(false);
        }
        return notification;
    }

    /// <inheritdoc />
    public Task<List<Notification>> ListAsync(ulong serverId)
    {
        return Session.Query<Notification>().Where(x => x.Server == serverId).ToListAsync();
    }
}