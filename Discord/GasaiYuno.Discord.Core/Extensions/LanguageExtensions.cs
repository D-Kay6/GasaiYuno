using GasaiYuno.Discord.Core.Attributes;
using GasaiYuno.Discord.Core.Models;
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