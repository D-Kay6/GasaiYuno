using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUnitOfWork unitOfWork, IMediator mediator, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public Task WelcomeUserAsync(IGuildUser user) => WelcomeUsersAsync(user.Guild, user);
        public async Task WelcomeUsersAsync(IGuild guild, params IGuildUser[] users)
        {
            var notification = await _unitOfWork.Notifications.GetOrAddAsync(guild.Id, NotificationType.Welcome).ConfigureAwait(false);
            if (notification?.Channel == null) return;

            var translation = await _mediator.Send(new GetTranslationRequest(notification.Server.Language)).ConfigureAwait(false);
            var channel = await guild.GetChannelAsync(notification.Channel.Value).ConfigureAwait(false);
            if (channel is not ITextChannel textChannel)
            {
                var owner = await guild.GetOwnerAsync().ConfigureAwait(false);
                var ownerDm = await owner.CreateDMChannelAsync().ConfigureAwait(false);
                if (ownerDm != null)
                    await ownerDm.SendMessageAsync(translation.Message("Notification.Welcome.Exception", guild.Name)).ConfigureAwait(false);

                return;
            }

            try
            {
                var mentions = string.Join(", ", users.Select(x => x.Mention));
                if (!string.IsNullOrEmpty(notification.Image))
                    await textChannel.SendFileAsync(new FileAttachment(notification.Image), notification.Message.Replace("[user]", mentions)).ConfigureAwait(false);
                else
                    await textChannel.SendMessageAsync(notification.Message.Replace("[user]", mentions)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to handle a welcome notification {Notification}.", notification);
            }
        }

        public async Task WelcomeUsersAsync(SocketInteraction interaction, params IGuildUser[] users)
        {
            if (interaction.Channel is not ITextChannel channel) return;
            
            var notification = await _unitOfWork.Notifications.GetOrAddAsync(channel.GuildId, NotificationType.Welcome).ConfigureAwait(false);
            var mentions = string.Join(", ", users.Select(x => x.Mention));

            try
            {
                if (!string.IsNullOrEmpty(notification.Image))
                    await interaction.RespondWithFileAsync(new FileAttachment(notification.Image), notification.Message.Replace("[user]", mentions)).ConfigureAwait(false);
                else
                    await interaction.RespondAsync(notification.Message.Replace("[user]", mentions)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to handle a welcome notification {Notification}.", notification);
            }
        }
    }
}