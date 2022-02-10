using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Listing.Listeners;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Mediator.Events
{
    internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
    {
        private readonly ListingListener _listingListener;

        public ClientReadyEventHandler(ListingListener listingListener)
        {
            _listingListener = listingListener;
        }

        public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
        {
            await _listingListener.Start();
        }
    }
}