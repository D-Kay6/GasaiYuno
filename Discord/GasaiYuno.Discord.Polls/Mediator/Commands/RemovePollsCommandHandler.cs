using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public class RemovePollsCommandHandler : INotificationHandler<RemovePollsCommand>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;
    
    public RemovePollsCommandHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemovePollsCommand command, CancellationToken cancellationToken)
    {
        var polls = await _unitOfWork.WhereAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, cancellationToken).ConfigureAwait(false);
        if (polls.Count == 0) return;
        
        _unitOfWork.RemoveRange(polls);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}