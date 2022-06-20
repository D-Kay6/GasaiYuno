using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public record AddRaffleEntryCommand : INotification
{
    public ulong RaffleId { get; }
    public ulong UserId { get; }
    
    public AddRaffleEntryCommand(ulong raffleId, ulong userId)
    {
        RaffleId = raffleId;
        UserId = userId;
    }
}