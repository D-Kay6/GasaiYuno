using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed class AddServerCommandHandler : INotificationHandler<AddServerCommand>
{
    private readonly IUnitOfWork<Server> _unitOfWork;

    public AddServerCommandHandler(IUnitOfWork<Server> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddServerCommand command, CancellationToken cancellationToken)
    {
        var server = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.Id, cancellationToken).ConfigureAwait(false);
        if (server != null) return;
        
        server = new Server
        {
            Identity = command.Id,
            Language = Languages.English,
            Name = command.Name,
            Prefix = "!",
            WarningDisabled = false
        };
        await _unitOfWork.AddAsync(server, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}