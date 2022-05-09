using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Automation;

[DontAutoRegister]
[EnabledInDm(false)]
[RequireOwner]
[Group("game-roles", "Manage the automated role distribution for games.")]
public class GameRoleModule : BaseInteractionModule<GameRoleModule>
{
    [SlashCommand("list", "See an overview of all the game role configurations.")]
    public async Task ListGameRoleCommand([Summary(description: "The type of automation.")] AutomationType? type = null)
    {
        List<GameRole> configurations = null;
        if (type == null)
            configurations = await UnitOfWork.GameRoles.ListAsync(Context.Guild.Id);
        else
            configurations = await UnitOfWork.GameRoles.ListAsync(Context.Guild.Id, type.Value);

        if (!configurations.Any())
        {
            await RespondAsync("No configurations found.");
        }
    }
}