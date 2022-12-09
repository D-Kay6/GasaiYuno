using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.StickyMessages.Mediator.Requests;
using GasaiYuno.Discord.StickyMessages.Models;
using MediatR;
using RestSharp;

namespace GasaiYuno.Discord.StickyMessages.Listeners;

public class StickyMessageListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly RestClient _restClient;
    private readonly Timer _timer;

    public StickyMessageListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
        _restClient = new RestClient();
        //_timer = new Timer();
    }
    
    public Task Start()
    {
        //_client.MessageReceived += MessageReceived;
        return Task.CompletedTask;
    }

    private async Task MessageReceived(SocketMessage message)
    {
        if (message.Channel is not SocketTextChannel channel) return;
        
        var stickyMessages = await _mediator.Send(new ListStickyMessagesRequest(channel.Guild.Id, channel.Id)).ConfigureAwait(false);
        if (!stickyMessages.Any()) return;

        var task = ReplaceMessages(channel, stickyMessages);
    }

    private async Task ReplaceMessages(SocketTextChannel channel, IEnumerable<StickyMessage> stickyMessages)
    {
        foreach (var stickyMessage in stickyMessages)
        {
            await channel.DeleteMessageAsync(stickyMessage.Message).ConfigureAwait(false);
            if (stickyMessage.IsEmbed)
            {
                var embedBuilder = new EmbedBuilder()
                    .WithDescription(stickyMessage.Text);

                if (!string.IsNullOrEmpty(stickyMessage.Image))
                    embedBuilder.WithImageUrl(stickyMessage.Image);

                await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                continue;
            }
            
            if (!string.IsNullOrEmpty(stickyMessage.Image))
            {
                var image = await _restClient.DownloadStreamAsync(new RestRequest(stickyMessage.Image)).ConfigureAwait(false);
                if (image != null)
                {
                    await channel.SendFileAsync(image, string.Empty, stickyMessage.Text).ConfigureAwait(false);
                    continue;
                }
            }
            
            await channel.SendMessageAsync(stickyMessage.Text).ConfigureAwait(false);
        }
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}