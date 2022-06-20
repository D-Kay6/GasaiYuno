using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Notifications.Mediator.Commands;
using GasaiYuno.Discord.Notifications.Models;
using MediatR;

namespace GasaiYuno.Discord.Notifications.Mediator.Requests;

public class GetNotificationRequestHandler : IRequestHandler<GetNotificationRequest, Notification>
{
    private readonly IUnitOfWork<Notification> _unitOfWork;
    private readonly IMediator _mediator;

    public GetNotificationRequestHandler(IUnitOfWork<Notification> unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Notification> Handle(GetNotificationRequest request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.SingleOrDefaultAsync(x => x.Server == request.ServerId && x.Type == request.Type, cancellationToken).ConfigureAwait(false);
        if (notification == null)
        {
            notification = new Notification
            {
                Server = request.ServerId,
                Type = request.Type
            };
            await _mediator.Publish(new AddNotificationCommand(notification), cancellationToken).ConfigureAwait(false);
        }

        return notification;
    }
}