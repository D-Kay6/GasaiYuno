using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.TypeReaders;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Mediator.Events;
using GasaiYuno.Discord.Mediator.Requests;
using GasaiYuno.Discord.Models;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandListener> _logger;

        public CommandListener(DiscordConnectionClient client, CommandService commandService, IMediator mediator, IServiceProvider serviceProvider, ILogger<CommandListener> logger)
        {
            _client = client;
            _commandService = commandService;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _logger = logger;

            client.Ready += OnReady;
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
            var translation = await _mediator.Send(new GetTranslationRequest(server.Language?.Name)).ConfigureAwait(false);

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
            var translation = await _mediator.Send(new GetTranslationRequest(server.Language?.Name)).ConfigureAwait(false);

            _logger.LogError(exception, "Unhandled exception occurred during the handling of {Module} {Command}", exception.Command.Module.Name, exception.Command.Name);
            await exception.Context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Command", server.Prefix)).ConfigureAwait(false);
        }
    }
}