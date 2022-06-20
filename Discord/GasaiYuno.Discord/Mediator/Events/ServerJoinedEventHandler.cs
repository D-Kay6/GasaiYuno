using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Mediator.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events;

internal sealed class ServerJoinedEventHandler : INotificationHandler<ServerJoinedEvent>
{
    private readonly IMediator _mediator;

    public ServerJoinedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(ServerJoinedEvent command, CancellationToken cancellationToken)
    {
        return _mediator.Publish(new DeleteServerCommand(command.Id), cancellationToken);
    }
}