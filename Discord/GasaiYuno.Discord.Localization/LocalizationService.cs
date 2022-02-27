using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Localization.Interfaces;
using GasaiYuno.Discord.Localization.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GasaiYuno.Discord.Localization
{
    internal class LocalizationService : ILocalization
    {
        public Languages DefaultLanguage => Languages.English;

        private static DirectoryInfo Source => new(Path.Combine(Directory.GetCurrentDirectory(), "Files"));
        
        private readonly Dictionary<Languages, Translation> _translations;
        private readonly ILogger<LocalizationService> _logger;

        public LocalizationService(ILogger<LocalizationService> logger)
        {
            _translations = new Dictionary<Languages, Translation>();
            _logger = logger;

            foreach (var file in Source.EnumerateFiles())
            {
                if (!Enum.TryParse<Languages>(Path.GetFileNameWithoutExtension(file.FullName), true, out var language)) continue;
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

        public ITranslation GetTranslation(Languages language)
        {
            if (_translations.TryGetValue(language, out var translation))
                return translation;

            _logger.LogWarning("Unable to find translation file for language {name}. Using default language {name}", language, DefaultLanguage);
            return _translations[DefaultLanguage];
        }
    }
}