﻿using Discord.Interactions;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System.Reflection;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Events;

internal sealed class RegisterCommandsEventHandler : INotificationHandler<RegisterCommandsEvent>
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    public RegisterCommandsEventHandler(InteractionService interactionService, IServiceProvider serviceProvider)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task Handle(RegisterCommandsEvent notification, CancellationToken cancellationToken)
    {
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider).ConfigureAwait(false);
    }
}