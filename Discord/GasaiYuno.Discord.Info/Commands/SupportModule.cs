using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.Info.Commands;

public class SupportModule : BaseInteractionModule<SupportModule>
{
    [SlashCommand("support", "Link to my support discord.")]
    public Task SupportDefaultAsync() => RespondAsync(Translation.Message("Info.Support"), ephemeral: true);
}