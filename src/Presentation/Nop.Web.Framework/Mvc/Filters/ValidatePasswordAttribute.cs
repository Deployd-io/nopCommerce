using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Http;
using Nop.Data;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates customer password expiration
/// </summary>
public sealed class ValidatePasswordAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ValidatePasswordAttribute() : base(typeof(ValidatePasswordFilter))
    {
	_logger.LogError(">>> Entering ValidatePasswordAttribute()");
    _logger.LogWarning("<<< Exiting ValidatePasswordAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that validates customer password expiration
    /// </summary>
    private class ValidatePasswordFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ValidatePasswordFilter(ICustomerService customerService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
			_logger.LogError(">>> Entering ValidatePasswordFilter(customerService={CustomerService},webHelper={WebHelper},workContext={WorkContext})", customerService, webHelper, workContext);
            _customerService = customerService;
            _webHelper = webHelper;
			_logger.LogDebug("ValidatePasswordFilter(customerService={CustomerService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", customerService, webHelper, workContext, _webHelper);
            _workContext = workContext;
			_logger.LogDebug("ValidatePasswordFilter(customerService={CustomerService},webHelper={WebHelper},workContext={WorkContext}): _workContext → {_workContext}", customerService, webHelper, workContext, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidatePasswordAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering ValidatePasswordAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (context.HttpContext.Request == null)
                return;

            //ignore AJAX requests
            if (_webHelper.IsAjaxRequest(context.HttpContext.Request))
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            //don't validate on the 'Change Password' page
            if (controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("ChangePassword", StringComparison.InvariantCultureIgnoreCase))
                return;

            //check whether this filter has been overridden for the Action
            //we're checking CheckAccessPublicStore attribute because it applies to all required actions that should be available regardless of password validation
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckAccessPublicStoreAttribute>()
                .FirstOrDefault();

            //don't validate actions that are available even if navigation is not allowed
            if (actionFilter?.IgnoreFilter == true)
                return;

            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsPasswordExpiredAsync(customer) && !customer.MustChangePassword)
                return;

            var returnUrl = _webHelper.GetRawUrl(context.HttpContext.Request);
            //redirect to ChangePassword page if expires
            context.Result = new RedirectToRouteResult(NopRouteNames.Standard.CUSTOMER_CHANGE_PASSWORD, new { returnUrl = returnUrl });
			_logger.LogWarning("<<< Exiting ValidatePasswordAsync(context={Context})", context);
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
            await ValidatePasswordAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}