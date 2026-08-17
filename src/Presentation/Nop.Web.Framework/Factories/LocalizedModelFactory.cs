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
		_logger.LogDebug("<<< Exiting LocalizedModelFactory(languageService={LanguageService})", languageService);
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
        //get all available languages
        var availableLanguages = await _languageService.GetAllLanguagesAsync(true);

        //prepare models
        var localizedModels = await availableLanguages.SelectAwait(async language =>
        {
            //create localized model
            var localizedModel = Activator.CreateInstance<T>();

            //set language
            localizedModel.LanguageId = language.Id;

            //invoke the model configuration action
            if (configure != null)
                await configure(localizedModel, localizedModel.LanguageId);

			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("PrepareLocalizedModelsAsync(Func<T,int,configure)={Arg0},{Int},{Configure}: database query executed in {Arg3} ms", Func<T, int, configure, __logStart.ElapsedMilliseconds);
            return localizedModel;
        }).ToListAsync();

		_logger.LogDebug("<<< Exiting PrepareLocalizedModelsAsync(Func<T={Arg0},int={Int},configure={Configure})", Func<T, int, configure);
        return localizedModels;
    }

    #endregion
}