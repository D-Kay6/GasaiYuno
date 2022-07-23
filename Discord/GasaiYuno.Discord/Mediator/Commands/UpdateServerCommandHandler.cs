using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Commands;

public class UpdateServerCommandHandler : INotificationHandler<UpdateServerCommand>
{
    private readonly IUnitOfWork<Server> _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateServerCommandHandler(IUnitOfWork<Server> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(UpdateServerCommand command, CancellationToken cancellationToken)
    {
        var server = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.Id, cancellationToken).ConfigureAwait(false);
        if (server == null)
        {
            await _mediator.Publish(new AddServerCommand(command.Id, command.Name), cancellationToken).ConfigureAwait(false);
            return;
        }

        server.Name = command.Name;
        if (command.Language.HasValue)
            server.Language = command.Language.Value;
        if (command.WarningDisabled.HasValue)
            server.WarningDisabled = command.WarningDisabled.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}