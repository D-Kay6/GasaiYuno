using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Commands;

public record UpdateServerCommand : INotification
{
    public ulong Id { get; }
    public string Name { get; }
    public Languages? Language { get; }
    public bool? WarningDisabled { get; }

    public UpdateServerCommand(Server server)
    {
        Id = server.Identity;
        Name = server.Name;
        Language = server.Language;
        WarningDisabled = server.WarningDisabled;
    }

    public UpdateServerCommand(ulong id, string name, Languages? language = null, bool? warningDisabled = null)
    {
        Id = id;
        Name = name;
        Language = language;
        WarningDisabled = warningDisabled;
    }
}