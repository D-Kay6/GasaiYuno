using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Commands;

public class AddAutoChannelCommandHandler : INotificationHandler<AddAutoChannelCommand>
{
    private readonly IUnitOfWork<AutoChannel> _unitOfWork;

    public AddAutoChannelCommandHandler(IUnitOfWork<AutoChannel> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddAutoChannelCommand command, CancellationToken cancellationToken)
    {
        var autoChannel = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Channel == command.ChannelId, cancellationToken).ConfigureAwait(false);
        if (autoChannel != null) return;

        autoChannel = new AutoChannel
        {
            Server = command.ServerId,
            Channel = command.ChannelId,
            Type = command.Type
        };
        if (!string.IsNullOrWhiteSpace(command.GenerationName))
            autoChannel.GenerationName = command.GenerationName;
        
        await _unitOfWork.AddAsync(autoChannel, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}