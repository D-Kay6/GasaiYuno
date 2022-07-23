namespace GasaiYuno.Discord.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class LocalizationAttribute : Attribute
{
    public string Value { get; init; }

    /// <inheritdoc />
    public LocalizationAttribute(string value)
    {
        Value = value;
    }
}