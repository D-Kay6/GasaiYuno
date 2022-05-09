using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info;

public class SupportModule : BaseInteractionModule<SupportModule>
{
    [SlashCommand("support", "Link to my support discord.")]
    public Task SupportDefaultAsync() => RespondAsync(Translation.Message("Info.Support"), ephemeral: true);
}