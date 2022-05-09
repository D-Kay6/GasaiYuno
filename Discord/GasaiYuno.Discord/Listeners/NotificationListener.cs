using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

internal class NotificationListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly Func<IUnitOfWork> _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationListener> _logger;

    public NotificationListener(DiscordShardedClient client, Func<IUnitOfWork> unitOfWork, IMediator mediator, ILogger<NotificationListener> logger)
    {
        _client = client;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public Task Start()
    {
        _client.UserJoined += OnUserJoinedAsync;
        return Task.CompletedTask;
    }

    private async Task OnUserJoinedAsync(SocketGuildUser user)
    {
        var unitOfWork = _unitOfWork();
        var notification = await unitOfWork.Notifications.GetOrAddAsync(user.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
        if (notification?.Channel == null) return;

        var translation = await _mediator.Send(new GetTranslationRequest(user.Guild.Id)).ConfigureAwait(false);
        ITextChannel channel = user.Guild.GetTextChannel(notification.Channel.Value);
        if (channel == null)
        {
            var ownerDm = await user.Guild.Owner.CreateDMChannelAsync().ConfigureAwait(false);
            if (ownerDm != null)
                await ownerDm.SendMessageAsync(translation.Message("Notification.Welcome.Exception", user.Guild.Name)).ConfigureAwait(false);

            return;
        }

        try
        {
            var embedBuilder = new EmbedBuilder()
                .WithDescription(notification.Message.Replace("[user]", user.Mention));
            if (string.IsNullOrEmpty(notification.Image))
            {
                await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                return;
            }

            var attachment = new FileAttachment(notification.Image);
            embedBuilder.WithImageUrl($"attachment://{attachment.FileName}");
            await channel.SendFileAsync(attachment, embed: embedBuilder.Build()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to handle a welcome notification {@Notification}", notification);
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}