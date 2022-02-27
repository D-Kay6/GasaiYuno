using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Notifications
{
    [Group("welcome", "A welcome notification.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class WelcomeModule : BaseInteractionModule<WelcomeModule>
    {
        private readonly NotificationService _notificationService;

        public WelcomeModule(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [SlashCommand("user", "Send a welcome notification for a specific user.")]
        public async Task UsersWelcomeCommand(IGuildUser user)
        {
            await _notificationService.WelcomeUsersAsync(Context.Interaction, user).ConfigureAwait(false);
        }

        [SlashCommand("enable", "Enable the automated welcome notifications.")]
        public async Task EnableWelcomeCommand([Summary(description: "The channel to send the messages in.")] ITextChannel channel)
        {
            var notification = await UnitOfWork.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
            if (notification == null)
            {
                await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (notification.Channel == channel.Id)
            {
                await RespondAsync(Translation.Message("Notification.Welcome.Invalid.Enabled"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            notification.Channel = channel.Id;
            UnitOfWork.Notifications.Update(notification);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Notification.Welcome.Enabled", channel.Mention), ephemeral: true);
        }

        [SlashCommand("disable", "Disable the automated welcome notifications.")]
        public async Task DisableWelcomeCommand()
        {
            var notification = await UnitOfWork.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
            if (notification?.Channel == null)
            {
                await RespondAsync(Translation.Message("Notification.Welcome.Invalid.Disabled"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            notification.Channel = null;
            UnitOfWork.Notifications.Update(notification);
            await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync(Translation.Message("Notification.Welcome.Disabled"), ephemeral: true).ConfigureAwait(false);
        }

        [Group("message", "Configure the message of the notification.")]
        public class WelcomeMessageModule : BaseInteractionModule<WelcomeMessageModule>
        {
            private readonly IUnitOfWork _unitOfWork;

            public WelcomeMessageModule(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            [SlashCommand("current", "Display the current message used in the notification.")]
            public async Task CurrentWelcomeMessageCommand()
            {
                var notification = await _unitOfWork.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                await RespondAsync(Translation.Message("Notification.Welcome.Message.Current", notification.Message, Context.Guild.Name, "{0}"), ephemeral: true).ConfigureAwait(false);
            }

            [SlashCommand("change", "Change the message used in the notification.")]
            public async Task ChangeWelcomeMessageCommand(string message)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    await RespondAsync(Translation.Message("Notification.Welcome.Message.Invalid"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                var notification = await _unitOfWork.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                notification.Message = message;
                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                await RespondAsync(Translation.Message("Notification.Welcome.Message.Changed", notification.Message, Context.Guild.Name), ephemeral: true).ConfigureAwait(false);
            }
        }

        [Group("image", "Configure the image of the notification.")]
        public class WelcomeImageModule : BaseInteractionModule<WelcomeImageModule>
        {
            private readonly IUnitOfWork _repository;

            private const string DefaultImage = "GasaiYunoWelcome.jpg";

            public WelcomeImageModule(IUnitOfWork repository)
            {
                _repository = repository;
            }

            [SlashCommand("current", "Display the current image used in the notification.")]
            public async Task CurrentWelcomeImageCommand()
            {
                var notification = await _repository.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrEmpty(notification.Image))
                    await RespondAsync(Translation.Message("Notification.Welcome.Image.None"), ephemeral: true).ConfigureAwait(false);
                else
                    await RespondWithFileAsync(new FileAttachment(notification.Image), Translation.Message("Notification.Welcome.Image.Current"), ephemeral: true).ConfigureAwait(false);
            }

            [SlashCommand("change", "Change the image used in the notification.")]
            public async Task ChangeWelcomeImageCommand([Summary("url", "The url of the image. If none is provided, the default image will be used.")] string imageUrl = null)
            {
                var notification = await _repository.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(notification.Image))
                    await Mediator.Send(new DeleteImageCommand(notification.Image)).ConfigureAwait(false);

                notification.Image = await Mediator.Send(new GetImageRequest(DefaultImage)).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    try
                    {
                        notification.Image = await Mediator.Send(new SaveImageCommand(imageUrl, Context.Guild.Id.ToString())).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "The image {url} could not be obtained.", imageUrl);
                        await RespondAsync(Translation.Message("Notification.Welcome.Image.Invalid"), ephemeral: true).ConfigureAwait(false);
                        return;
                    }
                }

                _repository.Notifications.Update(notification);
                await _repository.SaveChangesAsync().ConfigureAwait(false);

                await RespondWithFileAsync(new FileAttachment(notification.Image), Translation.Message("Notification.Welcome.Image.Enabled"), ephemeral: true).ConfigureAwait(false);
            }

            [SlashCommand("disable", "Disable the image used in the notification.")]
            public async Task DisableWelcomeImageCommand()
            {
                var notification = await _repository.Notifications.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await RespondAsync(Translation.Message("Generic.Exception"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrEmpty(notification.Image))
                {
                    await RespondAsync(Translation.Message("Notification.Welcome.Image.None"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                try
                {
                    await Mediator.Send(new DeleteImageCommand(notification.Image)).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "The notification {notification} image {name} could not be deleted.", notification, notification.Image);
                }

                notification.Image = null;
                _repository.Notifications.Update(notification);
                await _repository.SaveChangesAsync().ConfigureAwait(false);

                await RespondAsync(Translation.Message("Notification.Welcome.Image.Disabled"), ephemeral: true).ConfigureAwait(false);
            }
        }
    }
}