using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that saves last customer activity date
/// </summary>
public sealed class SaveLastActivityAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveLastActivityAttribute() : base(typeof(SaveLastActivityFilter))
    {
	_logger.LogError(">>> Entering SaveLastActivityAttribute()");
    _logger.LogWarning("<<< Exiting SaveLastActivityAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that saves last customer activity date
    /// </summary>
    private class SaveLastActivityFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly IRepository<Customer> _customerRepository;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SaveLastActivityFilter(CustomerSettings customerSettings,
            IRepository<Customer> customerRepository,
            IWorkContext workContext)
        {
			_logger.LogError(">>> Entering SaveLastActivityFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},workContext={WorkContext})", customerSettings, customerRepository, workContext);
            _customerSettings = customerSettings;
            _customerRepository = customerRepository;
			_logger.LogDebug("SaveLastActivityFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},workContext={WorkContext}): _customerRepository → {_customerRepository}", customerSettings, customerRepository, workContext, _customerRepository);
			_logger.LogDebug("SaveLastActivityFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},workContext={WorkContext}): _customerSettings → {_customerSettings}", customerSettings, customerRepository, workContext, _customerSettings);
        _logger.LogWarning("<<< Exiting SaveLastActivityFilter(customerSettings={CustomerSettings},customerRepository={CustomerRepository},workContext={WorkContext})", customerSettings, customerRepository, workContext);
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SaveLastActivityAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering SaveLastActivityAsync(context={Context})", context);
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //update last activity date
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("SaveLastActivityAsync(context={Context}): customer → {Customer}", context, customer);
            if (customer.LastActivityDateUtc.AddMinutes(_customerSettings.LastActivityMinutes) < DateTime.UtcNow)
            {
                customer.LastActivityDateUtc = DateTime.UtcNow;
				_logger.LogDebug("SaveLastActivityAsync(context={Context}): customer.LastActivityDateUtc → {Arg1}", context, customer.LastActivityDateUtc);

                //update customer without event notification
				var __logStart = System.Diagnostics.Stopwatch.StartNew();
				_logger.LogError("SaveLastActivityAsync(context)={Context}: update query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
                await _customerRepository.UpdateAsync(customer, false);
            }
			_logger.LogWarning("<<< Exiting SaveLastActivityAsync(context={Context})", context);
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
            await SaveLastActivityAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}