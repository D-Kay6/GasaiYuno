using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public class AddDynamicChannelCommandHandler : INotificationHandler<AddDynamicChannelCommand>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;
    
    public AddDynamicChannelCommandHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddDynamicChannelCommand command, CancellationToken cancellationToken)
    {
        var dynamicChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Name == command.Name, false, cancellationToken).ConfigureAwait(false);
        if (dynamicChannel != null) return;
        
        dynamicChannel = new DynamicChannel
        {
            Server = command.ServerId,
            Name = command.Name,
            Type = command.Type
        };
        await _unitOfWork.AddAsync(dynamicChannel, cancellationToken).ConfigureAwait(false);
    }
}