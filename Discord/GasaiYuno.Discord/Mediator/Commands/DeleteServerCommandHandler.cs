using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Commands;

internal sealed class DeleteServerCommandHandler : INotificationHandler<DeleteServerCommand>
{
    private readonly IUnitOfWork<Server> _unitOfWork;

    public DeleteServerCommandHandler(IUnitOfWork<Server> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteServerCommand command, CancellationToken cancellationToken)
    {
        var server = await _unitOfWork.SingleOrDefaultAsync(x => x.Identity == command.Id, cancellationToken).ConfigureAwait(false);
        if (server == null) return;

        _unitOfWork.Remove(server);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}