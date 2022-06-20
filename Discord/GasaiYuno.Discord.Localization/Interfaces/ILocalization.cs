using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;

namespace GasaiYuno.Discord.Localization.Interfaces;

public interface ILocalization
{
    Languages DefaultLanguage { get; }
    ITranslation GetTranslation(Languages language);
}