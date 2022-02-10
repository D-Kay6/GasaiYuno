using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Birthday")]
    public class BirthdayModule : BaseModule<BirthdayModule>
    {
        [Priority(-1)]
        [Command]
        public async Task BirthdayDefaultAsync([Remainder] string name)
        {
            await ReplyAsync(Translation.Message("Generic.Invalid.User", name));
        }

        [Command]
        public async Task BirthdayDefaultAsync(SocketGuildUser user)
        {
            if (user == null)
            {
                await ReplyAsync(Translation.Message("Generic.Invalid.User", Context.Message));
                return;
            }
            
            await ReplyAsync(Translation.Message("Entertainment.Birthday", user.ToPossessive(), user.Nickname()));
        }
    }
}