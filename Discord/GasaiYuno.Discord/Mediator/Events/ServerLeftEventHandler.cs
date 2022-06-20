using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Mediator.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events;

internal sealed class ServerLeftEventHandler : INotificationHandler<ServerLeftEvent>
{
    private readonly IMediator _mediator;

    public ServerLeftEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(ServerLeftEvent command, CancellationToken cancellationToken)
    {
        return _mediator.Publish(new DeleteServerCommand(command.Id), cancellationToken);
    }
}