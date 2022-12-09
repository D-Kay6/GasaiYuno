using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Mediator.Commands;
using GasaiYuno.Discord.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Events;

internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
{
    private readonly IListener[] _listeners;
    private readonly IMediator _mediator;

    public ClientReadyEventHandler(IListener[] listeners, IMediator mediator)
    {
        _listeners = listeners;
        _mediator = mediator;
    }

    public async Task Handle(ClientReadyEvent command, CancellationToken cancellationToken)
    {
        foreach (var listener in _listeners.OrderByDescending(x => x.Priority))
        {
            await listener.Start().ConfigureAwait(false);
        }
        
        var servers = await _mediator.Send(new ListServersRequest(), cancellationToken).ConfigureAwait(false);
        foreach (var server in servers.Where(server => command.Client.GetGuild(server.Identity) == null))
            await _mediator.Publish(new DeleteServerCommand(server.Identity), cancellationToken).ConfigureAwait(false);

        foreach (var guild in command.Client.Guilds)
        {
            await _mediator.Publish(new UpdateServerCommand(guild.Id, guild.Name), cancellationToken).ConfigureAwait(false);
        }
    }
}