using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Localization.Interfaces;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Localization.Commands.Modules
{
    [Group("Language")]
    [Alias("Lang", "Localization")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LanguageModule : BaseModule<LanguageModule>
    {
        private readonly ILocalization _localization;
        private readonly IUnitOfWork _unitOfWork;

        public LanguageModule(ILocalization localization, IUnitOfWork unitOfWork)
        {
            _localization = localization;
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task LanguageDefaultAsync() => ReplyAsync(Translation.Message("Moderation.Language.Default", Server.Language?.Name ?? _localization.DefaultLanguage));

        [Command("List")]
        public async Task LanguageListAsync()
        {
            var languages = await _unitOfWork.Languages.ListAsync().ConfigureAwait(false);
            await ReplyAsync(Translation.Message("Moderation.Language.List", string.Join(", ", languages.Select(x => x.LocalizedName)))).ConfigureAwait(false);
        }

        [Command("Set")]
        public async Task LanguageSetAsync(string value)
        {
            var language = await _unitOfWork.Languages.GetAsync(value).ConfigureAwait(false);
            if (language == null)
            {
                await ReplyAsync(Translation.Message("Moderation.Language.Unsupported", value)).ConfigureAwait(false);
                return;
            }

            Server.Language = language;
            _unitOfWork.Servers.Update(Server);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            Translation = _localization.GetTranslation(language.Name);
            await ReplyAsync(Translation.Message("Moderation.Language.Set", language.LocalizedName)).ConfigureAwait(false);
        }
    }
}