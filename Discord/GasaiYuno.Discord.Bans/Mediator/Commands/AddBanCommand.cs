using GasaiYuno.Discord.Bans.Models;
using MediatR;

namespace GasaiYuno.Discord.Bans.Mediator.Commands;

public record AddBanCommand : INotification
{
    public ulong ServerId { get; }
    public ulong UserId { get; }
    public DateTime EndDate { get; }
    public string Reason { get; }

    public AddBanCommand(Ban ban)
    {
        ServerId = ban.Server;
        UserId = ban.User;
        EndDate = ban.EndDate;
        Reason = ban.Reason;
    }

    public AddBanCommand(ulong serverId, ulong userId, DateTime endDate, string reason = null)
    {
        ServerId = serverId;
        UserId = userId;
        EndDate = endDate;
        Reason = reason;
    }
}