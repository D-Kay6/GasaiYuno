using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events;

internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
{
    private readonly IListener[] _listeners;

    public ClientReadyEventHandler(IListener[] listeners)
    {
        _listeners = listeners;
    }

    public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
    {
        foreach (var listener in _listeners.OrderByDescending(x => x.Priority))
        {
            await listener.Start().ConfigureAwait(false);
        }
    }
}