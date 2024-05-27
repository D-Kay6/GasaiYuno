using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Models;

namespace GasaiYuno.Discord.Localization.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageGuild)]
[RequireUserPermission(GuildPermission.ManageGuild)]
[Group("language", "Manage the language of the server.")]
public class LanguageModule : BaseInteractionModule<LanguageModule>
{
    private readonly ILocalizationService _localizationService;

    public LanguageModule(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    [SlashCommand("current", "Show what language is currently being used.")]
    public Task CurrentLanguageCommand() => RespondAsync(Localization.Translate("Moderation.Language.Default", Server.Language.ToLocalized()), ephemeral: true);

    [SlashCommand("set", "Change the language for this server.")]
    public async Task SetLanguageCommand([Summary("language", "The new language.")] Languages language)
    {
        Server.Language = language;
        await Mediator.Publish(new UpdateServerCommand(Server)).ConfigureAwait(false);

        Localization = _localizationService.GetLocalization(language);
        await RespondAsync(Localization.Translate("Moderation.Language.Set", language.ToLocalized()), ephemeral: true).ConfigureAwait(false);
    }
}