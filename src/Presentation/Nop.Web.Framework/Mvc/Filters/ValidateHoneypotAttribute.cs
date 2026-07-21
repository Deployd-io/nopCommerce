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
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ValidateHoneypotAttribute() : base(typeof(ValidateHoneypotFilter))
    {
	logger.LogDebug(">>> Entering ValidateHoneypotAttribute()");
	logger.LogDebug("<<< Exiting ValidateHoneypotAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter enabling honeypot validation
    /// </summary>
    private class ValidateHoneypotFilter : IAsyncAuthorizationFilter
    {
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
            _logger = logger;
            _webHelper = webHelper;
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
			logger.LogDebug(">>> Entering ValidateHoneypotAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether honeypot is enabled
            if (!_securitySettings.HoneypotEnabled)
                return;

            //try get honeypot input value 
            var inputValue = await context.HttpContext.Request.GetFormValueAsync(_securitySettings.HoneypotInputName);

			logger.LogDebug("ValidateHoneypotAsync(context={Context}): inputValue → {InputValue}", context, inputValue);
            //if exists, bot is caught
            if (!StringValues.IsNullOrEmpty(inputValue))
            {
                //warning admin about it
                if (_securitySettings.LogHoneypotDetection) 
                    await _logger.WarningAsync(await _localizationService.GetResourceAsync("Honeypot.BotDetected"));

                //and redirect to the original page
                var page = _webHelper.GetThisPageUrl(true);
                context.Result = new RedirectResult(page);
				logger.LogDebug("ValidateHoneypotAsync(context={Context}): page → {Page}", context, page);
            }
			logger.LogDebug("<<< Exiting ValidateHoneypotAsync(context={Context})", context);
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
			logger.LogDebug(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await ValidateHoneypotAsync(context);
			logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}
