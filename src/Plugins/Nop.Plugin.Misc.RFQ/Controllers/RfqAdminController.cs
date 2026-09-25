using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Factories;
using Nop.Plugin.Misc.RFQ.Models.Admin;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class RfqAdminController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly AdminModelFactory _modelFactory;
    private readonly ICustomerModelFactory _customerModelFactory;
    private readonly ICustomerService _customerService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly ISettingService _settingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly RfqMessageService _rfqMessageService;
    private readonly RfqService _rfqService;
    private readonly RfqSettings _rfqSettings;

    #endregion

    #region Ctor

    public RfqAdminController(AdminModelFactory modelFactory,
        ICustomerModelFactory customerModelFactory,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        RfqMessageService rfqMessageService,
        RfqService rfqService,
        RfqSettings rfqSettings)
    {
		_logger.LogError(">>> Entering RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings})", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings);
        _modelFactory = modelFactory;
        _customerModelFactory = customerModelFactory;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _customerModelFactory → {_customerModelFactory}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _customerModelFactory);
        _customerService = customerService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _customerService → {_customerService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _localizationService → {_localizationService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _localizationService);
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _productAttributeParser → {_productAttributeParser}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _productAttributeParser);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _productAttributeService → {_productAttributeService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _productAttributeService);
        _productService = productService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _productService → {_productService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _productService);
        _notificationService = notificationService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _notificationService → {_notificationService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _notificationService);
        _settingService = settingService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _settingService → {_settingService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _settingService);
        _shoppingCartService = shoppingCartService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _shoppingCartService → {_shoppingCartService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _shoppingCartService);
        _rfqMessageService = rfqMessageService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqMessageService → {_rfqMessageService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _rfqMessageService);
        _rfqService = rfqService;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqService → {_rfqService}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _rfqService);
        _rfqSettings = rfqSettings;
		_logger.LogDebug("RfqAdminController(modelFactory={ModelFactory},customerModelFactory={CustomerModelFactory},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},productService={ProductService},settingService={SettingService},shoppingCartService=<redacted>,rfqMessageService={RfqMessageService},rfqService={RfqService},rfqSettings={RfqSettings}): _rfqSettings → {_rfqSettings}", modelFactory, customerModelFactory, customerService, localizationService, notificationService, productAttributeParser, productAttributeService, productService, settingService, rfqMessageService, rfqService, rfqSettings, _rfqSettings);
    }

    #endregion

    #region Methods

    #region Configure

    public async Task<IActionResult> Configure()
    {
		_logger.LogError(">>> Entering Configure()");
        var model = new ConfigurationModel
        {
            Enabled = _rfqSettings.Enabled
        };

		_logger.LogWarning("<<< Exiting Configure()");
		_logger.LogWarning("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/Misc.RFQ/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();
        
        _rfqSettings.Enabled = model.Enabled;

        await _settingService.SaveSettingAsync(_rfqSettings);
        
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    #endregion

    #region Add product to quote

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddNewProduct(ProductSearchModel searchModel)
    {
		_logger.LogError(">>> Entering AddNewProduct(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _modelFactory.PrepareProductListModelAsync(searchModel);
		_logger.LogError("AddNewProduct(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting AddNewProduct(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductDetails(int productId, int quoteId)
    {
		_logger.LogError(">>> Entering AddProductDetails(productId={ProductId},quoteId={QuoteId})", productId, quoteId);
        var quote = await _rfqService.GetQuoteByIdAsync(quoteId)
            ?? throw new ArgumentException("No quote found with the specified id");

        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //prepare model
        var model = await _modelFactory.PrepareAddProductModelAsync(new AddProductModel(), quote, product);
		_logger.LogError("AddProductDetails(productId={ProductId},quoteId={QuoteId}): model → {Model}", productId, quoteId, model);

		_logger.LogWarning("<<< Exiting AddProductDetails(productId={ProductId},quoteId={QuoteId})", productId, quoteId);
		_logger.LogWarning("<<< Exiting AddProductDetails(quoteId={QuoteId},productId={ProductId},form={Form})", quoteId, productId, form);
        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductDetails.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductDetails(int quoteId, int productId, IFormCollection form)
    {
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //basic properties
        _ = decimal.TryParse(form["UnitPriceInclTax"], NumberStyles.Any, CultureInfo.InvariantCulture, out var unitPriceInclTax);
        _ = int.TryParse(form["Quantity"], out var quantity);

        //warnings
        var warnings = new List<string>();

        //attributes
        var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, warnings);

        //rental product
        _productAttributeParser.ParseRentalDates(product, form, out _, out _);

        var quote = await _rfqService.GetQuoteByIdAsync(quoteId)
                ?? throw new ArgumentException("No quote found with the specified id");

        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId)
            ?? throw new ArgumentException("No customer found with the specified id");

        //warnings
        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));

        if (!warnings.Any())
        {
            var quoteItem = new QuoteItem
            {
                AttributesXml = attributesXml,
                ProductId = product.Id,
                OfferedQty = quantity,
                QuoteId = quoteId,
                OfferedUnitPrice = unitPriceInclTax
            };

            await _rfqService.InsertQuoteItemAsync(quoteItem);

            return RedirectToAction("AdminQuote", new { id = quoteId });
        }

        //prepare model
        var model = await _modelFactory.PrepareAddProductModelAsync(new AddProductModel(), quote, product);
        model.Warnings.AddRange(warnings);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductDetails.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> ProductDetailsAttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
    {
		_logger.LogError(">>> Entering ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form})", productId, validateAttributeConditions, form);
        var product = await _productService.GetProductByIdAsync(productId);
			_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): product → {Product}", productId, validateAttributeConditions, form, product);
        if (product == null) {
            return new NullJsonResult();
        }

        var errors = new List<string>();
		_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): errors → {Errors}", productId, validateAttributeConditions, form, errors);
        var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);
		_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): attributeXml → {AttributeXml}", productId, validateAttributeConditions, form, attributeXml);

        //conditional attributes
        var enabledAttributeMappingIds = new List<int>();
		_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): enabledAttributeMappingIds → {EnabledAttributeMappingIds}", productId, validateAttributeConditions, form, enabledAttributeMappingIds);
        var disabledAttributeMappingIds = new List<int>();
		_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): disabledAttributeMappingIds → {DisabledAttributeMappingIds}", productId, validateAttributeConditions, form, disabledAttributeMappingIds);

        if (validateAttributeConditions)
        {
            var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
			_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): attributes → {Attributes}", productId, validateAttributeConditions, form, attributes);

            foreach (var attribute in attributes)
            {
                var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
				_logger.LogError("ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form}): conditionMet → {ConditionMet}", productId, validateAttributeConditions, form, conditionMet);

                if (!conditionMet.HasValue)
                    continue;

                if (conditionMet.Value)
                    enabledAttributeMappingIds.Add(attribute.Id);
                else
                    disabledAttributeMappingIds.Add(attribute.Id);
            }
        }

		_logger.LogWarning("<<< Exiting ProductDetailsAttributeChange(productId={ProductId},validateAttributeConditions={ValidateAttributeConditions},form={Form})", productId, validateAttributeConditions, form);
        return Json(new
        {
            enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
            disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
            message = errors.Any() ? errors.ToArray() : null
        });
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public async Task<IActionResult> CustomerList(CustomerSearchModel searchModel)
    {
		_logger.LogError(">>> Entering CustomerList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerListModelAsync(searchModel);
		_logger.LogError("CustomerList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting CustomerList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Customers.CUSTOMERS_VIEW)]
    public async Task<IActionResult> SelectCustomerPopup()
    {
		_logger.LogError(">>> Entering SelectCustomerPopup()");
        //prepare model
        var model = await _customerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());
		_logger.LogError("SelectCustomerPopup(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting SelectCustomerPopup()");
        return View("~/Plugins/Misc.RFQ/Views/Admin/SelectCustomerPopup.cshtml", model);
    }

    #endregion

    #region Request a quote

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequests()
    {
		_logger.LogError(">>> Entering AdminRequests()");
        //prepare model
        var model = await _modelFactory.PrepareRequestQuoteSearchModelAsync();
		_logger.LogError("AdminRequests(): model → {Model}", model);

		_logger.LogWarning("<<< Exiting AdminRequests()");
        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequests.cshtml", model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequest(int id)
    {
		_logger.LogError(">>> Entering AdminRequest(id={Id})", id);
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);
		_logger.LogError("AdminRequest(id={Id}): requestQuote → {RequestQuote}", id, requestQuote);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        var model = await _modelFactory.PreparedRequestQuoteModelAsync(requestQuote);
		_logger.LogError("AdminRequest(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting AdminRequest(id={Id})", id);
		_logger.LogWarning("<<< Exiting AdminRequest(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequest.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminRequest(RequestQuoteModel model, bool continueEditing)
    {
        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        if (ModelState.IsValid)
        {
            if (requestQuote.AdminNotes != model.AdminNotes)
            {
                requestQuote.AdminNotes = model.AdminNotes;
                await _rfqService.UpdateRequestQuoteAsync(requestQuote);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

            return !continueEditing ? RedirectToAction("AdminRequests") : RedirectToAction("AdminRequest", new { id = requestQuote.Id });
        }

        //if we got this far, something failed, redisplay form
        model = await _modelFactory.PreparedRequestQuoteModelAsync(requestQuote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminRequest.cshtml", model);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSave")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> EditRequestItem(int id, IFormCollection form)
    {
		_logger.LogError(">>> Entering EditRequestItem(id={Id},form={Form})", id, form);
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);
		_logger.LogError("EditRequestItem(id={Id},form={Form}): requestQuote → {RequestQuote}", id, form, requestQuote);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        //get request a quote item identifier
        var requestQuoteItemId = 0;
		_logger.LogError("EditRequestItem(id={Id},form={Form}): requestQuoteItemId → {RequestQuoteItemId}", id, form, requestQuoteItemId);

        var saveButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnSave", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(saveButtonId))
            requestQuoteItemId = Convert.ToInt32(saveButtonId["btnSave".Length..]);

        if (requestQuoteItemId <= 0)
            return await AdminRequest(id);

        int.TryParse(form[$"quantity{requestQuoteItemId}"], out var requestedQty);
        decimal.TryParse(form[$"unitPrice{requestQuoteItemId}"], NumberStyles.Any, CultureInfo.InvariantCulture, out var requestedUnitPrice);

        await _rfqService.UpdateRequestQuoteItemAsync(requestQuoteItemId, requestedQty, requestedUnitPrice);

        if (requestQuote.AdminNotes != form["AdminNotes"])
        {
            requestQuote.AdminNotes = form["AdminNotes"];
			_logger.LogDebug("EditRequestItem(id={Id},form={Form}): requestQuote.AdminNotes → {Arg2}", id, form, requestQuote.AdminNotes);
            await _rfqService.UpdateRequestQuoteAsync(requestQuote);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

		_logger.LogWarning("<<< Exiting EditRequestItem(id={Id},form={Form})", id, form);
        return await AdminRequest(id);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDelete")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteRequestItem(int id, IFormCollection form)
    {
		_logger.LogError(">>> Entering DeleteRequestItem(id={Id},form={Form})", id, form);
        if (id <= 0)
            return RedirectToAction("AdminRequests");

        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(id);
		_logger.LogError("DeleteRequestItem(id={Id},form={Form}): requestQuote → {RequestQuote}", id, form, requestQuote);

        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        //get request a quote item identifier
        var requestQuoteItemId = 0;
		_logger.LogError("DeleteRequestItem(id={Id},form={Form}): requestQuoteItemId → {RequestQuoteItemId}", id, form, requestQuoteItemId);

        var deleteButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnDelete", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(deleteButtonId))
            requestQuoteItemId = Convert.ToInt32(deleteButtonId["btnDelete".Length..]);

        if (requestQuoteItemId <= 0)
            return await AdminRequest(id);

        await _rfqService.DeleteRequestQuoteItemAsync(requestQuoteItemId);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Updated"));

		_logger.LogWarning("<<< Exiting DeleteRequestItem(id={Id},form={Form})", id, form);
        return await AdminRequest(id);
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired("deleteRequest")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteRequest(RequestQuoteModel model)
    {
		_logger.LogError(">>> Entering DeleteRequest(model={Model})", model);
        await _rfqService.DeleteRequestQuoteAsync(model.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteRequest(model={Model})", model);
        return RedirectToAction("AdminRequests");
    }

    [HttpPost, ActionName("AdminRequest")]
    [FormValueRequired("cancelRequest")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> CancelRequest(RequestQuoteModel model)
    {
		_logger.LogError(">>> Entering CancelRequest(model={Model})", model);
        var requestQuote = await _rfqService.GetRequestQuoteByIdAsync(model.Id);

		_logger.LogError("CancelRequest(model={Model}): requestQuote → {RequestQuote}", model, requestQuote);
        if (requestQuote == null)
            return RedirectToAction("AdminRequests");

        await _rfqService.ChangeAndLogRequestQuoteStatusAsync(requestQuote, RequestQuoteStatus.Canceled);
        await _rfqService.UpdateRequestQuoteAsync(requestQuote);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Canceled"));

		_logger.LogWarning("<<< Exiting CancelRequest(model={Model})", model);
        return RedirectToAction("AdminRequest", new { id = requestQuote.Id });
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteSelectedRequests(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelectedRequests(selectedIds={SelectedIds})", selectedIds);
        await _rfqService.DeleteRequestsQuoteByIdsAsync(selectedIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminRequest.Selected.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteSelectedRequests(selectedIds={SelectedIds})", selectedIds);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> RequestList(RequestQuoteSearchModel searchModel)
    {
		_logger.LogError(">>> Entering RequestList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _modelFactory.PrepareRequestQuoteListModelAsync(searchModel);
		_logger.LogError("RequestList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting RequestList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Quote

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> CreateQuote(RequestQuoteModel model)
    {
		_logger.LogError(">>> Entering CreateQuote(model={Model})", model);
        var quoteId = await _rfqService.CreateQuoteByRequestAsync(model.Id);

		_logger.LogError("CreateQuote(model={Model}): quoteId → {QuoteId}", model, quoteId);
		_logger.LogWarning("<<< Exiting CreateQuote(model={Model})", model);
        return RedirectToAction("AdminQuote", new { id = quoteId });
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuote(int id)
    {
		_logger.LogError(">>> Entering AdminQuote(id={Id})", id);
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);
		_logger.LogError("AdminQuote(id={Id}): quote → {Quote}", id, quote);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        var model = await _modelFactory.PreparedQuoteModelAsync(quote);
		_logger.LogError("AdminQuote(id={Id}): model → {Model}", id, model);

		_logger.LogWarning("<<< Exiting AdminQuote(id={Id})", id);
		_logger.LogWarning("<<< Exiting AdminQuote(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuote.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("btnCreate")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuotes(QuoteSearchModel model)
    {
		_logger.LogError(">>> Entering AdminQuotes(model={Model})", model);
        var requestQuote = await _rfqService.CreateQuoteAsync(model.CustomerId);

		_logger.LogError("AdminQuotes(model={Model}): requestQuote → {RequestQuote}", model, requestQuote);
		_logger.LogWarning("<<< Exiting AdminQuotes(model={Model})", model);
		_logger.LogWarning("<<< Exiting AdminQuotes()");
        return RedirectToAction("AdminQuote", new { id = requestQuote.Id });
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuotes()
    {
        //prepare model
        var model = await _modelFactory.PrepareQuoteSearchModelAsync();

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuotes.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteSelectedQuotes(ICollection<int> selectedIds)
    {
		_logger.LogError(">>> Entering DeleteSelectedQuotes(selectedIds={SelectedIds})", selectedIds);
        await _rfqService.DeleteQuotesByIdsAsync(selectedIds);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Selected.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteSelectedQuotes(selectedIds={SelectedIds})", selectedIds);
        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> QuoteList(QuoteSearchModel searchModel)
    {
		_logger.LogError(">>> Entering QuoteList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _modelFactory.PrepareQuoteListModelAsync(searchModel);
		_logger.LogError("QuoteList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogWarning("<<< Exiting QuoteList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public async Task<IActionResult> AddProductToQuote(int quoteId)
    {
		_logger.LogError(">>> Entering AddProductToQuote(quoteId={QuoteId})", quoteId);
        var quote = await _rfqService.GetQuoteByIdAsync(quoteId);

		_logger.LogError("AddProductToQuote(quoteId={QuoteId}): quote → {Quote}", quoteId, quote);
        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //prepare model
        var model = await _modelFactory.PrepareAddProductSearchModelAsync(new ProductSearchModel(), quote.Id);
		_logger.LogError("AddProductToQuote(quoteId={QuoteId}): model → {Model}", quoteId, model);

		_logger.LogWarning("<<< Exiting AddProductToQuote(quoteId={QuoteId})", quoteId);
        return View("~/Plugins/Misc.RFQ/Views/Admin/AddProductToQuote.cshtml", model);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired("sendQuote")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> SendQuote(QuoteModel model)
    {
		_logger.LogError(">>> Entering SendQuote(model={Model})", model);
        var quote = await _rfqService.GetQuoteByIdAsync(model.Id);

		_logger.LogError("SendQuote(model={Model}): quote → {Quote}", model, quote);
        if (quote == null)
            return RedirectToAction("AdminQuotes");

        await _rfqService.ChangeAndLogQuoteStatusAsync(quote, QuoteStatus.Submitted);
        await _rfqService.UpdateQuoteAsync(quote);

        await _rfqMessageService.AdminSentNewQuoteAsync(quote);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.QuoteSubmitted"));

		_logger.LogWarning("<<< Exiting SendQuote(model={Model})", model);
        return RedirectToAction("AdminQuote", new { id = quote.Id });
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired("deleteQuote")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteQuote(QuoteModel model)
    {
		_logger.LogError(">>> Entering DeleteQuote(model={Model})", model);
        await _rfqService.DeleteQuoteAsync(model.Id);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Deleted"));

		_logger.LogWarning("<<< Exiting DeleteQuote(model={Model})", model);
        return RedirectToAction("AdminQuotes");
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> AdminQuote(QuoteModel model, bool continueEditing)
    {
        var quote = await _rfqService.GetQuoteByIdAsync(model.Id);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        if (ModelState.IsValid)
        {
            var needUpdate = false;

            if (quote.AdminNotes != model.AdminNotes)
            {
                needUpdate = true;
                quote.AdminNotes = model.AdminNotes;
            }

            if (quote.ExpirationDateUtc != model.ExpirationDateUtc)
            {
                needUpdate = true;
                quote.ExpirationDateUtc = model.ExpirationDateUtc;
            }

            if (needUpdate)
                await _rfqService.UpdateQuoteAsync(quote);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

            return !continueEditing ? RedirectToAction("AdminQuotes") : RedirectToAction("AdminQuote", new { id = quote.Id });
        }

        //if we got this far, something failed, redisplay form
        model = await _modelFactory.PreparedQuoteModelAsync(quote);

        return View("~/Plugins/Misc.RFQ/Views/Admin/AdminQuote.cshtml", model);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnSave")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> EditQuoteItem(int id, IFormCollection form)
    {
		_logger.LogError(">>> Entering EditQuoteItem(id={Id},form={Form})", id, form);
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);
		_logger.LogError("EditQuoteItem(id={Id},form={Form}): quote → {Quote}", id, form, quote);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //get the quote item identifier
        var quoteItemId = 0;
		_logger.LogError("EditQuoteItem(id={Id},form={Form}): quoteItemId → {QuoteItemId}", id, form, quoteItemId);

        var saveButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnSave", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(saveButtonId))
            quoteItemId = Convert.ToInt32(saveButtonId["btnSave".Length..]);

        if (quoteItemId <= 0)
            return await AdminQuote(id);

        int.TryParse(form[$"quantity{quoteItemId}"], out var offeredQty);
        decimal.TryParse(form[$"unitPrice{quoteItemId}"], NumberStyles.Any, CultureInfo.InvariantCulture, out var offeredUnitPrice);

        await _rfqService.UpdateQuoteItemAsync(quoteItemId, offeredQty, offeredUnitPrice);

        if (quote.AdminNotes != form["AdminNotes"])
        {
            quote.AdminNotes = form["AdminNotes"];
			_logger.LogDebug("EditQuoteItem(id={Id},form={Form}): quote.AdminNotes → {Arg2}", id, form, quote.AdminNotes);
            await _rfqService.UpdateQuoteAsync(quote);
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

		_logger.LogWarning("<<< Exiting EditQuoteItem(id={Id},form={Form})", id, form);
        return await AdminQuote(id);
    }

    [HttpPost, ActionName("AdminQuote")]
    [FormValueRequired(FormValueRequirement.StartsWith, "btnDelete")]
    [CheckPermission(RfqPermissionConfigManager.ADMIN_ACCESS_RFQ)]
    public async Task<IActionResult> DeleteQuoteItem(int id, IFormCollection form)
    {
		_logger.LogError(">>> Entering DeleteQuoteItem(id={Id},form={Form})", id, form);
        if (id <= 0)
            return RedirectToAction("AdminQuotes");

        var quote = await _rfqService.GetQuoteByIdAsync(id);
		_logger.LogError("DeleteQuoteItem(id={Id},form={Form}): quote → {Quote}", id, form, quote);

        if (quote == null)
            return RedirectToAction("AdminQuotes");

        //get the quote item identifier
        var quoteItemId = 0;
		_logger.LogError("DeleteQuoteItem(id={Id},form={Form}): quoteItemId → {QuoteItemId}", id, form, quoteItemId);

        var deleteButtonId = form
            .FirstOrDefault(p => p.Key.StartsWith("btnDelete", StringComparison.InvariantCultureIgnoreCase)).Key;

        if (!string.IsNullOrEmpty(deleteButtonId))
            quoteItemId = Convert.ToInt32(deleteButtonId["btnDelete".Length..]);

        if (quoteItemId <= 0)
            return await AdminQuote(id);

        await _rfqService.DeleteQuoteItemAsync(quoteItemId);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.AdminQuote.Updated"));

		_logger.LogWarning("<<< Exiting DeleteQuoteItem(id={Id},form={Form})", id, form);
        return await AdminQuote(id);
    }

    #endregion

    #endregion
}