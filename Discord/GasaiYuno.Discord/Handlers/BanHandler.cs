using Discord.WebSocket;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public class BanHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork<IBanRepository>> _banRepository;
        private readonly ILocalization _localization;
        private readonly ILogger<BanHandler> _logger;
        private readonly Timer _timer;

        public BanHandler(DiscordShardedClient client, Func<IUnitOfWork<IBanRepository>> banRepository, ILocalization localization, ILogger<BanHandler> logger)
        {
            _client = client;
            _banRepository = banRepository;
            _localization = localization;
            _logger = logger;

            _timer = new Timer(CheckBans, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public Task Ready()
        {
            _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async void CheckBans(object stateInfo)
        {
            var repository = _banRepository();
            var bans = await repository.DataSet.ListAsync(expired: true).ConfigureAwait(false);
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

                await repository.BeginAsync().ConfigureAwait(false);
                repository.DataSet.Remove(ban);
                await repository.SaveAsync().ConfigureAwait(false);
            }

            _timer.Change(TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
        }
    }
}