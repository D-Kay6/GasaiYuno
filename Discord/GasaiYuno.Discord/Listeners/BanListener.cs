﻿using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class BanListener : IDisposable
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly ILocalization _localization;
        private readonly ILogger<BanListener> _logger;
        private readonly Timer _timer;

        public BanListener(Connection connection, Func<IUnitOfWork> unitOfWorkFactory, ILocalization localization, ILogger<BanListener> logger)
        {
            _client = connection.Client;
            _unitOfWorkFactory = unitOfWorkFactory;
            _localization = localization;
            _logger = logger;
            _timer = new Timer(CheckBans, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            connection.Ready += OnReady;
        }

        private Task OnReady()
        {
            _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async void CheckBans(object stateInfo)
        {
            var unitOfWork = _unitOfWorkFactory();
            var bans = await unitOfWork.Bans.ListAsync(expired: true).ConfigureAwait(false);
            foreach (var ban in bans)
            {
                var server = _client.GetGuild(ban.Server.Id);
                if (server == null) continue;

                var serverBan = await server.GetBanAsync(ban.User).ConfigureAwait(false);
                if (serverBan != null)
                {
                    try
                    {
                        await server.RemoveBanAsync(ban.User).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unable to remove ban {ban} from server {serverName}({serverId})", ban, server.Name, server.Id);
                    }
                }
                
                unitOfWork.Bans.Remove(ban);
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}