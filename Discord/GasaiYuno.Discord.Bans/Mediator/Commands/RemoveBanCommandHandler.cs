using GasaiYuno.Discord.Bans.Models;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR;

namespace GasaiYuno.Discord.Bans.Mediator.Commands;

public class RemoveBanCommandHandler : INotificationHandler<RemoveBanCommand>
{
    private readonly IUnitOfWork<Ban> _unitOfWork;

    public RemoveBanCommandHandler(IUnitOfWork<Ban> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveBanCommand command, CancellationToken cancellationToken)
    {
        var ban = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.User == command.UserId, cancellationToken).ConfigureAwait(false);
        if (ban == null) return;
        
        _unitOfWork.Remove(ban);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}