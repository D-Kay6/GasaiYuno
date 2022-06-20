using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.GameRoles.Models;

namespace GasaiYuno.Discord.GameRoles.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("", "")]
public class GameRoleModule : BaseInteractionModule<GameRoleModule>
{
    [SlashCommand("list", "See an overview of all the game role configurations.")]
    public async Task ListGameRoleCommand([Summary(description: "The type of automation.")] AutomationType? type = null)
    {
    }
}