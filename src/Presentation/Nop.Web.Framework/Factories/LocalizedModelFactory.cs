using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents the base localized model factory implementation
/// </summary>
public partial class LocalizedModelFactory : ILocalizedModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields
        
    protected readonly ILanguageService _languageService;

    #endregion

    #region Ctor

    public LocalizedModelFactory(ILanguageService languageService)
    {
		_logger.LogDebug(">>> Entering LocalizedModelFactory(languageService={LanguageService})", languageService);
        _languageService = languageService;
		_logger.LogDebug("LocalizedModelFactory(languageService={LanguageService}): _languageService → {_languageService}", languageService, _languageService);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare localized model for localizable entities
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="configure">Model configuration action</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of localized model
    /// </returns>
    public virtual async Task<IList<T>> PrepareLocalizedModelsAsync<T>(Func<T, int, Task> configure = null) where T : ILocalizedLocaleModel
    {
		_logger.LogDebug(">>> Entering PrepareLocalizedModelsAsync(configure={Configure})", configure);
        //get all available languages
        var availableLanguages = await _languageService.GetAllLanguagesAsync(true);
		_logger.LogDebug("PrepareLocalizedModelsAsync(configure={Configure}): availableLanguages → {AvailableLanguages}", configure, availableLanguages);

        //prepare models
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var localizedModels = await availableLanguages.SelectAwait(async language =>
        {
            //create localized model
            var localizedModel = Activator.CreateInstance<T>();
			_logger.LogDebug("PrepareLocalizedModelsAsync(configure={Configure}): localizedModel → {LocalizedModel}", configure, localizedModel);

            //set language
            localizedModel.LanguageId = language.Id;
			_logger.LogDebug("PrepareLocalizedModelsAsync(configure={Configure}): localizedModel.LanguageId → {Arg1}", configure, localizedModel.LanguageId);

            //invoke the model configuration action
            if (configure != null)
                await configure(localizedModel, localizedModel.LanguageId);

            return localizedModel;
        }).ToListAsync();
		_logger.LogInformation("PrepareLocalizedModelsAsync(configure)={Configure}: database query executed in {Arg1} ms", configure, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting PrepareLocalizedModelsAsync(configure={Configure})", configure);
        return localizedModels;
    }

    #endregion
}