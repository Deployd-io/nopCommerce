using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Tax.Avalara.Models.EntityUseCode;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Components;

/// <summary>
/// Represents a view component to render an additional field on customer details, customer role details, product details, checkout attribute details views
/// </summary>
public class EntityUseCodeViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly ITaxPluginManager _taxPluginManager;

    #endregion

    #region Ctor

    public EntityUseCodeViewComponent(AvalaraTaxManager avalaraTaxManager,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        IProductService productService,
        IStaticCacheManager staticCacheManager,
        ITaxPluginManager taxPluginManager)
    {
		_logger.LogDebug(">>> Entering EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager})", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager);
        _avalaraTaxManager = avalaraTaxManager;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _avalaraTaxManager);
        _checkoutAttributeService = checkoutAttributeService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _checkoutAttributeService → {_checkoutAttributeService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _checkoutAttributeService);
        _customerService = customerService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _customerService → {_customerService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _genericAttributeService → {_genericAttributeService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _localizationService → {_localizationService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _localizationService);
        _permissionService = permissionService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _permissionService → {_permissionService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _permissionService);
        _productService = productService;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _productService → {_productService}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _productService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _staticCacheManager → {_staticCacheManager}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _staticCacheManager);
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("<<< Exiting EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager})", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager);
		_logger.LogDebug("EntityUseCodeViewComponent(avalaraTaxManager={AvalaraTaxManager},checkoutAttributeService={CheckoutAttributeService},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},permissionService={PermissionService},productService={ProductService},staticCacheManager={StaticCacheManager},taxPluginManager={TaxPluginManager}): _taxPluginManager → {_taxPluginManager}", avalaraTaxManager, checkoutAttributeService, customerService, genericAttributeService, localizationService, permissionService, productService, staticCacheManager, taxPluginManager, _taxPluginManager);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke the widget view component
    /// </summary>
    /// <param name="widgetZone">Widget zone</param>
    /// <param name="additionalData">Additional parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
		_logger.LogDebug(">>> Entering InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //ensure that model is passed
        if (additionalData is not BaseNopEntityModel entityModel)
            return Content(string.Empty);

        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return Content(string.Empty);

        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_TAX_SETTINGS))
            return Content(string.Empty);

        //ensure that it's a proper widget zone
        if (!widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock) &&
            !widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop) &&
            !widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock) &&
            !widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
        {
            return Content(string.Empty);
        }

        //get Avalara pre-defined entity use codes
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.EntityUseCodesCacheKey);
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): cacheKey → {CacheKey}", widgetZone, additionalData, cacheKey);
        var cachedEntityUseCodes = await _staticCacheManager.GetAsync(cacheKey, async () => await _avalaraTaxManager.GetEntityUseCodesAsync());
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): cachedEntityUseCodes → {CachedEntityUseCodes}", widgetZone, additionalData, cachedEntityUseCodes);

        var entityUseCodes = cachedEntityUseCodes?.Select(useCode => new SelectListItem
        {
            Value = useCode.code,
            Text = $"{useCode.name} ({string.Join(", ", useCode.validCountries)})"
        }).ToList() ?? new List<SelectListItem>();

        //add the special item for 'undefined' with empty guid value
        var defaultValue = Guid.Empty.ToString();
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): defaultValue → {DefaultValue}", widgetZone, additionalData, defaultValue);
        entityUseCodes.Insert(0, new SelectListItem
        {
            Value = defaultValue,
            Text = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.EntityUseCode.None")
        });

        //prepare model
        var model = new EntityUseCodeModel
        {
            Id = entityModel.Id,
            EntityUseCodes = entityUseCodes
        };

        //get entity by the model identifier
        BaseEntity entity = null;
		_logger.LogDebug("InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData}): entity → {Entity}", widgetZone, additionalData, entity);
        if (widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock))
        {
            model.PrecedingElementId = nameof(CustomerModel.IsTaxExempt);
            entity = await _customerService.GetCustomerByIdAsync(entityModel.Id);
        }

        if (widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
        {
            model.PrecedingElementId = nameof(CustomerRoleModel.TaxExempt);
            entity = await _customerService.GetCustomerRoleByIdAsync(entityModel.Id);
        }

        if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
        {
            model.PrecedingElementId = nameof(ProductModel.IsTaxExempt);
            entity = await _productService.GetProductByIdAsync(entityModel.Id);
        }

        if (widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
        {
            model.PrecedingElementId = nameof(CheckoutAttributeModel.IsTaxExempt);
            entity = await _checkoutAttributeService.GetAttributeByIdAsync(entityModel.Id);
        }

        //try to get previously saved entity use code
        model.AvalaraEntityUseCode = entity == null ? defaultValue :
            await _genericAttributeService.GetAttributeAsync<string>(entity, AvalaraTaxDefaults.EntityUseCodeAttribute);

		_logger.LogInformation("InvokeAsync(widgetZone,additionalData)={WidgetZone},{AdditionalData}: external service call took {Arg2} ms", widgetZone, additionalData, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting InvokeAsync(widgetZone={WidgetZone},additionalData={AdditionalData})", widgetZone, additionalData);
        return View("~/Plugins/Tax.Avalara/Views/EntityUseCode/EntityUseCode.cshtml", model);
    }

    #endregion
}