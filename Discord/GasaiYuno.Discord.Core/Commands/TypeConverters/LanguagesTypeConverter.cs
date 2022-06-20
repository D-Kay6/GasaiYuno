using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.TypeConverters;

public class LanguagesTypeConverter : TypeConverter<Languages>
{
    /// <inheritdoc />
    public override ApplicationCommandOptionType GetDiscordType()
    {
        return ApplicationCommandOptionType.String;
    }

    /// <inheritdoc />
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
    {
        return Task.FromResult(Enum.TryParse<Languages>((string)option.Value, out var language) ? TypeConverterResult.FromSuccess(language) : TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Value {option.Value} cannot be converted to {nameof(Languages)}"));
    }

    /// <inheritdoc />
    public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter)
    {
        var values = Enum.GetValues<Languages>();
        if (values.Length <= 25)
        {
            properties.Choices = values.Select(value => new ApplicationCommandOptionChoiceProperties { Name = value.ToLocalized(), Value = value.ToString() }).ToList();
        }
    }
}