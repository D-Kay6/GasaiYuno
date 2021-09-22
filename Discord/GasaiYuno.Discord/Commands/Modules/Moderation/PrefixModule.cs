using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Prefix")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class PrefixModule : BaseModule<PrefixModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PrefixModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task CommandPrefixDefault() => ReplyAsync(Translation.Message("Moderation.Prefix.Current", Server.Prefix));

        [Command("Set")]
        public async Task CommandPrefixSet(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                await ReplyAsync(Translation.Message("Moderation.Prefix.Invalid")).ConfigureAwait(false);
                return;
            }

            Server.Prefix = prefix.Trim();
            _unitOfWork.Servers.Update(Server);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Moderation.Prefix.New", Server.Prefix)).ConfigureAwait(false);
        }
    }
}