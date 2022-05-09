using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.Repositories;
using GasaiYuno.Discord.RavenDB.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB.Repositories;

public class CommandRepository : Repository<CustomCommand>, ICommandRepository
{
    /// <inheritdoc />
    public CommandRepository(IAsyncDocumentSession session) : base(session) { }
        
    /// <inheritdoc />
    public Task<CustomCommand> GetAsync(ulong serverId, string command)
    {
        return Session.Query<CustomCommand, CustomCommands_ByServerAndCommand>().Where(x => x.Server == serverId && x.Command == command, false).SingleOrDefaultAsync();
    }

    /// <inheritdoc />
    public Task<List<CustomCommand>> SearchAsync(ulong serverId, string command)
    {
        return Session.Query<CustomCommand, CustomCommands_ByServerAndCommand>().Where(x => x.Server == serverId).Search(x => x.Command, $"*{command}*").ToListAsync();
    }

    /// <inheritdoc />
    public Task<List<CustomCommand>> ListAsync(ulong serverId)
    {
        return Session.Query<CustomCommand>().Where(x => x.Server == serverId).ToListAsync();
    }
}