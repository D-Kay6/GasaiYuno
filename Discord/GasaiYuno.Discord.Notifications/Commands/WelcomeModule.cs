using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands;
using GasaiYuno.Discord.Core.Mediator.Commands;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Notifications.Mediator.Commands;
using GasaiYuno.Discord.Notifications.Mediator.Requests;
using GasaiYuno.Discord.Notifications.Models;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Notifications.Commands;

[EnabledInDm(false)]
[DefaultMemberPermissions(GuildPermission.ManageMessages)]
[RequireUserPermission(GuildPermission.ManageMessages)]
[Group("welcome", "A welcome notification.")]
public class WelcomeModule : BaseInteractionModule<WelcomeModule>
{
    [SlashCommand("user", "Send a welcome notification for a specific user.")]
    public async Task UsersWelcomeCommand(IGuildUser user)
    {
        if (Context.Guild == null) return;

        var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
        try
        {
            var embedBuilder = new EmbedBuilder()
                .WithDescription(notification.Message.Replace("[user]", user.Mention));
            if (string.IsNullOrEmpty(notification?.Image))
            {
                await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                return;
            }

            var attachment = new FileAttachment(notification.Image);
            embedBuilder.WithImageUrl($"attachment://{attachment.FileName}");
            await RespondWithFileAsync(attachment, embed: embedBuilder.Build()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Unable to handle a welcome notification {@Notification}", notification);
        }
    }

    [SlashCommand("enable", "Enable the automated welcome notifications.")]
    public async Task EnableWelcomeCommand([Summary(description: "The channel to send the messages in.")] ITextChannel channel)
    {
        var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
        if (notification.Channel == channel.Id)
        {
            await RespondAsync(Localization.Translate("Notification.Welcome.Invalid.Enabled"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        notification.Channel = channel.Id;
        await Mediator.Publish(new UpdateNotificationCommand(notification)).ConfigureAwait(false);
        await RespondAsync(Localization.Translate("Notification.Welcome.Enabled", channel.Mention), ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("disable", "Disable the automated welcome notifications.")]
    public async Task DisableWelcomeCommand()
    {
        var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
        if (notification.Channel == null)
        {
            await RespondAsync(Localization.Translate("Notification.Welcome.Invalid.Disabled"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        notification.Channel = null;
        await Mediator.Publish(new UpdateNotificationCommand(notification)).ConfigureAwait(false);
        await RespondAsync(Localization.Translate("Notification.Welcome.Disabled"), ephemeral: true).ConfigureAwait(false);
    }

    [Group("message", "Configure the message of the notification.")]
    public class WelcomeMessageModule : BaseInteractionModule<WelcomeMessageModule>
    {
        [SlashCommand("current", "Display the current message used in the notification.")]
        public async Task CurrentWelcomeMessageCommand()
        {
            var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
            var embedBuilder = new EmbedBuilder()
                .WithTitle(Localization.Translate("Notification.Welcome.Message.Current"))
                .WithDescription(notification.Message);
            await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("change", "Change the message used in the notification.")]
        public Task ChangeWelcomeMessageCommand() => RespondWithModalAsync<WelcomeModal>("welcome message form");

        public class WelcomeModal : IModal
        {
            /// <inheritdoc />
            public string Title => "Welcome message";

            [InputLabel("Use [user] in place of a mention.")]
            [ModalTextInput("form message", TextInputStyle.Paragraph, "Welcome to the party [user]. Hope you will have a good time with us.")]
            public string Message { get; set; }
        }

        public class WelcomeMessageModalModule : BaseInteractionModule<WelcomeMessageModalModule, SocketModal>
        {
            [ModalInteraction("form")]
            public async Task WelcomeResponse(WelcomeModal modal)
            {
                if (string.IsNullOrWhiteSpace(modal.Message))
                {
                    await RespondAsync(Localization.Translate("Notification.Welcome.Message.Invalid"), ephemeral: true).ConfigureAwait(false);
                    return;
                }

                var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
                notification.Message = modal.Message;
                await Mediator.Publish(new UpdateNotificationCommand(notification)).ConfigureAwait(false);

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(Localization.Translate("Notification.Welcome.Message.Changed"))
                    .WithDescription(notification.Message);
                await RespondAsync(embed: embedBuilder.Build(), ephemeral: true).ConfigureAwait(false);
            }
        }
    }

    [Group("image", "Configure the image of the notification.")]
    public class WelcomeImageModule : BaseInteractionModule<WelcomeImageModule>
    {
        private const string DefaultImage = "GasaiYunoWelcome.jpg";
        
        [SlashCommand("current", "Display the current image used in the notification.")]
        public async Task CurrentWelcomeImageCommand()
        {
            var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
            if (string.IsNullOrEmpty(notification.Image))
                await RespondAsync(Localization.Translate("Notification.Welcome.Image.None"), ephemeral: true).ConfigureAwait(false);
            else
                await RespondWithFileAsync(new FileAttachment(notification.Image), Localization.Translate("Notification.Welcome.Image.Current"), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("change", "Change the image used in the notification.")]
        public async Task ChangeWelcomeImageCommand([Summary("url", "The url of the image. If none is provided, the default image will be used.")] string imageUrl = null)
        {
            var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
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
                    Logger.LogError(e, "The image {Url} could not be obtained", imageUrl);
                    await RespondAsync(Localization.Translate("Notification.Welcome.Image.Invalid"), ephemeral: true).ConfigureAwait(false);
                    return;
                }
            }

            await Mediator.Publish(new UpdateNotificationCommand(notification)).ConfigureAwait(false);
            await RespondWithFileAsync(new FileAttachment(notification.Image), Localization.Translate("Notification.Welcome.Image.Enabled"), ephemeral: true).ConfigureAwait(false);
        }

        [SlashCommand("disable", "Disable the image used in the notification.")]
        public async Task DisableWelcomeImageCommand()
        {
            var notification = await Mediator.Send(new GetNotificationRequest(Context.Guild.Id, NotificationType.Welcome)).ConfigureAwait(false);
            if (string.IsNullOrEmpty(notification.Image))
            {
                await RespondAsync(Localization.Translate("Notification.Welcome.Image.None"), ephemeral: true).ConfigureAwait(false);
                return;
            }

            try
            {
                await Mediator.Send(new DeleteImageCommand(notification.Image)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "The notification {@Notification} image {Name} could not be deleted", notification, notification.Image);
            }

            notification.Image = null;
            await Mediator.Publish(new UpdateNotificationCommand(notification)).ConfigureAwait(false);
            await RespondAsync(Localization.Translate("Notification.Welcome.Image.Disabled"), ephemeral: true).ConfigureAwait(false);
        }
    }
}