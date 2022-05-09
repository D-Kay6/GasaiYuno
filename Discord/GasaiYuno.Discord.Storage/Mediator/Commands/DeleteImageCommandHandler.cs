using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Storage.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Storage.Mediator.Commands;

public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
{
    private readonly IImageStorage _imageStorage;

    public DeleteImageCommandHandler(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        await _imageStorage.DeleteImageAsync(request.Path).ConfigureAwait(false);
        return Unit.Value;
    }
}