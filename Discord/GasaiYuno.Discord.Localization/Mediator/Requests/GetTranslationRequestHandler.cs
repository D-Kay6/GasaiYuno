using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Localization.Interfaces;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Localization.Mediator.Requests
{
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
            var language = request.Language ?? _localization.DefaultLanguage;
            if (request.ServerId != 0)
            {
                var server = await _unitOfWork.Servers.GetAsync(request.ServerId);
                language = server?.Language?.Name ?? language;
            }
            return _localization.GetTranslation(language);
        }
    }
}