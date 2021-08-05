using GasaiYuno.Interface.Listing;
using GasaiYuno.Interface.Storage;
using GasaiYuno.Listing.Discord.Configuration;
using GasaiYuno.Listing.Discord.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Listing.Discord
{
    public class UpdateService : IListingUpdater
    {
        private readonly BlockingCollection<IEndpoint> _endpoints;
        private readonly ILogger<UpdateService> _logger;

        public UpdateService(IConfigStorage configStorage, ILogger<UpdateService> logger)
        {
            _endpoints = new BlockingCollection<IEndpoint>();
            _logger = logger;

            var config = configStorage.Read<ListingConfig>();
            _endpoints.TryAdd(new TopGg(config.TopGg));

            _endpoints.CompleteAdding();
        }

        /// <inheritdoc />
        public Task UpdateStatsAsync(ulong botId, int guildCount)
        {
            var tasks = _endpoints.Select(endpoint => Task.Run(async () =>
                {
                    var result = await endpoint.SendUpdateAsync(botId, guildCount).ConfigureAwait(false);
                    if (result.IsSuccessful) return;

                    _logger.LogError(result.ErrorException, "Couldn't update stats for {Endpoint}. Http: {Code}, {Message}", result.ResponseUri, result.StatusCode, result.StatusDescription);
                }))
                .ToList();

            return Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public Task UpdateStatsAsync(ulong botId, int shardCount, int guildCount)
        {
            var tasks = _endpoints.Select(restClient => Task.Run(async () =>
                {
                    var result = await restClient.SendUpdateAsync(botId, shardCount, guildCount).ConfigureAwait(false);
                    if (result.IsSuccessful) return;

                    _logger.LogError(result.ErrorException, "Couldn't update stats for {Endpoint}. Http: {Code}, {Message}", result.ResponseUri, result.StatusCode, result.StatusDescription);
                }))
                .ToList();

            return Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public Task UpdateStatsAsync(ulong botId, int shardCount, int shardId, int guildCount)
        {
            var tasks = _endpoints.Select(restClient => Task.Run(async () =>
                {
                    var result = await restClient.SendUpdateAsync(botId, shardCount, shardId, guildCount).ConfigureAwait(false);
                    if (result.IsSuccessful) return;

                    _logger.LogError(result.ErrorException, "Couldn't update stats for {Endpoint}. Http: {Code}, {Message}", result.ResponseUri, result.StatusCode, result.StatusDescription);
                }))
                .ToList();

            return Task.WhenAll(tasks);
        }

        /// <inheritdoc />
        public Task UpdateStatsAsync(ulong botId, int shardCount, int[] guildCounts)
        {
            var tasks = guildCounts.Select((t, i) => UpdateStatsAsync(botId, shardCount, i, t)).ToList();
            return Task.WhenAll(tasks);
        }
    }
}