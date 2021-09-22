using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("CustomCommands")]
    [Alias("cc", "CustomCommand")]
    public class CustomCommandModule : BaseModule<CustomCommandModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomCommandModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public async Task CommandCustomDefault()
        {
            var commands = await _unitOfWork.Commands.CountAsync(x => x.Server.Id == Context.Guild.Id);
            await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Default", Server.Prefix, commands));
        }

        [Command("List")]
        public async Task CommandCustomList()
        {
            var commands = await _unitOfWork.Commands.ListAsync(Context.Guild.Id).ConfigureAwait(false);
            if (!commands.Any())
            {
                await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Invalid.None")).ConfigureAwait(false);
                return;
            }

            var commandMsg = string.Join(Environment.NewLine, commands.Select(x => $"{Server.Prefix}{x.Command}"));
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Entertainment.CustomCommand.Title"));
            embedBuilder.WithDescription(commandMsg);

            await ReplyAsync(embed: embedBuilder.Build());
        }
        
        [Command("Add")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CommandCustomAdd(string command, string response)
        {
            var conflictingCommand = await _unitOfWork.Commands.GetAsync(Context.Guild.Id, command).ConfigureAwait(false);
            if (conflictingCommand != null)
            {
                await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Invalid.Exists", conflictingCommand.Command)).ConfigureAwait(false);
                return;
            }

            var customCommand = new CustomCommand
            {
                Server = Server,
                Command = command,
                Response = response.Trim()
            };
            
            _unitOfWork.Commands.Add(customCommand);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Added", command)).ConfigureAwait(false);
        }

        [Command("Remove")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CommandCustomRemove(string command)
        {
            var customCommand = await _unitOfWork.Commands.GetAsync(Context.Guild.Id, command.Trim()).ConfigureAwait(false);
            if (customCommand == null)
            {
                await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Invalid.Missing", command)).ConfigureAwait(false);
                return;
            }
            
            _unitOfWork.Commands.Remove(customCommand);
            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Entertainment.CustomCommand.Removed", command)).ConfigureAwait(false);
        }
    }
}