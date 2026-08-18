using ClosedXML.Excel;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.ExportImport;

[TestFixture]
public class ExportManagerTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private CustomerSettings _customerSettings;
    private CatalogSettings _catalogSettings;
    private IAddressService _addressService;
    private ICategoryService _categoryService;
    private ICountryService _countryService;
    private ICustomerService _customerService;
    private IDateRangeService _dateRangeService;
    private IExportManager _exportManager;
    private ILanguageService _languageService;
    private IManufacturerService _manufacturerService;
    private IMeasureService _measureService;
    private IOrderService _orderService;
    private IProductTemplateService _productTemplateService;
    private IRepository<Product> _productRepository;
    private ITaxCategoryService _taxCategoryService;
    private IVendorService _vendorService;
    private ProductEditorSettings _productEditorSettings;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _customerSettings = GetService<CustomerSettings>();
        _catalogSettings = GetService<CatalogSettings>();
        _addressService = GetService<IAddressService>();
        _categoryService = GetService<ICategoryService>();
        _countryService = GetService<ICountryService>();
        _customerService = GetService<ICustomerService>();
        _dateRangeService = GetService<IDateRangeService>();
        _exportManager = GetService<IExportManager>();
        _languageService = GetService<ILanguageService>();
        _manufacturerService = GetService<IManufacturerService>();
        _measureService = GetService<IMeasureService>();
        _orderService = GetService<IOrderService>();
        _productTemplateService = GetService<IProductTemplateService>();
		_logger.LogDebug("SetUp(): _productTemplateService → {_productTemplateService}", _productTemplateService);
		_logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
        _productRepository = GetService<IRepository<Product>>();
		_logger.LogDebug("SetUp(): _productRepository → {_productRepository}", _productRepository);
        _taxCategoryService = GetService<ITaxCategoryService>();
		_logger.LogDebug("SetUp(): _taxCategoryService → {_taxCategoryService}", _taxCategoryService);
        _vendorService = GetService<IVendorService>();
		_logger.LogDebug("SetUp(): _vendorService → {_vendorService}", _vendorService);

        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                true);
				_logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
				_logger.LogDebug("SetUp(): _categoryService → {_categoryService}", _categoryService);
				_logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
				_logger.LogDebug("SetUp(): _countryService → {_countryService}", _countryService);
				_logger.LogDebug("SetUp(): _dateRangeService → {_dateRangeService}", _dateRangeService);
				_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
				_logger.LogDebug("SetUp(): _manufacturerService → {_manufacturerService}", _manufacturerService);
				_logger.LogDebug("SetUp(): _exportManager → {_exportManager}", _exportManager);
				_logger.LogDebug("SetUp(): _languageService → {_languageService}", _languageService);
				_logger.LogDebug("SetUp(): _measureService → {_measureService}", _measureService);
				_logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                true);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                true);

        _productEditorSettings = GetService<ProductEditorSettings>();
		_logger.LogDebug("SetUp(): _productEditorSettings → {_productEditorSettings}", _productEditorSettings);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                false);
				_logger.LogDebug("<<< Exiting TearDown()");
    }

    #endregion

    #region Utilities

    protected static T PropertiesShouldEqual<T, Tp>(T actual, PropertyManager<Tp> manager, IDictionary<string, string> replacePairs, params string[] filter)
    {
		_logger.LogDebug(">>> Entering PropertiesShouldEqual(actual={Actual},manager={Manager},replacePairs={ReplacePairs},filter={Filter})", actual, manager, replacePairs, filter);
        var objectProperties = typeof(T).GetProperties();
        foreach (var property in manager.GetDefaultProperties)
        {
		_logger.LogDebug("PropertiesShouldEqual(actual={Actual},manager={Manager},replacePairs={ReplacePairs},filter={Filter}): objectProperties → {ObjectProperties}", actual, manager, replacePairs, filter, objectProperties);
            if (filter.Contains(property.PropertyName))
                continue;

            var objectProperty = replacePairs.TryGetValue(property.PropertyName, out var value) ? objectProperties.FirstOrDefault(p => p.Name == value)
                : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

            if (objectProperty == null)
                continue;

            var objectPropertyValue = objectProperty.GetValue(actual);
			_logger.LogDebug("PropertiesShouldEqual(actual={Actual},manager={Manager},replacePairs={ReplacePairs},filter={Filter}): objectPropertyValue → {ObjectPropertyValue}", actual, manager, replacePairs, filter, objectPropertyValue);
            var propertyValue = property.PropertyValue;
			_logger.LogDebug("PropertiesShouldEqual(actual={Actual},manager={Manager},replacePairs={ReplacePairs},filter={Filter}): propertyValue → {PropertyValue}", actual, manager, replacePairs, filter, propertyValue);

            if (propertyValue is XLCellValue { IsBlank: true })
                propertyValue = null;

            if (string.IsNullOrEmpty(propertyValue?.ToString() ?? string.Empty) && objectPropertyValue is null)
                continue;

            switch (objectPropertyValue)
            {
                case int:
                    propertyValue = property.IntValue;
                    break;
                case Guid:
                    propertyValue = property.GuidValue;
                    break;
                case string:
                    propertyValue = property.StringValue;
                    break;
                case DateTime time:
                    ;
                    objectPropertyValue = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                    if (DateTime.TryParse(property.StringValue, out var date))
                        propertyValue = date;
                    else
                        propertyValue = null;
                    break;
                case bool:
                    propertyValue = property.BooleanValue;
                    break;
                case decimal:
                    propertyValue = property.DecimalValue;
                    break;
            }

            if (objectProperty.PropertyType.IsEnum && objectPropertyValue != null)
            {
                objectPropertyValue = (int)objectPropertyValue;
                propertyValue = property.IntValue;
            }

            propertyValue.Should().Be(objectPropertyValue, $"The property \"{typeof(T).Name}.{property.PropertyName}\" of these objects is not equal");
        }

		_logger.LogDebug("<<< Exiting PropertiesShouldEqual(actual={Actual},manager={Manager},replacePairs={ReplacePairs},filter={Filter})", actual, manager, replacePairs, filter);
        return actual;
    }

    protected async Task<PropertyManager<T>> GetPropertyManagerAsync<T>(XLWorkbook workbook)
    {
        var languages = await _languageService.GetAllLanguagesAsync();

        //the columns
        var metadata = ImportManager.GetWorkbookMetadata<T>(workbook, languages);
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        return new PropertyManager<T>(defaultProperties, _catalogSettings, localizedProperties);
    }

    protected XLWorkbook GetWorkbook(byte[] excelData)
    {
		_logger.LogDebug(">>> Entering GetWorkbook(excelData={ExcelData})", excelData);
        var stream = new MemoryStream(excelData);
		_logger.LogDebug("GetWorkbook(excelData={ExcelData}): stream → {Stream}", excelData, stream);
		_logger.LogDebug("<<< Exiting GetWorkbook(excelData={ExcelData})", excelData);
        return new XLWorkbook(stream);
    }

    protected T AreAllObjectPropertiesPresent<T>(T obj, PropertyManager<T> manager, params string[] filters)
    {
		_logger.LogDebug(">>> Entering AreAllObjectPropertiesPresent(obj={Obj},manager={Manager},filters={Filters})", obj, manager, filters);
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            if (filters.Contains(propertyInfo.Name))
                continue;

            if (manager.GetDefaultProperties.Any(p => p.PropertyName == propertyInfo.Name))
                continue;

            Assert.Fail($"The property \"{typeof(T).Name}.{propertyInfo.Name}\" no present on excel file");
        }

		_logger.LogDebug("<<< Exiting AreAllObjectPropertiesPresent(obj={Obj},manager={Manager},filters={Filters})", obj, manager, filters);
        return obj;
    }

    #endregion

    #region Test export to excel

    [Test]
    public async Task CanExportOrdersXlsx()
    {
		_logger.LogDebug(">>> Entering CanExportOrdersXlsx()");
        var orders = await _orderService.SearchOrdersAsync();

		_logger.LogDebug("CanExportOrdersXlsx(): orders → {Orders}", orders);
        var excelData = await _exportManager.ExportOrdersToXlsxAsync(orders);
		_logger.LogDebug("CanExportOrdersXlsx(): excelData → {ExcelData}", excelData);
        var workbook = GetWorkbook(excelData);
		_logger.LogDebug("CanExportOrdersXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Order>(workbook);
		_logger.LogDebug("CanExportOrdersXlsx(): manager → {Manager}", manager);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        var replacePairs = new Dictionary<string, string>
		_logger.LogDebug("CanExportOrdersXlsx(): replacePairs → {ReplacePairs}", replacePairs);
        {
            { "OrderId", "Id" },
            { "OrderStatus", "OrderStatusId" },
            { "PaymentStatus", "PaymentStatusId" },
            { "ShippingStatus", "ShippingStatusId" },
            { "ShippingPickupInStore", "PickupInStore" }
        };

        var order = orders.First();
		_logger.LogDebug("CanExportOrdersXlsx(): order → {Order}", order);

        var ignore = new List<string>();
		_logger.LogDebug("CanExportOrdersXlsx(): ignore → {Ignore}", ignore);
        ignore.AddRange(replacePairs.Values);

        //not exported fields
        ignore.AddRange(new[]
        {
            "BillingAddressId", "ShippingAddressId", "PickupAddressId", "CustomerTaxDisplayTypeId",
            "RewardPointsHistoryEntryId", "CheckoutAttributeDescription", "CheckoutAttributesXml",
            "CustomerLanguageId", "CustomerIp", "AllowStoringCreditCardNumber", "CardType", "CardName",
            "CardNumber", "MaskedCreditCardNumber", "CardCvv2", "CardExpirationMonth", "CardExpirationYear",
            "AuthorizationTransactionId", "AuthorizationTransactionCode", "AuthorizationTransactionResult",
            "CaptureTransactionId", "CaptureTransactionResult", "SubscriptionTransactionId", "PaidDateUtc",
            "Deleted", "PickupAddress", "RedeemedRewardPointsEntryId", "DiscountUsageHistory", "GiftCardUsageHistory",
            "OrderNotes", "OrderItems", "Shipments", "OrderStatus", "PaymentStatus", "ShippingStatus",
            "CustomerTaxDisplayType", "CustomOrderNumber"
        });

        //fields tested individually
        ignore.AddRange(new[]
        {
            "Customer", "BillingAddressId", "ShippingAddressId", "EntityCacheKey"
        });

        manager.SetSelectList("OrderStatus", await OrderStatus.Pending.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("PaymentStatus", await PaymentStatus.Pending.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("ShippingStatus", await ShippingStatus.ShippingNotRequired.ToSelectListAsync(useLocalization: false));

        AreAllObjectPropertiesPresent(order, manager, ignore.ToArray());
        PropertiesShouldEqual(order, manager, replacePairs);

        var addressFields = new List<string>
        {
            "FirstName",
            "LastName",
            "Email",
            "Company",
            "Country",
            "StateProvince",
            "City",
            "County",
            "Address1",
            "Address2",
            "ZipPostalCode",
            "PhoneNumber",
            "FaxNumber"
        };

        const string billingPattern = "Billing";
        replacePairs = addressFields.ToDictionary(p => billingPattern + p, p => p);

        var testBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
		_logger.LogDebug("CanExportOrdersXlsx(): testBillingAddress → {TestBillingAddress}", testBillingAddress);

        PropertiesShouldEqual(testBillingAddress, manager, replacePairs, "CreatedOnUtc", "BillingCountry");

        var country = await _countryService.GetCountryByAddressAsync(testBillingAddress);
		_logger.LogDebug("CanExportOrdersXlsx(): country → {Country}", country);
        manager.GetDefaultProperties.First(p => p.PropertyName == "BillingCountry").StringValue.Should().Be(country.Name);

        const string shippingPattern = "Shipping";
        replacePairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);
        var testShippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
		_logger.LogDebug("CanExportOrdersXlsx(): testShippingAddress → {TestShippingAddress}", testShippingAddress);
        PropertiesShouldEqual(testShippingAddress, manager, replacePairs, "CreatedOnUtc", "ShippingCountry");
        country = await _countryService.GetCountryByAddressAsync(testShippingAddress);
        manager.GetDefaultProperties.First(p => p.PropertyName == "ShippingCountry").StringValue.Should().Be(country.Name);
		_logger.LogDebug("<<< Exiting CanExportOrdersXlsx()");
    }

    [Test]
    public async Task CanExportManufacturersXlsx()
    {
		_logger.LogDebug(">>> Entering CanExportManufacturersXlsx()");
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync();

		_logger.LogDebug("CanExportManufacturersXlsx(): manufacturers → {Manufacturers}", manufacturers);
        var excelData = await _exportManager.ExportManufacturersToXlsxAsync(manufacturers);
		_logger.LogDebug("CanExportManufacturersXlsx(): excelData → {ExcelData}", excelData);
        var workbook = GetWorkbook(excelData);
		_logger.LogDebug("CanExportManufacturersXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Manufacturer>(workbook);
		_logger.LogDebug("CanExportManufacturersXlsx(): manager → {Manager}", manager);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        var manufacturer = manufacturers.First();
		_logger.LogDebug("CanExportManufacturersXlsx(): manufacturer → {Manufacturer}", manufacturer);

        var ignore = new List<string> { "Picture", "EntityCacheKey", "PictureId", "SubjectToAcl", "LimitedToStores", "Deleted", "CreatedOnUtc", "UpdatedOnUtc", "AppliedDiscounts", "DiscountManufacturerMappings" };
		_logger.LogDebug("CanExportManufacturersXlsx(): ignore → {Ignore}", ignore);

        AreAllObjectPropertiesPresent(manufacturer, manager, ignore.ToArray());
        PropertiesShouldEqual(manufacturer, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanExportManufacturersXlsx()");
    }

    [Test]
    public async Task CanExportCustomersToXlsx()
    {
		_logger.LogDebug(">>> Entering CanExportCustomersToXlsx()");
        var customers = await _customerService.GetAllCustomersAsync();

		_logger.LogDebug("CanExportCustomersToXlsx(): customers → {Customers}", customers);
        var excelData = await _exportManager.ExportCustomersToXlsxAsync(customers);
		_logger.LogDebug("CanExportCustomersToXlsx(): excelData → {ExcelData}", excelData);
        var workbook = GetWorkbook(excelData);
		_logger.LogDebug("CanExportCustomersToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Customer>(workbook);
		_logger.LogDebug("CanExportCustomersToXlsx(): manager → {Manager}", manager);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        manager.SetSelectList("VatNumberStatus", await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));

        var customer = customers.First();
		_logger.LogDebug("CanExportCustomersToXlsx(): customer → {Customer}", customer);

        var ignore = new List<string> { "Id", "ExternalAuthenticationRecords", "ShoppingCartItems",
            "ReturnRequests", "BillingAddress", "ShippingAddress", "Addresses", "AdminComment",
            "EmailToRevalidate", "HasShoppingCartItems", "RequireReLogin", "FailedLoginAttempts",
            "CannotLoginUntilDateUtc", "Deleted", "IsSystemAccount", "SystemName", "LastIpAddress",
            "LastLoginDateUtc", "LastActivityDateUtc", "RegisteredInStoreId", "BillingAddressId", "ShippingAddressId",
            "CustomerCustomerRoleMappings", "CustomerAddressMappings", "EntityCacheKey", "VendorId",
            "DateOfBirth", "CountryId",
            "StateProvinceId", "VatNumberStatusId", "TimeZoneId",
            "CurrencyId", "LanguageId", "TaxDisplayTypeId", "TaxDisplayType", "TaxDisplayType", "VatNumberStatusId", "MustChangePassword" };

        if (!_customerSettings.FirstNameEnabled)
            ignore.Add("FirstName");

        if (!_customerSettings.LastNameEnabled)
            ignore.Add("LastName");

        if (!_customerSettings.GenderEnabled)
            ignore.Add("Gender");

        if (!_customerSettings.CompanyEnabled)
            ignore.Add("Company");

        if (!_customerSettings.StreetAddressEnabled)
            ignore.Add("StreetAddress");

        if (!_customerSettings.StreetAddress2Enabled)
            ignore.Add("StreetAddress2");

        if (!_customerSettings.ZipPostalCodeEnabled)
            ignore.Add("ZipPostalCode");

        if (!_customerSettings.CityEnabled)
            ignore.Add("City");

        if (!_customerSettings.CountyEnabled)
            ignore.Add("County");

        if (!_customerSettings.CountryEnabled)
            ignore.Add("Country");

        if (!_customerSettings.StateProvinceEnabled)
            ignore.Add("StateProvince");

        if (!_customerSettings.PhoneEnabled)
            ignore.Add("Phone");

        if (!_customerSettings.FaxEnabled)
            ignore.Add("Fax");

        AreAllObjectPropertiesPresent(customer, manager, ignore.ToArray());
        PropertiesShouldEqual(customer, manager, new Dictionary<string, string>());
		_logger.LogDebug("<<< Exiting CanExportCustomersToXlsx()");
    }

    [Test]
    public async Task CanExportCategoriesToXlsx()
    {
		_logger.LogDebug(">>> Entering CanExportCategoriesToXlsx()");
        var categories = await _categoryService.GetAllCategoriesAsync();

		_logger.LogDebug("CanExportCategoriesToXlsx(): categories → {Categories}", categories);
        var excelData = await _exportManager.ExportCategoriesToXlsxAsync(categories);
		_logger.LogDebug("CanExportCategoriesToXlsx(): excelData → {ExcelData}", excelData);
        var workbook = GetWorkbook(excelData);
		_logger.LogDebug("CanExportCategoriesToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Category>(workbook);
		_logger.LogDebug("CanExportCategoriesToXlsx(): manager → {Manager}", manager);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        var category = categories.First();
		_logger.LogDebug("CanExportCategoriesToXlsx(): category → {Category}", category);

        var ignore = new List<string> { "CreatedOnUtc", "EntityCacheKey", "Picture", "PictureId", "AppliedDiscounts", "UpdatedOnUtc", "SubjectToAcl", "LimitedToStores", "Deleted", "DiscountCategoryMappings", "RestrictFromVendors" };
		_logger.LogDebug("CanExportCategoriesToXlsx(): ignore → {Ignore}", ignore);

        AreAllObjectPropertiesPresent(category, manager, ignore.ToArray());
        PropertiesShouldEqual(category, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanExportCategoriesToXlsx()");
    }

    [Test]
    public async Task CanExportProductsToXlsx()
    {
		_logger.LogDebug(">>> Entering CanExportProductsToXlsx()");
        var replacePairs = new Dictionary<string, string>
        {
            { "ProductId", "Id" },
            { "ProductType", "ProductTypeId" },
            { "GiftCardType", "GiftCardTypeId" },
            { "Vendor", "VendorId" },
            { "ProductTemplate", "ProductTemplateId" },
            { "DeliveryDate", "DeliveryDateId" },
            { "TaxCategory", "TaxCategoryId" },
            { "ManageInventoryMethod", "ManageInventoryMethodId" },
            { "ProductAvailabilityRange", "ProductAvailabilityRangeId" },
            { "LowStockActivity", "LowStockActivityId" },
            { "BackorderMode", "BackorderModeId" },
            { "BasepriceUnit", "BasepriceUnitId" },
            { "BasepriceBaseUnit", "BasepriceBaseUnitId" },
            { "SKU", "Sku" },
            { "DownloadActivationType", "DownloadActivationTypeId" },
            { "RecurringCyclePeriod", "RecurringCyclePeriodId" },
            { "RentalPricePeriod", "RentalPricePeriodId" }
        };

        var ignore = new List<string> { "Categories", "Manufacturers", "AdminComment",
            "ProductType", "BackorderMode", "DownloadActivationType", "GiftCardType", "LowStockActivity",
            "ManageInventoryMethod", "RecurringCyclePeriod", "RentalPricePeriod", "ProductCategories",
            "ProductManufacturers", "ProductPictures", "ProductReviews", "ProductSpecificationAttributes",
            "ProductTags", "ProductAttributeMappings", "ProductAttributeCombinations", "TierPrices",
            "AppliedDiscounts", "ProductWarehouseInventory", "ApprovedRatingSum", "NotApprovedRatingSum",
            "ApprovedTotalReviews", "NotApprovedTotalReviews", "SubjectToAcl", "LimitedToStores", "Deleted",
            "DownloadExpirationDays", "AvailableStartDateTimeUtc",
            "AvailableEndDateTimeUtc", "DisplayOrder", "CreatedOnUtc", "UpdatedOnUtc", "ProductProductTagMappings",
            "DiscountProductMappings", "EntityCacheKey" };

        ignore.AddRange(replacePairs.Values);

        var product = _productRepository.Table.ToList().First();
		_logger.LogDebug("CanExportProductsToXlsx(): product → {Product}", product);

        var excelData = await _exportManager.ExportProductsToXlsxAsync(new[] { product });
		_logger.LogDebug("CanExportProductsToXlsx(): excelData → {ExcelData}", excelData);
        var workbook = GetWorkbook(excelData);
		_logger.LogDebug("CanExportProductsToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Product>(workbook);
		_logger.LogDebug("CanExportProductsToXlsx(): manager → {Manager}", manager);

        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.SetSelectList("ProductType", await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("DownloadActivationType", await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("ManageInventoryMethod", await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("LowStockActivity", await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("RecurringCyclePeriod", await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

        var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true);
		_logger.LogDebug("CanExportProductsToXlsx(): vendors → {Vendors}", vendors);
        manager.SetSelectList("Vendor", vendors.Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
        var templates = await _productTemplateService.GetAllProductTemplatesAsync();
		_logger.LogDebug("CanExportProductsToXlsx(): templates → {Templates}", templates);
        manager.SetSelectList("ProductTemplate", templates.Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
        var dates = await _dateRangeService.GetAllDeliveryDatesAsync();
		_logger.LogDebug("CanExportProductsToXlsx(): dates → {Dates}", dates);
        manager.SetSelectList("DeliveryDate", dates.Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
        var availabilityRanges = await _dateRangeService.GetAllProductAvailabilityRangesAsync();
		_logger.LogDebug("CanExportProductsToXlsx(): availabilityRanges → {AvailabilityRanges}", availabilityRanges);
        manager.SetSelectList("ProductAvailabilityRange", availabilityRanges.Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
        var categories = await _taxCategoryService.GetAllTaxCategoriesAsync();
		_logger.LogDebug("CanExportProductsToXlsx(): categories → {Categories}", categories);
        manager.SetSelectList("TaxCategory", categories.Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
        var measureWeights = await _measureService.GetAllMeasureWeightsAsync();
		_logger.LogDebug("CanExportProductsToXlsx(): measureWeights → {MeasureWeights}", measureWeights);
        manager.SetSelectList("BasepriceUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
        manager.SetSelectList("BasepriceBaseUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));

        manager.Remove("ProductTags");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        AreAllObjectPropertiesPresent(product, manager, ignore.ToArray());
        PropertiesShouldEqual(product, manager, replacePairs);
		_logger.LogDebug("<<< Exiting CanExportProductsToXlsx()");
    }

    #endregion
}