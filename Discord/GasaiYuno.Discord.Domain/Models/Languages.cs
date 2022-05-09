using GasaiYuno.Discord.Domain.Attributes;

namespace GasaiYuno.Discord.Domain.Models;

public enum Languages
{
    [Localization("English")]
    English,
    [Localization("Français")]
    French,
    [Localization("Español")]
    Spanish,
    [Localization("Nederlands")]
    Dutch
}