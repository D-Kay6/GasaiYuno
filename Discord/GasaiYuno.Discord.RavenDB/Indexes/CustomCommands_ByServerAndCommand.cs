using GasaiYuno.Discord.Domain.Models;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace GasaiYuno.Discord.RavenDB.Indexes;

public class CustomCommands_ByServerAndCommand : AbstractIndexCreationTask<CustomCommand>
{
    public CustomCommands_ByServerAndCommand()
    {
        Map = commands => from customCommand in commands
            select new
            {
                customCommand.Server,
                customCommand.Command
            };
    }
}