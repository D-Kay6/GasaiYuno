using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class DistributionRoleRepository : Repository<DistributionRole>, IDistributionRoleRepository
{
    /// <inheritdoc />
    public DistributionRoleRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<DistributionRole> GetAsync(ulong serverId, ulong channelId, ulong messageId)
    {
        return Session.Query<DistributionRole>().SingleOrDefaultAsync(x => x.Server == serverId && x.Channel == channelId && x.Message == messageId);
    }

    /// <inheritdoc />
    public Task<List<DistributionRole>> ListAsync(ulong serverId)
    {
        return Session.Query<DistributionRole>().Where(x => x.Server == serverId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<DistributionRole>> ListAsync(ulong serverId, ulong channelId)
    {
        return Session.Query<DistributionRole>().Where(x => x.Server == serverId && x.Channel == channelId).ToListAsync();
    }
}