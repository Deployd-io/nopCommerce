using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that adds a detailed validation message that a value cannot be empty
/// </summary>
public sealed class NotNullValidationMessageAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public NotNullValidationMessageAttribute() : base(typeof(NotNullValidationMessageFilter))
    {
	_logger.LogError(">>> Entering NotNullValidationMessageAttribute()");
    _logger.LogWarning("<<< Exiting NotNullValidationMessageAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that adds a detailed validation message that a value cannot be empty
    /// </summary>
    private class NotNullValidationMessageFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public NotNullValidationMessageFilter(ILocalizationService localizationService)
        {
			_logger.LogError(">>> Entering NotNullValidationMessageFilter(localizationService={LocalizationService})", localizationService);
            _localizationService = localizationService;
			_logger.LogDebug("NotNullValidationMessageFilter(localizationService={LocalizationService}): _localizationService → {_localizationService}", localizationService, _localizationService);
        _logger.LogWarning("<<< Exiting NotNullValidationMessageFilter(localizationService={LocalizationService})", localizationService);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckNotNullValidationAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering CheckNotNullValidationAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in POST requests
            if (!context.HttpContext.Request.IsPostRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether the model state is invalid
            if (context.ModelState.ErrorCount == 0)
                return;

            var nullModelValues = context.ModelState
                .Where(modelState => modelState.Value.ValidationState == ModelValidationState.Invalid
                                     && modelState.Value.Errors.Any(error => error.ErrorMessage.Equals(NopValidationDefaults.NotNullValidationLocaleName)))
                .ToList();
            if (!nullModelValues.Any())
                return;

            //get model passed to the action
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            var model = context.ActionArguments.Values.OfType<BaseNopModel>().FirstOrDefault();
				_logger.LogError("CheckNotNullValidationAsync(context={Context}): model → {Model}", context, model);
				_logger.LogError("CheckNotNullValidationAsync(context)={Context}: database query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
            if (model is null) {
                return;
            }

            //get model properties that failed validation
            var modelType = model.GetType();
			_logger.LogError("CheckNotNullValidationAsync(context={Context}): modelType → {ModelType}", context, modelType);
            var properties = modelType.GetProperties();
			_logger.LogError("CheckNotNullValidationAsync(context={Context}): properties → {Properties}", context, properties);
            var locale = await _localizationService.GetResourceAsync(NopValidationDefaults.NotNullValidationLocaleName);
			_logger.LogError("CheckNotNullValidationAsync(context={Context}): locale → {Locale}", context, locale);
            foreach (var modelState in nullModelValues)
            {
                if (modelState.Value == null)
                    continue;

                var property = properties
                    .FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(modelState.Key, StringComparison.InvariantCultureIgnoreCase));
                
                if (property is null)
                    continue;

                var resourceName = modelType.GetProperty(property.Name)?.GetCustomAttributes(typeof(NopResourceDisplayNameAttribute), true)
                    .OfType<NopResourceDisplayNameAttribute>()
                    .FirstOrDefault()?.ResourceKey;
                
                string resourceValue = null;
				_logger.LogError("CheckNotNullValidationAsync(context={Context}): resourceValue → {ResourceValue}", context, resourceValue);

                if (!string.IsNullOrEmpty(resourceName))
                    //get locale resource value
                    resourceValue = await _localizationService.GetResourceAsync(resourceName);

                if (resourceValue?.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    resourceValue = property.Name;

                //set localized error message
                modelState.Value.Errors.Clear();
                modelState.Value.Errors.Add(string.Format(locale, resourceValue));
            }
			_logger.LogWarning("<<< Exiting CheckNotNullValidationAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
			_logger.LogError(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            await CheckNotNullValidationAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}