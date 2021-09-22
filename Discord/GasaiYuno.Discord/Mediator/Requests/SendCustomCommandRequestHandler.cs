using Discord.Commands;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Requests
{
    public class SendCustomCommandRequestHandler : IRequestHandler<SendCustomCommandRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendCustomCommandRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SendCustomCommandRequest request, CancellationToken cancellationToken)
        {
            var argPos = 0;
            if (!request.Context.Message.HasStringPrefix(request.Server.Prefix, ref argPos) &&
                !request.Context.Message.HasMentionPrefix(request.Context.Client.CurrentUser, ref argPos)) return false;

            var command = request.Context.Message.Content[argPos..];
            var customCommand = await _unitOfWork.Commands.GetAsync(request.Context.Guild.Id, command).ConfigureAwait(false);
            if (customCommand == null) return false;

            await request.Context.Channel.SendMessageAsync(customCommand.Response).ConfigureAwait(false);
            return true;
        }
    }
}