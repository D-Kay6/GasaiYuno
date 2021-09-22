﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Commands.TypeReaders;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Mediator.Events;
using GasaiYuno.Discord.Mediator.Requests;
using GasaiYuno.Discord.Models;
using GasaiYuno.Interface.Localization;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class CommandListener
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commandService;
        private readonly IMediator _mediator;
        private readonly ILocalization _localization;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandListener> _logger;

        public CommandListener(Connection connection, CommandService commandService, IMediator mediator, ILocalization localization, IServiceProvider serviceProvider, ILogger<CommandListener> logger)
        {
            _client = connection.Client;
            _commandService = commandService;
            _mediator = mediator;
            _localization = localization;
            _serviceProvider = serviceProvider;
            _logger = logger;

            connection.Ready += OnReady;
        }

        private Task OnReady()
        {
            _client.MessageReceived += HandleCommandAsync;
            _commandService.CommandExecuted += OnCommandExecutedAsync;
            _commandService.Log += OnLogAsync;

            _commandService.AddTypeReader(typeof(AutomationType), new AutomationTypeReader());
            _commandService.AddTypeReader(typeof(TimeSpan), new TimeSpanTypeReader());
            _commandService.AddTypeReader(typeof(PollOption[]), new PollOptionTypeReader());
            return _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            try
            {
                if (s.Author.IsBot) return;
                if (s is not SocketUserMessage { Channel: SocketGuildChannel channel } msg) return;

                var server = await _mediator.Send(new GetServerRequest(channel.Guild.Id, channel.Guild.Name)).ConfigureAwait(false);
                var argPos = 0;
                if (!msg.HasStringPrefix(server.Prefix, ref argPos) &&
                    !msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

                await _mediator.Publish(new CommandStartedEvent(argPos, msg, channel));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception occurred in the command listener.");
            }
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess) return;

            var server = await _mediator.Send(new GetServerRequest(context.Guild.Id, context.Guild.Name)).ConfigureAwait(false);
            var translation = _localization.GetTranslation(server.Language?.Name);

            switch (result.Error)
            {
                case CommandError.UnmetPrecondition:
                    await context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Permission")).ConfigureAwait(false);
                    break;
                case CommandError.BadArgCount:
                    await context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Argument", server.Prefix)).ConfigureAwait(false);
                    break;
                case CommandError.Exception:
                    if (result.ErrorReason == "InvalidArgument")
                        await context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Argument", server.Prefix)).ConfigureAwait(false);
                    break;
                default:
                    if (await _mediator.Send(new SendCustomCommandRequest(context, server))) return;
                    await context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Command", server.Prefix)).ConfigureAwait(false);
                    break;
            }
            _logger.LogInformation("Execution of {Command} failed with {Error} {Reason}.", context.Message.Content, result.Error, result.ErrorReason);
        }

        private async Task OnLogAsync(LogMessage logMessage)
        {
            if (logMessage.Exception is not CommandException exception) return;

            var server = await _mediator.Send(new GetServerRequest(exception.Context.Guild.Id, exception.Context.Guild.Name)).ConfigureAwait(false);
            var translation = _localization.GetTranslation(server.Language?.Name);

            _logger.LogError(exception, "Unhandled exception occurred during the handling of {Module} {Command}", exception.Command.Module.Name, exception.Command.Name);
            await exception.Context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Generic", server.Prefix)).ConfigureAwait(false);
        }
    }
}