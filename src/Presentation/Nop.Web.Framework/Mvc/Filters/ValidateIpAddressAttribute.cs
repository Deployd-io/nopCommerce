using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates IP address
/// </summary>
public sealed class ValidateIpAddressAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ValidateIpAddressAttribute() : base(typeof(ValidateIpAddressFilter))
    {
	_logger.LogDebug(">>> Entering ValidateIpAddressAttribute()");
	_logger.LogDebug("<<< Exiting ValidateIpAddressAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that validates IP address
    /// </summary>
    private class ValidateIpAddressFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly IWebHelper _webHelper;
        protected readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public ValidateIpAddressFilter(IWebHelper webHelper,
            SecuritySettings securitySettings)
        {
			_logger.LogDebug(">>> Entering ValidateIpAddressFilter(webHelper={WebHelper},securitySettings={SecuritySettings})", webHelper, securitySettings);
            _webHelper = webHelper;
            _securitySettings = securitySettings;
			_logger.LogDebug("ValidateIpAddressFilter(webHelper={WebHelper},securitySettings={SecuritySettings}): _securitySettings → {_securitySettings}", webHelper, securitySettings, _securitySettings);
			_logger.LogDebug("ValidateIpAddressFilter(webHelper={WebHelper},securitySettings={SecuritySettings}): _webHelper → {_webHelper}", webHelper, securitySettings, _webHelper);
			_logger.LogDebug("<<< Exiting ValidateIpAddressFilter(webHelper={WebHelper},securitySettings={SecuritySettings})", webHelper, securitySettings);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        private void ValidateIpAddress(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering ValidateIpAddress(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (context.HttpContext.Request == null)
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            //don't validate on the 'Access denied' page
            if (controllerName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("AccessDenied", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            //get allowed IP addresses
            var ipAddresses = _securitySettings.AdminAreaAllowedIpAddresses;

            //there are no restrictions
            if (ipAddresses == null || !ipAddresses.Any())
                return;

            //whether current IP is allowed
            var currentIp = _webHelper.GetCurrentIpAddress();
			_logger.LogDebug("ValidateIpAddress(context={Context}): currentIp → {CurrentIp}", context, currentIp);

            if (ipAddresses.Any(ip => ip.Equals(currentIp, StringComparison.InvariantCultureIgnoreCase)))
                return;

            //redirect to 'Access denied' page
            context.Result = new RedirectToActionResult("AccessDenied", "Security", context.RouteData.Values);
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
			_logger.LogDebug(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            ValidateIpAddress(context);
            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}