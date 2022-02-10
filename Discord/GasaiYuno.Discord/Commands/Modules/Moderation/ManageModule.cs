using Discord.Commands;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Services;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [RequireOwner]
    public class ManageModule : BaseModule<ManageModule>
    {
        private readonly LifetimeService _lifetimeService;

        public ManageModule(LifetimeService lifetimeService)
        {
            _lifetimeService = lifetimeService;
        }

        [Command("Restart")]
        public async Task RestartAsync()
        {
            await ReplyAsync(Translation.Message("Moderation.Manage.Restart")).ConfigureAwait(false);
            await _lifetimeService.RestartAsync();
        }

        [Command("Shutdown")]
        public async Task ShutdownAsync()
        {
            await ReplyAsync(Translation.Message("Moderation.Manage.Shutdown")).ConfigureAwait(false);
            await _lifetimeService.StopAsync();
        }
    }
}