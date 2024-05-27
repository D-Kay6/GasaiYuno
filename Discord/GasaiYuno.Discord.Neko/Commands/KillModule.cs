using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Mediator.Requests;

namespace GasaiYuno.Discord.Neko.Commands;

[EnabledInDm(false)]
public class KillModule : BaseInteractionModule<KillModule>
{
    [SlashCommand("kill", "Have me kill a user.")]
    public async Task KillCommand(IGuildUser user)
    {
        switch (user.Id)
        {
            case 255453041531158538:
                await RespondAsync(Localization.Translate("Entertainment.Kill.Creator", user.Nickname()), ephemeral: true).ConfigureAwait(false);
                break;
            case 286972781273546762:
            case 542706288849715202:
                await RespondAsync(Localization.Translate("Entertainment.Kill.Self"), ephemeral: true).ConfigureAwait(false);
                break;
            default:
                var image = await Mediator.Send(new GetImageRequest("GasaiYuno.gif", "Core")).ConfigureAwait(false);
                await RespondWithFileAsync(new FileAttachment(image), Localization.Translate("Entertainment.Kill.Default", user.Mention)).ConfigureAwait(false);
                break;
        }
    }
}