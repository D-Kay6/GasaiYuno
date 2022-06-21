using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public record UpdateExtNamePSCommand : INotification
{
    public UpdateExtNamePSCommand(ExtNamePS ExtNameCS)
    {
    }

    public UpdateExtNamePSCommand()
    {
    }
}