using Discord.Commands;
using GasaiYuno.Discord.Services;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [RequireOwner]
    public class ManageModule : BaseModule<ManageModule>
    {
        private readonly RestartService _restartService;

        public ManageModule(RestartService restartService)
        {
            _restartService = restartService;
        }

        [Command("Restart")]
        public async Task RestartAsync()
        {
            await ReplyAsync(Translation.Message("Moderation.Manage.Restart")).ConfigureAwait(false);
            _restartService.Restart();
        }

        [Command("Shutdown")]
        public async Task ShutdownAsync()
        {
            await ReplyAsync(Translation.Message("Moderation.Manage.Shutdown")).ConfigureAwait(false);
            _restartService.Shutdown();
        }
    }
}