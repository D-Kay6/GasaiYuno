using GasaiYuno.Discord.Bans.Models;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace GasaiYuno.Discord.Bans.Mediator.Requests;

public class ListBansRequestHandler : IRequestHandler<ListBansRequest, List<Ban>>
{
    private readonly IUnitOfWork<Ban> _unitOfWork;

    public ListBansRequestHandler(IUnitOfWork<Ban> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<Ban>> Handle(ListBansRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<Ban, bool>> expression = x => !request.ExpiredOnly || x.EndDate < DateTime.Now;
        if (request.ServerId.HasValue)
        {
            var compiled = expression.Compile();
            expression = x => compiled(x) && x.Server == request.ServerId.Value;
        }

        return _unitOfWork.WhereAsync(expression, cancellationToken);
    }
}