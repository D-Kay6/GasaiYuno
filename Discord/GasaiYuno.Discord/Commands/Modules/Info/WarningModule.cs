using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info
{
    public class WarningModule : BaseInteractionModule<WarningModule>
    {
        [SlashCommand("disable-warning", "Disable the warning about the prefix no longer working.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DisableWarningCommand()
        {
            Server.WarningDisabled = true;
            UnitOfWork.Servers.Update(Server);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await RespondAsync(":ok_hand:", ephemeral: true).ConfigureAwait(false);
        }
    }
}