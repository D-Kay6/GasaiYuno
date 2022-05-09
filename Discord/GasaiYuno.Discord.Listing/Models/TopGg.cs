using GasaiYuno.Discord.Listing.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listing.Models;

internal class TopGg : IEndpoint
{
    private readonly RestClient _client;

    public TopGg(EndpointConfig configuration)
    {
        if (string.IsNullOrEmpty(configuration.Url) || string.IsNullOrEmpty(configuration.Token))
            throw new ArgumentNullException(nameof(configuration));

        var clientOptions = new RestClientOptions()
        {
            CachePolicy = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            },
            BaseUrl = new Uri(configuration.Url)
        };
        _client = new RestClient(clientOptions)
        {
            Authenticator = new JwtAuthenticator(configuration.Token)
        };
    }

    /// <inheritdoc />
    public Task<RestResponse> SendUpdateAsync(ulong botId, int guildCount)
    {
        var request = new RestRequest($"bots/{botId}/stats")
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        request.AddOrUpdateParameter("server_count", guildCount);

        return _client.ExecutePostAsync(request);
    }

    /// <inheritdoc />
    public Task<RestResponse> SendUpdateAsync(ulong botId, int shardCount, int guildCount)
    {
        var request = new RestRequest($"bots/{botId}/stats")
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        request.AddOrUpdateParameter("server_count", guildCount);
        request.AddOrUpdateParameter("shard_count", shardCount);

        return _client.ExecutePostAsync(request);
    }

    /// <inheritdoc />
    public Task<RestResponse> SendUpdateAsync(ulong botId, int shardCount, int shardId, int guildCount)
    {
        var request = new RestRequest($"bots/{botId}/stats")
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        request.AddOrUpdateParameter("server_count", guildCount);
        request.AddOrUpdateParameter("shard_count", shardCount);
        request.AddOrUpdateParameter("shard_id", shardId);

        return _client.ExecutePostAsync(request);
    }
}