using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;
using System.Linq.Expressions;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Requests;

public class ListDynamicChannelsRequestHandler : IRequestHandler<ListDynamicChannelsRequest, List<DynamicChannel>>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;

    public ListDynamicChannelsRequestHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<DynamicChannel>> Handle(ListDynamicChannelsRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DynamicChannel, bool>> expression = x => x.Server == request.ServerId;
        if (request.Type.HasValue)
        {
            var compiled = expression.Compile();
            expression = x => compiled(x) && x.Type == request.Type.Value;
        }
        
        return _unitOfWork.WhereAsync(expression, cancellationToken);
    }
}