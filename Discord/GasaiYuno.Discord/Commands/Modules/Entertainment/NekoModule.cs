using Discord.Commands;
using HtmlAgilityPack;
using RestSharp;
using System.IO;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Neko")]
    public class NekoModule : BaseModule<NekoModule>
    {
        private readonly RestClient _restClient;

        public NekoModule()
        {
            _restClient = new RestClient("https://nekos.life/");
        }

        [Command]
        public async Task DefaultNekoAsync()
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

        [Command("lewd")]
        [RequireNsfw]
        public async Task LewdNekoAsync()
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
                await ReplyAsync("Error");
                return;
            }

            var url = imageNode.GetAttributeValue("src", null);
            if (url == null)
            {
                await ReplyAsync("Error");
                return;
            }

            try
            {
                var extension = Path.GetExtension(url);
                var imageData = _restClient.DownloadData(new RestRequest(url));
                var stream = new MemoryStream(imageData);
                await Context.Channel.SendFileAsync(stream, $"Neko{extension}").ConfigureAwait(false);
            }
            catch
            {
                await ReplyAsync(url).ConfigureAwait(false);
            }
        }
    }
}