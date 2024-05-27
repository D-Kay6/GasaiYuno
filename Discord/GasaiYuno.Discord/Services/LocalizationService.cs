using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Models;
using GasaiYuno.Discord.Models;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.Services;

internal class LocalizationService : ILocalizationService
{
    private static DirectoryInfo Source => new(Path.Combine(Directory.GetCurrentDirectory(), "Files"));
    
    private readonly ILogger<LocalizationService> _logger;
    private readonly Dictionary<Languages, Localization> _localizations;
    
    public Languages DefaultLanguage => Languages.English;

    public LocalizationService(ILogger<LocalizationService> logger)
    {
        _logger = logger;
        _localizations = new Dictionary<Languages, Localization>();
        
        foreach (var file in Source.EnumerateFiles())
        {
            if (!Enum.TryParse<Languages>(Path.GetFileNameWithoutExtension(file.FullName), true, out var language)) 
                continue;
            
            try
            {
                var translation = TranslationFile.Read(file.FullName);
                if (_localizations.ContainsKey(language))
                    _localizations[language].Merge(translation);
                else
                    _localizations.Add(language, translation);

                _logger.LogInformation("Added translation {@Translation} for language {Language}", translation, language);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add a translation for language {Language}", language);
            }
        }
    }

    internal void RegisterTranslation(Languages language, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var localization = TranslationFile.Read(filePath);
        if (_localizations.ContainsKey(language))
            _localizations[language].Merge(localization);
        else
            _localizations.Add(language, localization);
    }

    public ILocalization GetLocalization(Languages language)
    {
        if (!_localizations.ContainsKey(language))
            throw new ArgumentOutOfRangeException(nameof(language), "No localization could be found for this language.");

        return _localizations[language];
    }

    public string Translate(string code) => Translate(Languages.English, code);
    public string Translate(Languages language, string code)
    {
        var localization = GetLocalization(language);
        return localization.Translate(code);
    }

    public string Translate(string code, char separator) => Translate(Languages.English, code, separator);
    public string Translate(Languages language, string code, char separator)
    {
        var localization = GetLocalization(language);
        return localization.Translate(code, separator);
    }

    public string Translate(string code, params object[] objects) => Translate(Languages.English, code, objects);
    public string Translate(Languages language, string code, params object[] objects)
    {
        var localization = GetLocalization(language);
        return localization.Translate(code, objects);
    }

    public string Translate(string code, char separator, params object[] objects) => Translate(Languages.English, code, separator, objects);
    public string Translate(Languages language, string code, char separator, params object[] objects)
    {
        var localization = GetLocalization(language);
        return localization.Translate(code, separator, objects);
    }
}