using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.StickyMessages.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("", "")]
public class StickyMessageModule : BaseInteractionModule<StickyMessageModule>
{
}