using GasaiYuno.Discord.AutoChannels.Models;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Mediator.Requests;

public record ListDynamicChannelsRequest : IRequest<List<DynamicChannel>>;