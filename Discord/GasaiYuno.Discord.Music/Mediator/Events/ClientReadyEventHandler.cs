using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Music.Listeners;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Music.Mediator.Events
{
    internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
    {
        private readonly MusicListener _musicListener;

        public ClientReadyEventHandler(MusicListener musicListener)
        {
            _musicListener = musicListener;
        }

        public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
        {
            await _musicListener.Start();
        }
    }
}