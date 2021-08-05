using Discord.WebSocket;
using GasaiYuno.Interface.Listing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public class ListingHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly IListingUpdater _listingUpdater;
        private readonly ILogger<ListingHandler> _logger;

        public ListingHandler(DiscordShardedClient client, IListingUpdater listingUpdater, ILogger<ListingHandler> logger)
        {
            _client = client;
            _listingUpdater = listingUpdater;
            _logger = logger;
        }

        public Task Ready()
        {
#if !DEBUG
            _client.JoinedGuild += OnJoinedGuild;
            _client.LeftGuild += OnLeftGuild;

            return _listingUpdater.UpdateStatsAsync(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count);
#else
            return Task.CompletedTask;
#endif
        }

        private Task OnJoinedGuild(SocketGuild arg)
        {
            return _listingUpdater.UpdateStatsAsync(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count);
        }

        private Task OnLeftGuild(SocketGuild arg)
        {
            return _listingUpdater.UpdateStatsAsync(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count);
        }
    }
}