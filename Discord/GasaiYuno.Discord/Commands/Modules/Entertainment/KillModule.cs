using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Mediator.Requests;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Kill")]
    public class KillModule : BaseModule<KillModule>
    {
        [Priority(-1)]
        [Command]
        public Task KillDefaultAsync([Remainder] string name) => ReplyAsync(Translation.Message("Generic.Invalid.User", name));

        [Command]
        public async Task KillDefaultAsync(SocketGuildUser user)
        {
            switch (user.Id)
            {
                case 255453041531158538:
                    await ReplyAsync(Translation.Message("Entertainment.Kill.Creator", user.Nickname())).ConfigureAwait(false);
                    break;
                case 286972781273546762:
                case 542706288849715202:
                    await ReplyAsync(Translation.Message("Entertainment.Kill.Self")).ConfigureAwait(false);
                    break;
                default:
                    var image = await Mediator.Send(new GetImageRequest("GasaiYuno.gif", "Core")).ConfigureAwait(false);
                    await Context.Channel.SendFileAsync(new FileAttachment(image), Translation.Message("Entertainment.Kill.Default", user.Mention)).ConfigureAwait(false);
                    break;
            }
        }
    }
}