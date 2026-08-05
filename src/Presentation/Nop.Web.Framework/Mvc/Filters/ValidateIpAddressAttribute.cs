using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Data;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates IP address
/// </summary>
public sealed class ValidateIpAddressAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ValidateIpAddressAttribute() : base(typeof(ValidateIpAddressFilter))
    {
	logger.LogDebug(">>> Entering ValidateIpAddressAttribute()");
	logger.LogDebug("<<< Exiting ValidateIpAddressAttribute()");
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
            _webHelper = webHelper;
            _securitySettings = securitySettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        private void ValidateIpAddress(ActionExecutingContext context)
        {
			logger.LogDebug(">>> Entering ValidateIpAddress(context={Context})", context);
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

			logger.LogDebug("ValidateIpAddress(context={Context}): currentIp → {CurrentIp}", context, currentIp);
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
			logger.LogDebug(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            ValidateIpAddress(context);
            if (context.Result == null)
                await next();
				logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}