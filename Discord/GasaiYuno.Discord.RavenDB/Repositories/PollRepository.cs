using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class PollRepository : Repository<Poll>, IPollRepository
{
    /// <inheritdoc />
    public PollRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<Poll> GetAsync(ulong id)
    {
        return Session.Query<Poll>().SingleOrDefaultAsync(x => x.Identity == id);
    }

    /// <inheritdoc />
    public Task<Poll> GetAsync(ulong serverId, ulong channelId, ulong messageId)
    {
        return Session.Query<Poll>().SingleOrDefaultAsync(x => x.Server == serverId && x.Channel == channelId && x.Message == messageId);
    }

    /// <inheritdoc />
    public Task<List<Poll>> ListAsync(ulong serverId)
    {
        return Session.Query<Poll>().Where(x => x.Server == serverId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<Poll>> ListAsync(ulong serverId, ulong channelId)
    {
        return Session.Query<Poll>().Where(x => x.Server == serverId && x.Channel == channelId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<Poll>> ListAsync(ulong? serverId = null, bool? expired = null)
    {
        var query = Session.Query<Poll>();
        if (serverId.HasValue) query = query.Where(x => x.Server == serverId);
        if (expired.HasValue) query = query.Where(x => x.EndDate < DateTime.Now);
        return query.ToListAsync();
    }
}