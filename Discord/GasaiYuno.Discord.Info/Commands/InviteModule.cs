using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.Info.Commands;

public class InviteModule : BaseInteractionModule<InviteModule>
{
    [SlashCommand("invite", "My invite link, so you, or someone else, can add me to a server.")]
    public Task InviteCommand() => RespondAsync(Translation.Message("Info.Invite"), ephemeral: true);
}