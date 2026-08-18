using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Components;

/// <summary>
/// Represents the view component to display additional buttons in the shopping cart
/// </summary>
public class AddRfqComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public AddRfqComponent(IProductService productService,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IWorkContext workContext,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
		_logger.LogDebug(">>> Entering AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings})", productService, storeContext, workContext, rfqService, rfqSettings);
        _productService = productService;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _productService → {_productService}", productService, storeContext, workContext, rfqService, rfqSettings, _productService);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _shoppingCartService → {_shoppingCartService}", productService, storeContext, workContext, rfqService, rfqSettings, _shoppingCartService);
        _storeContext = storeContext;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _storeContext → {_storeContext}", productService, storeContext, workContext, rfqService, rfqSettings, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _workContext → {_workContext}", productService, storeContext, workContext, rfqService, rfqSettings, _workContext);
        _rfqService = rfqService;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqService → {_rfqService}", productService, storeContext, workContext, rfqService, rfqSettings, _rfqService);
        _rfqSettings = rfqSettings;
		_logger.LogDebug("AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqSettings → {_rfqSettings}", productService, storeContext, workContext, rfqService, rfqSettings, _rfqSettings);
		_logger.LogDebug("<<< Exiting AddRfqComponent(productService={ProductService},shoppingCartService=<redacted>,storeContext={StoreContext},workContext={WorkContext},rfqService={RfqService},rfqSettings={RfqSettings})", productService, storeContext, workContext, rfqService, rfqSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="widgetZone">Widget zone name</param>
    /// <param name="additionalData">Additional data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        if (!_rfqSettings.Enabled)
            return Content(string.Empty);

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): customer → {Customer}", widgetZone, additionalData, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): store → {Store}", widgetZone, additionalData, store);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): cart → {Cart}", widgetZone, additionalData, cart);

        var products = await _productService.GetProductsByIdsAsync(cart.Select(i => i.ProductId).ToArray());
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): products → {Products}", widgetZone, additionalData, products);

        //"enter your price" or rental products in the cart, so we should not show the "Request a quote" button
        if (products.Any(product => product.CustomerEntersPrice || product.IsRental))
            return Content(string.Empty);

        //is shopping cart created by quote
        if (await cart.AnyAwaitAsync(async shoppingCartItemModel => (await _rfqService.GetQuoteItemByShoppingCartItemIdAsync(shoppingCartItemModel.Id)) != null))
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return View("~/Plugins/Misc.RFQ/Views/Components/ClearRfq.cshtml");

        return View("~/Plugins/Misc.RFQ/Views/Components/AddRfq.cshtml");
    }

    #endregion
}