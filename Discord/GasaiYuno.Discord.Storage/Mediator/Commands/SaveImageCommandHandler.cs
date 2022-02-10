using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Storage.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Storage.Mediator.Commands
{
    public sealed record SaveImageCommandHandler : IRequestHandler<SaveImageCommand, string>
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
}