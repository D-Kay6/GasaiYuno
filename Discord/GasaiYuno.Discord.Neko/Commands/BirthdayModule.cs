using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Extensions;

namespace GasaiYuno.Discord.Neko.Commands;

[EnabledInDm(false)]
public class BirthdayModule : BaseInteractionModule<BirthdayModule>
{
    [SlashCommand("birthday", "Sing a song for a happy fellow.")]
    public async Task BirthdayCommand([Summary("user", "The user to direct it towards.")] IGuildUser user)
    {
        if (user == null || user.IsBot)
        {
            await RespondAsync(Localization.Translate("Generic.Invalid.User", Context.Interaction.Data), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(Localization.Translate("Entertainment.Birthday", user.ToPossessive(), user.Nickname())).ConfigureAwait(false);
    }
}