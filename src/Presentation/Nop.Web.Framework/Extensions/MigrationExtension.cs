using System.Globalization;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Extensions;

public static partial class MigrationExtension
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Get language data
    /// </summary>
    /// <returns>Default language identifier and all languages</returns>
    public static (int? defaultLanguageId, IList<Language> allLanguages) GetLanguageData(this IMigration _, ILanguageService languageService = null)
    {
		_logger.LogDebug(">>> Entering GetLanguageData(_={_},languageService={LanguageService})", _, languageService);
        languageService ??= EngineContext.Current.Resolve<ILanguageService>();

        var languages = languageService.GetAllLanguages(true);
		_logger.LogDebug("GetLanguageData(_={_},languageService={LanguageService}): languages → {Languages}", _, languageService, languages);
        var languageId = languages
            .FirstOrDefault(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)?.Id;

		_logger.LogDebug("<<< Exiting GetLanguageData(_={_},languageService={LanguageService})", _, languageService);
        return (languageId, languages);
    }

    /// <summary>
    /// Rename locales
    /// </summary>
    /// <param name="_">Migration</param>
    /// <param name="localesToRename">Locales to rename. Key - old name, Value - new name</param>
    /// <param name="allLanguages">All languages</param>
    /// <param name="localizationService">Localization service</param>
    public static void RenameLocales(this IMigration _, Dictionary<string, string> localesToRename, IList<Language> allLanguages, ILocalizationService localizationService)
    {
		_logger.LogDebug(">>> Entering RenameLocales(_={_},localesToRename={LocalesToRename},allLanguages={AllLanguages},localizationService={LocalizationService})", _, localesToRename, allLanguages, localizationService);
        foreach (var lang in allLanguages)
        foreach (var locale in localesToRename)
        {
            var lsr = localizationService.GetLocaleStringResourceByName(locale.Key, lang.Id, false);
			_logger.LogDebug("RenameLocales(_={_},localesToRename={LocalesToRename},allLanguages={AllLanguages},localizationService={LocalizationService}): lsr → {Lsr}", _, localesToRename, allLanguages, localizationService, lsr);
            
            if (lsr == null) 
                continue;

            var exist = localizationService.GetLocaleStringResourceByName(locale.Value, lang.Id, false);
			_logger.LogDebug("RenameLocales(_={_},localesToRename={LocalesToRename},allLanguages={AllLanguages},localizationService={LocalizationService}): exist → {Exist}", _, localesToRename, allLanguages, localizationService, exist);

            if (exist != null)
                localizationService.DeleteLocaleStringResource(lsr);
            else
            {
                lsr.ResourceName = locale.Value.ToLowerInvariant();
                localizationService.UpdateLocaleStringResource(lsr);
            }
        }
		_logger.LogDebug("<<< Exiting RenameLocales(_={_},localesToRename={LocalesToRename},allLanguages={AllLanguages},localizationService={LocalizationService})", _, localesToRename, allLanguages, localizationService);
    }
}