using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;
using System.Linq.Expressions;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public class ListAutoChannelsRequestHandler : IRequestHandler<ListAutoChannelsRequest, List<AutoChannel>>
{
    private readonly IUnitOfWork<AutoChannel> _unitOfWork;

    public ListAutoChannelsRequestHandler(IUnitOfWork<AutoChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<AutoChannel>> Handle(ListAutoChannelsRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<AutoChannel, bool>> expression = x => x.Server == request.ServerId;
        if (request.Type.HasValue)
        {
            var compiled = expression.Compile();
            expression = x => compiled(x) && x.Type == request.Type.Value;
        }
        
        return _unitOfWork.WhereAsync(expression, cancellationToken);
    }
}