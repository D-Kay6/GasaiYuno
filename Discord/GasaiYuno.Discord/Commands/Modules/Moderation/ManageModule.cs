using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Services;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    public class ManageModule : BaseInteractionModule<ManageModule>
    {
        private readonly LifetimeService _lifetimeService;

        public ManageModule(LifetimeService lifetimeService)
        {
            _lifetimeService = lifetimeService;
        }

        [SlashCommand("restart", "Restart the bot (owner only).", true)]
        [RequireOwner]
        public async Task RestartCommand()
        {
            await RespondAsync(Translation.Message("Moderation.Manage.Restart"), ephemeral: true).ConfigureAwait(false);
            await _lifetimeService.RestartAsync();
        }

        [SlashCommand("shutdown", "shut the bot down (owner only).", true)]
        [RequireOwner]
        public async Task ShutdownCommand()
        {
            await RespondAsync(Translation.Message("Moderation.Manage.Shutdown"), ephemeral: true).ConfigureAwait(false);
            await _lifetimeService.StopAsync();
        }
    }
}