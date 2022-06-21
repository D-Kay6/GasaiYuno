using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public record RemoveExtNamePSCommand : INotification
{
    public RemoveExtNamePSCommand(ExtNamePS ExtNameCS)
    {
    }
    
    public RemoveExtNamePSCommand()
    {
    }
}