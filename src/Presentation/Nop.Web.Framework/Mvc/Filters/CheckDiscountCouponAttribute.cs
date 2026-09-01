using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that checks and applied discount coupon code to customer
/// </summary>
public sealed class CheckDiscountCouponAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public CheckDiscountCouponAttribute() : base(typeof(CheckDiscountCouponFilter))
    {
	_logger.LogDebug(">>> Entering CheckDiscountCouponAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that checks and applied discount coupon code to customer
    /// </summary>
    private class CheckDiscountCouponFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IDiscountService _discountService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CheckDiscountCouponFilter(ICustomerService customerService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IWorkContext workContext)
        {
			_logger.LogDebug(">>> Entering CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext})", customerService, discountService, localizationService, notificationService, workContext);
            _customerService = customerService;
			_logger.LogDebug("CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _customerService → {_customerService}", customerService, discountService, localizationService, notificationService, workContext, _customerService);
            _discountService = discountService;
			_logger.LogDebug("CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _discountService → {_discountService}", customerService, discountService, localizationService, notificationService, workContext, _discountService);
            _localizationService = localizationService;
			_logger.LogDebug("CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _localizationService → {_localizationService}", customerService, discountService, localizationService, notificationService, workContext, _localizationService);
            _notificationService = notificationService;
			_logger.LogDebug("CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _notificationService → {_notificationService}", customerService, discountService, localizationService, notificationService, workContext, _notificationService);
            _workContext = workContext;
			_logger.LogDebug("CheckDiscountCouponFilter(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},notificationService={NotificationService},workContext={WorkContext}): _workContext → {_workContext}", customerService, discountService, localizationService, notificationService, workContext, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckDiscountCouponAsync(ActionExecutingContext context)
        {
			_logger.LogDebug(">>> Entering CheckDiscountCouponAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //check request query parameters
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            if (!context.HttpContext.Request.Query.Any())
                return;
				_logger.LogInformation("CheckDiscountCouponAsync(context)={Context}: database query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogDebug("CheckDiscountCouponAsync(context={Context}): customer → {Customer}", context, customer);

            //ignore search engines
            if (customer.IsSearchEngineAccount())
                return;

            //try to get discount coupon code
            var queryKey = NopDiscountDefaults.DiscountCouponQueryParameter;
            if (!context.HttpContext.Request.Query.TryGetValue(queryKey, out var couponCodes) || StringValues.IsNullOrEmpty(couponCodes))
                return;

            //get validated discounts with passed coupon codes

            var validCouponCodes = new List<string>();
			_logger.LogDebug("CheckDiscountCouponAsync(context={Context}): validCouponCodes → {ValidCouponCodes}", context, validCouponCodes);
            var discounts = await couponCodes
                .SelectManyAwait(async couponCode => await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                .Distinct()
                .ToListAsync();

            foreach (var discount in discounts)
            {
                var result = await _discountService.ValidateDiscountAsync(discount, customer, [.. couponCodes]);
				_logger.LogDebug("CheckDiscountCouponAsync(context={Context}): result → {Result}", context, result);
                if (!result.IsValid)
                    continue;

                //apply discount coupon codes to customer
                await _customerService.ApplyDiscountCouponCodeAsync(customer, discount.CouponCode);

                validCouponCodes.Add(discount.CouponCode);
            }

            //show notifications for activated coupon codes
            var locale = await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Activated");
			_logger.LogDebug("CheckDiscountCouponAsync(context={Context}): locale → {Locale}", context, locale);
            foreach (var validCouponCode in validCouponCodes.Distinct())
            {
                _notificationService.SuccessNotification(string.Format(locale, WebUtility.HtmlEncode(validCouponCode)));
            }

            //show notifications for invalid coupon codes
            var invalidLocale = await _localizationService.GetResourceAsync("ShoppingCart.DiscountCouponCode.Invalid");
			_logger.LogDebug("CheckDiscountCouponAsync(context={Context}): invalidLocale → {InvalidLocale}", context, invalidLocale);
            foreach (var invalidCouponCode in couponCodes.Except(validCouponCodes.Distinct()))
            {
                _notificationService.WarningNotification(string.Format(invalidLocale, WebUtility.HtmlEncode(invalidCouponCode)));
            }
			_logger.LogDebug("<<< Exiting CheckDiscountCouponAsync(context={Context})", context);
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
            await CheckDiscountCouponAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}