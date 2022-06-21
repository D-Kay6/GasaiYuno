using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;
using System.Linq.Expressions;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Requests;

public class ListExtNamePPRequestHandler : IRequestHandler<ListExtNamePPRequest, List<ExtNamePS>>
{
    private readonly IUnitOfWork<ExtNamePS> _unitOfWork;

    public ListExtNamePPRequestHandler(IUnitOfWork<ExtNamePS> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<ExtNamePS>> Handle(ListExtNamePPRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.WhereAsync(, cancellationToken);
    }
}