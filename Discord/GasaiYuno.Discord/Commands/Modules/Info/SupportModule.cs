using Discord.Commands;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info
{
    [Group("Support")]
    public class SupportModule : BaseModule<SupportModule>
    {
        [Command]
        public Task SupportDefaultAsync() => ReplyAsync(Translation.Message("Info.Support"));
    }
}