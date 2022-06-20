using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand>
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