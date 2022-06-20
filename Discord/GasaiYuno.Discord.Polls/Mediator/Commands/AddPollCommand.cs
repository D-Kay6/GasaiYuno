using GasaiYuno.Discord.Polls.Models;
using MediatR;

namespace GasaiYuno.Discord.Polls.Mediator.Commands;

public record AddPollCommand : INotification
{
    public ulong Identity { get; }
    public ulong ServerId { get; }
    public ulong ChannelId { get; }
    public ulong MessageId { get; }
    public DateTime EndDate { get; }
    public string Text { get; }
    public string[] Options { get; }

    public AddPollCommand(Poll poll)
    {
        Identity = poll.Identity;
        ServerId = poll.Server;
        ChannelId = poll.Channel;
        MessageId = poll.Message;
        EndDate = poll.EndDate;
        Text = poll.Text;
        Options = poll.Options.Select(x => x.Value).ToArray();
    }
    
    public AddPollCommand(ulong identity, ulong server, ulong channel, ulong message, DateTime endDate, string text, string[] options)
    {
        Identity = identity;
        ServerId = server;
        ChannelId = channel;
        MessageId = message;
        EndDate = endDate;
        Text = text;
        Options = options;
    }
}