using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace GasaiYuno.Discord.Chatbot.Models;

internal class EndPoint
{
    private readonly RestClient _restClient;

    public EndPoint(string apiKey)
    {
        _restClient = new RestClient("https://www.cleverbot.com/", configureSerialization: x => x.UseNewtonsoftJson());
        _restClient.AddDefaultParameter("key", apiKey);
    }

    public async Task<Reply> GetReplyAsync(string message, string state)
    {
        var request = new RestRequest("getreply");
        request.AddQueryParameter("input", message);
        if (!string.IsNullOrEmpty(state))
            request.AddQueryParameter("cs", state);
            
        var response = await _restClient.GetAsync<Reply>(request).ConfigureAwait(false);
        return response;
    }
}