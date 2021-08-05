namespace GasaiYuno.Interface.Localization
{
    public interface ILocalization
    {
        string DefaultLanguage { get; }
        ITranslation GetTranslation(string language);
    }
}