using GasaiYuno.Discord.Listing.Interfaces;
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

    public Task Handle(GuildCountChangedEvent command, CancellationToken cancellationToken)
    {
        if (command.ShardId >= 0)
            return _listingUpdater.UpdateStatsAsync(command.BotId, command.ShardCount, command.ShardId, command.GuildCount);

        if (command.ShardCount >= 0)
            return _listingUpdater.UpdateStatsAsync(command.BotId, command.ShardCount, command.GuildCount);

        return _listingUpdater.UpdateStatsAsync(command.BotId, command.GuildCount);
    }
}