using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Localization.Interfaces
{
    public interface ILocalization
    {
        string DefaultLanguage { get; }
        ITranslation GetTranslation(string language);
    }
}