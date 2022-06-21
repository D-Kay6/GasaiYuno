using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.ExtNamePP.Models;
using GasaiYuno.Discord.ExtNamePP.Commands.Autocomplete;
using GasaiYuno.Discord.ExtNamePP.Mediator.Commands;
using GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

namespace GasaiYuno.Discord.ExtNamePP.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("", "")]
public class ExtNamePSCommand : BaseInteractionModule<ExtNamePSCommand>
{
}