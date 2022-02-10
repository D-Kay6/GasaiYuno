using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Storage.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Storage.Mediator.Requests
{
    public class GetImageRequestHandler : IRequestHandler<GetImageRequest, string>
    {
        private readonly IImageStorage _imageStorage;

        public GetImageRequestHandler(IImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        public Task<string> Handle(GetImageRequest request, CancellationToken cancellationToken)
        {
            return _imageStorage.GetImageAsync(request.Name, request.Directory);
        }
    }
}