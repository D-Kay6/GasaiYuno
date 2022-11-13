using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public class UpdateAutoChannelCommandHandler : INotificationHandler<UpdateAutoChannelCommand>
{
    private readonly IUnitOfWork<AutoChannel> _unitOfWork;

    public UpdateAutoChannelCommandHandler(IUnitOfWork<AutoChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateAutoChannelCommand command, CancellationToken cancellationToken)
    {
        var autoChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, false, cancellationToken).ConfigureAwait(false);
        if (autoChannel == null) return;

        autoChannel.GenerationName = command.GenerationName;
        if (command.RelatedChannels != null)
            autoChannel.RelatedChannels = command.RelatedChannels;
        if (command.GeneratedChannels != null)
            autoChannel.GeneratedChannels = command.GeneratedChannels;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}