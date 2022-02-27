using System;

namespace GasaiYuno.Discord.Domain.Attributes
{
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
}