using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed class DeleteImageCommandHandler : INotificationHandler<DeleteImageCommand>
{
    private readonly IImageStorage _imageStorage;

    public DeleteImageCommandHandler(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public async Task Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        await _imageStorage.DeleteImageAsync(request.Path).ConfigureAwait(false);
    }
}