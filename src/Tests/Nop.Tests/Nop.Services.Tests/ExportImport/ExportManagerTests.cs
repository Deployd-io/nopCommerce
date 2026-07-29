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

namespace Nop.Tests.Nop.Services.Tests.ExportImport;

[TestFixture]
public class ExportManagerTests : ServiceTest
{
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
		logger.LogDebug(">>> Entering SetUp()");
        _customerSettings = GetService<CustomerSettings>();
        _catalogSettings = GetService<CatalogSettings>();
		logger.LogDebug("SetUp(): _customerSettings → {_customerSettings}", _customerSettings);
		logger.LogDebug("SetUp(): _catalogSettings → {_catalogSettings}", _catalogSettings);
        _addressService = GetService<IAddressService>();
        _categoryService = GetService<ICategoryService>();
		logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
		logger.LogDebug("SetUp(): _categoryService → {_categoryService}", _categoryService);
        _countryService = GetService<ICountryService>();
        _customerService = GetService<ICustomerService>();
		logger.LogDebug("SetUp(): _countryService → {_countryService}", _countryService);
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        _dateRangeService = GetService<IDateRangeService>();
        _exportManager = GetService<IExportManager>();
		logger.LogDebug("SetUp(): _dateRangeService → {_dateRangeService}", _dateRangeService);
		logger.LogDebug("SetUp(): _exportManager → {_exportManager}", _exportManager);
        _languageService = GetService<ILanguageService>();
        _manufacturerService = GetService<IManufacturerService>();
		logger.LogDebug("SetUp(): _languageService → {_languageService}", _languageService);
		logger.LogDebug("SetUp(): _manufacturerService → {_manufacturerService}", _manufacturerService);
        _measureService = GetService<IMeasureService>();
        _orderService = GetService<IOrderService>();
		logger.LogDebug("SetUp(): _measureService → {_measureService}", _measureService);
		logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);
        _productTemplateService = GetService<IProductTemplateService>();
        _productRepository = GetService<IRepository<Product>>();
		logger.LogDebug("SetUp(): _productTemplateService → {_productTemplateService}", _productTemplateService);
		logger.LogDebug("SetUp(): _productRepository → {_productRepository}", _productRepository);
        _taxCategoryService = GetService<ITaxCategoryService>();
        _vendorService = GetService<IVendorService>();
		logger.LogDebug("SetUp(): _taxCategoryService → {_taxCategoryService}", _taxCategoryService);
		logger.LogDebug("SetUp(): _vendorService → {_vendorService}", _vendorService);

        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                true);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                true);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                true);

        _productEditorSettings = GetService<ProductEditorSettings>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _productEditorSettings → {_productEditorSettings}", _productEditorSettings);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "category-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "manufacturer-advanced-mode",
                false);
        await GetService<IGenericAttributeService>()
            .SaveAttributeAsync(await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail), "product-advanced-mode",
                false);
				logger.LogDebug("<<< Exiting TearDown()");
    }

    #endregion

    #region Utilities

    protected static T PropertiesShouldEqual<T, Tp>(T actual, PropertyManager<Tp> manager, IDictionary<string, string> replacePairs, params string[] filter)
    {
        var objectProperties = typeof(T).GetProperties();
        foreach (var property in manager.GetDefaultProperties)
        {
            if (filter.Contains(property.PropertyName))
                continue;

            var objectProperty = replacePairs.TryGetValue(property.PropertyName, out var value) ? objectProperties.FirstOrDefault(p => p.Name == value)
                : objectProperties.FirstOrDefault(p => p.Name == property.PropertyName);

            if (objectProperty == null)
                continue;

            var objectPropertyValue = objectProperty.GetValue(actual);
            var propertyValue = property.PropertyValue;

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
        var stream = new MemoryStream(excelData);
        return new XLWorkbook(stream);
    }

    protected T AreAllObjectPropertiesPresent<T>(T obj, PropertyManager<T> manager, params string[] filters)
    {
        foreach (var propertyInfo in typeof(T).GetProperties())
        {
            if (filters.Contains(propertyInfo.Name))
                continue;

            if (manager.GetDefaultProperties.Any(p => p.PropertyName == propertyInfo.Name))
                continue;

            Assert.Fail($"The property \"{typeof(T).Name}.{propertyInfo.Name}\" no present on excel file");
        }

        return obj;
    }

    #endregion

    #region Test export to excel

    [Test]
    public async Task CanExportOrdersXlsx()
    {
		logger.LogDebug(">>> Entering CanExportOrdersXlsx()");
        var orders = await _orderService.SearchOrdersAsync();

		logger.LogDebug("CanExportOrdersXlsx(): orders → {Orders}", orders);
        var excelData = await _exportManager.ExportOrdersToXlsxAsync(orders);
        var workbook = GetWorkbook(excelData);
		logger.LogDebug("CanExportOrdersXlsx(): excelData → {ExcelData}", excelData);
		logger.LogDebug("CanExportOrdersXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Order>(workbook);

		logger.LogDebug("CanExportOrdersXlsx(): manager → {Manager}", manager);
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        var replacePairs = new Dictionary<string, string>
        {
			logger.LogDebug("CanExportOrdersXlsx(): replacePairs → {ReplacePairs}", replacePairs);
            { "OrderId", "Id" },
            { "OrderStatus", "OrderStatusId" },
            { "PaymentStatus", "PaymentStatusId" },
            { "ShippingStatus", "ShippingStatusId" },
            { "ShippingPickupInStore", "PickupInStore" }
        };

        var order = orders.First();

		logger.LogDebug("CanExportOrdersXlsx(): order → {Order}", order);
        var ignore = new List<string>();
        ignore.AddRange(replacePairs.Values);
		logger.LogDebug("CanExportOrdersXlsx(): ignore → {Ignore}", ignore);

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

		logger.LogDebug("CanExportOrdersXlsx(): testBillingAddress → {TestBillingAddress}", testBillingAddress);
        PropertiesShouldEqual(testBillingAddress, manager, replacePairs, "CreatedOnUtc", "BillingCountry");

        var country = await _countryService.GetCountryByAddressAsync(testBillingAddress);
        manager.GetDefaultProperties.First(p => p.PropertyName == "BillingCountry").StringValue.Should().Be(country.Name);
		logger.LogDebug("CanExportOrdersXlsx(): country → {Country}", country);

        const string shippingPattern = "Shipping";
        replacePairs = addressFields.ToDictionary(p => shippingPattern + p, p => p);
        var testShippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
        PropertiesShouldEqual(testShippingAddress, manager, replacePairs, "CreatedOnUtc", "ShippingCountry");
		logger.LogDebug("CanExportOrdersXlsx(): testShippingAddress → {TestShippingAddress}", testShippingAddress);
        country = await _countryService.GetCountryByAddressAsync(testShippingAddress);
        manager.GetDefaultProperties.First(p => p.PropertyName == "ShippingCountry").StringValue.Should().Be(country.Name);
		logger.LogDebug("<<< Exiting CanExportOrdersXlsx()");
    }

    [Test]
    public async Task CanExportManufacturersXlsx()
    {
		logger.LogDebug(">>> Entering CanExportManufacturersXlsx()");
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync();

		logger.LogDebug("CanExportManufacturersXlsx(): manufacturers → {Manufacturers}", manufacturers);
        var excelData = await _exportManager.ExportManufacturersToXlsxAsync(manufacturers);
        var workbook = GetWorkbook(excelData);
		logger.LogDebug("CanExportManufacturersXlsx(): excelData → {ExcelData}", excelData);
		logger.LogDebug("CanExportManufacturersXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Manufacturer>(workbook);

		logger.LogDebug("CanExportManufacturersXlsx(): manager → {Manager}", manager);
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        var manufacturer = manufacturers.First();

		logger.LogDebug("CanExportManufacturersXlsx(): manufacturer → {Manufacturer}", manufacturer);
        var ignore = new List<string> { "Picture", "EntityCacheKey", "PictureId", "SubjectToAcl", "LimitedToStores", "Deleted", "CreatedOnUtc", "UpdatedOnUtc", "AppliedDiscounts", "DiscountManufacturerMappings" };

		logger.LogDebug("CanExportManufacturersXlsx(): ignore → {Ignore}", ignore);
        AreAllObjectPropertiesPresent(manufacturer, manager, ignore.ToArray());
        PropertiesShouldEqual(manufacturer, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanExportManufacturersXlsx()");
    }

    [Test]
    public async Task CanExportCustomersToXlsx()
    {
		logger.LogDebug(">>> Entering CanExportCustomersToXlsx()");
        var customers = await _customerService.GetAllCustomersAsync();

		logger.LogDebug("CanExportCustomersToXlsx(): customers → {Customers}", customers);
        var excelData = await _exportManager.ExportCustomersToXlsxAsync(customers);
        var workbook = GetWorkbook(excelData);
		logger.LogDebug("CanExportCustomersToXlsx(): excelData → {ExcelData}", excelData);
		logger.LogDebug("CanExportCustomersToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Customer>(workbook);

		logger.LogDebug("CanExportCustomersToXlsx(): manager → {Manager}", manager);
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        manager.SetSelectList("VatNumberStatus", await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));

        var customer = customers.First();

		logger.LogDebug("CanExportCustomersToXlsx(): customer → {Customer}", customer);
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
		logger.LogDebug("<<< Exiting CanExportCustomersToXlsx()");
    }

    [Test]
    public async Task CanExportCategoriesToXlsx()
    {
		logger.LogDebug(">>> Entering CanExportCategoriesToXlsx()");
        var categories = await _categoryService.GetAllCategoriesAsync();

		logger.LogDebug("CanExportCategoriesToXlsx(): categories → {Categories}", categories);
        var excelData = await _exportManager.ExportCategoriesToXlsxAsync(categories);
        var workbook = GetWorkbook(excelData);
		logger.LogDebug("CanExportCategoriesToXlsx(): excelData → {ExcelData}", excelData);
		logger.LogDebug("CanExportCategoriesToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Category>(workbook);

		logger.LogDebug("CanExportCategoriesToXlsx(): manager → {Manager}", manager);
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        manager.ReadDefaultFromXlsx(worksheet, 2);
        var category = categories.First();

		logger.LogDebug("CanExportCategoriesToXlsx(): category → {Category}", category);
        var ignore = new List<string> { "CreatedOnUtc", "EntityCacheKey", "Picture", "PictureId", "AppliedDiscounts", "UpdatedOnUtc", "SubjectToAcl", "LimitedToStores", "Deleted", "DiscountCategoryMappings", "RestrictFromVendors" };

		logger.LogDebug("CanExportCategoriesToXlsx(): ignore → {Ignore}", ignore);
        AreAllObjectPropertiesPresent(category, manager, ignore.ToArray());
        PropertiesShouldEqual(category, manager, new Dictionary<string, string>());

        manager.GetDefaultProperties.First(p => p.PropertyName == "Picture").PropertyValue.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanExportCategoriesToXlsx()");
    }

    [Test]
    public async Task CanExportProductsToXlsx()
    {
		logger.LogDebug(">>> Entering CanExportProductsToXlsx()");
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

		logger.LogDebug("CanExportProductsToXlsx(): product → {Product}", product);
        var excelData = await _exportManager.ExportProductsToXlsxAsync(new[] { product });
        var workbook = GetWorkbook(excelData);
		logger.LogDebug("CanExportProductsToXlsx(): excelData → {ExcelData}", excelData);
		logger.LogDebug("CanExportProductsToXlsx(): workbook → {Workbook}", workbook);
        var manager = await GetPropertyManagerAsync<Product>(workbook);

		logger.LogDebug("CanExportProductsToXlsx(): manager → {Manager}", manager);
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
        manager.SetSelectList("Vendor", vendors.Select(v => v as BaseEntity).ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): vendors → {Vendors}", vendors);
        var templates = await _productTemplateService.GetAllProductTemplatesAsync();
        manager.SetSelectList("ProductTemplate", templates.Select(pt => pt as BaseEntity).ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): templates → {Templates}", templates);
        var dates = await _dateRangeService.GetAllDeliveryDatesAsync();
        manager.SetSelectList("DeliveryDate", dates.Select(dd => dd as BaseEntity).ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): dates → {Dates}", dates);
        var availabilityRanges = await _dateRangeService.GetAllProductAvailabilityRangesAsync();
        manager.SetSelectList("ProductAvailabilityRange", availabilityRanges.Select(range => range as BaseEntity).ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): availabilityRanges → {AvailabilityRanges}", availabilityRanges);
        var categories = await _taxCategoryService.GetAllTaxCategoriesAsync();
        manager.SetSelectList("TaxCategory", categories.Select(tc => tc as BaseEntity).ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): categories → {Categories}", categories);
        var measureWeights = await _measureService.GetAllMeasureWeightsAsync();
        manager.SetSelectList("BasepriceUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
		logger.LogDebug("CanExportProductsToXlsx(): measureWeights → {MeasureWeights}", measureWeights);
        manager.SetSelectList("BasepriceBaseUnit", measureWeights.Select(mw => mw as BaseEntity).ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));

        manager.Remove("ProductTags");

        manager.ReadDefaultFromXlsx(worksheet, 2);

        AreAllObjectPropertiesPresent(product, manager, ignore.ToArray());
        PropertiesShouldEqual(product, manager, replacePairs);
		logger.LogDebug("<<< Exiting CanExportProductsToXlsx()");
    }

    #endregion
}