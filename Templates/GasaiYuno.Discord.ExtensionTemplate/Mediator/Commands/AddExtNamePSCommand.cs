using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Mediator.Commands;

public record AddExtNamePSCommand : INotification
{
    public AddExtNamePSCommand(ExtNamePS ExtNameCS)
    {
    }
    
    public AddExtNamePSCommand()
    {
    }
}