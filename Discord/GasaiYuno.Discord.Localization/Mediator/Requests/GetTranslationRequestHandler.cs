using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.Localization.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Localization.Mediator.Requests;

internal class GetTranslationRequestHandler : IRequestHandler<GetTranslationRequest, ITranslation>
{
    private readonly ILocalization _localization;
    private readonly IUnitOfWork _unitOfWork;

    public GetTranslationRequestHandler(ILocalization localization, IUnitOfWork unitOfWork)
    {
        _localization = localization;
        _unitOfWork = unitOfWork;
    }

    public async Task<ITranslation> Handle(GetTranslationRequest request, CancellationToken cancellationToken)
    {
        var language = request.Language;
        if (request.ServerId != 0)
        {
            var server = await _unitOfWork.Servers.GetAsync(request.ServerId);
            language = server?.Language ?? _localization.DefaultLanguage;
        }
        return _localization.GetTranslation(language);
    }
}