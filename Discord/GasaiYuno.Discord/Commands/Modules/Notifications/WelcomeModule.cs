using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Discord.Services;
using GasaiYuno.Interface.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Notifications
{
    [Group("Welcome")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class WelcomeModule : BaseModule<WelcomeModule>
    {
        private readonly NotificationService _notificationService;
        private readonly IUnitOfWork<INotificationRepository> _repository;

        public WelcomeModule(NotificationService notificationService, IUnitOfWork<INotificationRepository> repository)
        {
            _notificationService = notificationService;
            _repository = repository;
        }
        
        [Command]
        public Task WelcomeDefaultAsync() => ReplyAsync(Translation.Message("Notification.Welcome.Invalid.Missing"));

        [Command]
        public Task WelcomeDefaultAsync(params SocketGuildUser[] users) => _notificationService.WelcomeUsersAsync(Context.Channel as ITextChannel, users);

        [Priority(-1)]
        [Command]
        public async Task WelcomeDefaultAsync([Remainder] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) name = string.Empty;
            await ReplyAsync(Translation.Message("Generic.Invalid.User", name)).ConfigureAwait(false);
        }

        [Command("Enable")]
        [Alias("Activate", "On")]
        public async Task WelcomeEnableAsync(SocketTextChannel channel)
        {
            var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
            if (notification == null)
            {
                await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                return;
            }

            if (notification.Channel == channel.Id)
            {
                await ReplyAsync(Translation.Message("Notification.Welcome.Invalid.Enabled")).ConfigureAwait(false);
                return;
            }

            notification.Channel = channel.Id;
            await _repository.BeginAsync().ConfigureAwait(false);
            _repository.DataSet.Update(notification);
            await _repository.SaveAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Notification.Welcome.Enabled", channel.Mention));
        }

        [Command("Disable")]
        [Alias("Deactivate", "Off")]
        public async Task WelcomeDisableAsync()
        {
            var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
            if (notification?.Channel == null)
            {
                await ReplyAsync(Translation.Message("Notification.Welcome.Invalid.Disabled")).ConfigureAwait(false);
                return;
            }

            notification.Channel = null;
            await _repository.BeginAsync().ConfigureAwait(false);
            _repository.DataSet.Update(notification);
            await _repository.SaveAsync().ConfigureAwait(false);

            await ReplyAsync(Translation.Message("Notification.Welcome.Disabled")).ConfigureAwait(false);
        }

        [Group("Message")]
        public class WelcomeMessageModule : BaseModule<WelcomeMessageModule>
        {
            private readonly IUnitOfWork<INotificationRepository> _repository;

            public WelcomeMessageModule(IUnitOfWork<INotificationRepository> repository)
            {
                _repository = repository;
            }

            [Command]
            public async Task WelcomeMessageDefaultAsync()
            {
                var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                    return;
                }

                await ReplyAsync(Translation.Message("Notification.Welcome.Message.Current", notification.Message, Context.Guild.Name, "{0}")).ConfigureAwait(false);
            }

            [Command("Set")]
            public async Task WelcomeMessageSetAsync([Remainder] string message)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    await ReplyAsync(Translation.Message("Notification.Welcome.Message.Invalid")).ConfigureAwait(false);
                    return;
                }

                var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                    return;
                }

                notification.Message = message;
                await _repository.BeginAsync().ConfigureAwait(false);
                _repository.DataSet.Update(notification);
                await _repository.SaveAsync().ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Notification.Welcome.Message.Changed", notification.Message, Context.Guild.Name)).ConfigureAwait(false);
            }
        }

        [Group("Image")]
        public class WelcomeImageModule : BaseModule<WelcomeImageModule>
        {
            private readonly IImageStorage _imageStorage;
            private readonly IUnitOfWork<INotificationRepository> _repository;

            private const string DefaultImage = "GasaiYunoWelcome.jpg";

            public WelcomeImageModule(IImageStorage imageStorage, IUnitOfWork<INotificationRepository> repository)
            {
                _imageStorage = imageStorage;
                _repository = repository;
            }

            [Command]
            public async Task WelcomeImageDefaultAsync()
            {
                var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrEmpty(notification.Image))
                    await ReplyAsync(Translation.Message("Notification.Welcome.Image.None")).ConfigureAwait(false);
                else
                    await Context.Channel.SendFileAsync(notification.Image, Translation.Message("Notification.Welcome.Image.Current")).ConfigureAwait(false);
            }

            [Command("Enable")]
            [Alias("On", "Change")]
            public async Task WelcomeImageEnableAsync(string imageUrl = null)
            {
                var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(notification.Image))
                    await _imageStorage.DeleteImageAsync(notification.Image).ConfigureAwait(false);

                notification.Image = await _imageStorage.GetImageAsync(DefaultImage).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    try
                    {
                        notification.Image = await _imageStorage.SaveImageAsync(imageUrl, Context.Guild.Id.ToString()).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "The image {url} could not be obtained.", imageUrl);
                        await ReplyAsync(Translation.Message("Notification.Welcome.Image.Invalid")).ConfigureAwait(false);
                        return;
                    }
                }

                await _repository.BeginAsync().ConfigureAwait(false);
                _repository.DataSet.Update(notification);
                await _repository.SaveAsync().ConfigureAwait(false);
                
                await Context.Channel.SendFileAsync(notification.Image, Translation.Message("Notification.Welcome.Image.Enabled")).ConfigureAwait(false);
            }

            [Command("Disable")]
            [Alias("Off")]
            public async Task WelcomeImageDisableAsync()
            {
                var notification = await _repository.DataSet.GetOrAddAsync(Context.Guild.Id, NotificationType.Welcome).ConfigureAwait(false);
                if (notification == null)
                {
                    await ReplyAsync(Translation.Message("Generic.Exception")).ConfigureAwait(false);
                    return;
                }

                if (string.IsNullOrEmpty(notification.Image))
                {
                    await ReplyAsync(Translation.Message("Notification.Welcome.Image.None")).ConfigureAwait(false);
                    return;
                }

                try
                {
                    await _imageStorage.DeleteImageAsync(notification.Image).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "The notification {notification} image {name} could not be deleted.", notification,  notification.Image);
                }

                notification.Image = null;
                await _repository.BeginAsync().ConfigureAwait(false);
                _repository.DataSet.Update(notification);
                await _repository.SaveAsync().ConfigureAwait(false);

                await ReplyAsync(Translation.Message("Notification.Welcome.Image.Disabled")).ConfigureAwait(false);
            }
        }
    }
}