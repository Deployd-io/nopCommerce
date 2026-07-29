using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
/// </summary>
public sealed class ValidateVendorAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public ValidateVendorAttribute(bool ignore = false) : base(typeof(ValidateVendorFilter))
    {
		logger.LogDebug(">>> Entering ValidateVendorAttribute(ignore={Ignore})", ignore);
        IgnoreFilter = ignore;
        Arguments = [ignore];
		logger.LogDebug("<<< Exiting ValidateVendorAttribute(ignore={Ignore})", ignore);
		logger.LogDebug("ValidateVendorAttribute(ignore={Ignore}): Arguments → {Arguments}", ignore, Arguments);
		logger.LogDebug("ValidateVendorAttribute(ignore={Ignore}): IgnoreFilter → {IgnoreFilter}", ignore, IgnoreFilter);
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
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ValidateVendorFilter(bool ignoreFilter, IWorkContext workContext, ICustomerService customerService)
        {
			logger.LogDebug(">>> Entering ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService})", ignoreFilter, workContext, customerService);
            _ignoreFilter = ignoreFilter;
            _customerService = customerService;
			logger.LogDebug("ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService}): _customerService → {_customerService}", ignoreFilter, workContext, customerService, _customerService);
			logger.LogDebug("ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService}): _ignoreFilter → {_ignoreFilter}", ignoreFilter, workContext, customerService, _ignoreFilter);
            _workContext = workContext;
			logger.LogDebug("<<< Exiting ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService})", ignoreFilter, workContext, customerService);
			logger.LogDebug("ValidateVendorFilter(ignoreFilter={IgnoreFilter},workContext={WorkContext},customerService={CustomerService}): _workContext → {_workContext}", ignoreFilter, workContext, customerService, _workContext);
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
			logger.LogDebug(">>> Entering ValidateVendorAsync(context={Context})", context);
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
            if (!await _customerService.IsVendorAsync(customer))
				logger.LogDebug("ValidateVendorAsync(context={Context}): customer → {Customer}", context, customer);
                return;

            //ensure that this user has active vendor record associated
            var vendor = await _workContext.GetCurrentVendorAsync();
            if (vendor == null)
				logger.LogDebug("ValidateVendorAsync(context={Context}): vendor → {Vendor}", context, vendor);
                context.Result = new ChallengeResult();
				logger.LogDebug("<<< Exiting ValidateVendorAsync(context={Context})", context);
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
            await ValidateVendorAsync(context);
			logger.LogDebug("<<< Exiting OnAuthorizationAsync(context={Context})", context);
        }

        #endregion
    }

    #endregion
}