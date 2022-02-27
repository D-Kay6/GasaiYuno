using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.Localization.Interfaces;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Localization.Commands.Modules
{
    [Group("language", "Manage the language of the server.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LanguageModule : BaseInteractionModule<LanguageModule>
    {
        private readonly ILocalization _localization;
        private readonly IUnitOfWork _unitOfWork;

        public LanguageModule(ILocalization localization, IUnitOfWork unitOfWork)
        {
            _localization = localization;
            _unitOfWork = unitOfWork;
        }

        [SlashCommand("current", "Show what language is currently being used.")]
        public Task CurrentLanguageCommand() => RespondAsync(Translation.Message("Moderation.Language.Default", Server.Language.ToLocalized()), ephemeral: true);
        
        [SlashCommand("set", "Change the language for this server.")]
        public async Task SetLanguageCommand([Summary("language", "The new language.")] Languages language)
        {
            Server.Language = language;
            _unitOfWork.Servers.Update(Server);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            Translation = _localization.GetTranslation(language);
            await RespondAsync(Translation.Message("Moderation.Language.Set", language.ToLocalized()), ephemeral: true).ConfigureAwait(false);
        }
    }
}