using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info;

[EnabledInDm(false)]
public class WarningModule : BaseInteractionModule<WarningModule>
{
    [DefaultPermission(false)]
    [DefaultMemberPermissions(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [SlashCommand("disable-warning", "Disable the warning about the prefix no longer working.")]
    public async Task DisableWarningCommand()
    {
        Server.WarningDisabled = true;
        UnitOfWork.Servers.Update(Server);
        await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
        await RespondAsync(":ok_hand:", ephemeral: true).ConfigureAwait(false);
    }
}