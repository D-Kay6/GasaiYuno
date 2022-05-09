using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class ServerRepository : Repository<Server>, IServerRepository
{
    /// <inheritdoc />
    public ServerRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public async Task AddOrUpdateAsync(ulong serverId, string name)
    {
        var server = await Session.Query<Server>().FirstOrDefaultAsync(x => x.Identity == serverId).ConfigureAwait(false);
        if (server == null)
        {
            server = new Server
            {
                Identity = serverId,
                Language = Languages.English,
                Name = name,
                Prefix = "!",
                WarningDisabled = false
            };
            await Session.StoreAsync(server).ConfigureAwait(false);
            return;
        }

        server.Name = name;
    }

    /// <inheritdoc />
    public Task<Server> GetAsync(ulong serverId)
    {
        return Session.Query<Server>().FirstOrDefaultAsync(x => x.Identity == serverId);
    }

    /// <inheritdoc />
    public async Task<Server> GetOrAddAsync(ulong serverId, string name)
    {
        var server = await Session.Query<Server>().FirstOrDefaultAsync(x => x.Identity == serverId).ConfigureAwait(false);
        if (server == null)
        {
            server = new Server
            {
                Identity = serverId,
                Language = Languages.English,
                Name = name,
                Prefix = "!",
                WarningDisabled = false
            };
            await Session.StoreAsync(server).ConfigureAwait(false);
        }
        if (!server.Name.Equals(name, StringComparison.Ordinal))
        {
            server.Name = name;
        }
        if (string.IsNullOrEmpty(server.Prefix))
        {
            server.Prefix = "!";
        }

        await Session.SaveChangesAsync().ConfigureAwait(false);

        return server;
    }

    /// <inheritdoc />
    public Task<List<Server>> ListAsync()
    {
        return Session.Query<Server>().ToListAsync();
    }
}