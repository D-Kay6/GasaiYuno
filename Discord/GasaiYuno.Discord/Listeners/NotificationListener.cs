using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class NotificationListener
    {
        private readonly DiscordShardedClient _client;
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationListener> _logger;

        public NotificationListener(Connection connection, NotificationService notificationService, ILogger<NotificationListener> logger)
        {
            _client = connection.Client;
            _notificationService = notificationService;
            _logger = logger;

            connection.Ready += OnReady;
        }

        private Task OnReady()
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