using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Plugin.Misc.RFQ.Domains;
using Nop.Plugin.Misc.RFQ.Models.Admin;
using Nop.Plugin.Misc.RFQ.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;

using ProductSearchModel = Nop.Plugin.Misc.RFQ.Models.Admin.ProductSearchModel;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.RFQ.Factories;

public class AdminModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private readonly CurrencySettings _currencySettings;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly ICurrencyService _currencyService;
    private readonly ICustomerService _customerService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILocalizationService _localizationService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IProductAttributeFormatter _productAttributeFormatter;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductService _productService;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IStoreContext _storeContext;
    private readonly ITaxService _taxService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly RfqService _rfqService;

    #endregion

    #region Ctor

    public AdminModelFactory(CurrencySettings currencySettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IRepository<Customer> customerRepository,
        IStoreContext storeContext,
        ITaxService taxService,
        IUrlRecordService urlRecordService,
        RfqService rfqService)
    {
		_logger.LogDebug(">>> Entering AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService})", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService);
        _currencySettings = currencySettings;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _currencySettings → {_currencySettings}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _currencySettings);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _baseAdminModelFactory → {_baseAdminModelFactory}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _baseAdminModelFactory);
        _currencyService = currencyService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _currencyService → {_currencyService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _currencyService);
        _customerService = customerService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _customerService → {_customerService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _dateTimeHelper → {_dateTimeHelper}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _dateTimeHelper);
        _localizationService = localizationService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _localizationService → {_localizationService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _localizationService);
        _priceCalculationService = priceCalculationService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _priceCalculationService → {_priceCalculationService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _priceCalculationService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _priceFormatter → {_priceFormatter}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _priceFormatter);
        _productAttributeFormatter = productAttributeFormatter;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _productAttributeFormatter → {_productAttributeFormatter}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _productAttributeFormatter);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _productAttributeService → {_productAttributeService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _productAttributeService);
        _productService = productService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _productService → {_productService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _productService);
        _customerRepository = customerRepository;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _customerRepository → {_customerRepository}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _customerRepository);
        _storeContext = storeContext;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _storeContext → {_storeContext}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _storeContext);
        _taxService = taxService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _taxService → {_taxService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _taxService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _urlRecordService → {_urlRecordService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _urlRecordService);
        _rfqService = rfqService;
		_logger.LogDebug("<<< Exiting AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService})", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService);
		_logger.LogDebug("AdminModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},currencyService={CurrencyService},customerService={CustomerService},dateTimeHelper={DateTimeHelper},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeFormatter={ProductAttributeFormatter},productAttributeService={ProductAttributeService},productService={ProductService},customerRepository={CustomerRepository},storeContext={StoreContext},taxService={TaxService},urlRecordService={UrlRecordService},rfqService={RfqService}): _rfqService → {_rfqService}", currencySettings, baseAdminModelFactory, currencyService, customerService, dateTimeHelper, localizationService, priceCalculationService, priceFormatter, productAttributeFormatter, productAttributeService, productService, customerRepository, storeContext, taxService, urlRecordService, rfqService, _rfqService);
    }

    #endregion

    #region Utilities

    private async Task PrepareProductAttributeModelsAsync(IList<AddProductModel.ProductAttributeModel> models, Customer customer, Product product)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product})", models, customer, product);
        ArgumentNullException.ThrowIfNull(models);
        ArgumentNullException.ThrowIfNull(product);

        var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product}): attributes → {Attributes}", models, customer, product, attributes);
        foreach (var attribute in attributes)
        {
            var attributeModel = new AddProductModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
                TextPrompt = attribute.TextPrompt,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
					_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product}): attributeModel.AllowedFileExtensions → {Arg3}", models, customer, product, attributeModel.AllowedFileExtensions);
            }

            if (attribute.ShouldHaveValues())
            {
                var store = await _storeContext.GetCurrentStoreAsync();
				_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product}): store → {Store}", models, customer, product, store);

                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
				_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product}): attributeValues → {AttributeValues}", models, customer, product, attributeValues);
                foreach (var attributeValue in attributeValues)
                {
                    //price adjustment
                    var (priceAdjustment, _) = await _taxService.GetProductPriceAsync(product,
                        await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store));

                    var priceAdjustmentStr = string.Empty;
					_logger.LogDebug("PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product}): priceAdjustmentStr → {PriceAdjustmentStr}", models, customer, product, priceAdjustmentStr);
                    if (priceAdjustment != 0)
                    {
                        if (attributeValue.PriceAdjustmentUsePercentage)
                        {
                            priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                            priceAdjustmentStr = priceAdjustment > 0 ? $"+{priceAdjustmentStr}%" : $"{priceAdjustmentStr}%";
                        }
                        else
                        {
                            priceAdjustmentStr = priceAdjustment > 0
                                ? $"+{await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false)}"
                                : $"-{await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false)}";
                        }
                    }

                    attributeModel.Values.Add(new AddProductModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected,
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        Quantity = attributeValue.Quantity,
                        PriceAdjustment = priceAdjustmentStr
                    });
                }
            }

            models.Add(attributeModel);
        }
		_logger.LogDebug("<<< Exiting PrepareProductAttributeModelsAsync(models={Models},customer={Customer},product={Product})", models, customer, product);
    }

    private async Task<RequestQuoteItemModel> PreparedRequestQuoteItemModelAsync(RequestQuoteItem item, Currency primaryStoreCurrency)
    {
		_logger.LogDebug(">>> Entering PreparedRequestQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency})", item, primaryStoreCurrency);
        var product = await _productService.GetProductByIdAsync(item.ProductId);

		_logger.LogDebug("PreparedRequestQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency}): product → {Product}", item, primaryStoreCurrency, product);
		_logger.LogDebug("<<< Exiting PreparedRequestQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency})", item, primaryStoreCurrency);
        return new RequestQuoteItemModel
        {
            Id = item.Id,
            RequestedQty = item.RequestedQty,
            AdminNotes = item.AdminNotes.Replace("\r\n", "<br />"),
            OriginalProductPrice = await _priceFormatter.FormatPriceAsync(item.OriginalProductPrice, true, primaryStoreCurrency),
            ProductName = product != null ? product.Name : await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.ProductDeleted"),
            ProductId = item.ProductId,
            RequestedUnitPrice = await _priceFormatter.FormatPriceAsync(item.RequestedUnitPrice, true, primaryStoreCurrency),
            RequestedUnitPriceValue = item.RequestedUnitPrice,
            ProductAttributeInfo = product != null ? await _productAttributeFormatter.FormatAttributesAsync(product, item.ProductAttributesXml) : string.Empty
        };
    }

    private async Task<QuoteItemModel> PreparedQuoteItemModelAsync(QuoteItem item, Currency primaryStoreCurrency)
    {
		_logger.LogDebug(">>> Entering PreparedQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency})", item, primaryStoreCurrency);
        var product = await _productService.GetProductByIdAsync(item.ProductId);

		_logger.LogDebug("PreparedQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency}): product → {Product}", item, primaryStoreCurrency, product);
		_logger.LogDebug("<<< Exiting PreparedQuoteItemModelAsync(item={Item},primaryStoreCurrency={PrimaryStoreCurrency})", item, primaryStoreCurrency);
        return new QuoteItemModel
        {
            Id = item.Id,
            OfferedQty = item.OfferedQty,
            ProductName = product != null ? product.Name : await _localizationService.GetResourceAsync("Plugins.Misc.RFQ.ProductDeleted"),
            ProductId = item.ProductId,
            OfferedUnitPrice = await _priceFormatter.FormatPriceAsync(item.OfferedUnitPrice, true, primaryStoreCurrency),
            OfferedUnitPriceValue = item.OfferedUnitPrice,
            ProductAttributeInfo = product != null ? await _productAttributeFormatter.FormatAttributesAsync(product, item.AttributesXml) : string.Empty,
            RequestedQty = item.RequestedQty,
            RequestedUnitPrice = item.RequestedUnitPrice.HasValue ? await _priceFormatter.FormatPriceAsync(item.RequestedUnitPrice.Value, true, primaryStoreCurrency) : string.Empty,
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare request a quote search model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote search model
    /// </returns>
    public async Task<RequestQuoteSearchModel> PrepareRequestQuoteSearchModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareRequestQuoteSearchModelAsync()");
        var searchModel = new RequestQuoteSearchModel();

		_logger.LogDebug("PrepareRequestQuoteSearchModelAsync(): searchModel → {SearchModel}", searchModel);
        //prepare available statuses
        searchModel.AvailableRequestQuoteStatuses.AddRange(await RequestQuoteStatus.Submitted.ToSelectListAsync(false));

        //prepare default item text
        var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.All");
		_logger.LogDebug("PrepareRequestQuoteSearchModelAsync(): defaultItemText → {DefaultItemText}", defaultItemText);
        //insert this default item at first
        searchModel.AvailableRequestQuoteStatuses.Insert(0, new SelectListItem { Text = defaultItemText, Value = "-1" });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareRequestQuoteSearchModelAsync()");
        return searchModel;
    }

    /// <summary>
    /// Prepare a quote search model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote search model
    /// </returns>
    public async Task<QuoteSearchModel> PrepareQuoteSearchModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareQuoteSearchModelAsync()");
        var searchModel = new QuoteSearchModel();

		_logger.LogDebug("PrepareQuoteSearchModelAsync(): searchModel → {SearchModel}", searchModel);
        //prepare available statuses
        searchModel.AvailableQuoteStatuses.AddRange(await QuoteStatus.CreatedFromRequestQuote.ToSelectListAsync(false));

        //prepare default item text
        var defaultItemText = await _localizationService.GetResourceAsync("Admin.Common.All");
		_logger.LogDebug("PrepareQuoteSearchModelAsync(): defaultItemText → {DefaultItemText}", defaultItemText);
        //insert this default item at first
        searchModel.AvailableQuoteStatuses.Insert(0, new SelectListItem { Text = defaultItemText, Value = "-1" });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareQuoteSearchModelAsync()");
        return searchModel;
    }

    /// <summary>
    /// Prepare a request a quote list model
    /// </summary>
    /// <param name="searchModel">Request a quote search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote list model
    /// </returns>
    public async Task<RequestListModel> PrepareRequestQuoteListModelAsync(RequestQuoteSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareRequestQuoteListModelAsync(searchModel={SearchModel})", searchModel);
        var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdOnToValue = !searchModel.CreatedOnTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);


        var items = await _rfqService.SearchRequestsQuoteAsync(searchModel.RequestQuoteStatus, createdOnFromValue,
            createdOnToValue, searchModel.CustomerEmail, searchModel.Page - 1, searchModel.PageSize);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customerEmails = (await _customerRepository.GetByIdsAsync(items.Select(p => p.CustomerId).Distinct().ToArray())).ToDictionary(p => p.Id, p => p.Email);

		_logger.LogDebug("PrepareRequestQuoteListModelAsync(searchModel={SearchModel}): customerEmails → {CustomerEmails}", searchModel, customerEmails);
		_logger.LogInformation("PrepareRequestQuoteListModelAsync(searchModel)={SearchModel}: find query executed in {Arg1} ms", searchModel, __logStart.ElapsedMilliseconds);
        //prepare list model
        var model = await new RequestListModel().PrepareToGridAsync(searchModel, items, () =>
        {
            //fill in model values from the entity
			_logger.LogDebug("PrepareRequestQuoteListModelAsync(searchModel={SearchModel}): item → {Item}", searchModel, item);
		_logger.LogDebug("<<< Exiting PrepareRequestQuoteListModelAsync(searchModel={SearchModel})", searchModel);
            return items.SelectAwait(async item => await PreparedRequestQuoteModelAsync(item, customerEmails));
        });

        return model;
    }

    /// <summary>
    /// Prepare a request a quote model
    /// </summary>
    /// <param name="requestQuote">The request a quote</param>
    /// <param name="customerEmails">Preloaded customers emails</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote model
    /// </returns>
    public async Task<RequestQuoteModel> PreparedRequestQuoteModelAsync(RequestQuote requestQuote, IDictionary<int, string> customerEmails = null)
    {
		_logger.LogDebug(">>> Entering PreparedRequestQuoteModelAsync(requestQuote={RequestQuote},customerEmails={CustomerEmails})", requestQuote, customerEmails);
        string email;

        if (customerEmails != null)
            customerEmails.TryGetValue(requestQuote.CustomerId, out email);
        else
            email = (await _customerService.GetCustomerByIdAsync(requestQuote.CustomerId))?.Email;

		_logger.LogDebug("PreparedRequestQuoteModelAsync(requestQuote={RequestQuote},customerEmails={CustomerEmails}): email → {Email}", requestQuote, customerEmails, email);
		_logger.LogDebug("<<< Exiting PreparedRequestQuoteModelAsync(requestQuote={RequestQuote},customerEmails={CustomerEmails})", requestQuote, customerEmails);
        return new RequestQuoteModel
        {
            Id = requestQuote.Id,
            CustomerId = requestQuote.CustomerId,
            CreatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(requestQuote.CreatedOnUtc, DateTimeKind.Utc),
            //fill in additional values (not existing in the entity)
            Status = await _localizationService.GetLocalizedEnumAsync(requestQuote.Status),
            StatusType = requestQuote.Status,
            CustomerNotes = requestQuote.CustomerNotes,
            CustomerEmail = email ?? string.Empty,
            AdminNotes = requestQuote.AdminNotes,
            QuoteId = requestQuote.QuoteId,
            Items = await PrepareRequestItemListModelAsync(requestQuote.Id)
        };
    }

    /// <summary>
    /// Prepare a request a quote item list model
    /// </summary>
    /// <param name="requestQuoteId">Request a quote identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the request a quote item list model
    /// </returns>
    public async Task<List<RequestQuoteItemModel>> PrepareRequestItemListModelAsync(int requestQuoteId)
    {
		_logger.LogDebug(">>> Entering PrepareRequestItemListModelAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
        var items = await _rfqService.GetRequestQuoteItemsAsync(requestQuoteId);
        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("PrepareRequestItemListModelAsync(requestQuoteId={RequestQuoteId}): primaryStoreCurrency → {PrimaryStoreCurrency}", requestQuoteId, primaryStoreCurrency);
		_logger.LogDebug("PrepareRequestItemListModelAsync(requestQuoteId={RequestQuoteId}): items → {Items}", requestQuoteId, items);

		_logger.LogDebug("<<< Exiting PrepareRequestItemListModelAsync(requestQuoteId={RequestQuoteId})", requestQuoteId);
        return await items.SelectAwait(async p => await PreparedRequestQuoteItemModelAsync(p, primaryStoreCurrency)).ToListAsync();
    }

    /// <summary>
    /// Prepare a quote item list model
    /// </summary>
    /// <param name="quoteId">Quote item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote item list model
    /// </returns>
    public async Task<List<QuoteItemModel>> PrepareQuoteItemListModelAsync(int quoteId)
    {
		_logger.LogDebug(">>> Entering PrepareQuoteItemListModelAsync(quoteId={QuoteId})", quoteId);
        var items = await _rfqService.GetQuoteItemsAsync(quoteId);
        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogDebug("PrepareQuoteItemListModelAsync(quoteId={QuoteId}): primaryStoreCurrency → {PrimaryStoreCurrency}", quoteId, primaryStoreCurrency);
		_logger.LogDebug("PrepareQuoteItemListModelAsync(quoteId={QuoteId}): items → {Items}", quoteId, items);

		_logger.LogDebug("<<< Exiting PrepareQuoteItemListModelAsync(quoteId={QuoteId})", quoteId);
        return await items.SelectAwait(async p => await PreparedQuoteItemModelAsync(p, primaryStoreCurrency)).ToListAsync();
    }

    /// <summary>
    /// Prepare product model to add to the quote
    /// </summary>
    /// <param name="model">Product model to add to the quote</param>
    /// <param name="quote">Quote</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model to add to the quote
    /// </returns>
    public async Task<AddProductModel> PrepareAddProductModelAsync(AddProductModel model, Quote quote, Product product)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product})", model, quote, product);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(quote);

        var customerId = quote.CustomerId;
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): customerId → {CustomerId}", model, quote, product, customerId);

        var customer = await _customerService.GetCustomerByIdAsync(customerId);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): customer → {Customer}", model, quote, product, customer);
        var taxDisplayType = await _customerService.GetCustomerTaxDisplayTypeAsync(customer);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): taxDisplayType → {TaxDisplayType}", model, quote, product, taxDisplayType);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): store → {Store}", model, quote, product, store);

        model.ProductId = product.Id;
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.ProductId → {Arg3}", model, quote, product, model.ProductId);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.QuoteId → {Arg3}", model, quote, product, model.QuoteId);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.Name → {Arg3}", model, quote, product, model.Name);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.IsRental → {Arg3}", model, quote, product, model.IsRental);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.ProductType → {Arg3}", model, quote, product, model.ProductType);
        model.QuoteId = quote.Id;
        model.Name = product.Name;
        model.IsRental = product.IsRental;
        model.ProductType = product.ProductType;

        var presetQty = 1;
        var (_, presetPrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, decimal.Zero, true, presetQty);

        var (presetPriceInclTax, _) = await _taxService.GetProductPriceAsync(product, presetPrice, taxDisplayType == TaxDisplayType.IncludingTax, customer);

        model.UnitPriceInclTax = presetPriceInclTax;
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.UnitPriceInclTax → {Arg3}", model, quote, product, model.UnitPriceInclTax);
		_logger.LogDebug("PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product}): model.Quantity → {Arg3}", model, quote, product, model.Quantity);
        model.Quantity = presetQty;

        //attributes
        await PrepareProductAttributeModelsAsync(model.ProductAttributes, customer, product);
        model.HasCondition = model.ProductAttributes.Any(attribute => attribute.HasCondition);

        //gift card
        model.GiftCard.IsGiftCard = product.IsGiftCard;
        if (model.GiftCard.IsGiftCard)
            model.GiftCard.GiftCardType = product.GiftCardType;

		_logger.LogDebug("<<< Exiting PrepareAddProductModelAsync(model={Model},quote={Quote},product={Product})", model, quote, product);
        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the quote
    /// </summary>
    /// <param name="searchModel">Product search model to add to the order</param>
    /// <param name="entityId">The quote identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the quote
    /// </returns>
    public async Task<ProductSearchModel> PrepareAddProductSearchModelAsync(ProductSearchModel searchModel, int entityId)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductSearchModelAsync(searchModel={SearchModel},entityId={EntityId})", searchModel, entityId);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.EntityId = entityId;
		_logger.LogDebug("PrepareAddProductSearchModelAsync(searchModel={SearchModel},entityId={EntityId}): searchModel.EntityId → {Arg2}", searchModel, entityId, searchModel.EntityId);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddProductSearchModelAsync(searchModel={SearchModel},entityId={EntityId})", searchModel, entityId);
        return searchModel;
    }

    /// <summary>
    /// Prepare a product to a quote list model
    /// </summary>
    /// <param name="searchModel">Product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product to a quote list model
    /// </returns>
    public async Task<ProductToRequestListModel> PrepareProductListModelAsync(ProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareProductListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new ProductToRequestListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();
				_logger.LogDebug("PrepareProductListModelAsync(searchModel={SearchModel}): productModel → {ProductModel}", searchModel, productModel);

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareProductListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare a quote model
    /// </summary>
    /// <param name="quote">The quote</param>
    /// <param name="customerEmails">Preloaded customers emails</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote model
    /// </returns>
    public async Task<QuoteModel> PreparedQuoteModelAsync(Quote quote, IDictionary<int, string> customerEmails = null)
    {
		_logger.LogDebug(">>> Entering PreparedQuoteModelAsync(quote={Quote},customerEmails={CustomerEmails})", quote, customerEmails);
        string email;

        if (customerEmails != null)
            customerEmails.TryGetValue(quote.CustomerId, out email);
        else
            email = (await _customerService.GetCustomerByIdAsync(quote.CustomerId))?.Email;

		_logger.LogDebug("PreparedQuoteModelAsync(quote={Quote},customerEmails={CustomerEmails}): email → {Email}", quote, customerEmails, email);
        var model = new QuoteModel
        {
            Id = quote.Id,
            CustomerId = quote.CustomerId,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOnUtc, DateTimeKind.Utc),
            //fill in additional values (not existing in the entity)
            Status = await _localizationService.GetLocalizedEnumAsync(quote.Status),
            StatusType = quote.Status,
            CustomerEmail = email ?? string.Empty,
            AdminNotes = quote.AdminNotes,
            RequestQuoteId = quote.RequestQuoteId,
            OrderId = quote.OrderId,
            Items = await PrepareQuoteItemListModelAsync(quote.Id)
        };

        if (model.Editable)
            model.ExpirationDateUtc = quote.ExpirationDateUtc;
        else if (quote.ExpirationDateUtc.HasValue)
            model.ExpirationDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(quote.ExpirationDateUtc.Value, DateTimeKind.Utc);

		_logger.LogDebug("<<< Exiting PreparedQuoteModelAsync(quote={Quote},customerEmails={CustomerEmails})", quote, customerEmails);
        return model;
    }

    /// <summary>
    /// Prepare a quote list model
    /// </summary>
    /// <param name="searchModel">Quote search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the quote list model
    /// </returns>
    public async Task<QuoteListModel> PrepareQuoteListModelAsync(QuoteSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareQuoteListModelAsync(searchModel={SearchModel})", searchModel);
        var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdOnToValue = !searchModel.CreatedOnTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        var items = await _rfqService.SearchQuotesAsync(searchModel.QuoteStatus, createdOnFromValue, createdOnToValue, searchModel.CustomerEmail, searchModel.Page - 1, searchModel.PageSize);
		_logger.LogDebug("PrepareQuoteListModelAsync(searchModel={SearchModel}): items → {Items}", searchModel, items);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var customerEmails = (await _customerRepository.GetByIdsAsync(items.Select(p => p.CustomerId).Distinct().ToArray())).ToDictionary(p => p.Id, p => p.Email);

		_logger.LogDebug("PrepareQuoteListModelAsync(searchModel={SearchModel}): customerEmails → {CustomerEmails}", searchModel, customerEmails);
		_logger.LogInformation("PrepareQuoteListModelAsync(searchModel)={SearchModel}: find query executed in {Arg1} ms", searchModel, __logStart.ElapsedMilliseconds);
        //prepare list model
        var model = await new QuoteListModel().PrepareToGridAsync(searchModel, items, () =>
        {
			_logger.LogDebug("PrepareQuoteListModelAsync(searchModel={SearchModel}): model → {Model}", searchModel, model);
            //fill in model values from the entity
            return items.SelectAwait(async item =>
            {
                var model = await PreparedQuoteModelAsync(item, customerEmails);

                if (model.Editable && model.ExpirationDateUtc.HasValue)
                    model.ExpirationDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(model.ExpirationDateUtc.Value, DateTimeKind.Utc);

                return model;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareQuoteListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}