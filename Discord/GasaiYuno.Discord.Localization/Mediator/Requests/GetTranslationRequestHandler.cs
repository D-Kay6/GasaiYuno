using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Localization.Interfaces;
using MediatR;

namespace GasaiYuno.Discord.Localization.Mediator.Requests;

internal class GetTranslationRequestHandler : IRequestHandler<GetTranslationRequest, ITranslation>
{
    private readonly ILocalization _localization;
    private readonly IMediator _mediator;

    public GetTranslationRequestHandler(ILocalization localization, IMediator mediator)
    {
        _localization = localization;
        _mediator = mediator;
    }

    public async Task<ITranslation> Handle(GetTranslationRequest request, CancellationToken cancellationToken)
    {
        var language = request.Language;
        if (request.ServerId != 0)
        {
            var server = await _mediator.Send(new GetServerRequest(request.ServerId), cancellationToken);
            language = server?.Language ?? _localization.DefaultLanguage;
        }
        return _localization.GetTranslation(language);
    }
}