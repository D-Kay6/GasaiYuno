using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Requests;

public class GetServerRequestHandler : IRequestHandler<GetServerRequest, Server>
{
    private readonly IUnitOfWork<Server> _unitOfWork;

    public GetServerRequestHandler(IUnitOfWork<Server> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Server> Handle(GetServerRequest request, CancellationToken cancellationToken)
    {
        var server = await _unitOfWork.FirstOrDefaultAsync(x => x.Identity == request.Id, cancellationToken).ConfigureAwait(false);
        if (server == null)
        {
            server = new Server
            {
                Identity = request.Id,
                Language = Languages.English,
                Name = request.Name,
                Prefix = "!",
                WarningDisabled = false
            };
            await _unitOfWork.AddAsync(server, cancellationToken).ConfigureAwait(false);
        }
        if (!string.IsNullOrWhiteSpace(request.Name))
            server.Name = request.Name;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return server;
    }
}