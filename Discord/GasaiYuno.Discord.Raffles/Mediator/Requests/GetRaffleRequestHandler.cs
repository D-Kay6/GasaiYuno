using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Models;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Mediator.Requests;

public class GetRaffleRequestHandler : IRequestHandler<GetRaffleRequest, Raffle>
{
    private readonly IUnitOfWork<Raffle> _unitOfWork;
    
    public GetRaffleRequestHandler(IUnitOfWork<Raffle> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<Raffle> Handle(GetRaffleRequest request, CancellationToken cancellationToken)
    {
        return _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Channel == request.ChannelId && x.Message == request.MessageId, cancellationToken);
    }
}