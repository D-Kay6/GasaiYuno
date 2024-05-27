using GasaiYuno.Discord.Core.Models;

namespace GasaiYuno.Discord.Core.Interfaces;

public interface ILocalizationService : ILocalization
{
    ILocalization GetLocalization(Languages language);

    string Translate(Languages language, string code);
    string Translate(Languages language, string code, char separator);
    string Translate(Languages language, string code, params object[] objects);
    string Translate(Languages language, string code, char separator, params object[] objects);
}