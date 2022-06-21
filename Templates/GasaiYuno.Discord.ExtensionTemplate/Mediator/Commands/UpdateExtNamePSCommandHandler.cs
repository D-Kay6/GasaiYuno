using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public class UpdateExtNamePSCommandHandler : INotificationHandler<UpdateExtNamePSCommand>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;
    
    public UpdateExtNamePSCommandHandler(IUnitOfWork<ExtNamePS> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateExtNamePSCommand command, CancellationToken cancellationToken)
    {
        var ExtNameCS = await _unitOfWork.SingleOrDefaultAsync(, cancellationToken).ConfigureAwait(false);
        if (ExtNameCS == null) return;

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}