using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public class AddExtNamePSCommandHandler : INotificationHandler<AddExtNamePSCommand>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;
    
    public AddExtNamePSCommandHandler(IUnitOfWork<ExtNamePS> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddExtNamePSCommand command, CancellationToken cancellationToken)
    {
        var ExtNameCS = await _unitOfWork.SingleOrDefaultAsync(, cancellationToken).ConfigureAwait(false);
        if (ExtNameCS != null) return;
        
        ExtNameCS = new ExtNamePS
        {
        };
        await _unitOfWork.AddAsync(ExtNameCS, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}