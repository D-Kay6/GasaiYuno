using GasaiYuno.Interface.Localization;
using GasaiYuno.Localization.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GasaiYuno.Localization
{
    internal class LocalizationService : ILocalization
    {
        public string DefaultLanguage { get; } = "English";

        private static DirectoryInfo Source => new(Path.Combine(Directory.GetCurrentDirectory(), "Files"));
        
        private readonly Dictionary<string, Translation> _translations;
        private readonly ILogger<LocalizationService> _logger;

        public LocalizationService(ILogger<LocalizationService> logger)
        {
            _translations = new Dictionary<string, Translation>();
            _logger = logger;

            foreach (var file in Source.EnumerateFiles())
            {
                var language = Path.GetFileNameWithoutExtension(file.FullName);
                try
                {
                    var translation = Translation.Read(file);
                    _translations.Add(language, translation);

                    _logger.LogInformation("Added translation {translation} for language {name}", translation, language);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to add a translation for language {name}", language);
                }
            }
        }

        public ITranslation GetTranslation(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                _logger.LogWarning("No language specified. Using default language {name}", DefaultLanguage);
                return _translations[DefaultLanguage];
            }

            if (_translations.TryGetValue(language, out var translation))
                return translation;

            _logger.LogWarning("Unable to find translation file for language {name}. Using default language {name}", language, DefaultLanguage);
            return _translations[DefaultLanguage];
        }
    }
}