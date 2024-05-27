using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Requests;

public sealed record GetTranslationRequest : IRequest<ILocalization>
{
    public ulong ServerId { get; init; }
    public Languages Language { get; init; }

    public GetTranslationRequest() { }

    public GetTranslationRequest(ulong serverId)
    {
        ServerId = serverId;
    }

    public GetTranslationRequest(Languages language)
    {
        ServerId = 0;
        Language = language;
    }
}