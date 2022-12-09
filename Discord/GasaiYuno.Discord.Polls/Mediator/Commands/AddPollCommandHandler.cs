using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public class AddPollCommandHandler : INotificationHandler<AddPollCommand>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;

    public AddPollCommandHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddPollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.Identity, cancellationToken).ConfigureAwait(false);
        if (poll != null) return;

        poll = new Poll
        {
            Identity = command.Identity,
            Server = command.ServerId,
            Channel = command.ChannelId,
            Message = command.MessageId,
            EndDate = command.EndDate,
            Text = command.Text,
            Options = command.Options
        };
        await _unitOfWork.AddAsync(poll, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}