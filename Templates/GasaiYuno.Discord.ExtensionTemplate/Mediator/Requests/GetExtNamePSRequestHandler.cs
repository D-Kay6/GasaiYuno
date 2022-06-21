using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public class GetExtNamePSRequestHandler : IRequestHandler<GetExtNamePSRequest, ExtNamePS>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;

    public GetExtNamePSRequestHandler(IUnitOfWork<ExtNamePS> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<ExtNamePS> Handle(GetExtNamePSRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x, cancellationToken);
    }
}