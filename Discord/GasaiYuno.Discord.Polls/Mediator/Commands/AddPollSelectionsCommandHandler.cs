using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public class AddPollSelectionsCommandHandler : INotificationHandler<AddPollSelectionsCommand>
{
    private readonly IUnitOfWork<Poll> _unitOfWork;

    public AddPollSelectionsCommandHandler(IUnitOfWork<Poll> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddPollSelectionsCommand command, CancellationToken cancellationToken)
    {
        var poll = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.ReferenceId, cancellationToken).ConfigureAwait(false);
        if (poll == null) return;
        
        for (var i = 0; i < poll.Options.Count; i++)
        {
            var pollOption = poll.Options[i];
            if (command.SelectedOptions.Contains(i))
            {
                if (!pollOption.Selectors.Contains(command.UserId))
                    pollOption.Selectors.Add(command.UserId);
            }
            else
            {
                if (pollOption.Selectors.Contains(command.UserId))
                    pollOption.Selectors.Remove(command.UserId);
            }
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}