using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Mediator.Requests;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    public class KillModule : BaseInteractionModule<KillModule>
    {
        [SlashCommand("kill", "Have me kill a user.", true)]
        public async Task KillCommand(IGuildUser user)
        {
            switch (user.Id)
            {
                case 255453041531158538:
                    await RespondAsync(Translation.Message("Entertainment.Kill.Creator", user.Nickname()), ephemeral: true).ConfigureAwait(false);
                    break;
                case 286972781273546762:
                case 542706288849715202:
                    await RespondAsync(Translation.Message("Entertainment.Kill.Self"), ephemeral: true).ConfigureAwait(false);
                    break;
                default:
                    var image = await Mediator.Send(new GetImageRequest("GasaiYuno.gif", "Core")).ConfigureAwait(false);
                    await RespondWithFileAsync(new FileAttachment(image), Translation.Message("Entertainment.Kill.Default", user.Mention)).ConfigureAwait(false);
                    break;
            }
        }
    }
}