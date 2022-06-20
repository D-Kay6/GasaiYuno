using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Commands;

public class AddNotificationCommandHandler : INotificationHandler<AddNotificationCommand>
{
    private readonly IUnitOfWork<Notification> _unitOfWork;

    public AddNotificationCommandHandler(IUnitOfWork<Notification> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(AddNotificationCommand command, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == command.ServerId, cancellationToken).ConfigureAwait(false);
        if (notification != null) return;

        notification = new Notification
        {
            Server = command.ServerId,
            Type = command.Type,
            Channel = command.ChannelId,
            Message = command.Message,
            Image = command.Image
        };
        await _unitOfWork.AddAsync(notification, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}