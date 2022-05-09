using GasaiYuno.Discord.Listing.Ínterfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Mediator.Events;

public class GuildCountChangedEventHandler : INotificationHandler<GuildCountChangedEvent>
{
    private readonly IListingUpdater _listingUpdater;

    public GuildCountChangedEventHandler(IListingUpdater listingUpdater)
    {
        _listingUpdater = listingUpdater;
    }

    public Task Handle(GuildCountChangedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ShardId >= 0)
            return _listingUpdater.UpdateStatsAsync(notification.BotId, notification.ShardCount, notification.ShardId, notification.GuildCount);

        if (notification.ShardCount >= 0)
            return _listingUpdater.UpdateStatsAsync(notification.BotId, notification.ShardCount, notification.GuildCount);

        return _listingUpdater.UpdateStatsAsync(notification.BotId, notification.GuildCount);
    }
}