using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.TypeConverters
{
    public class MessageTypeConverter : TypeConverter<IMessage>
    {
        /// <inheritdoc />
        public override ApplicationCommandOptionType GetDiscordType()
        {
            return ApplicationCommandOptionType.String;
        }

        /// <inheritdoc />
        public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            var input = (string)option.Value;
            if (!ulong.TryParse(input, out var messageId))
                return TypeConverterResult.FromError(new ArgumentException("The value was an incorrect format."));

            var message = await context.Channel.GetMessageAsync(messageId).ConfigureAwait(false);
            return message == null ? TypeConverterResult.FromError(new ArgumentException("No message with that id was found.")) : TypeConverterResult.FromSuccess(message);
        }
    }
}