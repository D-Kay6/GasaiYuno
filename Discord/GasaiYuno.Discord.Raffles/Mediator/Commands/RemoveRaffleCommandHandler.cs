using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public class RemoveRaffleCommandHandler : INotificationHandler<RemoveRaffleCommand>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;
    
    public RemoveRaffleCommandHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveRaffleCommand command, CancellationToken cancellationToken)
    {
        var raffle = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (raffle == null) return;
        
        _unitOfWork.Remove(raffle);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}