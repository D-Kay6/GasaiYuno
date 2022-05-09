using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using HtmlAgilityPack;
using RestSharp;
using System.IO;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment;

[EnabledInDm(false)]
[Group("neko", "Get a random image of a neko.")]
public class NekoModule : BaseInteractionModule<NekoModule>
{
    private readonly RestClient _restClient;

    public NekoModule()
    {
        _restClient = new RestClient("https://nekos.life/");
    }

    [SlashCommand("normal", "Get a random image of a neko.")]
    public async Task NormalNekoCommand()
    {
        var typingState = Context.Channel.EnterTypingState();
        try
        {
            var response = await _restClient.ExecuteGetAsync(new RestRequest()).ConfigureAwait(false);
            await SendNekoAsync(response.Content).ConfigureAwait(false);
        }
        finally
        {
            typingState.Dispose();
        }
    }

    [RequireNsfw]
    [SlashCommand("lewd", "Get a random lewd image of a neko. Only usable in NSFW channels.")]
    public async Task LewdNekoCommand()
    {
        var typingState = Context.Channel.EnterTypingState();
        try
        {
            var response = await _restClient.ExecuteGetAsync(new RestRequest("/lewd")).ConfigureAwait(false);
            await SendNekoAsync(response.Content).ConfigureAwait(false);
        }
        finally
        {
            typingState.Dispose();
        }
    }

    private async Task SendNekoAsync(string content)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);
        var imageNode = htmlDocument.DocumentNode.SelectSingleNode("//img[@alt='neko']");
        if (imageNode == null)
        {
            await RespondAsync("Error", ephemeral: true).ConfigureAwait(false);
            return;
        }

        var url = imageNode.GetAttributeValue("src", null);
        if (url == null)
        {
            await RespondAsync("Error", ephemeral: true).ConfigureAwait(false);
            return;
        }

        try
        {
            var extension = Path.GetExtension(url);
            var imageData = await _restClient.DownloadStreamAsync(new RestRequest(url)).ConfigureAwait(false);
            await RespondWithFileAsync(imageData, $"Neko{extension}").ConfigureAwait(false);
        }
        catch
        {
            await RespondAsync(url).ConfigureAwait(false);
        }
    }
}