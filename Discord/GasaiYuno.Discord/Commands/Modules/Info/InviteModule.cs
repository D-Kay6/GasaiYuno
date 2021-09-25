using Discord.Commands;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info
{
    [Group("Invite")]
    public class InviteModule : BaseModule<InviteModule>
    {
        [Command]
        public async Task DefaultInvite()
        {
            var dmChannel = await Context.User.CreateDMChannelAsync().ConfigureAwait(false);
            await dmChannel.SendMessageAsync(Translation.Message("Info.Invite"));
        }
    }
}