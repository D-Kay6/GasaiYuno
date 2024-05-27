using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Bans.Commands;

[DontAutoRegister]
[EnabledInDm(false)]
[RequireOwner]
public class ManageModule : BaseInteractionModule<ManageModule>
{
    private readonly ILifetimeService _lifetimeService;

    public ManageModule(ILifetimeService lifetimeService)
    {
        _lifetimeService = lifetimeService;
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("restart", "Restart the bot (owner only).")]
    public async Task RestartCommand()
    {
        await RespondAsync(Localization.Translate("Moderation.Manage.Restart"), ephemeral: true).ConfigureAwait(false);
        await _lifetimeService.RestartAsync().ConfigureAwait(false);
    }

    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("shutdown", "shut the bot down (owner only).")]
    public async Task ShutdownCommand()
    {
        await RespondAsync(Localization.Translate("Moderation.Manage.Shutdown"), ephemeral: true).ConfigureAwait(false);
        await _lifetimeService.StopAsync().ConfigureAwait(false);
    }
}