using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class GetTranslationRequestHandler : IRequestHandler<GetTranslationRequest, ITranslation>
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
            var server = await _unitOfWork.Servers.GetOrAddAsync(request.Guild.Id, request.Guild.Name).ConfigureAwait(false);
            return _localization.GetTranslation(server.Language?.Name);
        }
    }
}