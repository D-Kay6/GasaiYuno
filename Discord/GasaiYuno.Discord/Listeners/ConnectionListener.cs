using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Models;
using MediatR;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    public class ConnectionListener : IDisposable
    {
        private readonly DiscordConnectionClient _client;
        private readonly IMediator _mediator;

        public ConnectionListener(DiscordConnectionClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        public void Start()
        {
            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
        {
            return _mediator.Publish(new ClientReadyEvent(_client));
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}