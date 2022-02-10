using Discord;
using Discord.Commands;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using Interactivity;
using Interactivity.Pagination;
using System;
using System.Collections.Generic;
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

            var fieldMessages = new List<string>();
            var commandMessages = commands.Select(x => $"{Server.Prefix}{x.Command}").ToList();
            var commandsPerField = 20;
            bool retry;
            do
            {
                retry = false;
                fieldMessages.Clear();
                for (var i = 0; i < commandMessages.Count; i += commandsPerField)
                {
                    fieldMessages.Add(string.Join(Environment.NewLine, commandMessages.Skip(i).Take(commandsPerField)));
                }
                if (fieldMessages.Any(x => x.Length > 1000))
                {
                    retry = true;
                    commandsPerField -= 5;
                }
            } while (retry);

            if (fieldMessages.Count > 1)
            {
                var pages = new List<PageBuilder>();
                foreach (var fieldMessage in fieldMessages)
                {
                    var embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle(Translation.Message("Entertainment.CustomCommand.Title"));
                    embedBuilder.WithDescription(fieldMessage);
                    pages.Add(PageBuilder.FromEmbedBuilder(embedBuilder));
                }

                var paginator = new StaticPaginatorBuilder()
                    .WithUsers(Context.User)
                    .WithPages(pages)
                    .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
                    .WithDefaultEmotes()
                    .Build();
                await Interactivity.SendPaginatorAsync(Context.Client, paginator, Context.Channel, TimeSpan.FromMinutes(2)).ConfigureAwait(false);
            }
            else
            {
                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(Translation.Message("Entertainment.CustomCommand.Title"));
                embedBuilder.WithDescription(fieldMessages[0]);
                await ReplyAsync(embed: embedBuilder.Build());
            }
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