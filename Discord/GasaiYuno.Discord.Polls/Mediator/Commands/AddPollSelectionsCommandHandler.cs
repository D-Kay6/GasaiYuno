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

        poll.Selections.RemoveAll(x => x.User == command.UserId);
        if (command.SelectedOptions.Length > 0)
            poll.Selections.AddRange(command.SelectedOptions.Select(x => new PollSelection{User = command.UserId, SelectedOption = x}));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}