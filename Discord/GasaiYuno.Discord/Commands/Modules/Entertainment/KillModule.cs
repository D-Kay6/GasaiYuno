using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Extensions;
using GasaiYuno.Interface.Storage;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Kill")]
    public class KillModule : BaseModule<KillModule>
    {
        private readonly IImageStorage _imageStorage;

        public KillModule(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        [Priority(-1)]
        [Command]
        public Task KillDefaultAsync([Remainder] string name) => ReplyAsync(Translation.Message("Generic.Invalid.User", name));

        [Command]
        public async Task KillDefaultAsync(SocketGuildUser user)
        {
            switch (user.Id)
            {
                case 255453041531158538:
                    await ReplyAsync(Translation.Message("Entertainment.Kill.Creator", user.Nickname()));
                    break;
                case 286972781273546762:
                    await ReplyAsync(Translation.Message("Entertainment.Kill.Self"));
                    break;
                default:
                    var image = await _imageStorage.GetImageAsync("GasaiYuno.gif", "Core").ConfigureAwait(false);
                    await Context.Channel.SendFileAsync(image, Translation.Message("Entertainment.Kill.Default", user.Mention));
                    break;
            }
        }
    }
}