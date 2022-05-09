using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment;

[EnabledInDm(false)]
public class BirthdayModule : BaseInteractionModule<BirthdayModule>
{
    [SlashCommand("birthday", "Sing a song for a happy fellow.")]
    public async Task BirthdayCommand([Summary("user", "The user to direct it towards.")] IGuildUser user)
    {
        if (user == null || user.IsBot)
        {
            await RespondAsync(Translation.Message("Generic.Invalid.User", Context.Interaction.Data), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(Translation.Message("Entertainment.Birthday", user.ToPossessive(), user.Nickname())).ConfigureAwait(false);
    }
}