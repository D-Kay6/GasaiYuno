using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Requests;

internal class GetTranslationRequestHandler : IRequestHandler<GetTranslationRequest, ILocalization>
{
    private readonly ILocalizationService _localizationService;
    private readonly IMediator _mediator;

    public GetTranslationRequestHandler(ILocalizationService localizationService, IMediator mediator)
    {
        _localizationService = localizationService;
        _mediator = mediator;
    }

    public async Task<ILocalization> Handle(GetTranslationRequest request, CancellationToken cancellationToken)
    {
        var language = Languages.English;
        if (request.ServerId != 0)
        {
            var server = await _mediator.Send(new GetServerRequest(request.ServerId), cancellationToken).ConfigureAwait(false);
            language = server?.Language ?? Languages.English;
        }

        return _localizationService.GetLocalization(language);
    }
}