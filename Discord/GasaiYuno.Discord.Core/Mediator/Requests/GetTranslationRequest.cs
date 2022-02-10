using GasaiYuno.Discord.Core.Interfaces;
using MediatR;

namespace GasaiYuno.Discord.Core.Mediator.Requests
{
    public sealed record GetTranslationRequest : IRequest<ITranslation>
    {
        public ulong ServerId { get; init; }
        public string Language { get; init; }

        public GetTranslationRequest(ulong serverId)
        {
            ServerId = serverId;
        }

        public GetTranslationRequest(string language)
        {
            ServerId = 0;
            Language = language;
        }
    }
}