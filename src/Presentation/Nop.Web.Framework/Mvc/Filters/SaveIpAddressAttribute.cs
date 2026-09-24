using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that saves last IP address of customer
/// </summary>
public sealed class SaveIpAddressAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveIpAddressAttribute() : base(typeof(SaveIpAddressFilter))
    {
	_logger.LogError(">>> Entering SaveIpAddressAttribute()");
    _logger.LogWarning("<<< Exiting SaveIpAddressAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that saves last IP address of customer
    /// </summary>
    private class SaveIpAddressFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly IRepository<Customer> _customerRepository;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SaveIpAddressFilter(CustomerSettings customerSettings,
            IRepository<Customer> customerRepository,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
			_logger.LogError(">>> Entering SaveIpAddressFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},webHelper={WebHelper},workContext={WorkContext})", customerSettings, customerRepository, webHelper, workContext);
            _customerSettings = customerSettings;
            _customerRepository = customerRepository;
			_logger.LogDebug("SaveIpAddressFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},webHelper={WebHelper},workContext={WorkContext}): _customerRepository → {_customerRepository}", customerSettings, customerRepository, webHelper, workContext, _customerRepository);
            _webHelper = webHelper;
			_logger.LogDebug("SaveIpAddressFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", customerSettings, customerRepository, webHelper, workContext, _webHelper);
            _workContext = workContext;
			_logger.LogDebug("SaveIpAddressFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},webHelper={WebHelper},workContext={WorkContext}): _workContext → {_workContext}", customerSettings, customerRepository, webHelper, workContext, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SaveIpAddressAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering SaveIpAddressAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether we store IP addresses
            if (!_customerSettings.StoreIpAddresses)
                return;

            //get current IP address
            var currentIpAddress = _webHelper.GetCurrentIpAddress();
			_logger.LogError("SaveIpAddressAsync(context={Context}): currentIpAddress → {CurrentIpAddress}", context, currentIpAddress);

            if (string.IsNullOrEmpty(currentIpAddress))
                return;

            //update customer's IP address
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("SaveIpAddressAsync(context={Context}): customer → {Customer}", context, customer);
            if (_workContext.OriginalCustomerIfImpersonated == null &&
                !currentIpAddress.Equals(customer.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                customer.LastIpAddress = currentIpAddress;
				_logger.LogDebug("SaveIpAddressAsync(context={Context}): customer.LastIpAddress → {Arg1}", context, customer.LastIpAddress);

                //update customer without event notification
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("SaveIpAddressAsync(context)={Context}: update query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
                await _customerRepository.UpdateAsync(customer, false);
            }
			_logger.LogWarning("<<< Exiting SaveIpAddressAsync(context={Context})", context);
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
            await SaveIpAddressAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}