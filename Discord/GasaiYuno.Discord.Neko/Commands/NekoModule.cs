using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using Neko_Love.Net.V1;
using Neko_Love.Net.V1.Endpoints;
using Nekos.Net.V3;
using Nekos.Net.V3.Endpoints;
using RestSharp;

namespace GasaiYuno.Discord.Neko.Commands;

[EnabledInDm(false)]
[Group("neko", "Get a random image of a neko.")]
public class NekoModule : BaseInteractionModule<NekoModule>
{
    private readonly NekosV3Client _nekoClient;
    private readonly NekoV1Client _nsfwNekoClient;
    private readonly RestClient _restClient;

    public NekoModule(NekosV3Client nekoClient, NekoV1Client nsfwNekoClient)
    {
        _nekoClient = nekoClient;
        _nsfwNekoClient = nsfwNekoClient;
        _restClient = new RestClient();
    }

    public enum SfwImages
    {
        Nekomimi,
        Nekomimi_gif,
        Kitsune,
        Cat
    }

    [SlashCommand("random", "Get a random image of a neko.")]
    public async Task NormalNekoCommand([Summary("Category", "What category to pick from")]SfwImages category = SfwImages.Nekomimi)
    {
        await DeferAsync().ConfigureAwait(false);
        switch (category)
        {
            case SfwImages.Nekomimi:
                _nekoClient.WithSfwImgEndpoint(SfwImgEndpoint.Neko);
                break;
            case SfwImages.Nekomimi_gif:
                _nekoClient.WithSfwGifEndpoint(SfwGifEndpoint.Neko);
                break;
            case SfwImages.Kitsune:
                _nekoClient.WithSfwImgEndpoint(SfwImgEndpoint.Kitsune);
                break;
            case SfwImages.Cat:
                _nekoClient.WithSfwImgEndpoint(SfwImgEndpoint.Cat);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
        
        var response = await _nekoClient.GetAsync().ConfigureAwait(false);
        if (response.Status is { IsSuccess: false } || string.IsNullOrWhiteSpace(response.Data?.Response?.Url))
        {
            await FollowupAsync("I was unable to get an image at this time.").ConfigureAwait(false);
            return;
        }

        await SendNekoAsync(response.Data.Response.Url).ConfigureAwait(false);
    }

    [RequireNsfw]
    [SlashCommand("lewd", "Get a random lewd image of a neko. Only usable in NSFW channels.")]
    public async Task LewdNekoCommand()
    {
        await DeferAsync().ConfigureAwait(false);
        
        var response = await _nsfwNekoClient.RequestNsfwResultsAsync(NsfwEndpoint.Nekolewd).ConfigureAwait(false);
        var image = response?.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(image?.Url))
        {
            await FollowupAsync("I was unable to get an image at this time.").ConfigureAwait(false);
            return;
        }

        await SendNekoAsync(image.Url).ConfigureAwait(false);
    }

    private async Task SendNekoAsync(string url)
    {
        try
        {
            var extension = Path.GetExtension(url);
            var imageData = await _restClient.DownloadStreamAsync(new RestRequest(url)).ConfigureAwait(false);
            await FollowupWithFileAsync(imageData, $"Neko{extension}").ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await FollowupAsync(url).ConfigureAwait(false);
        }
    }
}