using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public class RemoveAutoChannelCommandHandler : INotificationHandler<RemoveAutoChannelCommand>
{
    private readonly IUnitOfWork<AutoChannel> _unitOfWork;

    public RemoveAutoChannelCommandHandler(IUnitOfWork<AutoChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveAutoChannelCommand command, CancellationToken cancellationToken)
    {
        var autoChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, cancellationToken).ConfigureAwait(false);
        if (autoChannel == null) return;

        _unitOfWork.Remove(autoChannel);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}