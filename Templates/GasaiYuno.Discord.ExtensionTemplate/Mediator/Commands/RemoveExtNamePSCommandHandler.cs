using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public class RemoveExtNamePSCommandHandler : INotificationHandler<RemoveExtNamePSCommand>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;
    
    public RemoveExtNamePSCommandHandler(IUnitOfWork<ExtNamePS> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveExtNamePSCommand command, CancellationToken cancellationToken)
    {
        var ExtNameCS = await _unitOfWork.SingleOrDefaultAsync(, cancellationToken).ConfigureAwait(false);
        if (ExtNameCS == null) return;
        
        _unitOfWork.Remove(ExtNameCS);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}