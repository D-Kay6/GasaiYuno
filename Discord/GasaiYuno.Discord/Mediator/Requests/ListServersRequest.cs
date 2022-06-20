using GasaiYuno.Discord.Core.Models;
using MediatR;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Mediator.Requests;

public record ListServersRequest : IRequest<List<Server>>
{
    
}