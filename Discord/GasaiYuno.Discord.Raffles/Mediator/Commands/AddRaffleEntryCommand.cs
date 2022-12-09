using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public record AddRaffleEntryCommand : INotification
{
    public Guid RaffleId { get; }
    public ulong UserId { get; }
    
    public AddRaffleEntryCommand(Guid raffleId, ulong userId)
    {
        RaffleId = raffleId;
        UserId = userId;
    }
}