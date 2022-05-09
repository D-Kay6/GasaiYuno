using Discord.Interactions;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Localization.Mediator.Events;

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
        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
    }
}