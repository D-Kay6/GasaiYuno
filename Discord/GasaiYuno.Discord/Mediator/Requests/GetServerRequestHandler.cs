using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Requests;

public class GetServerRequestHandler : IRequestHandler<GetServerRequest, Server>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetServerRequestHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Server> Handle(GetServerRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.Servers.GetOrAddAsync(request.Id, request.Name);
    }
}