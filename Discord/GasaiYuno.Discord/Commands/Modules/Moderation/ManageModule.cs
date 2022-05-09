using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Services;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation;

[DontAutoRegister]
[EnabledInDm(false)]
[RequireOwner]
public class ManageModule : BaseInteractionModule<ManageModule>
{
    private readonly LifetimeService _lifetimeService;

    public ManageModule(LifetimeService lifetimeService)
    {
        _lifetimeService = lifetimeService;
    }

    [DefaultPermission(false)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("restart", "Restart the bot (owner only).")]
    public async Task RestartCommand()
    {
        await RespondAsync(Translation.Message("Moderation.Manage.Restart"), ephemeral: true).ConfigureAwait(false);
        await _lifetimeService.RestartAsync();
    }

    [DefaultPermission(false)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("shutdown", "shut the bot down (owner only).")]
    public async Task ShutdownCommand()
    {
        await RespondAsync(Translation.Message("Moderation.Manage.Shutdown"), ephemeral: true).ConfigureAwait(false);
        await _lifetimeService.StopAsync();
    }
}