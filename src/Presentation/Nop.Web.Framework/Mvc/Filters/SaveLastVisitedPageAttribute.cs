using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Data;
using Nop.Services.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents filter attribute that saves last visited page by customer
/// </summary>
public sealed class SaveLastVisitedPageAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveLastVisitedPageAttribute() : base(typeof(SaveLastVisitedPageFilter))
    {
	_logger.LogError(">>> Entering SaveLastVisitedPageAttribute()");
    _logger.LogWarning("<<< Exiting SaveLastVisitedPageAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that saves last visited page by customer
    /// </summary>
    private class SaveLastVisitedPageFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly CustomerSettings _customerSettings;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SaveLastVisitedPageFilter(CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            IRepository<GenericAttribute> genericAttributeRepository,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
			_logger.LogError(">>> Entering SaveLastVisitedPageFilter(customerSettings={CustomerSettings},genericAttributeService={GenericAttributeService},genericAttributeRepository={GenericAttributeRepository},webHelper={WebHelper},workContext={WorkContext})", customerSettings, genericAttributeService, genericAttributeRepository, webHelper, workContext);
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
			_logger.LogDebug("SaveLastVisitedPageFilter(customerSettings={CustomerSettings},genericAttributeService={GenericAttributeService},genericAttributeRepository={GenericAttributeRepository},webHelper={WebHelper},workContext={WorkContext}): _genericAttributeService → {_genericAttributeService}", customerSettings, genericAttributeService, genericAttributeRepository, webHelper, workContext, _genericAttributeService);
            _genericAttributeRepository = genericAttributeRepository;
			_logger.LogDebug("SaveLastVisitedPageFilter(customerSettings={CustomerSettings},genericAttributeService={GenericAttributeService},genericAttributeRepository={GenericAttributeRepository},webHelper={WebHelper},workContext={WorkContext}): _genericAttributeRepository → {_genericAttributeRepository}", customerSettings, genericAttributeService, genericAttributeRepository, webHelper, workContext, _genericAttributeRepository);
            _webHelper = webHelper;
			_logger.LogDebug("SaveLastVisitedPageFilter(customerSettings={CustomerSettings},genericAttributeService={GenericAttributeService},genericAttributeRepository={GenericAttributeRepository},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", customerSettings, genericAttributeService, genericAttributeRepository, webHelper, workContext, _webHelper);
            _workContext = workContext;
			_logger.LogDebug("SaveLastVisitedPageFilter(customerSettings={CustomerSettings},genericAttributeService={GenericAttributeService},genericAttributeRepository={GenericAttributeRepository},webHelper={WebHelper},workContext={WorkContext}): _workContext → {_workContext}", customerSettings, genericAttributeService, genericAttributeRepository, webHelper, workContext, _workContext);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SaveLastVisitedPageAsync(ActionExecutingContext context)
        {
			_logger.LogError(">>> Entering SaveLastVisitedPageAsync(context={Context})", context);
            var __logStart = System.Diagnostics.Stopwatch.StartNew();
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests
            if (!context.HttpContext.Request.IsGetRequest())
                return;

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether we store last visited page URL
            if (!_customerSettings.StoreLastVisitedPage)
                return;

            //get current page
            var pageUrl = _webHelper.GetThisPageUrl(true);
			_logger.LogError("SaveLastVisitedPageAsync(context={Context}): pageUrl → {PageUrl}", context, pageUrl);

            if (string.IsNullOrEmpty(pageUrl))
                return;

            //get previous last page
            var customer = await _workContext.GetCurrentCustomerAsync();
			_logger.LogError("SaveLastVisitedPageAsync(context={Context}): customer → {Customer}", context, customer);
            var previousPageAttribute = (await _genericAttributeService
                    .GetAttributesForEntityAsync(customer.Id, nameof(Customer)))
                .FirstOrDefault(attribute => attribute.Key
                    .Equals(NopCustomerDefaults.LastVisitedPageAttribute, StringComparison.InvariantCultureIgnoreCase));

            //save new one if don't match
            if (previousPageAttribute == null)
            {
                //insert without event notification
                await _genericAttributeRepository.InsertAsync(new GenericAttribute
                {
                    EntityId = customer.Id,
                    Key = NopCustomerDefaults.LastVisitedPageAttribute,
                    KeyGroup = nameof(Customer),
                    Value = pageUrl,
                    CreatedOrUpdatedDateUTC = DateTime.UtcNow
                }, false);
				_logger.LogError("SaveLastVisitedPageAsync(context)={Context}: save query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
            }
            else if (!pageUrl.Equals(previousPageAttribute.Value, StringComparison.InvariantCultureIgnoreCase))
            {
				_logger.LogDebug("SaveLastVisitedPageAsync(context={Context}): CreatedOrUpdatedDateUTC → {CreatedOrUpdatedDateUTC}", context, CreatedOrUpdatedDateUTC);
                //update without event notification
                previousPageAttribute.Value = pageUrl;
				_logger.LogDebug("SaveLastVisitedPageAsync(context={Context}): previousPageAttribute.Value → {Arg1}", context, previousPageAttribute.Value);
				_logger.LogDebug("SaveLastVisitedPageAsync(context={Context}): previousPageAttribute.CreatedOrUpdatedDateUTC → {Arg1}", context, previousPageAttribute.CreatedOrUpdatedDateUTC);
                previousPageAttribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow;

                await _genericAttributeRepository.UpdateAsync(previousPageAttribute, false);
				_logger.LogError("SaveLastVisitedPageAsync(context)={Context}: update query executed in {Arg1} ms", context, __logStart.ElapsedMilliseconds);
            }
			_logger.LogWarning("<<< Exiting SaveLastVisitedPageAsync(context={Context})", context);
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
            await SaveLastVisitedPageAsync(context);
            if (context.Result == null)
                await next();
				_logger.LogWarning("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}