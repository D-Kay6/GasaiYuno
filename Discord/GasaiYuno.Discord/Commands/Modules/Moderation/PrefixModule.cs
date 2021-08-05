using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Prefix")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class PrefixModule : BaseModule<PrefixModule>
    {
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
            await ServerRepository.BeginAsync().ConfigureAwait(false);
            ServerRepository.DataSet.Update(Server);
            await ServerRepository.SaveAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Moderation.Prefix.New", Server.Prefix)).ConfigureAwait(false);
        }
    }
}