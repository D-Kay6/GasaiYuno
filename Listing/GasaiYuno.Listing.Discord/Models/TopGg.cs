using GasaiYuno.Listing.Discord.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net.Cache;
using System.Threading.Tasks;

namespace GasaiYuno.Listing.Discord.Models
{
    internal class TopGg : IEndpoint
    {
        private readonly RestClient _client;

        public TopGg(EndpointConfig configuration)
        {
            if (string.IsNullOrEmpty(configuration.Url) || string.IsNullOrEmpty(configuration.Token))
                throw new ArgumentNullException(nameof(configuration));

            _client = new RestClient
            {
                Authenticator = new JwtAuthenticator(configuration.Token),
                CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
                BaseUrl = new Uri(configuration.Url)
            };
        }

        /// <inheritdoc />
        public Task<IRestResponse> SendUpdateAsync(ulong botId, int guildCount)
        {
            var request = new RestRequest($"bots/{botId}/stats")
            {
                RequestFormat = DataFormat.Json,
                Method = Method.POST
            };
            request.AddOrUpdateParameter("server_count", guildCount);

            return _client.ExecutePostAsync(request);
        }

        /// <inheritdoc />
        public Task<IRestResponse> SendUpdateAsync(ulong botId, int shardCount, int guildCount)
        {
            var request = new RestRequest($"bots/{botId}/stats")
            {
                RequestFormat = DataFormat.Json,
                Method = Method.POST
            };
            request.AddOrUpdateParameter("server_count", guildCount);
            request.AddOrUpdateParameter("shard_count", shardCount);

            return _client.ExecutePostAsync(request);
        }

        /// <inheritdoc />
        public Task<IRestResponse> SendUpdateAsync(ulong botId, int shardCount, int shardId, int guildCount)
        {
            var request = new RestRequest($"bots/{botId}/stats")
            {
                RequestFormat = DataFormat.Json,
                Method = Method.POST
            };
            request.AddOrUpdateParameter("server_count", guildCount);
            request.AddOrUpdateParameter("shard_count", shardCount);
            request.AddOrUpdateParameter("shard_id", shardId);

            return _client.ExecutePostAsync(request);
        }
    }
}