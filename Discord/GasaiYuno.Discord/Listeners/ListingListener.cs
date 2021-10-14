using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using GasaiYuno.Interface.Listing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class ListingListener
    {
        private readonly DiscordShardedClient _client;
        private readonly IListingUpdater _listingUpdater;
        private readonly ILogger<ListingListener> _logger;

        public ListingListener(DiscordConnectionClient client, IListingUpdater listingUpdater, ILogger<ListingListener> logger)
        {
            _client = client;
            _listingUpdater = listingUpdater;
            _logger = logger;

            client.Ready += OnReady;
        }

        private Task OnReady()
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