using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Commands;

public class RemoveCustomCommandCommandHandler : INotificationHandler<RemoveCustomCommandCommand>
{
    private readonly IUnitOfWork<CustomCommand> _unitOfWork;
    
    public RemoveCustomCommandCommandHandler(IUnitOfWork<CustomCommand> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveCustomCommandCommand command, CancellationToken cancellationToken)
    {
        var customCommand = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Command == command.Command, cancellationToken).ConfigureAwait(false);
        if (customCommand == null) return;
        
        _unitOfWork.Remove(customCommand);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}