using System.Reflection;
using DeepL;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Translation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;

namespace Nop.Web.Framework.Factories;

/// <summary>
/// Represents translation model factory implementation
/// </summary>
public partial class TranslationModelFactory : ITranslationModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected TranslationClientHelper _translationClientHelper;
    protected readonly TranslationSettings _translationSettings;

    #endregion

    #region Ctor

    public TranslationModelFactory(ILanguageService languageService,
        ILocalizationService localizationService,
        ILogger logger,
        TranslationSettings translationSettings)
    {
        _languageService = languageService;
		_logger.LogDebug("TranslationModelFactory(languageService={LanguageService},localizationService={LocalizationService},logger={Logger},translationSettings={TranslationSettings}): _languageService → {_languageService}", languageService, localizationService, logger, translationSettings, _languageService);
        _localizationService = localizationService;
		_logger.LogDebug("TranslationModelFactory(languageService={LanguageService},localizationService={LocalizationService},logger={Logger},translationSettings={TranslationSettings}): _localizationService → {_localizationService}", languageService, localizationService, logger, translationSettings, _localizationService);
        _logger = logger;
		_logger.LogError(">>> Entering TranslationModelFactory(languageService={LanguageService},localizationService={LocalizationService},logger={Logger},translationSettings={TranslationSettings})", languageService, localizationService, logger, translationSettings);
        _translationClientHelper = null;
        _translationSettings = translationSettings;
		_logger.LogWarning("<<< Exiting TranslationModelFactory(languageService={LanguageService},localizationService={LocalizationService},logger={Logger},translationSettings={TranslationSettings})", languageService, localizationService, logger, translationSettings);
		_logger.LogDebug("TranslationModelFactory(languageService={LanguageService},localizationService={LocalizationService},logger={Logger},translationSettings={TranslationSettings}): _translationClientHelper → {_translationClientHelper}", languageService, localizationService, logger, translationSettings, _translationClientHelper);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    public virtual async Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params string[] propertiesToTranslate)
        where T : ILocalizedLocaleModel
    {
        return await PrepareTranslationModelAsync(model, propertiesToTranslate.Select(p => (p, false)).ToArray());
    }

    /// <summary>
    /// Prepare translation model by the passed localized model
    /// </summary>
    /// <typeparam name="T">Localized model type</typeparam>
    /// <param name="model">The localized model to translate</param>
    /// <param name="propertiesToTranslate">List of properties which should be translated</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation model
    /// </returns>
    public virtual async Task<TranslationModel> PrepareTranslationModelAsync<T>(ILocalizedModel<T> model,
        params (string PropertyName, bool IsHtml)[] propertiesToTranslate)
        where T : ILocalizedLocaleModel
    {
		_logger.LogError(">>> Entering PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate})", model, propertiesToTranslate);
        _translationClientHelper ??= new TranslationClientHelper(_translationSettings);

        var result = new TranslationModel();
		_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): result → {Result}", model, propertiesToTranslate, result);

        var properties = propertiesToTranslate.Select(p => new KeyValuePair<string, bool>(p.PropertyName, p.IsHtml)).ToDictionary();
		_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): properties → {Properties}", model, propertiesToTranslate, properties);

        //get model properties to use as original text for translation
        var modelProperties = model.GetType().GetProperties()
            .Where(propertyFilter)
            .ToList();

        //get properties to save translated text
        var localizedProperties = typeof(T).GetProperties()
            .Where(propertyFilter)
            .ToList();

        //get original language
        var originalLanguage = await _languageService.GetLanguageByIdAsync(_translationSettings.TranslateFromLanguageId);
		_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): originalLanguage → {OriginalLanguage}", model, propertiesToTranslate, originalLanguage);
        var position = 0;

        //the loop by locales which should be translated
        foreach (var modelLocale in model.Locales)
        {
            position++;

            //we ignore the original language
            if (modelLocale.LanguageId == _translationSettings.TranslateFromLanguageId)
                continue;

            //and languages which should be ignored
            if (_translationSettings.NotTranslateLanguages.Contains(modelLocale.LanguageId))
                continue;

            //get target languages to translation for
            var translateToLanguage = await _languageService.GetLanguageByIdAsync(modelLocale.LanguageId);
			_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): translateToLanguage → {TranslateToLanguage}", model, propertiesToTranslate, translateToLanguage);

            //the loop by properties which should be translated
            foreach (var prop in localizedProperties)
            {
                //get the current value of property
                var currentTranslatedText = prop.GetValue(modelLocale, null)?.ToString();
				_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): currentTranslatedText → {CurrentTranslatedText}", model, propertiesToTranslate, currentTranslatedText);

                //ignore the property which already has a value
                if (!string.IsNullOrEmpty(currentTranslatedText))
                    continue;

                //get original text to translate
                var originText = modelProperties.FirstOrDefault(p => p.Name.Equals(prop.Name))?.GetValue(model, null)?.ToString();
				_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): originText → {OriginText}", model, propertiesToTranslate, originText);

                //ignore the empty original text
                if (string.IsNullOrEmpty(originText))
                    continue;

                try
                {
                    var translatedText = await _translationClientHelper.TranslateAsync(originalLanguage, originText, translateToLanguage, properties[prop.Name]);
					_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): translatedText → {TranslatedText}", model, propertiesToTranslate, translatedText);
                    if (string.IsNullOrEmpty(translatedText))
                        continue;

                    //set translated text to property
                    prop.SetValue(modelLocale, translatedText);
                    var inputName = $"{nameof(model.Locales)}_{position - 1}__{prop.Name}";
					_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): inputName → {InputName}", model, propertiesToTranslate, inputName);
                    result.Translations.Add(new()
                    {
                        Name = inputName,
                        Value = translatedText,
                        OriginValue = originText,
                        Language = translateToLanguage.UniqueSeoCode,
                        OriginLanguage = originalLanguage.UniqueSeoCode
                    });
					_logger.LogDebug("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): OriginLanguage → {OriginLanguage}", model, propertiesToTranslate, OriginLanguage);
                }
                catch (Exception e)
                {
                    var serviceName = await _localizationService.GetLocalizedEnumAsync((TranslationServiceType)_translationSettings.TranslationServiceId);
					_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): serviceName → {ServiceName}", model, propertiesToTranslate, serviceName);
                    var errorMessage = $"{serviceName}: {e.Message}";
					_logger.LogError("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): errorMessage → {ErrorMessage}", model, propertiesToTranslate, errorMessage);
                    await _logger.ErrorAsync(errorMessage, e);
                    result.HasErrors = true;
					_logger.LogDebug("PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate}): result.HasErrors → {Arg2}", model, propertiesToTranslate, result.HasErrors);

                    //DeepL: stop translate if one of the languages aren't support
                    //to reduce error count
                    if (e.Message.Contains("Value for 'target_lang' not supported", StringComparison.InvariantCultureIgnoreCase) || e.Message.Contains("Value for 'source_lang' not supported", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
        }

		_logger.LogWarning("<<< Exiting PrepareTranslationModelAsync(model={Model},propertiesToTranslate={PropertiesToTranslate})", model, propertiesToTranslate);
        return result;

        //filter for get only string property which should be translated
        bool propertyFilter(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType == typeof(string) && properties.ContainsKey(propertyInfo.Name);
        }
    }

    #endregion

    #region Nested class

    /// <summary>
    /// Helper class to translation client
    /// </summary>
    protected class TranslationClientHelper
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly Google.Cloud.Translation.V2.TranslationClient _googleClient;
        protected readonly DeepLClient _deeplClient;

        #endregion

        #region Ctor

        public TranslationClientHelper(TranslationSettings translationSettings)
        {
			_logger.LogError(">>> Entering TranslationClientHelper(translationSettings={TranslationSettings})", translationSettings);
            switch ((TranslationServiceType)translationSettings.TranslationServiceId)
            {
                case TranslationServiceType.GoogleTranslate:
                    {
                        if (!string.IsNullOrEmpty(translationSettings.GoogleApiKey))
                            _googleClient = Google.Cloud.Translation.V2.TranslationClient.CreateFromApiKey(translationSettings.GoogleApiKey);
							_logger.LogError("TranslationClientHelper(translationSettings={TranslationSettings}): _googleClient → {_googleClient}", translationSettings, _googleClient);
							_logger.LogDebug("TranslationClientHelper(translationSettings={TranslationSettings}): _googleClient → {_googleClient}", translationSettings, _googleClient);
                        _deeplClient = null;
						_logger.LogError("TranslationClientHelper(translationSettings={TranslationSettings}): _deeplClient → {_deeplClient}", translationSettings, _deeplClient);
                    }
					_logger.LogDebug("TranslationClientHelper(translationSettings={TranslationSettings}): _deeplClient → {_deeplClient}", translationSettings, _deeplClient);
                    break;

                case TranslationServiceType.DeepL:
                    {
                        if (!string.IsNullOrEmpty(translationSettings.DeepLAuthKey))
                            _deeplClient = new DeepLClient(translationSettings.DeepLAuthKey);
                        _googleClient = null;
                    }
                    break;
            }
			_logger.LogWarning("<<< Exiting TranslationClientHelper(translationSettings={TranslationSettings})", translationSettings);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Translate text or html
        /// </summary>
        /// <param name="originalLanguage">The language to translate from</param>
        /// <param name="originText">The text or HTML to translate</param>
        /// <param name="targetLanguage">The target language to translate</param>
        /// <param name="isHtml">Indicate whether the text to translate should be considered as HTML</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the translated text
        /// </returns>
        public virtual async Task<string> TranslateAsync(Language originalLanguage, string originText, Language targetLanguage, bool isHtml)
        {
			_logger.LogError(">>> Entering TranslateAsync(originalLanguage={OriginalLanguage},originText={OriginText},targetLanguage={TargetLanguage},isHtml={IsHtml})", originalLanguage, originText, targetLanguage, isHtml);
            var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (_googleClient != null)
            {
                var response = isHtml
                    ? await _googleClient.TranslateHtmlAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode)
                    : await _googleClient.TranslateTextAsync(originText, targetLanguage.UniqueSeoCode, originalLanguage.UniqueSeoCode);

                return response.TranslatedText;
            }

            if (_deeplClient != null)
            {
                var response = await _deeplClient.TranslateTextAsync(originText, originalLanguage.UniqueSeoCode, targetLanguage.UniqueSeoCode, new TextTranslateOptions
                {
                    TagHandling = isHtml ? "html" : null
                });
				_logger.LogDebug("TranslateAsync(originalLanguage={OriginalLanguage},originText={OriginText},targetLanguage={TargetLanguage},isHtml={IsHtml}): TagHandling → {TagHandling}", originalLanguage, originText, targetLanguage, isHtml, TagHandling);

                return response.Text;
            }

			_logger.LogError("TranslateAsync(originalLanguage,originText,targetLanguage,isHtml)={OriginalLanguage},{OriginText},{TargetLanguage},{IsHtml}: external service call took {Arg4} ms", originalLanguage, originText, targetLanguage, isHtml, __logStart.ElapsedMilliseconds);
			_logger.LogWarning("<<< Exiting TranslateAsync(originalLanguage={OriginalLanguage},originText={OriginText},targetLanguage={TargetLanguage},isHtml={IsHtml})", originalLanguage, originText, targetLanguage, isHtml);
            return string.Empty;
        }

        #endregion
    }

    #endregion
}