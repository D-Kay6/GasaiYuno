using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public record AddPollSelectionsCommand : INotification
{
    public Guid ReferenceId { get; }
    public ulong UserId { get; }
    public int[] SelectedOptions { get; }
    
    public AddPollSelectionsCommand(Guid referenceId, ulong userId, int[] selectedOptions)
    {
        ReferenceId = referenceId;
        UserId = userId;
        SelectedOptions = selectedOptions;
    }
}