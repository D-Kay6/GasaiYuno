using GasaiYuno.Discord.Chatbot.Listeners;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Mediator.Events
{
    internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
    {
        private readonly ChatListener _threadListener;

        public ClientReadyEventHandler(ChatListener threadListener)
        {
            _threadListener = threadListener;
        }

        public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
        {
            await _threadListener.Start().ConfigureAwait(false);
        }
    }
}