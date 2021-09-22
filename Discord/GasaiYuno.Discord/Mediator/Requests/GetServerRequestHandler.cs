using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class GetServerRequestHandler : IRequestHandler<GetServerRequest, Server>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetServerRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Server> Handle(GetServerRequest request, CancellationToken cancellationToken)
        {
            var server = await _unitOfWork.Servers.GetOrAddAsync(request.Id, request.Name).ConfigureAwait(false);
            if (!server.Name.Equals(request.Name, StringComparison.Ordinal))
            {
                server.Name = request.Name;
                _unitOfWork.Servers.Update(server);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return server;
        }
    }
}