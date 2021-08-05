using Discord.WebSocket;
using GasaiYuno.Discord.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public class NotificationHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationHandler> _logger;

        public NotificationHandler(DiscordShardedClient client, NotificationService notificationService, ILogger<NotificationHandler> logger)
        {
            _client = client;
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task Ready()
        {
            _client.UserJoined += OnUserJoinedAsync;
            return Task.CompletedTask;
        }

        private async Task OnUserJoinedAsync(SocketGuildUser user)
        {
            await _notificationService.WelcomeUserAsync(user).ConfigureAwait(false);
        }
    }
}