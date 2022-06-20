using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public class AddRaffleCommandHandler : INotificationHandler<AddRaffleCommand>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;

    public AddRaffleCommandHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddRaffleCommand command, CancellationToken cancellationToken)
    {
        var raffle = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.Identity, cancellationToken).ConfigureAwait(false);
        if (raffle != null) return;

        raffle = new Raffle
        {
            Identity = command.Identity,
            Server = command.ServerId,
            Channel = command.ChannelId,
            Message = command.MessageId,
            Title = command.Title,
            EndDate = command.EndDate
        };
        await _unitOfWork.AddAsync(raffle, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}