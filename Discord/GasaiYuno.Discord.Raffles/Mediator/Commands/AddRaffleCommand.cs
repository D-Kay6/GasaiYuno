using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Commands;

public record AddRaffleCommand : INotification
{
    public ulong Identity { get; }
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    public string Title { get; }
    public DateTime EndDate { get; }

    public AddRaffleCommand(Raffle raffle)
    {
        Identity = raffle.Identity;
        ServerId = raffle.Server;
        ChannelId = raffle.Channel;
        MessageId = raffle.Message;
        Title = raffle.Title;
        EndDate = raffle.EndDate;
    }
    
    public AddRaffleCommand(ulong identity, ulong server, ulong channel, ulong message, string title, DateTime endDate)
    {
        Identity = identity;
        ServerId = server;
        ChannelId = channel;
        MessageId = message;
        Title = title;
        EndDate = endDate;
    }
}