using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Affiliates;
using Nop.Services.Customers;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that checks and updates affiliate of customer
/// </summary>
public sealed class CheckAffiliateAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public CheckAffiliateAttribute() : base(typeof(CheckAffiliateFilter))
    {
	_logger.LogError(">>> Entering CheckAffiliateAttribute()");
    _logger.LogWarning("<<< Exiting CheckAffiliateAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that checks and updates affiliate of customer
    /// </summary>
    private class CheckAffiliateFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Constants

        private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
        private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

        #endregion

        #region Fields

        protected readonly IAffiliateService _affiliateService;
        protected readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CheckAffiliateFilter(IAffiliateService affiliateService,
            ICustomerService customerService,
            IWorkContext workContext)
        {
			_logger.LogError(">>> Entering CheckAffiliateFilter(affiliateService={AffiliateService},customerService={CustomerService},workContext={WorkContext})", affiliateService, customerService, workContext);
            _affiliateService = affiliateService;
            _customerService = customerService;
			_logger.LogDebug("CheckAffiliateFilter(affiliateService={AffiliateService},customerService={CustomerService},workContext={WorkContext}): _customerService → {_customerService}", affiliateService, customerService, workContext, _customerService);
			_logger.LogDebug("CheckAffiliateFilter(affiliateService={AffiliateService},customerService={CustomerService},workContext={WorkContext}): _affiliateService → {_affiliateService}", affiliateService, customerService, workContext, _affiliateService);
        _logger.LogWarning("<<< Exiting CheckAffiliateFilter(affiliateService={AffiliateService},customerService={CustomerService},workContext={WorkContext})", affiliateService, customerService, workContext);
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Set the affiliate identifier of current customer
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SetCustomerAffiliateIdAsync(Affiliate affiliate, Customer customer)
        {
			_logger.LogError(">>> Entering SetCustomerAffiliateIdAsync(affiliate={Affiliate},customer={Customer})", affiliate, customer);
            if (affiliate == null || affiliate.Deleted || !affiliate.Active)
                return;

            if (affiliate.Id == customer.AffiliateId)
                return;

            //ignore system accounts
            if (customer.IsSystemAccount)
                return;

            //update affiliate identifier
            customer.AffiliateId = affiliate.Id;
			_logger.LogDebug("SetCustomerAffiliateIdAsync(affiliate={Affiliate},customer={Customer}): customer.AffiliateId → {Arg2}", affiliate, customer, customer.AffiliateId);
            await _customerService.UpdateCustomerAsync(customer);
			_logger.LogWarning("<<< Exiting SetCustomerAffiliateIdAsync(affiliate={Affiliate},customer={Customer})", affiliate, customer);
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckAffiliateAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering CheckAffiliateAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //check request query parameters
            var request = context.HttpContext.Request;
            if (request?.Query == null || !request.Query.Any())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //try to find by ID
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("CheckAffiliateAsync(context={Context}): customer → {Customer}", context, customer);
            var affiliateIds = request.Query[AFFILIATE_ID_QUERY_PARAMETER_NAME];

            if (int.TryParse(affiliateIds.FirstOrDefault(), out var affiliateId) && affiliateId > 0 && affiliateId != customer.AffiliateId)
            {
                var affiliate = await _affiliateService.GetAffiliateByIdAsync(affiliateId);
				_logger.LogError("CheckAffiliateAsync(context={Context}): affiliate → {Affiliate}", context, affiliate);
                await SetCustomerAffiliateIdAsync(affiliate, customer);
                return;
            }

            //try to find by friendly name
            var affiliateNames = request.Query[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
            var affiliateName = affiliateNames.FirstOrDefault();
			_logger.LogError("CheckAffiliateAsync(context={Context}): affiliateName → {AffiliateName}", context, affiliateName);
            if (!string.IsNullOrEmpty(affiliateName))
            {
                var affiliate = await _affiliateService.GetAffiliateByFriendlyUrlNameAsync(affiliateName);
                await SetCustomerAffiliateIdAsync(affiliate, customer);
            }
			_logger.LogWarning("<<< Exiting CheckAffiliateAsync(context={Context})", context);
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
            await CheckAffiliateAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}