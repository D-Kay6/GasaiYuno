using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Language")]
    [Alias("Lang", "Localization")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LanguageModule : BaseModule<LanguageModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LanguageModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task LanguageDefaultAsync() => ReplyAsync(Translation.Message("Moderation.Language.Default", Server.Language?.Name ?? Localization.DefaultLanguage));

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

            Translation = Localization.GetTranslation(language.Name);
            await ReplyAsync(Translation.Message("Moderation.Language.Set", language.LocalizedName)).ConfigureAwait(false);
        }
    }
}