using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Data;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that validates force of the multi-factor authentication
/// </summary>
public sealed class ForceMultiFactorAuthenticationAttribute : TypeFilterAttribute
{
private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public ForceMultiFactorAuthenticationAttribute() : base(typeof(ForceMultiFactorAuthenticationFilter))
    {
	_logger.LogError(">>> Entering ForceMultiFactorAuthenticationAttribute()");
    _logger.LogWarning("<<< Exiting ForceMultiFactorAuthenticationAttribute()");
    }

    #endregion

    #region Nested filter

    private class ForceMultiFactorAuthenticationFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        protected readonly IPermissionService _permissionService;
        protected readonly IWorkContext _workContext;
        protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

        #endregion

        #region Ctor

        public ForceMultiFactorAuthenticationFilter(IGenericAttributeService genericAttributeService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            IPermissionService permissionService,
            IWorkContext workContext,
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
        {
			_logger.LogError(">>> Entering ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings})", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings);
            _genericAttributeService = genericAttributeService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
			_logger.LogDebug("ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _multiFactorAuthenticationPluginManager → {_multiFactorAuthenticationPluginManager}", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings, _multiFactorAuthenticationPluginManager);
			_logger.LogDebug("ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings, _genericAttributeService);
        _logger.LogWarning("<<< Exiting ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings})", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings);
            _permissionService = permissionService;
			_logger.LogDebug("ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _permissionService → {_permissionService}", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings, _permissionService);
            _workContext = workContext;
			_logger.LogDebug("ForceMultiFactorAuthenticationFilter(genericAttributeService={GenericAttributeService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},workContext={WorkContext},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _workContext → {_workContext}", genericAttributeService, multiFactorAuthenticationPluginManager, permissionService, workContext, multiFactorAuthenticationSettings, _workContext);
            _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidateAuthenticationForceAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering ValidateAuthenticationForceAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (context.HttpContext.Request == null)
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether the feature is enabled
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Security.ENABLE_MULTI_FACTOR_AUTHENTICATION))
                return;

            //don't validate on the 'Multi-factor authentication settings' page
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;

            if (controllerName.Equals("Customer", StringComparison.InvariantCultureIgnoreCase) &&
                actionName.Equals("MultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            //whether multi-factor authentication is enforced
            if (!_multiFactorAuthenticationSettings.ForceMultifactorAuthentication ||
                !await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                return;
            }

            //check selected provider of MFA
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("ValidateAuthenticationForceAsync(context={Context}): customer → {Customer}", context, customer);
            var selectedProvider = await _genericAttributeService
                .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
			_logger.LogWarning("<<< Exiting ValidateAuthenticationForceAsync(context={Context})", context);
            if (!string.IsNullOrEmpty(selectedProvider)) {
                return;
            }

            //redirect to MultiFactorAuthenticationSettings page if force is enabled
            context.Result = new RedirectToRouteResult(NopRouteNames.Standard.MULTI_FACTOR_AUTHENTICATION_SETTINGS, null);
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
            await ValidateAuthenticationForceAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}