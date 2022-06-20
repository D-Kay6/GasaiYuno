using GasaiYuno.Discord.Bans.Models;
using MediatR;

namespace GasaiYuno.Discord.Bans.Mediator.Requests;

public record ListBansRequest : IRequest<List<Ban>>
{
    public ulong? ServerId { get; }
    public bool ExpiredOnly { get; }

    public ListBansRequest(ulong? serverId = null, bool expiredOnly = false)
    {
        ServerId = serverId;
        ExpiredOnly = expiredOnly;
    }
}