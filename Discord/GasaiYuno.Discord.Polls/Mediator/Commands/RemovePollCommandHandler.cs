using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public class RemovePollCommandHandler : INotificationHandler<RemovePollCommand>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;
    
    public RemovePollCommandHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemovePollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId && x.Message == command.MessageId, cancellationToken).ConfigureAwait(false);
        if (poll == null) return;
        
        _unitOfWork.Remove(poll);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}