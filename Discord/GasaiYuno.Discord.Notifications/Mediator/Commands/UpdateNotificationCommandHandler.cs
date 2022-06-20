using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public class UpdateNotificationCommandHandler : INotificationHandler<UpdateNotificationCommand>
{
    private readonly IUnitOfWork<Notification> _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateNotificationCommandHandler(IUnitOfWork<Notification> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task Handle(UpdateNotificationCommand command, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId && x.Type == command.Type, cancellationToken).ConfigureAwait(false);
        if (notification == null)
        {
            await _mediator.Publish(new AddNotificationCommand(command.ServerId, command.Type, command.ChannelId, command.Message, command.Image), cancellationToken).ConfigureAwait(false);
            return;
        }

        notification.Channel = command.ChannelId;
        notification.Message = command.Message;
        notification.Image = command.Image;
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}