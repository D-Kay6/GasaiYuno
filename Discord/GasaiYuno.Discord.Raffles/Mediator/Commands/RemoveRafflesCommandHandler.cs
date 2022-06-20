using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public class RemoveRafflesCommandHandler : INotificationHandler<RemoveRafflesCommand>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;
    
    public RemoveRafflesCommandHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveRafflesCommand command, CancellationToken cancellationToken)
    {
        var raffles = await _unitOfWork.WhereAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, cancellationToken).ConfigureAwait(false);
        if (raffles.Count == 0) return;
        
        _unitOfWork.RemoveRange(raffles);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}