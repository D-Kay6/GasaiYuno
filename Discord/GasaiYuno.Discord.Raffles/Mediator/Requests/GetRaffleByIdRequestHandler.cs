using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public class GetRaffleByIdRequestHandler : IRequestHandler<GetRaffleByIdRequest, Raffle>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;
    
    public GetRaffleByIdRequestHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Raffle> Handle(GetRaffleByIdRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Identity == request.Id, cancellationToken);
    }
}