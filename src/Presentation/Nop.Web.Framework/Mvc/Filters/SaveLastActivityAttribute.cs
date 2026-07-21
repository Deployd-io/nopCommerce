using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Data;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that saves last customer activity date
/// </summary>
public sealed class SaveLastActivityAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveLastActivityAttribute() : base(typeof(SaveLastActivityFilter))
    {
	logger.LogDebug(">>> Entering SaveLastActivityAttribute()");
	logger.LogDebug("<<< Exiting SaveLastActivityAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that saves last customer activity date
    /// </summary>
    private class SaveLastActivityFilter : IAsyncActionFilter
    {
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
            _customerSettings = customerSettings;
            _customerRepository = customerRepository;
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
			logger.LogDebug(">>> Entering SaveLastActivityAsync(context={Context})", context);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //update last activity date
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer.LastActivityDateUtc.AddMinutes(_customerSettings.LastActivityMinutes) < DateTime.UtcNow)
			logger.LogDebug("SaveLastActivityAsync(context={Context}): customer → {Customer}", context, customer);
            {
                customer.LastActivityDateUtc = DateTime.UtcNow;
				logger.LogDebug("SaveLastActivityAsync(context={Context}): customer.LastActivityDateUtc → {Arg1}", context, customer.LastActivityDateUtc);

                //update customer without event notification
                await _customerRepository.UpdateAsync(customer, false);
				logger.LogInformation("SaveLastActivityAsync(context)={Context}: update query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
            }
			logger.LogDebug("<<< Exiting SaveLastActivityAsync(context={Context})", context);
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
            await SaveLastActivityAsync(context);
            if (context.Result == null)
                await next();
				logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}
