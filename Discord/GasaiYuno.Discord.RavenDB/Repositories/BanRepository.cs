using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class BanRepository : Repository<Ban>, IBanRepository
{
    /// <inheritdoc />
    public BanRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<Ban> GetAsync(ulong serverId, ulong userId)
    {
        return Session.Query<Ban>().SingleOrDefaultAsync(x => x.Server == serverId && x.User == userId);
    }

    /// <inheritdoc />
    public Task<List<Ban>> ListAsync(ulong? serverId = null, bool? expired = null)
    {
        var query = Session.Query<Ban>();
        if (serverId.HasValue) query = query.Where(x => x.Server == serverId);
        if (expired.HasValue) query = query.Where(x => x.EndDate < DateTime.Now);
        return query.ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<Ban>> ListAsync(Expression<Func<Ban, bool>> predicate)
    {
        return Session.Query<Ban>().Where(predicate).ToListAsync();
    }
}