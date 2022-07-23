using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.CustomCommands.Models;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Mediator.Commands;

public class AddCustomCommandCommandHandler : INotificationHandler<AddCustomCommandCommand>
{
    private readonly IUnitOfWork<CustomCommand> _unitOfWork;
    
    public AddCustomCommandCommandHandler(IUnitOfWork<CustomCommand> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddCustomCommandCommand command, CancellationToken cancellationToken)
    {
        var customCommand = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Command == command.Command, cancellationToken).ConfigureAwait(false);
        if (customCommand != null)
        {
            customCommand.Response = command.Response;
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return;
        }
        
        customCommand = new CustomCommand
        {
            Server = command.ServerId,
            Command = command.Command,
            Response = command.Response
        };
        await _unitOfWork.AddAsync(customCommand, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}