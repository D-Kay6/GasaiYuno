using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Commands.TypeReaders;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Discord.Services;
using GasaiYuno.Interface.Localization;
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

        private readonly ServerService _serverService;
        private readonly Func<IUnitOfWork<ICommandRepository>> _unitOfWork;
        private readonly ILocalization _localization;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandListener> _logger;

        public CommandListener(Connection connection, CommandService commandService, ServerService serverService, Func<IUnitOfWork<ICommandRepository>> unitOfWork, ILocalization localization, IServiceProvider serviceProvider, ILogger<CommandListener> logger)
        {
            _client = connection.Client;
            _commandService = commandService;
            _serverService = serverService;
            _unitOfWork = unitOfWork;
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

                var server = await _serverService.Load(channel.Guild).ConfigureAwait(false);
                var argPos = 0;
                if (!msg.HasStringPrefix(server.Prefix, ref argPos) &&
                    !msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;
#if DEBUG
                if (!s.Author.Id.Equals(255453041531158538))
                {
                    await msg.Channel.SendMessageAsync("Sorry, I cannot do that right now. I'm under development").ConfigureAwait(false);
                    return;
                }
#endif
                var context = new ShardedCommandContext(_client, msg);
                _logger.LogInformation("{Server}({ServerId}): {User}({UserId}) executed command '{Command}'.", context.Guild.Name, context.Guild.Id, context.User.Username, context.User.Id, context.Message.Content);
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider).ConfigureAwait(false);
                if (result.IsSuccess) return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception occurred in the command handler.");
            }
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess) return;

            var server = await _serverService.Load(context.Guild).ConfigureAwait(false);
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
                    if (await SendCustomCommandAsync(context, server)) return;
                    await context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Command", server.Prefix)).ConfigureAwait(false);
                    break;
            }
            _logger.LogInformation("Execution of {Command} failed with {Error} {Reason}.", context.Message.Content, result.Error, result.ErrorReason);
        }

        private async Task OnLogAsync(LogMessage logMessage)
        {
            if (logMessage.Exception is not CommandException exception) return;

            var server = await _serverService.Load(exception.Context.Guild).ConfigureAwait(false);
            var translation = _localization.GetTranslation(server.Language?.Name);

            _logger.LogError(exception, "Unhandled exception occurred during the handling of {Module} {Command}", exception.Command.Module.Name, exception.Command.Name);
            await exception.Context.Channel.SendMessageAsync(translation.Message("Generic.Invalid.Generic", server.Prefix)).ConfigureAwait(false);
        }

        private async Task<bool> SendCustomCommandAsync(ICommandContext context, Server server)
        {
            var repository = _unitOfWork();
            var argPos = 0;
            if (!context.Message.HasStringPrefix(server.Prefix, ref argPos) &&
                !context.Message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return false;

            var command = context.Message.Content[argPos..];
            var customCommand = await repository.DataSet.GetAsync(context.Guild.Id, command).ConfigureAwait(false);
            if (customCommand == null) return false;

            await context.Channel.SendMessageAsync(customCommand.Response).ConfigureAwait(false);
            return true;
        }
    }
}