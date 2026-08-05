using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Factories;
using Nop.Plugin.Payments.PayPalCommerce.Models.Public;
using Nop.Plugin.Payments.PayPalCommerce.Services;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Payments.PayPalCommerce.Components.Public;

/// <summary>
/// Represents the view component to display PayPal buttons in the public store
/// </summary>
public class ButtonsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly IProductService _productService;
    private readonly PayPalCommerceModelFactory _modelFactory;
    private readonly PayPalCommerceServiceManager _serviceManager;
    private readonly PayPalCommerceSettings _settings;

    #endregion

    #region Ctor

    public ButtonsViewComponent(IProductService productService,
        PayPalCommerceModelFactory modelFactory,
        PayPalCommerceServiceManager serviceManager,
        PayPalCommerceSettings settings)
    {
        _productService = productService;
        _modelFactory = modelFactory;
        _serviceManager = serviceManager;
        _settings = settings;
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
        var (active, _) = await _serviceManager.IsActiveAsync(_settings);
        if (!active)
            return Content(string.Empty);

        PaymentInfoModel model = null;

		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): model → {Model}", widgetZone, additionalData, model);
        if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo))
        {
            if (_settings.DisplayButtonsOnProductDetails)
            {
                var productId = additionalData is ProductDetailsModel.AddToCartModel product ? (int?)product.ProductId : null;
                if (productId is null || (await _productService.GetProductByIdAsync(productId ?? 0))?.ParentGroupedProductId == 0)
					_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): productId → {ProductId}", widgetZone, additionalData, productId);
                    model = await _modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.Product, productId);
            }
        }
        else if (widgetZone.Equals(PublicWidgetZones.OrderSummaryContentBefore))
        {
            if (_settings.DisplayButtonsOnShoppingCart)
            {
                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (routeName == NopRouteNames.General.CART)
					_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): routeName → {RouteName}", widgetZone, additionalData, routeName);
                    model = await _modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.Cart);
            }
        }
        else
        {
            if (_settings.DisplayButtonsOnPaymentMethod)
                model = await _modelFactory.PreparePaymentInfoModelAsync(ButtonPlacement.PaymentMethod);
        }

        if (model?.Cart.IsRecurring is null)
            return Content(string.Empty);

        if (model.Cart.IsRecurring == true)
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
            return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Buttons.Recurring.cshtml", model);

        return View("~/Plugins/Payments.PayPalCommerce/Views/Public/_Buttons.cshtml", model);
    }

    #endregion
}