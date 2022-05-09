using GasaiYuno.Discord.Domain.Attributes;
using GasaiYuno.Discord.Domain.Models;
using System.Linq;
using System.Reflection;

namespace GasaiYuno.Discord.Core.Extensions;

public static class LanguageExtensions
{
    public static string ToLocalized(this Languages language)
    {
        return typeof(Languages).GetMember(language.ToString()).FirstOrDefault()?.GetCustomAttribute<LocalizationAttribute>()?.Value;
    }
}