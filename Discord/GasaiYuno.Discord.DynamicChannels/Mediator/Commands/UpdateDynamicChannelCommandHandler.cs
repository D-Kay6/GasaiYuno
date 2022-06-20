using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.DynamicChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Mediator.Commands;

public class UpdateDynamicChannelCommandHandler : INotificationHandler<UpdateDynamicChannelCommand>
{
    private readonly IUnitOfWork<DynamicChannel> _unitOfWork;
    
    public UpdateDynamicChannelCommandHandler(IUnitOfWork<DynamicChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDynamicChannelCommand command, CancellationToken cancellationToken)
    {
        var dynamicChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Name == command.Name, false, cancellationToken).ConfigureAwait(false);
        if (dynamicChannel == null) return;

        dynamicChannel.GenerationName = command.GenerationName;
        dynamicChannel.Channels = command.Channels;
        dynamicChannel.GeneratedChannels = command.GeneratedChannels;
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}