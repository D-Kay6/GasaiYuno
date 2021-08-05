using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Persistence.Repositories;
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
        private readonly IUnitOfWork<ILanguageRepository> _repository;

        public LanguageModule(IUnitOfWork<ILanguageRepository> repository)
        {
            _repository = repository;
        }

        [Command]
        public Task LanguageDefaultAsync() => ReplyAsync(Translation.Message("Moderation.Language.Default", Server.Language?.Name ?? Localization.DefaultLanguage));

        [Command("List")]
        public async Task LanguageListAsync()
        {
            var languages = await _repository.DataSet.ListAsync().ConfigureAwait(false);
            await ReplyAsync(Translation.Message("Moderation.Language.List", string.Join(", ", languages.Select(x => x.Name)))).ConfigureAwait(false);
        }

        [Command("Set")]
        public async Task LanguageSetAsync(string value)
        {
            var language = await _repository.DataSet.GetAsync(value).ConfigureAwait(false);
            if (language == null)
            {
                await ReplyAsync(Translation.Message("Moderation.Language.Unsupported", value)).ConfigureAwait(false);
                return;
            }

            Server.Language = language;
            await ServerRepository.BeginAsync().ConfigureAwait(false);
            ServerRepository.DataSet.Update(Server);
            await ServerRepository.SaveAsync().ConfigureAwait(false);

            Translation = Localization.GetTranslation(language.Name);
            await ReplyAsync(Translation.Message("Moderation.Language.Set", language)).ConfigureAwait(false);
        }
    }
}