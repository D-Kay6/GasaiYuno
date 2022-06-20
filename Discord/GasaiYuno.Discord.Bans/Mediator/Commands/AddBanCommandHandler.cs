using GasaiYuno.Discord.Bans.Models;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR;

namespace GasaiYuno.Discord.Bans.Mediator.Commands;

public class AddBanCommandHandler : INotificationHandler<AddBanCommand>
{
    private readonly IUnitOfWork<Ban> _unitOfWork;

    public AddBanCommandHandler(IUnitOfWork<Ban> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddBanCommand command, CancellationToken cancellationToken)
    {
        var ban = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.User == command.UserId, cancellationToken).ConfigureAwait(false);
        if (ban != null) return;

        ban = new Ban
        {
            Server = command.ServerId,
            User = command.UserId,
            EndDate = command.EndDate,
            Reason = command.Reason,
        };
        await _unitOfWork.AddAsync(ban, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}