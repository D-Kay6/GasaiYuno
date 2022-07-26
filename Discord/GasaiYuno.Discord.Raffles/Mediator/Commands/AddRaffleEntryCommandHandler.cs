using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public class AddRaffleEntryCommandHandler : INotificationHandler<AddRaffleEntryCommand>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;

    public AddRaffleEntryCommandHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddRaffleEntryCommand command, CancellationToken cancellationToken)
    {
        var raffle = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.RaffleId, cancellationToken).ConfigureAwait(false);
        if (raffle == null || raffle.Entries.Contains(command.UserId)) return;
        
        raffle.Entries.Add(command.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}