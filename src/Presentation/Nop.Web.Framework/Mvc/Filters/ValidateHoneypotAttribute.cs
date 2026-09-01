using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute enabling honeypot validation
/// </summary>
public sealed class ValidateHoneypotAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ValidateHoneypotAttribute() : base(typeof(ValidateHoneypotFilter))
    {
	_logger.LogDebug(">>> Entering ValidateHoneypotAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter enabling honeypot validation
    /// </summary>
    private class ValidateHoneypotFilter : IAsyncAuthorizationFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly ILogger _logger;
        protected readonly IWebHelper _webHelper;
        protected readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public ValidateHoneypotFilter(ILocalizationService localizationService,
            ILogger logger,
            IWebHelper webHelper,
            SecuritySettings securitySettings)
        {
            _localizationService = localizationService;
			_logger.LogDebug("ValidateHoneypotFilter(localizationService={LocalizationService},logger={Logger},webHelper={WebHelper},securitySettings={SecuritySettings}): _localizationService → {_localizationService}", localizationService, logger, webHelper, securitySettings, _localizationService);
            _logger = logger;
			_logger.LogDebug(">>> Entering ValidateHoneypotFilter(localizationService={LocalizationService},logger={Logger},webHelper={WebHelper},securitySettings={SecuritySettings})", localizationService, logger, webHelper, securitySettings);
            _webHelper = webHelper;
			_logger.LogDebug("ValidateHoneypotFilter(localizationService={LocalizationService},logger={Logger},webHelper={WebHelper},securitySettings={SecuritySettings}): _webHelper → {_webHelper}", localizationService, logger, webHelper, securitySettings, _webHelper);
			_logger.LogDebug("<<< Exiting ValidateHoneypotFilter(localizationService={LocalizationService},logger={Logger},webHelper={WebHelper},securitySettings={SecuritySettings})", localizationService, logger, webHelper, securitySettings);
            _securitySettings = securitySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidateHoneypotAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering ValidateHoneypotAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether honeypot is enabled
            if (!_securitySettings.HoneypotEnabled)
                return;

            //try get honeypot input value 
            var inputValue = await context.HttpContext.Request.GetFormValueAsync(_securitySettings.HoneypotInputName);
			_logger.LogDebug("ValidateHoneypotAsync(context={Context}): inputValue → {InputValue}", context, inputValue);

            //if exists, bot is caught
            if (!StringValues.IsNullOrEmpty(inputValue))
            {
                //warning admin about it
                if (_securitySettings.LogHoneypotDetection) 
                    await _logger.WarningAsync(await _localizationService.GetResourceAsync("Honeypot.BotDetected"));

                //and redirect to the original page
                var page = _webHelper.GetThisPageUrl(true);
				_logger.LogDebug("ValidateHoneypotAsync(context={Context}): page → {Page}", context, page);
                context.Result = new RedirectResult(page);
            }
			_logger.LogDebug("<<< Exiting ValidateHoneypotAsync(context={Context})", context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
			_logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await ValidateHoneypotAsync(context);
			_logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}