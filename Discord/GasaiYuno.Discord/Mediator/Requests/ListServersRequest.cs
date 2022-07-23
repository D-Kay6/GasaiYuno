using GasaiYuno.Discord.Core.Models;
using MediatR;

namespace GasaiYuno.Discord.Mediator.Requests;

public record ListServersRequest : IRequest<List<Server>>;