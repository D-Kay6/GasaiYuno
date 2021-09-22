﻿using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events
{
    public class CommandStartedEventHandler : INotificationHandler<CommandStartedEvent>
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commandService;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandStartedEventHandler> _logger;

        public CommandStartedEventHandler(DiscordShardedClient client, CommandService commandService, IMediator mediator, IServiceProvider serviceProvider, ILogger<CommandStartedEventHandler> logger)
        {
            _client = client;
            _commandService = commandService;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(CommandStartedEvent request, CancellationToken cancellationToken)
        {
#if DEBUG
            if (!request.Message.Author.Id.Equals(255453041531158538))
            {
                await request.Message.Channel.SendMessageAsync("Sorry, I cannot do that right now. I'm under development").ConfigureAwait(false);
                return;
            }
#endif
            var context = new ShardedCommandContext(_client, request.Message);
            _logger.LogInformation("{Server}({ServerId}): {User}({UserId}) executed command '{Command}'.", context.Guild.Name, context.Guild.Id, context.User.Username, context.User.Id, context.Message.Content);
            var result = await _commandService.ExecuteAsync(context, request.ArgumentPosition, _serviceProvider).ConfigureAwait(false);
        }
    }
}