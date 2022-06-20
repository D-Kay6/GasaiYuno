using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events;

internal sealed class ServerChangedEventHandler : INotificationHandler<ServerChangedEvent>
{
    private readonly IMediator _mediator;

    public ServerChangedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(ServerChangedEvent command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.OldGuild.Name)) return;
        if (command.OldGuild.Name.Equals(command.NewGuild.Name)) return;
        
        await _mediator.Publish(new UpdateServerCommand(command.NewGuild.Id, command.NewGuild.Name), cancellationToken).ConfigureAwait(false);
    }
}