using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Core.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Core.Commands;

public abstract class BaseInteractionModule<TImplementor> : BaseInteractionModule<TImplementor, SocketSlashCommand> { }

public abstract class BaseInteractionModule<TImplementor, TInteraction> : InteractionModuleBase<ShardedInteractionContext<TInteraction>> where TInteraction : SocketInteraction
{
    public InteractiveService Interactivity { get; init; }
    public IMediator Mediator { get; init; }
    public ILogger<TImplementor> Logger { get; init; }

    protected Server Server { get; private set; }
    protected ILocalization Localization { get; set; }

    /// <inheritdoc />
    public override async Task BeforeExecuteAsync(ICommandInfo command)
    {
        Server = await Mediator.Send(new GetServerRequest(Context.Guild.Id, Context.Guild.Name)).ConfigureAwait(false);
        Localization = await Mediator.Send(new GetTranslationRequest(Server.Language)).ConfigureAwait(false);
    }

    protected Task ConfirmAsync()
    {
        return Context.Interaction.RespondAsync(":thumbsup:", ephemeral: true);
    }
}