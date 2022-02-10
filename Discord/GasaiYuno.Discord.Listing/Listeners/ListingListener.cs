using Discord.WebSocket;
using GasaiYuno.Discord.Listing.Mediator.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Listeners
{
    internal class ListingListener
    {
        private readonly DiscordShardedClient _client;
        private readonly IMediator _mediator;
        private readonly ILogger<ListingListener> _logger;

        public ListingListener(DiscordShardedClient client, IMediator mediator, ILogger<ListingListener> logger)
        {
            _client = client;
            _mediator = mediator;
            _logger = logger;
        }

        public Task Start()
        {
#if !DEBUG
        _client.JoinedGuild += OnJoinedGuild;
        _client.LeftGuild += OnLeftGuild;
        return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
#else
            return Task.CompletedTask;
#endif
        }

        private Task OnJoinedGuild(SocketGuild arg)
        {
            return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
        }

        private Task OnLeftGuild(SocketGuild arg)
        {
            return _mediator.Publish(new GuildCountChangedEvent(_client.CurrentUser.Id, _client.Shards.Count, _client.Guilds.Count));
        }
    }
}