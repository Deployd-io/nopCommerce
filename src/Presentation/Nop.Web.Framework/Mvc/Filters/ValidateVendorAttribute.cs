using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
/// </summary>
public sealed class ValidateVendorAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public ValidateVendorAttribute(bool ignore = false) : base(typeof(ValidateVendorFilter))
    {
		_logger.LogError(">>> Entering ValidateVendorAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
        Arguments = [ignore];
		_logger.LogDebug("ValidateVendorAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
    _logger.LogWarning("<<< Exiting ValidateVendorAttribute(ignore={Ignore})", ignore);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
    /// </summary>
    private class ValidateVendorFilter : IAsyncAuthorizationFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ValidateVendorFilter(bool ignoreFilter, IWorkContext workContext, ICustomerService customerService)
        {
			_logger.LogError(">>> Entering ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService})", ignoreFilter, workContext, customerService);
            _ignoreFilter = ignoreFilter;
            _customerService = customerService;
			_logger.LogDebug("ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService}): _customerService → {_customerService}", ignoreFilter, workContext, customerService, _customerService);
            _workContext = workContext;
			_logger.LogDebug("ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService}): _workContext → {_workContext}", ignoreFilter, workContext, customerService, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task ValidateVendorAsync(AuthorizationFilterContext context)
        {
			_logger.LogError(">>> Entering ValidateVendorAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<ValidateVendorAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if the current customer isn't a vendor)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //whether current customer is vendor
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("ValidateVendorAsync(context={Context}): customer → {Customer}", context, customer);
            if (!await _customerService.IsVendorAsync(customer))
                return;

            //ensure that this user has active vendor record associated
            var vendor = await _workContext.GetCurrentVendorAsync();
			_logger.LogError("ValidateVendorAsync(context={Context}): vendor → {Vendor}", context, vendor);
            if (vendor == null)
                context.Result = new ChallengeResult();
				_logger.LogWarning("<<< Exiting ValidateVendorAsync(context={Context})", context);
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
			_logger.LogError(">>> Entering OnAuthorizationAsync(context={Context})", context);
            await ValidateVendorAsync(context);
			_logger.LogWarning("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}