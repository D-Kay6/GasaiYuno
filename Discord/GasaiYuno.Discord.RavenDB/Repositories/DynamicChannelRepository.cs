using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class DynamicChannelRepository : Repository<DynamicChannel>, IDynamicChannelRepository
{
    /// <inheritdoc />
    public DynamicChannelRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<DynamicChannel> GetAsync(ulong serverId, string name)
    {
        return Session.Query<DynamicChannel>().Where(x => x.Server == serverId && x.Name == name, false).SingleOrDefaultAsync();
    }

    /// <inheritdoc />
    public Task<List<DynamicChannel>> SearchAsync(ulong serverId, string name)
    {
        return Session.Query<DynamicChannel>().Where(x => x.Server == serverId).Search(x => x.Name, $"*{name}*").ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<DynamicChannel>> ListAsync(ulong serverId)
    {
        return Session.Query<DynamicChannel>().Where(x => x.Server == serverId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<DynamicChannel>> ListAsync(ulong serverId, AutomationType type)
    {
        return Session.Query<DynamicChannel>().Where(x => x.Server == serverId && x.Type == type).ToListAsync();
    }
}