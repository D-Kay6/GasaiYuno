﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using GasaiYuno.Discord.Commands.Autocomplete;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment;

[EnabledInDm(false)]
public class CustomCommandResponderModule : BaseInteractionModule<CustomCommandModule>
{
    [SlashCommand("run", "Run a custom command.")]
    public async Task CustomCommandResponderCommand([Autocomplete(typeof(CustomCommandAutocompleteHandler))] [Summary(description: "The name of the custom command to run.")] string command)
    {
        var customCommand = await UnitOfWork.Commands.GetAsync(Context.Guild.Id, command).ConfigureAwait(false);
        if (customCommand == null)
        {
            await RespondAsync(Translation.Message("Generic.Invalid.Command"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(customCommand.Response).ConfigureAwait(false);
    }
}

[EnabledInDm(false)]
[DefaultPermission(false)]
[DefaultMemberPermissions(GuildPermission.ManageMessages)]
[Group("custom-commands", "Manage customized commands and their respective response.")]
public class CustomCommandModule : BaseInteractionModule<CustomCommandModule>
{
    //[SlashCommand("info", "Display some information about the usage of custom commands.")]
    public async Task InfoCustomCommandCommand()
    {
        var commands = await UnitOfWork.Commands.CountAsync(x => x.Server == Context.Guild.Id);
        await RespondAsync(Translation.Message("Entertainment.CustomCommand.Default", Server.Prefix, commands), ephemeral: true);
    }

    [SlashCommand("list", "Display all custom commands of this server.")]
    public async Task ListCustomCommandCommand()
    {
        var commands = await UnitOfWork.Commands.ListAsync(Context.Guild.Id).ConfigureAwait(false);
        if (!commands.Any())
        {
            await RespondAsync(Translation.Message("Entertainment.CustomCommand.Invalid.None"), ephemeral: true).ConfigureAwait(false);
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
            await Interactivity.SendPaginatorAsync(paginator, Context.Interaction, TimeSpan.FromMinutes(2), ephemeral: true).ConfigureAwait(false);
        }
        else
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Entertainment.CustomCommand.Title"));
            embedBuilder.WithDescription(fieldMessages[0]);
            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
        }
    }

    [RequireUserPermission(GuildPermission.ManageMessages)]
    [SlashCommand("add", "Add a custom command for this server.")]
    public Task AddCustomCommandCommand() => RespondWithModalAsync<CommandModal>("custom-commands form");

    [RequireUserPermission(GuildPermission.ManageMessages)]
    [SlashCommand("remove", "Remove one of this server's custom commands.")]
    public async Task RemoveCustomCommandCommand([Autocomplete(typeof(CustomCommandAutocompleteHandler))] [Summary(description: "The command without the prefix.")] string command)
    {
        var customCommand = await UnitOfWork.Commands.GetAsync(Context.Guild.Id, command.Trim()).ConfigureAwait(false);
        if (customCommand == null)
        {
            await RespondAsync(Translation.Message("Entertainment.CustomCommand.Invalid.Missing", command), ephemeral: true).ConfigureAwait(false);
            return;
        }

        UnitOfWork.Commands.Remove(customCommand);
        await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync(Translation.Message("Entertainment.CustomCommand.Removed", command), ephemeral: true).ConfigureAwait(false);
    }

    public class CommandModal : IModal
    {
        /// <inheritdoc />
        public string Title => "New custom command";

        [InputLabel("Command name.")]
        [ModalTextInput("form name", TextInputStyle.Short, maxLength: 100)]
        public string Name { get; set; }

        [InputLabel("The response to send back to the user.")]
        [ModalTextInput("form response", TextInputStyle.Paragraph, maxLength: 4000)]
        public string Response { get; set; }
    }

    public class CustomCommandModalModule : BaseInteractionModule<CustomCommandModalModule, SocketModal>
    {
        [ModalInteraction("form")]
        public async Task CustomCommandResponse(CommandModal modal)
        {
            var conflictingCommand = await UnitOfWork.Commands.GetAsync(Context.Guild.Id, modal.Name).ConfigureAwait(false);
            if (conflictingCommand != null)
            {
                await RespondAsync(Translation.Message("Entertainment.CustomCommand.Invalid.Exists", conflictingCommand.Command), ephemeral: true).ConfigureAwait(false);
                return;
            }

            var customCommand = new CustomCommand
            {
                Server = Context.Guild.Id,
                Command = modal.Name,
                Response = modal.Response.Trim()
            };

            await UnitOfWork.Commands.AddAsync(customCommand).ConfigureAwait(false);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            await RespondAsync(Translation.Message("Entertainment.CustomCommand.Added", modal.Name), ephemeral: true).ConfigureAwait(false);
        }
    }
}