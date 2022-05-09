using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class StickyMessageRepository : Repository<StickyMessage>, IStickyMessageRepository
{
    /// <inheritdoc />
    public StickyMessageRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<StickyMessage> GetAsync(ulong serverId, ulong channelId, ulong messageId)
    {
        return Session.Query<StickyMessage>().SingleOrDefaultAsync(x => x.Server == serverId && x.Channel == channelId && x.Message == messageId);
    }

    /// <inheritdoc />
    public Task<List<StickyMessage>> ListAsync(ulong serverId)
    {
        return Session.Query<StickyMessage>().Where(x => x.Server == serverId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<StickyMessage>> ListAsync(ulong serverId, ulong channelId)
    {
        return Session.Query<StickyMessage>().Where(x => x.Server == serverId && x.Channel == channelId).ToListAsync();
    }
}