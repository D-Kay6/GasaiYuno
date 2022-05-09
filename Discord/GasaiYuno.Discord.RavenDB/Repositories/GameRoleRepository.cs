using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class GameRoleRepository : Repository<GameRole>, IGameRoleRepository
{
    /// <inheritdoc />
    public GameRoleRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<GameRole> GetAsync(ulong serverId, string name, AutomationType type)
    {
        return Session.Query<GameRole>().Where(x => x.Server == serverId && x.Name == name && x.Type == type, false).SingleOrDefaultAsync();
    }

    /// <inheritdoc />
    public Task<List<GameRole>> ListAsync(ulong serverId)
    {
        return Session.Query<GameRole>().Where(x => x.Server == serverId).ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<GameRole>> ListAsync(ulong serverId, AutomationType type)
    {
        return Session.Query<GameRole>().Where(x => x.Server == serverId && x.Type == type).ToListAsync();
    }
}