using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public class ListRafflesRequestHandler : IRequestHandler<ListRafflesRequest, List<Raffle>>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;
    
    public ListRafflesRequestHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<Raffle>> Handle(ListRafflesRequest request, CancellationToken cancellationToken)
    {
        if (request.ExpiredOnly)
            return _unitOfWork.WhereAsync(x => x.EndDate < DateTime.Now, cancellationToken);

        return _unitOfWork.GetAllAsync(cancellationToken);
    }
}