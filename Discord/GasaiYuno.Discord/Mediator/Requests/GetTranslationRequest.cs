using Discord;
using GasaiYuno.Interface.Localization;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class GetTranslationRequest : IRequest<ITranslation>
    {
        public IGuild Guild { get; init; }

        public GetTranslationRequest(IGuild guild)
        {
            Guild = guild;
        }
    }
}