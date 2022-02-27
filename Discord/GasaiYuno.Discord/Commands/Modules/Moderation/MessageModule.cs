using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    //[RequireUserPermission(GuildPermission.ManageMessages)]
    //[Group("message", "Manage the messages in channels.")]
    public class MessageModule : BaseInteractionModule<MessageModule>
    {
        private readonly string[] _imageExtensions = { "jpg", "jpeg", "png", "gif" };

        public enum MoveType
        {
            Plain,
            Embed
        }

        //[SlashCommand("move", "Move a message to another channel.")]
        public async Task MoveMessageCommand(
            [Summary(description: "The channel to move the message to.")] ITextChannel channel,
            [Summary(description: "The id of the message to move.")] IMessage message = null,
            [Summary(description: "If the original message should be deleted.")] bool deleteOriginal = true,
            [Summary(description: "How the message should be placed.")] MoveType type = MoveType.Embed)
        {
            message ??= await GetMessageAsync().ConfigureAwait(false);
            if (message == null)
            {
                await RespondAsync("I could not find the message. Are you sure there are any?", ephemeral: true);
                return;
            }

            switch (type)
            {
                case MoveType.Plain:
                    await MoveMessageTextAsync(message, channel).ConfigureAwait(false);
                    break;
                case MoveType.Embed:
                    await MoveMessageEmbedAsync(message, channel).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (deleteOriginal)
                await message.DeleteAsync().ConfigureAwait(false);

            await RespondAsync("Done", ephemeral: true).ConfigureAwait(false);
        }

        private async Task<IMessage> GetMessageAsync(ulong messageId = 0)
        {
            if (messageId > 0)
                return await Context.Channel.GetMessageAsync(messageId).ConfigureAwait(false);

            var result = await Context.Channel.GetMessagesAsync(1).FirstOrDefaultAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        private async Task MoveMessageEmbedAsync(IMessage message, ITextChannel channel)
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(message.Author.GetAvatarUrl());
            embedBuilder.WithAuthor((message.Author as IGuildUser)?.Nickname() ?? message.Author.Username);
            embedBuilder.WithTimestamp(message.Timestamp);
            embedBuilder.WithFooter(Context.Channel.Name);
            if (!string.IsNullOrEmpty(message.Content))
                embedBuilder.WithDescription(message.Content);

            var attachment = message.Attachments.FirstOrDefault();
            if (attachment == null)
            {
                await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                return;
            }

            var extension = Path.GetExtension(attachment.Filename);
            if (!_imageExtensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                embedBuilder.WithImageUrl(attachment.Url);
                await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                return;
            }

            using var httpClient = new HttpClient();
            var file = await httpClient.GetStreamAsync(new Uri(attachment.Url)).ConfigureAwait(false);
            await channel.SendFileAsync(file, attachment.Filename, string.Empty, embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        private async Task MoveMessageTextAsync(IMessage message, ITextChannel channel)
        {
            var author = message.Author as SocketGuildUser;
            var attachment = message.Attachments.FirstOrDefault();

            var msg = $"{author.Nickname()} - {message.Timestamp.DateTime:HH:mm:ss yyyy/MM/dd}  •  {Context.Channel.Name}\n{message.Content}";
            if (attachment == null)
            {
                await channel.SendMessageAsync(msg).ConfigureAwait(false);
                return;
            }

            using var httpClient = new HttpClient();
            var file = await httpClient.GetStreamAsync(new Uri(attachment.Url)).ConfigureAwait(false);
            await channel.SendFileAsync(file, attachment.Filename, msg).ConfigureAwait(false);
        }
    }
}