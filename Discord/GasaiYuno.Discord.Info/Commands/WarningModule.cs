using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Mediator.Commands;

namespace GasaiYuno.Discord.Info.Commands;

[EnabledInDm(false)]
public class WarningModule : BaseInteractionModule<WarningModule>
{
    [DefaultMemberPermissions(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [SlashCommand("disable-warning", "Disable the warning about the prefix no longer working.")]
    public async Task DisableWarningCommand()
    {
        Server.WarningDisabled = true;
        await Mediator.Publish(new UpdateServerCommand(Server)).ConfigureAwait(false);
        await RespondAsync(":ok_hand:", ephemeral: true).ConfigureAwait(false);
    }
}