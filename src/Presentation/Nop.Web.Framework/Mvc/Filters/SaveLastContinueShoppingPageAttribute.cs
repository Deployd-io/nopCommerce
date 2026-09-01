using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that allow to store current URL in the generic attribute for "Continue Shopping" button
/// </summary>
public sealed class SaveLastContinueShoppingPageAttribute : TypeFilterAttribute
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveLastContinueShoppingPageAttribute() : base(typeof(SaveLastContinueShoppingPageFilter))
    {
	_logger.LogDebug(">>> Entering SaveLastContinueShoppingPageAttribute()");
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
    /// </summary>
    private class SaveLastContinueShoppingPageFilter : IAsyncActionFilter
    {
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        #region Fields

        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SaveLastContinueShoppingPageFilter(IGenericAttributeService genericAttributeService, IStoreContext storeContext, IWebHelper webHelper, IWorkContext workContext)
        {
			_logger.LogDebug(">>> Entering SaveLastContinueShoppingPageFilter(genericAttributeService={GenericAttributeService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext})", genericAttributeService, storeContext, webHelper, workContext);
            _genericAttributeService = genericAttributeService;
			_logger.LogDebug("SaveLastContinueShoppingPageFilter(genericAttributeService={GenericAttributeService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, storeContext, webHelper, workContext, _genericAttributeService);
            _storeContext = storeContext;
			_logger.LogDebug("SaveLastContinueShoppingPageFilter(genericAttributeService={GenericAttributeService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _storeContext → {_storeContext}", genericAttributeService, storeContext, webHelper, workContext, _storeContext);
            _webHelper = webHelper;
			_logger.LogDebug("SaveLastContinueShoppingPageFilter(genericAttributeService={GenericAttributeService},storeContext={StoreContext},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", genericAttributeService, storeContext, webHelper, workContext, _webHelper);
            _workContext = workContext;
        }

        #endregion
        
        #region Methods

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
			_logger.LogDebug(">>> Entering OnActionExecutionAsync(context={Context},next={Next})", context, next);
            var store = await _storeContext.GetCurrentStoreAsync();

			_logger.LogDebug("OnActionExecutionAsync(context={Context},next={Next}): store → {Store}", context, next, store);
            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            if (context.Result == null)
                await next();
				_logger.LogDebug("<<< Exiting OnActionExecutionAsync(context={Context},next={Next})", context, next);
        }

        #endregion
    }

    #endregion
}