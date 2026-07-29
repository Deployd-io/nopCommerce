using System.Globalization;
using FluentMigrator;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Extensions;

public static partial class MigrationExtension
{
    /// <summary>
    /// Get language data
    /// </summary>
    /// <returns>Default language identifier and all languages</returns>
    public static (int? defaultLanguageId, IList<Language> allLanguages) GetLanguageData(this IMigration _, ILanguageService languageService = null)
    {
		logger.LogDebug(">>> Entering GetLanguageData(defaultLanguageId={DefaultLanguageId},allLanguages={AllLanguages})", defaultLanguageId, allLanguages);
        languageService ??= EngineContext.Current.Resolve<ILanguageService>();

        var languages = languageService.GetAllLanguages(true);
        var languageId = languages
			logger.LogDebug("GetLanguageData(defaultLanguageId={DefaultLanguageId},allLanguages={AllLanguages}): languages → {Languages}", defaultLanguageId, allLanguages, languages);
            .FirstOrDefault(lang => lang.UniqueSeoCode == new CultureInfo(NopCommonDefaults.DefaultLanguageCulture).TwoLetterISOLanguageName)?.Id;

		logger.LogDebug("<<< Exiting GetLanguageData(defaultLanguageId={DefaultLanguageId},allLanguages={AllLanguages})", defaultLanguageId, allLanguages);
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
        foreach (var lang in allLanguages)
        foreach (var locale in localesToRename)
        {
            var lsr = localizationService.GetLocaleStringResourceByName(locale.Key, lang.Id, false);
            
            if (lsr == null) 
                continue;

            var exist = localizationService.GetLocaleStringResourceByName(locale.Value, lang.Id, false);

            if (exist != null)
                localizationService.DeleteLocaleStringResource(lsr);
            else
            {
                lsr.ResourceName = locale.Value.ToLowerInvariant();
                localizationService.UpdateLocaleStringResource(lsr);
            }
        }
    }
}