using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed class SaveImageCommandHandler : IRequestHandler<SaveImageCommand, string>
{
    private readonly IImageStorage _imageStorage;

    public SaveImageCommandHandler(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public Task<string> Handle(SaveImageCommand request, CancellationToken cancellationToken)
    {
        return _imageStorage.SaveImageAsync(request.Url, request.Directory);
    }
}