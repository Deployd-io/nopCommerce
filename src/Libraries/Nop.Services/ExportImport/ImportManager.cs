using ClosedXML.Excel;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.ExportImport;

/// <summary>
/// Import manager
/// </summary>
public partial class ImportManager : IImportManager
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IAddressService _addressService;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly ICategoryService _categoryService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomNumberFormatter _customNumberFormatter;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IDateRangeService _dateRangeService;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly ILogger _logger;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly IServiceScopeFactory _serviceScopeFactory;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWarehouseService _warehouseService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly SecuritySettings _securitySettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly string[] _separator = [">>"];

    #endregion

    #region Ctor

    public ImportManager(CatalogSettings catalogSettings,
        IAddressService addressService,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        ICategoryService categoryService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ICustomNumberFormatter customNumberFormatter,
        INopDataProvider dataProvider,
        IDateRangeService dateRangeService,
        IFilterLevelValueService filterLevelValueService,
        IGenericAttributeService genericAttributeService,
        IHttpClientFactory httpClientFactory,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        ILogger logger,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        INopFileProvider fileProvider,
        IOrderService orderService,
        IPictureService pictureService,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        IServiceScopeFactory serviceScopeFactory,
        ISpecificationAttributeService specificationAttributeService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
        ITaxCategoryService taxCategoryService,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWarehouseService warehouseService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        SecuritySettings securitySettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressService = addressService;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _catalogSettings = catalogSettings;
        _categoryService = categoryService;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _customNumberFormatter = customNumberFormatter;
        _dataProvider = dataProvider;
        _dateRangeService = dateRangeService;
        _filterLevelValueService = filterLevelValueService;
        _genericAttributeService = genericAttributeService;
        _httpClientFactory = httpClientFactory;
        _fileProvider = fileProvider;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _logger = logger;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _orderService = orderService;
        _pictureService = pictureService;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _serviceScopeFactory = serviceScopeFactory;
        _specificationAttributeService = specificationAttributeService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
        _taxCategoryService = taxCategoryService;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _warehouseService = warehouseService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _securitySettings = securitySettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    protected virtual ExportedAdditionalProductInfoType GetTypeOfExportedAdditionalProductInfo(IXLWorksheet defaultWorksheet, List<IXLWorksheet> localizedWorksheets, PropertyManager<ExportProductAttribute> productAttributeManager, PropertyManager<ExportSpecificationAttribute> specificationAttributeManager, PropertyManager<ExportTierPrice> tierPriceManager, int iRow)
    {
        productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

        if (productAttributeManager.IsCaption)
        {
            foreach (var worksheet in localizedWorksheets)
                productAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

            return ExportedAdditionalProductInfoType.ProductAttribute;
        }

        specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

        if (specificationAttributeManager.IsCaption)
        {
            foreach (var worksheet in localizedWorksheets)
                specificationAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

            return ExportedAdditionalProductInfoType.SpecificationAttribute;
        }

        tierPriceManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

        if (tierPriceManager.IsCaption)
            return ExportedAdditionalProductInfoType.TierPrices;

        return ExportedAdditionalProductInfoType.NotSpecified;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
    {
        var attributeType = (cellValue ?? string.Empty).ToString();

        if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
        {
            worksheet.Row(endRow).OutlineLevel = 1;
        }
        else
        {
            if ((await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false))
                .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                worksheet.Row(endRow).OutlineLevel = 1;
            else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                worksheet.Row(endRow).OutlineLevel = 1;
        }
    }

    protected virtual void CopyDataToNewFile(ImportProductMetadata metadata, IXLWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
    {
        using var workbook = new XLWorkbook();

        // get handles to the worksheets
        var outWorksheet = workbook.Worksheets.Add(nameof(Product));
        metadata.Manager.WriteDefaultCaption(outWorksheet);
        var outRow = 2;
        for (var row = startRow; row <= endRow; row++)
        {
            outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;

            for (var cell = 1; cell <= endCell; cell++)
                outWorksheet.Row(outRow).Cell(cell).Value = worksheet.Row(row).Cell(cell).Value;

            outRow += 1;
        }

        workbook.SaveAs(filePath);
    }

    protected virtual int GetColumnIndex(string[] properties, string columnName)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(columnName);

        for (var i = 0; i < properties.Length; i++)
            if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                return i + 1; //excel indexes start from 1
        return 0;
    }

    protected virtual string GetMimeTypeFromFilePath(string filePath)
    {
        new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

        //set to jpeg in case mime type cannot be found
        return mimeType ?? _pictureService.GetPictureContentTypeByFileExtension(_fileProvider.GetFileExtension(filePath));
    }

    /// <summary>
    /// Creates or loads the image
    /// </summary>
    /// <param name="picturePath">The path to the image file</param>
    /// <param name="name">The name of the object</param>
    /// <param name="picId">Image identifier, may be null</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the image or null if the image has not changed
    /// </returns>
    protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
    {
        if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
            return null;

        var mimeType = GetMimeTypeFromFilePath(picturePath);
        if (string.IsNullOrEmpty(mimeType))
            return null;

        var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
        var pictureAlreadyExists = false;
        if (picId != null)
        {
            //compare with existing product pictures
            var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
            if (existingPicture != null)
            {
                var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                //picture binary after validation (like in database)
                var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, name);
                if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                    existingBinary.SequenceEqual(newPictureBinary))
                {
                    pictureAlreadyExists = true;
                }
            }
        }

        if (pictureAlreadyExists)
            return null;

        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
        return newPicture;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task LogPictureInsertErrorAsync(string picturePath, Exception ex)
    {
		_logger.LogDebug(">>> Entering LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex})", picturePath, ex);
        var extension = _fileProvider.GetFileExtension(picturePath);
        var name = _fileProvider.GetFileNameWithoutExtension(picturePath);
		_logger.LogDebug("LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex}): name → {Name}", picturePath, ex, name);
		_logger.LogDebug("LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex}): extension → {Extension}", picturePath, ex, extension);

        var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
		_logger.LogDebug("LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex}): point → {Point}", picturePath, ex, point);
        var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;
		_logger.LogDebug("LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex}): fileName → {FileName}", picturePath, ex, fileName);

        await _logger.ErrorAsync($"Insert picture failed (file name: {fileName})", ex);
		_logger.LogDebug("<<< Exiting LogPictureInsertErrorAsync(picturePath={PicturePath},ex={Ex})", picturePath, ex);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductImagesUsingServicesAsync(IList<ProductPictureMetadata> productPictureMetadata)
    {
        foreach (var product in productPictureMetadata)
        {
            foreach (var picturePath in product.PicturePaths)
            {
                if (string.IsNullOrEmpty(picturePath))
                    continue;

                var mimeType = GetMimeTypeFromFilePath(picturePath);
                if (string.IsNullOrEmpty(mimeType))
                    continue;

                var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                var pictureAlreadyExists = false;
                if (!product.IsNew)
                {
                    //compare with existing product pictures
                    var existingPictures = await _pictureService.GetPicturesByProductIdAsync(product.ProductItem.Id);
                    foreach (var existingPicture in existingPictures)
                    {
                        var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                        //picture binary after validation (like in database)
                        var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, picturePath);
                        if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                            !existingBinary.SequenceEqual(newPictureBinary))
                            continue;
                        //the same picture content
                        pictureAlreadyExists = true;
                        break;
                    }
                }

                if (pictureAlreadyExists)
                    continue;

                try
                {
                    var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name));
                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = 1,
                        ProductId = product.ProductItem.Id
                    });
                    await _productService.UpdateProductAsync(product.ProductItem);
                }
                catch (Exception ex)
                {
                    await LogPictureInsertErrorAsync(picturePath, ex);
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductImagesUsingHashAsync(IList<ProductPictureMetadata> productPictureMetadata, IList<Product> allProductsBySku)
    {
        //performance optimization, load all pictures hashes
        //it will only be used if the images are stored in the SQL Server database (not compact)
        var trimByteCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
        var productsImagesIds = await _productService.GetProductsImagesIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());

        var allProductPictureIds = productsImagesIds.SelectMany(p => p.Value);

        var allPicturesHashes = allProductPictureIds.Any() ? await _dataProvider.GetFieldHashesAsync<PictureBinary>(p => allProductPictureIds.Contains(p.PictureId),
            p => p.PictureId, p => p.BinaryData) : new Dictionary<int, string>();

        foreach (var product in productPictureMetadata)
        {
            foreach (var picturePath in product.PicturePaths)
            {
                if (string.IsNullOrEmpty(picturePath))
                    continue;
                try
                {
                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    if (string.IsNullOrEmpty(mimeType))
                        continue;

                    var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                    var pictureAlreadyExists = false;
                    var seoFileName = await _pictureService.GetPictureSeNameAsync(product.ProductItem.Name);

                    if (!product.IsNew)
                    {
                        var newImageHash = HashHelper.CreateHash(
                            newPictureBinary,
                            ExportImportDefaults.ImageHashAlgorithm,
                            trimByteCount);

                        var newValidatedImageHash = HashHelper.CreateHash(
                            await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, seoFileName),
                            ExportImportDefaults.ImageHashAlgorithm,
                            trimByteCount);

                        var imagesIds = productsImagesIds.TryGetValue(product.ProductItem.Id, out var value) ? value : Array.Empty<int>();

                        pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                            .Select(p => p.Value)
                            .Any(p =>
                                p.Equals(newImageHash, StringComparison.OrdinalIgnoreCase) ||
                                p.Equals(newValidatedImageHash, StringComparison.OrdinalIgnoreCase));
                    }

                    if (pictureAlreadyExists)
                        continue;

                    var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, seoFileName);

                    await _productService.InsertProductPictureAsync(new ProductPicture
                    {
                        //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                        //pictures are duplicated
                        //maybe because entity size is too large
                        PictureId = newPicture.Id,
                        DisplayOrder = 1,
                        ProductId = product.ProductItem.Id
                    });

                    await _productService.UpdateProductAsync(product.ProductItem);
                }
                catch (Exception ex)
                {
                    await LogPictureInsertErrorAsync(picturePath, ex);
                }
            }
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(string seName, bool isParentCategoryExists)> UpdateCategoryByXlsxAsync(Category category, PropertyManager<Category> manager, Dictionary<string, ValueTask<Category>> allCategories, IList<Store> allStores, bool isNew)
    {
        var seName = string.Empty;
        var isParentCategoryExists = true;
        var isParentCategorySet = false;

        foreach (var property in manager.GetDefaultProperties)
        {
            switch (property.PropertyName)
            {
                case "Name":
                    category.Name = property.StringValue.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                    break;
                case "Description":
                    category.Description = property.StringValue;
                    break;
                case "CategoryTemplateId":
                    category.CategoryTemplateId = property.IntValue;
                    break;
                case "MetaKeywords":
                    category.MetaKeywords = property.StringValue;
                    break;
                case "MetaDescription":
                    category.MetaDescription = property.StringValue;
                    break;
                case "MetaTitle":
                    category.MetaTitle = property.StringValue;
                    break;
                case "ParentCategoryId":
                    if (!isParentCategorySet)
                    {
                        var parentCategory = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                        isParentCategorySet = parentCategory != null;

                        isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                        category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                    }

                    break;
                case "ParentCategoryName":
                    if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                    {
                        var categoryName = manager.GetDefaultProperty("ParentCategoryName").StringValue;
                        if (!string.IsNullOrEmpty(categoryName))
                        {
                            var parentCategory = allCategories.TryGetValue(categoryName, out var value)
                                //try find category by full name with all parent category names
                                ? await value
                                //try find category by name
                                : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));

                            if (parentCategory != null)
                            {
                                category.ParentCategoryId = parentCategory.Id;
                                isParentCategorySet = true;
                            }
                            else
                            {
                                isParentCategoryExists = false;
                            }
                        }
                    }

                    break;
                case "Picture":
                    var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                    if (picture != null)
                        category.PictureId = picture.Id;
                    break;
                case "PageSize":
                    category.PageSize = property.IntValue;
                    break;
                case "AllowCustomersToSelectPageSize":
                    category.AllowCustomersToSelectPageSize = property.BooleanValue;
                    break;
                case "PageSizeOptions":
                    category.PageSizeOptions = property.StringValue;
                    break;
                case "ShowOnHomepage":
                    category.ShowOnHomepage = property.BooleanValue;
                    break;
                case "PriceRangeFiltering":
                    category.PriceRangeFiltering = property.BooleanValue;
                    break;
                case "PriceFrom":
                    category.PriceFrom = property.DecimalValue;
                    break;
                case "PriceTo":
                    category.PriceTo = property.DecimalValue;
                    break;
                case "AutomaticallyCalculatePriceRange":
                    category.ManuallyPriceRange = property.BooleanValue;
                    break;
                case "IsLimitedToStores":
                    category.LimitedToStores = property.BooleanValue;
                    break;
                case "Published":
                    category.Published = property.BooleanValue;
                    break;
                case "DisplayOrder":
                    category.DisplayOrder = property.IntValue;
                    break;
                case "SeName":
                    seName = property.StringValue;
                    break;
            }
        }

        var tempProperty = manager.GetDefaultProperty("LimitedToStores");
        if (tempProperty != null)
        {
            var limitedToStoresList = tempProperty.StringValue;

            var importedStores = category.LimitedToStores ? limitedToStoresList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => allStores.FirstOrDefault(store => store.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList() : new List<int>();

            await _storeMappingService.SaveStoreMappingsAsync(category, importedStores);
        }

        category.UpdatedOnUtc = DateTime.UtcNow;
        return (seName, isParentCategoryExists);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(Category category, bool isNew, string curentCategoryBreadCrumb)> GetCategoryFromXlsxAsync(PropertyManager<Category> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Category>> allCategories)
    {
        manager.ReadDefaultFromXlsx(worksheet, iRow);

        //try get category from database by ID
        var category = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetDefaultProperty("Id")?.IntValue);

        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
        {
            var categoryName = manager.GetDefaultProperty("Name").StringValue;
            if (!string.IsNullOrEmpty(categoryName))
            {
                category = allCategories.TryGetValue(categoryName, out var value)
                    //try find category by full name with all parent category names
                    ? await value
                    //try find category by name
                    : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));
            }
        }

        var isNew = category == null;

        category ??= new Category();

        var curentCategoryBreadCrumb = string.Empty;

        if (isNew)
        {
            category.CreatedOnUtc = DateTime.UtcNow;
            //default values
            category.PageSize = _catalogSettings.DefaultCategoryPageSize;
            category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            category.Published = true;
            category.AllowCustomersToSelectPageSize = true;
        }
        else
            curentCategoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);

        return (category, isNew, curentCategoryBreadCrumb);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveCategoryAsync(bool isNew, Category category, Dictionary<string, ValueTask<Category>> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
    {
        if (isNew)
            await _categoryService.InsertCategoryAsync(category);
        else
            await _categoryService.UpdateCategoryAsync(category);

        var categoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
        if (!allCategories.ContainsKey(categoryBreadCrumb))
            allCategories.Add(categoryBreadCrumb, new ValueTask<Category>(category));
        if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
            categoryBreadCrumb != curentCategoryBreadCrumb)
            allCategories.Remove(curentCategoryBreadCrumb);

        //search engine name
        if (setSeName)
            await _urlRecordService.SaveSlugAsync(category, await _urlRecordService.ValidateSeNameAsync(category, seName, category.Name, true), 0);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportCategoryLocalizedAsync(Category category, WorkbookMetadata<Category> metadata, PropertyManager<Category> manager, int iRow, IList<Language> languages)
    {
        if (!metadata.LocalizedWorksheets.Any())
            return;

        var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            manager.CurrentLanguage = language;
            manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

            foreach (var property in manager.GetLocalizedProperties)
            {
                string localizedName = null;

                switch (property.PropertyName)
                {
                    case "Name":
                        localizedName = property.StringValue;
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Name, localizedName, language.Id);
                        break;
                    case "Description":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Description, property.StringValue, language.Id);
                        break;
                    case "MetaKeywords":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaKeywords, property.StringValue, language.Id);
                        break;
                    case "MetaDescription":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaDescription, property.StringValue, language.Id);
                        break;
                    case "MetaTitle":
                        await _localizedEntityService.SaveLocalizedValueAsync(category, m => m.MetaTitle, property.StringValue, language.Id);
                        break;
                    case "SeName":
                        //search engine name
                        if (setSeName)
                        {
                            var lSeName = await _urlRecordService.ValidateSeNameAsync(category, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(category, lSeName, language.Id);
                        }
                        break;
                }
            }
        }
		_logger.LogDebug("<<< Exiting ImportCategoryLocalizedAsync(category={Category},metadata={Metadata},manager={Manager},iRow={IRow},languages={Languages})", category, metadata, manager, iRow, languages);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportManufaturerLocalizedAsync(Manufacturer manufacturer, WorkbookMetadata<Manufacturer> metadata, PropertyManager<Manufacturer> manager, int iRow, IList<Language> languages)
    {
        if (!metadata.LocalizedWorksheets.Any())
            return;

        var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            manager.CurrentLanguage = language;
            manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

            foreach (var property in manager.GetLocalizedProperties)
            {
                string localizedName = null;

                switch (property.PropertyName)
                {
                    case "Name":
                        localizedName = property.StringValue;
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Name, localizedName, language.Id);
                        break;
                    case "Description":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Description, property.StringValue, language.Id);
                        break;
                    case "MetaKeywords":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaKeywords, property.StringValue, language.Id);
                        break;
                    case "MetaDescription":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaDescription, property.StringValue, language.Id);
                        break;
                    case "MetaTitle":
                        await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaTitle, property.StringValue, language.Id);
                        break;
                    case "SeName":
                        //search engine name
                        if (setSeName)
                        {
                            var localizedSeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(manufacturer, localizedSeName, language.Id);
                        }
                        break;
                }
            }
        }
		_logger.LogDebug("<<< Exiting ImportManufaturerLocalizedAsync(manufacturer={Manufacturer},metadata={Metadata},manager={Manager},iRow={IRow},languages={Languages})", manufacturer, metadata, manager, iRow, languages);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetOutLineForProductAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
    {
        try
        {
            var aid = Convert.ToInt32(cellValue ?? -1);

            var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(aid);

            if (productAttribute != null)
                worksheet.Row(endRow).OutlineLevel = 1;
        }
        catch (FormatException)
        {
            if ((cellValue ?? string.Empty).ToString() == "AttributeId")
                worksheet.Row(endRow).OutlineLevel = 1;
        }
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
    {
        var productAttributeManager = metadata.ProductAttributeManager;
        if (!_catalogSettings.ExportImportProductAttributes || lastLoadedProduct == null || productAttributeManager.IsCaption)
            return;

        var productAttributeId = productAttributeManager.GetDefaultProperty("AttributeId").IntValue;
        var attributeControlTypeId = productAttributeManager.GetDefaultProperty("AttributeControlType").IntValue;

        var productAttributeValueId = productAttributeManager.GetDefaultProperty("ProductAttributeValueId").IntValue;
        var associatedProductId = productAttributeManager.GetDefaultProperty("AssociatedProductId").IntValue;
        var valueName = productAttributeManager.GetDefaultProperty("ValueName").StringValue;
        var attributeValueTypeId = productAttributeManager.GetDefaultProperty("AttributeValueType").IntValue;
        var colorSquaresRgb = productAttributeManager.GetDefaultProperty("ColorSquaresRgb").StringValue;
        var imageSquaresPictureId = productAttributeManager.GetDefaultProperty("ImageSquaresPictureId").IntValue;
        var priceAdjustment = productAttributeManager.GetDefaultProperty("PriceAdjustment").DecimalValue;
        var priceAdjustmentUsePercentage = productAttributeManager.GetDefaultProperty("PriceAdjustmentUsePercentage").BooleanValue;
        var weightAdjustment = productAttributeManager.GetDefaultProperty("WeightAdjustment").DecimalValue;
        var cost = productAttributeManager.GetDefaultProperty("Cost").DecimalValue;
        var customerEntersQty = productAttributeManager.GetDefaultProperty("CustomerEntersQty").BooleanValue;
        var quantity = productAttributeManager.GetDefaultProperty("Quantity").IntValue;
        var isPreSelected = productAttributeManager.GetDefaultProperty("IsPreSelected").BooleanValue;
        var displayOrder = productAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
        var pictureIdsStr = productAttributeManager.GetDefaultProperty("PictureIds").StringValue;
        var textPrompt = productAttributeManager.GetDefaultProperty("AttributeTextPrompt").StringValue;
        var isRequired = productAttributeManager.GetDefaultProperty("AttributeIsRequired").BooleanValue;
        var attributeDisplayOrder = productAttributeManager.GetDefaultProperty("AttributeDisplayOrder").IntValue;

        var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(lastLoadedProduct.Id))
            .FirstOrDefault(pam => pam.ProductAttributeId == productAttributeId);
        var pictureIds = new List<int>();
        if (!string.IsNullOrWhiteSpace(pictureIdsStr))
            pictureIds = Array.ConvertAll(pictureIdsStr.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries), int.Parse).ToList();

        if (productAttributeMapping == null)
        {
            //insert mapping
            productAttributeMapping = new ProductAttributeMapping
            {
                ProductId = lastLoadedProduct.Id,
                ProductAttributeId = productAttributeId,
                TextPrompt = textPrompt,
                IsRequired = isRequired,
                AttributeControlTypeId = attributeControlTypeId,
                DisplayOrder = attributeDisplayOrder
            };
            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);
        }
        else
        {
            productAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
            productAttributeMapping.TextPrompt = textPrompt;
            productAttributeMapping.IsRequired = isRequired;
            productAttributeMapping.DisplayOrder = attributeDisplayOrder;
            await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
        }

        var pav = (await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id))
            .FirstOrDefault(p => p.Id == productAttributeValueId);

        //var pav = await _productAttributeService.GetProductAttributeValueByIdAsync(productAttributeValueId);

        var attributeControlType = (AttributeControlType)attributeControlTypeId;

        async Task saveAttributeValuePicturesAsync(Product product, ProductAttributeValue value)
        {
            var existingValuePictures =
                await _productAttributeService.GetProductAttributeValuePicturesAsync(value.Id);
            var productPictureIds = (await _pictureService.GetPicturesByProductIdAsync(product.Id))
                .Select(p => p.Id).ToList();

            //delete attribute value pictures
            var picturesToDelete = existingValuePictures
                .Where(p => pictureIds.Contains(p.PictureId) || !productPictureIds.Contains(p.PictureId))
                .ToList();
            await _productAttributeService.DeleteProductAttributeValuePicturesAsync(picturesToDelete);

            //add attribute value pictures
            foreach (var pictureId in pictureIds)
            {
                if (!productPictureIds.Contains(pictureId))
                    continue;

                if (_productAttributeService.FindProductAttributeValuePicture(existingValuePictures, value.Id,
                        pictureId) == null)
                {
                    await _productAttributeService.InsertProductAttributeValuePictureAsync(
                        new ProductAttributeValuePicture
                        {
                            ProductAttributeValueId = value.Id,
                            PictureId = pictureId
                        });
                }
            }

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws =>
                    ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                productAttributeManager.CurrentLanguage = language;
                productAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow,
                    ExportImportDefaults.ProductAdditionalInfoCellOffset);

                valueName = productAttributeManager.GetLocalizedProperty("ValueName").StringValue;
                textPrompt = productAttributeManager.GetLocalizedProperty("AttributeTextPrompt").StringValue;

                await _localizedEntityService.SaveLocalizedValueAsync(pav, p => p.Name, valueName, language.Id);
                await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping, p => p.TextPrompt,
                    textPrompt, language.Id);

                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (productAttributeMapping.ValidationRulesAllowed())
                        {
                            var defaultValue = productAttributeManager.GetLocalizedProperty("DefaultValue")
                                ?.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(productAttributeMapping,
                                p => p.DefaultValue, defaultValue, language.Id);
                        }

                        return;
                }
            }
        }

        if (pav == null)
        {
            switch (attributeControlType)
            {
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                case AttributeControlType.MultilineTextbox:
                case AttributeControlType.TextBox:
                    if (productAttributeMapping.ValidationRulesAllowed())
                    {
                        productAttributeMapping.ValidationMinLength = productAttributeManager.GetDefaultProperty("ValidationMinLength")?.IntValueNullable;
                        productAttributeMapping.ValidationMaxLength = productAttributeManager.GetDefaultProperty("ValidationMaxLength")?.IntValueNullable;
                        productAttributeMapping.ValidationFileMaximumSize = productAttributeManager.GetDefaultProperty("ValidationFileMaximumSize")?.IntValueNullable;
                        productAttributeMapping.ValidationFileAllowedExtensions = productAttributeManager.GetDefaultProperty("ValidationFileAllowedExtensions")?.StringValue;
                        productAttributeMapping.DefaultValue = productAttributeManager.GetDefaultProperty("DefaultValue")?.StringValue;

                        await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
                    }

                    return;
            }

            pav = new ProductAttributeValue
            {
                ProductAttributeMappingId = productAttributeMapping.Id,
                AttributeValueType = (AttributeValueType)attributeValueTypeId,
                AssociatedProductId = associatedProductId,
                Name = valueName,
                PriceAdjustment = priceAdjustment,
                PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage,
                WeightAdjustment = weightAdjustment,
                Cost = cost,
                IsPreSelected = isPreSelected,
                DisplayOrder = displayOrder,
                ColorSquaresRgb = colorSquaresRgb,
                ImageSquaresPictureId = imageSquaresPictureId,
                CustomerEntersQty = customerEntersQty,
                Quantity = quantity
            };

            await _productAttributeService.InsertProductAttributeValueAsync(pav);
            await saveAttributeValuePicturesAsync(lastLoadedProduct, pav);
        }
        else
        {
            pav.AttributeValueTypeId = attributeValueTypeId;
            pav.AssociatedProductId = associatedProductId;
            pav.Name = valueName;
            pav.ColorSquaresRgb = colorSquaresRgb;
            pav.ImageSquaresPictureId = imageSquaresPictureId;
            pav.PriceAdjustment = priceAdjustment;
            pav.PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage;
            pav.WeightAdjustment = weightAdjustment;
            pav.Cost = cost;
            pav.CustomerEntersQty = customerEntersQty;
            pav.Quantity = quantity;
            pav.IsPreSelected = isPreSelected;
            pav.DisplayOrder = displayOrder;

            await _productAttributeService.UpdateProductAttributeValueAsync(pav);
            await saveAttributeValuePicturesAsync(lastLoadedProduct, pav);
        }
		_logger.LogDebug("<<< Exiting ImportProductAttributeAsync(metadata={Metadata},lastLoadedProduct={LastLoadedProduct},languages={Languages},iRow={IRow})", metadata, lastLoadedProduct, languages, iRow);
    }

    /// <returns>A task that represents the asynchronous operation</returns>


    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportSpecificationAttributeAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
    {
        var specificationAttributeManager = metadata.SpecificationAttributeManager;
        if (!_catalogSettings.ExportImportProductSpecificationAttributes || lastLoadedProduct == null || specificationAttributeManager.IsCaption)
            return;

        var attributeTypeId = specificationAttributeManager.GetDefaultProperty("AttributeType").IntValue;
        var allowFiltering = specificationAttributeManager.GetDefaultProperty("AllowFiltering").BooleanValue;
        var specificationAttributeOptionId = specificationAttributeManager.GetDefaultProperty("SpecificationAttributeOptionId").IntValue;
        var productId = lastLoadedProduct.Id;
        var customValue = specificationAttributeManager.GetDefaultProperty("CustomValue").StringValue;
        var displayOrder = specificationAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
        var showOnProductPage = specificationAttributeManager.GetDefaultProperty("ShowOnProductPage").BooleanValue;

        //if specification attribute option isn't set, try to get first of possible specification attribute option for current specification attribute
        if (specificationAttributeOptionId == 0)
        {
            var specificationAttribute = specificationAttributeManager.GetDefaultProperty("SpecificationAttribute").IntValue;
            specificationAttributeOptionId =
                (await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                    specificationAttribute))
                .FirstOrDefault()?.Id ?? specificationAttributeOptionId;
        }

        var productSpecificationAttribute = specificationAttributeOptionId == 0
            ? null
            : (await _specificationAttributeService.GetProductSpecificationAttributesAsync(productId, specificationAttributeOptionId)).FirstOrDefault();

        var isNew = productSpecificationAttribute == null;

        if (isNew)
            productSpecificationAttribute = new ProductSpecificationAttribute();

        if (attributeTypeId != (int)SpecificationAttributeType.Option)
            //we allow filtering only for "Option" attribute type
            allowFiltering = false;

        //we don't allow CustomValue for "Option" attribute type
        if (attributeTypeId == (int)SpecificationAttributeType.Option)
            customValue = null;

        productSpecificationAttribute.AttributeTypeId = attributeTypeId;
        productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
        productSpecificationAttribute.ProductId = productId;
        productSpecificationAttribute.CustomValue = customValue;
        productSpecificationAttribute.AllowFiltering = allowFiltering;
        productSpecificationAttribute.ShowOnProductPage = showOnProductPage;
        productSpecificationAttribute.DisplayOrder = displayOrder;

        if (isNew)
            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);
        else
            await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(productSpecificationAttribute);

        if (!metadata.LocalizedWorksheets.Any())
            return;

        foreach (var language in languages)
        {
            var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (lWorksheet == null)
                continue;

            specificationAttributeManager.CurrentLanguage = language;
            specificationAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

            customValue = specificationAttributeManager.GetLocalizedProperty("CustomValue").StringValue;
            await _localizedEntityService.SaveLocalizedValueAsync(productSpecificationAttribute, p => p.CustomValue, customValue, language.Id);
        }
		_logger.LogDebug("<<< Exiting ImportSpecificationAttributeAsync(metadata={Metadata},lastLoadedProduct={LastLoadedProduct},languages={Languages},iRow={IRow})", metadata, lastLoadedProduct, languages, iRow);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportTierPriceAsync(ImportProductMetadata metadata, Product lastLoadedProduct, IList<Language> languages, int iRow)
    {
        var tierPriceManager = metadata.TierPriceManager;

        if (!_catalogSettings.ExportImportTierPrices || lastLoadedProduct == null || tierPriceManager.IsCaption)
            return;

        var id = tierPriceManager.GetDefaultProperty("TierPriceId").IntValue;
        var storeId = tierPriceManager.GetDefaultProperty("Store").IntValue;
        var customerRoleId = tierPriceManager.GetDefaultProperty("CustomerRole").IntValue;
        var quantity = tierPriceManager.GetDefaultProperty("Quantity").IntValue;
        var price = tierPriceManager.GetDefaultProperty("Price").DecimalValue;
        var startDateTimeUtc = tierPriceManager.GetDefaultProperty("StartDateTimeUtc").DateTimeNullable;
        var endDateTimeUtc = tierPriceManager.GetDefaultProperty("EndDateTimeUtc").DateTimeNullable;

        if (id == 0)
        {
            var tierPrice = new TierPrice
            {
                CustomerRoleId = customerRoleId != 0 ? customerRoleId : null,
                EndDateTimeUtc = endDateTimeUtc,
                Price = price,
                ProductId = lastLoadedProduct.Id,
                Quantity = quantity,
                StartDateTimeUtc = startDateTimeUtc,
                StoreId = storeId
            };

            await _productService.InsertTierPriceAsync(tierPrice);
        }
        else
        {
            var tierPrice = await _productService.GetTierPriceByIdAsync(id);

            tierPrice.CustomerRoleId = customerRoleId != 0 ? customerRoleId : null;
            tierPrice.EndDateTimeUtc = endDateTimeUtc;
            tierPrice.Price = price;
            tierPrice.ProductId = lastLoadedProduct.Id;
            tierPrice.Quantity = quantity;
            tierPrice.StartDateTimeUtc = startDateTimeUtc;
            tierPrice.StoreId = storeId;

            await _productService.UpdateTierPriceAsync(tierPrice);
        }
		_logger.LogDebug("<<< Exiting ImportTierPriceAsync(metadata={Metadata},lastLoadedProduct={LastLoadedProduct},languages={Languages},iRow={IRow})", metadata, lastLoadedProduct, languages, iRow);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
    {
        if (string.IsNullOrEmpty(urlString))
            return string.Empty;

        if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
            return urlString;

        if (!_catalogSettings.ExportImportAllowDownloadImages)
            return string.Empty;

        //ensure that temp directory is created
        var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
        _fileProvider.CreateDirectory(tempDirectory);

        var fileName = _fileProvider.GetFileName(urlString);
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;

        var filePath = _fileProvider.Combine(tempDirectory, fileName);
        try
        {
            var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
            var fileData = await client.GetByteArrayAsync(urlString);
            await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                fs.Write(fileData, 0, fileData.Length);

            downloadedFiles?.Add(filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Download image failed", ex);
        }

        return string.Empty;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<ImportProductMetadata> PrepareImportProductDataAsync(IXLWorkbook workbook, IList<Language> languages)
    {
        //the columns
        var metadata = GetWorkbookMetadata<Product>(workbook, languages);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Product>(defaultProperties, _catalogSettings, localizedProperties, languages);

        var productAttributeProperties = new[]
        {
            new PropertyByName<ExportProductAttribute>("AttributeId"),
            new PropertyByName<ExportProductAttribute>("AttributeName"),
            new PropertyByName<ExportProductAttribute>("DefaultValue"),
            new PropertyByName<ExportProductAttribute>("ValidationMinLength"),
            new PropertyByName<ExportProductAttribute>("ValidationMaxLength"),
            new PropertyByName<ExportProductAttribute>("ValidationFileAllowedExtensions"),
            new PropertyByName<ExportProductAttribute>("ValidationFileMaximumSize"),
            new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
            new PropertyByName<ExportProductAttribute>("AttributeIsRequired"),
            new PropertyByName<ExportProductAttribute>("AttributeControlType"),
            new PropertyByName<ExportProductAttribute>("AttributeDisplayOrder"),
            new PropertyByName<ExportProductAttribute>("ProductAttributeValueId"),
            new PropertyByName<ExportProductAttribute>("ValueName"),
            new PropertyByName<ExportProductAttribute>("AttributeValueType"),
            new PropertyByName<ExportProductAttribute>("AssociatedProductId"),
            new PropertyByName<ExportProductAttribute>("ColorSquaresRgb"),
            new PropertyByName<ExportProductAttribute>("ImageSquaresPictureId"),
            new PropertyByName<ExportProductAttribute>("PriceAdjustment"),
            new PropertyByName<ExportProductAttribute>("PriceAdjustmentUsePercentage"),
            new PropertyByName<ExportProductAttribute>("WeightAdjustment"),
            new PropertyByName<ExportProductAttribute>("Cost"),
            new PropertyByName<ExportProductAttribute>("CustomerEntersQty"),
            new PropertyByName<ExportProductAttribute>("Quantity"),
            new PropertyByName<ExportProductAttribute>("IsPreSelected"),
            new PropertyByName<ExportProductAttribute>("DisplayOrder"),
            new PropertyByName<ExportProductAttribute>("PictureIds")
        };

        var productAttributeLocalizedProperties = new[]
        {
            new PropertyByName<ExportProductAttribute>("DefaultValue"),
            new PropertyByName<ExportProductAttribute>("AttributeTextPrompt"),
            new PropertyByName<ExportProductAttribute>("ValueName")
        };

        var productAttributeManager = new PropertyManager<ExportProductAttribute>(productAttributeProperties, _catalogSettings, productAttributeLocalizedProperties, languages);

        var specificationAttributeProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute>("AttributeType", (p, l) => p.AttributeTypeId),
            new PropertyByName<ExportSpecificationAttribute>("SpecificationAttribute", (p, l) => p.SpecificationAttributeId),
            new PropertyByName<ExportSpecificationAttribute>("CustomValue", (p, l) => p.CustomValue),
            new PropertyByName<ExportSpecificationAttribute>("SpecificationAttributeOptionId", (p, l) => p.SpecificationAttributeOptionId),
            new PropertyByName<ExportSpecificationAttribute>("AllowFiltering", (p, l) => p.AllowFiltering),
            new PropertyByName<ExportSpecificationAttribute>("ShowOnProductPage", (p, l) => p.ShowOnProductPage),
            new PropertyByName<ExportSpecificationAttribute>("DisplayOrder", (p, l) => p.DisplayOrder)
        };

        var specificationAttributeLocalizedProperties = new[]
        {
            new PropertyByName<ExportSpecificationAttribute>("CustomValue")
        };

        var specificationAttributeManager = new PropertyManager<ExportSpecificationAttribute>(specificationAttributeProperties, _catalogSettings, specificationAttributeLocalizedProperties, languages);

        var tierPriceProperties = new[]
        {
            new PropertyByName<ExportTierPrice>("TierPriceId"),
            new PropertyByName<ExportTierPrice>("Store"),
            new PropertyByName<ExportTierPrice>("CustomerRole"),
            new PropertyByName<ExportTierPrice>("Quantity"),
            new PropertyByName<ExportTierPrice>("Price"),
            new PropertyByName<ExportTierPrice>("StartDateTimeUtc"),
            new PropertyByName<ExportTierPrice>("EndDateTimeUtc")
        };

        var tierPriceManager = new PropertyManager<ExportTierPrice>(tierPriceProperties, _catalogSettings, languages: languages);

        var endRow = 2;
        var allCategories = new List<string>();
        var allSku = new List<string>();

        var tempProperty = manager.GetDefaultProperty("Categories");
        var categoryCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        tempProperty = manager.GetDefaultProperty("SKU");
        var skuCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        var allManufacturers = new List<string>();
        tempProperty = manager.GetDefaultProperty("Manufacturers");
        var manufacturerCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        var allStores = new List<string>();
        tempProperty = manager.GetDefaultProperty("LimitedToStores");
        var limitedToStoresCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
        {
            tierPriceManager.SetSelectList("Store", (await _storeService.GetAllStoresAsync()).ToSelectList(p => (p as Store)?.Name ?? string.Empty));
            tierPriceManager.SetSelectList("CustomerRole", (await _customerService.GetAllCustomerRolesAsync()).ToSelectList(p => (p as CustomerRole)?.Name ?? string.Empty));

            productAttributeManager.SetSelectList("AttributeControlType", await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false));
            productAttributeManager.SetSelectList("AttributeValueType", await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false));

            specificationAttributeManager.SetSelectList("AttributeType", await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false));
            specificationAttributeManager.SetSelectList("SpecificationAttribute", (await _specificationAttributeService
                    .GetAllSpecificationAttributesAsync())
                .Select(sa => sa as BaseEntity)
                .ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty));

            manager.SetSelectList("ProductType", await ProductType.SimpleProduct.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("DownloadActivationType",
                await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ManageInventoryMethod",
                await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("LowStockActivity",
                await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RecurringCyclePeriod",
                await RecurringProductCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

            manager.SetSelectList("Vendor",
                (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity)
                .ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
            manager.SetSelectList("ProductTemplate",
                (await _productTemplateService.GetAllProductTemplatesAsync()).Select(pt => pt as BaseEntity)
                .ToSelectList(p => (p as ProductTemplate)?.Name ?? string.Empty));
            manager.SetSelectList("DeliveryDate",
                (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity)
                .ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
            manager.SetSelectList("ProductAvailabilityRange",
                (await _dateRangeService.GetAllProductAvailabilityRangesAsync()).Select(range => range as BaseEntity)
                .ToSelectList(p => (p as ProductAvailabilityRange)?.Name ?? string.Empty));
            manager.SetSelectList("TaxCategory",
                (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity)
                .ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceUnit",
                (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            manager.SetSelectList("BasepriceBaseUnit",
                (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
        }

        var allAttributeIds = new List<int>();
        var allSpecificationAttributeOptionIds = new List<int>();

        var attributeIdCellNum = 1 + ExportImportDefaults.ProductAdditionalInfoCellOffset;
        var specificationAttributeOptionIdCellNum =
            specificationAttributeManager.GetIndex("SpecificationAttributeOptionId") +
            ExportImportDefaults.ProductAdditionalInfoCellOffset;

        var productsInFile = new List<int>();

        //find end of data
        var typeOfExportedAttribute = ExportedAdditionalProductInfoType.NotSpecified;
        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            if (new[] { 1, 2 }.Select(cellNum => defaultWorksheet.Row(endRow).Cell(cellNum))
                    .All(cell => string.IsNullOrEmpty(cell?.Value.ToString())) &&
                defaultWorksheet.Row(endRow).OutlineLevel == 0)
            {
                var cellValue = defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value;
                await SetOutLineForProductAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                await SetOutLineForSpecificationAttributeRowAsync(cellValue, defaultWorksheet, endRow);
            }

            if (defaultWorksheet.Row(endRow).OutlineLevel != 0)
            {
                var newTypeOfExportedAttribute = GetTypeOfExportedAdditionalProductInfo(defaultWorksheet, metadata.LocalizedWorksheets, productAttributeManager, specificationAttributeManager, tierPriceManager, endRow);

                //skip caption row
                if (newTypeOfExportedAttribute != ExportedAdditionalProductInfoType.NotSpecified && newTypeOfExportedAttribute != typeOfExportedAttribute)
                {
                    typeOfExportedAttribute = newTypeOfExportedAttribute;
                    endRow++;
                    continue;
                }

                switch (typeOfExportedAttribute)
                {
                    case ExportedAdditionalProductInfoType.ProductAttribute:
                        productAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow,
                            ExportImportDefaults.ProductAdditionalInfoCellOffset);
                        if (int.TryParse(defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value.ToString(), out var aid))
                            allAttributeIds.Add(aid);

                        break;
                    case ExportedAdditionalProductInfoType.SpecificationAttribute:
                        specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow, ExportImportDefaults.ProductAdditionalInfoCellOffset);

                        if (int.TryParse(defaultWorksheet.Row(endRow).Cell(specificationAttributeOptionIdCellNum).Value.ToString(), out var saoid))
                            allSpecificationAttributeOptionIds.Add(saoid);

                        break;
                }

                endRow++;
                continue;
            }

            if (categoryCellNum > 0)
            {
                var categoryIds = defaultWorksheet.Row(endRow).Cell(categoryCellNum).Value.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(categoryIds))
                    allCategories.AddRange(categoryIds
                        .Split(new[] { ";", ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                        .Distinct());
            }

            if (skuCellNum > 0)
            {
                var sku = defaultWorksheet.Row(endRow).Cell(skuCellNum).Value.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(sku))
                    allSku.Add(sku);
            }

            if (manufacturerCellNum > 0)
            {
                var manufacturerIds = defaultWorksheet.Row(endRow).Cell(manufacturerCellNum).Value.ToString() ??
                                      string.Empty;
                if (!string.IsNullOrEmpty(manufacturerIds))
                    allManufacturers.AddRange(manufacturerIds
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            }

            if (limitedToStoresCellNum > 0)
            {
                var storeIds = defaultWorksheet.Row(endRow).Cell(limitedToStoresCellNum).Value.ToString() ??
                               string.Empty;
                if (!string.IsNullOrEmpty(storeIds))
                    allStores.AddRange(storeIds
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            }

            //counting the number of products
            productsInFile.Add(endRow);

            endRow++;
        }

        //performance optimization, the check for the existence of the categories in one SQL request
        var notExistingCategories = await _categoryService.GetNotExistingCategoriesAsync(allCategories.ToArray());
        if (notExistingCategories.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesDontExist"), string.Join(", ", notExistingCategories)));
        }

        //performance optimization, the check for the existence of the manufacturers in one SQL request
        var notExistingManufacturers = await _manufacturerService.GetNotExistingManufacturersAsync(allManufacturers.ToArray());
        if (notExistingManufacturers.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ManufacturersDontExist"), string.Join(", ", notExistingManufacturers)));
        }

        //performance optimization, the check for the existence of the product attributes in one SQL request
        var notExistingProductAttributes = await _productAttributeService.GetNotExistingAttributesAsync(allAttributeIds.ToArray());
        if (notExistingProductAttributes.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.ProductAttributesDontExist"), string.Join(", ", notExistingProductAttributes)));
        }

        //performance optimization, the check for the existence of the specification attribute options in one SQL request
        var notExistingSpecificationAttributeOptions = await _specificationAttributeService.GetNotExistingSpecificationAttributeOptionsAsync(allSpecificationAttributeOptionIds.Where(saoId => saoId != 0).ToArray());
        if (notExistingSpecificationAttributeOptions.Any())
        {
            throw new ArgumentException($"The following specification attribute option ID(s) don't exist - {string.Join(", ", notExistingSpecificationAttributeOptions)}");
        }

        //performance optimization, the check for the existence of the stores in one SQL request
        var notExistingStores = await _storeService.GetNotExistingStoresAsync(allStores.ToArray());
        if (notExistingStores.Any())
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.StoresDontExist"), string.Join(", ", notExistingStores)));

        return new ImportProductMetadata
        {
            EndRow = endRow,
            Manager = manager,
            Properties = defaultProperties,
            ProductsInFile = productsInFile,
            ProductAttributeManager = productAttributeManager,
            DefaultWorksheet = defaultWorksheet,
            LocalizedWorksheets = metadata.LocalizedWorksheets,
            SpecificationAttributeManager = specificationAttributeManager,
            TierPriceManager = tierPriceManager,
            SkuCellNum = skuCellNum,
            AllSku = allSku
        };
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductsFromSplitedXlsxAsync(IXLWorksheet worksheet, ImportProductMetadata metadata)
    {
        foreach (var path in SplitProductFile(worksheet, metadata))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            // Resolve
            var importManager = EngineContext.Current.Resolve<IImportManager>(scope);

            using var sr = new StreamReader(path);
            await importManager.ImportProductsFromXlsxAsync(sr.BaseStream);

            try
            {
                _fileProvider.DeleteFile(path);
            }
            catch
            {
                // ignored
            }
        }
    }

    protected virtual IList<string> SplitProductFile(IXLWorksheet worksheet, ImportProductMetadata metadata)
    {
        var fileIndex = 1;
        var fileName = Guid.NewGuid().ToString();
        var endCell = metadata.Properties.Max(p => p.PropertyOrderPosition);

        var filePaths = new List<string>();

        while (true)
        {
            var curIndex = fileIndex * _catalogSettings.ExportImportProductsCountInOneFile;

            var startRow = metadata.ProductsInFile[(fileIndex - 1) * _catalogSettings.ExportImportProductsCountInOneFile];

            var endRow = metadata.CountProductsInFile > curIndex
                ? metadata.ProductsInFile[curIndex - 1]
                : metadata.EndRow;

            var filePath = $"{_fileProvider.MapPath(ExportImportDefaults.UploadsTempPath)}/{fileName}_part_{fileIndex}.xlsx";

            CopyDataToNewFile(metadata, worksheet, filePath, startRow, endRow, endCell);

            filePaths.Add(filePath);
            fileIndex += 1;

            if (endRow == metadata.EndRow)
                break;
        }

        return filePaths;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<(ImportOrderMetadata, IXLWorksheet)> PrepareImportOrderDataAsync(IXLWorkbook workbook)
    {
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

        //the columns
        var metadata = GetWorkbookMetadata<Order>(workbook, languages);
        var worksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;

        var manager = new PropertyManager<Order>(defaultProperties, _catalogSettings);

        var orderItemProperties = new[]
        {
            new PropertyByName<OrderItem>("OrderItemGuid"),
            new PropertyByName<OrderItem>("Name"),
            new PropertyByName<OrderItem>("Sku"),
            new PropertyByName<OrderItem>("PriceExclTax"),
            new PropertyByName<OrderItem>("PriceInclTax"),
            new PropertyByName<OrderItem>("Quantity"),
            new PropertyByName<OrderItem>("DiscountExclTax"),
            new PropertyByName<OrderItem>("DiscountInclTax"),
            new PropertyByName<OrderItem>("TotalExclTax"),
            new PropertyByName<OrderItem>("TotalInclTax")
        };

        var orderItemManager = new PropertyManager<OrderItem>(orderItemProperties, _catalogSettings);

        var endRow = 2;
        var allOrderGuids = new List<Guid>();

        var tempProperty = manager.GetDefaultProperty("OrderGuid");
        var orderGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        tempProperty = manager.GetDefaultProperty("CustomerGuid");
        var customerGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

        manager.SetSelectList("OrderStatus", await OrderStatus.Cancelled.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("ShippingStatus", await ShippingStatus.Delivered.ToSelectListAsync(useLocalization: false));
        manager.SetSelectList("PaymentStatus", await PaymentStatus.Authorized.ToSelectListAsync(useLocalization: false));

        var allCustomerGuids = new List<Guid>();

        var allOrderItemSkus = new List<string>();

        var countOrdersInFile = 0;

        //find end of data
        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => worksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            if (worksheet.Row(endRow).OutlineLevel != 0)
            {
                orderItemManager.ReadDefaultFromXlsx(worksheet, endRow, 2);

                //skip caption row
                if (!orderItemManager.IsCaption)
                {
                    allOrderItemSkus.Add(orderItemManager.GetDefaultProperty("Sku").StringValue);
                }

                endRow++;
                continue;
            }

            if (orderGuidCellNum > 0)
            {
                var orderGuidString = worksheet.Row(endRow).Cell(orderGuidCellNum).Value.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(orderGuidString) && Guid.TryParse(orderGuidString, out var orderGuid))
                    allOrderGuids.Add(orderGuid);
            }

            if (customerGuidCellNum > 0)
            {
                var customerGuidString = worksheet.Row(endRow).Cell(customerGuidCellNum).Value.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(customerGuidString) && Guid.TryParse(customerGuidString, out var customerGuid))
                    allCustomerGuids.Add(customerGuid);
            }

            //counting the number of orders
            countOrdersInFile++;

            endRow++;
        }

        //performance optimization, the check for the existence of the customers in one SQL request
        var notExistingCustomerGuids = await _customerService.GetNotExistingCustomersAsync(allCustomerGuids.ToArray());
        if (notExistingCustomerGuids.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.CustomersDontExist"), string.Join(", ", notExistingCustomerGuids)));
        }

        //performance optimization, the check for the existence of the order items in one SQL request
        var notExistingProductSkus = await _productService.GetNotExistingProductsAsync(allOrderItemSkus.ToArray());
        if (notExistingProductSkus.Any())
        {
            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.ProductsDontExist"), string.Join(", ", notExistingProductSkus)));
        }

        return (new ImportOrderMetadata
        {
            EndRow = endRow,
            Manager = manager,
            Properties = defaultProperties,
            CountOrdersInFile = countOrdersInFile,
            OrderItemManager = orderItemManager,
            AllOrderGuids = allOrderGuids,
            AllCustomerGuids = allCustomerGuids
        }, worksheet);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportOrderItemAsync(PropertyManager<OrderItem> orderItemManager, Order lastLoadedOrder)
    {
        if (lastLoadedOrder == null || orderItemManager.IsCaption)
            return;

        var orderItemGuid = Guid.TryParse(orderItemManager.GetDefaultProperty("OrderItemGuid").StringValue, out var guidValue) ? guidValue : Guid.NewGuid();
        var sku = orderItemManager.GetDefaultProperty("Sku").StringValue;
        var priceExclTax = orderItemManager.GetDefaultProperty("PriceExclTax").DecimalValue;
        var priceInclTax = orderItemManager.GetDefaultProperty("PriceInclTax").DecimalValue;
        var quantity = orderItemManager.GetDefaultProperty("Quantity").IntValue;
        var discountExclTax = orderItemManager.GetDefaultProperty("DiscountExclTax").DecimalValue;
        var discountInclTax = orderItemManager.GetDefaultProperty("DiscountInclTax").DecimalValue;
        var totalExclTax = orderItemManager.GetDefaultProperty("TotalExclTax").DecimalValue;
        var totalInclTax = orderItemManager.GetDefaultProperty("TotalInclTax").DecimalValue;

        var orderItemProduct = await _productService.GetProductBySkuAsync(sku);
        var orderItem = (await _orderService.GetOrderItemsAsync(lastLoadedOrder.Id)).FirstOrDefault(f => f.OrderItemGuid == orderItemGuid);

        if (orderItem == null)
        {
            //insert order item
            orderItem = new OrderItem
            {
                DiscountAmountExclTax = discountExclTax,
                DiscountAmountInclTax = discountInclTax,
                OrderId = lastLoadedOrder.Id,
                OrderItemGuid = orderItemGuid,
                PriceExclTax = totalExclTax,
                PriceInclTax = totalInclTax,
                ProductId = orderItemProduct.Id,
                Quantity = quantity,
                OriginalProductCost = orderItemProduct.ProductCost,
                UnitPriceExclTax = priceExclTax,
                UnitPriceInclTax = priceInclTax
            };
            await _orderService.InsertOrderItemAsync(orderItem);
        }
        else
        {
            //update order item
            orderItem.DiscountAmountExclTax = discountExclTax;
            orderItem.DiscountAmountInclTax = discountInclTax;
            orderItem.OrderId = lastLoadedOrder.Id;
            orderItem.PriceExclTax = totalExclTax;
            orderItem.PriceInclTax = totalInclTax;
            orderItem.Quantity = quantity;
            orderItem.UnitPriceExclTax = priceExclTax;
            orderItem.UnitPriceInclTax = priceInclTax;
            await _orderService.UpdateOrderItemAsync(orderItem);
        }
		_logger.LogDebug("<<< Exiting ImportOrderItemAsync(orderItemManager={OrderItemManager},lastLoadedOrder={LastLoadedOrder})", orderItemManager, lastLoadedOrder);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ImportProductLocalizedAsync(Product product, ImportProductMetadata metadata, int iRow, IList<Language> languages)
    {
        if (metadata.LocalizedWorksheets.Any())
        {
            var manager = metadata.Manager;
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.Name, localizedName, language.Id);
                            break;
                        case "ShortDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.ShortDescription, property.StringValue, language.Id);
                            break;
                        case "FullDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.FullDescription, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(product, p => p.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            var localizedSeName = await _urlRecordService.ValidateSeNameAsync(product, property.StringValue, localizedName, false);
                            await _urlRecordService.SaveSlugAsync(product, localizedSeName, language.Id);
                            break;
                    }
                }
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get excel workbook metadata
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="workbook">Excel workbook</param>
    /// <param name="languages">Languages</param>
    /// <returns>Workbook metadata</returns>
    public static WorkbookMetadata<T> GetWorkbookMetadata<T>(IXLWorkbook workbook, IList<Language> languages)
    {
		_logger.LogDebug(">>> Entering GetWorkbookMetadata(workbook={Workbook},languages={Languages})", workbook, languages);
        // get the first worksheet in the workbook
        var worksheet = workbook.Worksheets.FirstOrDefault()
                        ?? throw new NopException("No worksheet found");

        var properties = new List<PropertyByName<T>>();
		_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): properties → {Properties}", workbook, languages, properties);
        var localizedProperties = new List<PropertyByName<T>>();
		_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): localizedProperties → {LocalizedProperties}", workbook, languages, localizedProperties);
        var localizedWorksheets = new List<IXLWorksheet>();
		_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): localizedWorksheets → {LocalizedWorksheets}", workbook, languages, localizedWorksheets);

        var poz = 1;
		_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): poz → {Poz}", workbook, languages, poz);
        while (true)
        {
            try
            {
                var cell = worksheet.Row(1).Cell(poz);
				_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): cell → {Cell}", workbook, languages, cell);

                if (string.IsNullOrEmpty(cell?.Value.ToString()))
                    break;

                poz += 1;
                properties.Add(new PropertyByName<T>(cell.Value.ToString()));
            }
            catch
            {
				_logger.LogError("Exception in GetWorkbookMetadata(workbook={Workbook},languages={Languages}): {Arg2}", workbook, languages, ex.Message);
                break;
            }
        }

        foreach (var ws in workbook.Worksheets.Skip(1))
            if (languages.Any(l => l.UniqueSeoCode.Equals(ws.Name, StringComparison.InvariantCultureIgnoreCase)))
                localizedWorksheets.Add(ws);

        if (localizedWorksheets.Any())
        {
            // get the first worksheet in the workbook
            var localizedWorksheet = localizedWorksheets.First();
			_logger.LogDebug("GetWorkbookMetadata(workbook={Workbook},languages={Languages}): localizedWorksheet → {LocalizedWorksheet}", workbook, languages, localizedWorksheet);

            poz = 1;
            while (true)
            {
                try
                {
                    var cell = localizedWorksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value.ToString()))
                        break;

                    poz += 1;
                    localizedProperties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
					_logger.LogError("Exception in GetWorkbookMetadata(workbook={Workbook},languages={Languages}): {Arg2}", workbook, languages, ex.Message);
                    break;
                }
            }
        }

		_logger.LogDebug("<<< Exiting GetWorkbookMetadata(workbook={Workbook},languages={Languages})", workbook, languages);
        return new WorkbookMetadata<T>
        {
            DefaultProperties = properties,
            LocalizedProperties = localizedProperties,
            DefaultWorksheet = worksheet,
            LocalizedWorksheets = localizedWorksheets
        };
    }

    /// <summary>
    /// Import customers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportCustomersFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportCustomersFromXlsxAsync(stream={Stream})", stream);
        using var workbook = new XLWorkbook(stream);

		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): languages → {Languages}", stream, languages);

        //the columns
        var metadata = GetWorkbookMetadata<Customer>(workbook, languages);
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): metadata → {Metadata}", stream, metadata);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;

        var manager = new PropertyManager<Customer>(defaultProperties, _catalogSettings);
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): manager → {Manager}", stream, manager);

        if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
        {
            manager.SetSelectList("VatNumberStatus",
                await VatNumberStatus.Unknown.ToSelectListAsync(useLocalization: false));
        }

        var iRow = 2;
        var allRoles = await _customerService.GetAllCustomerRolesAsync();
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): allRoles → {AllRoles}", stream, allRoles);
        var countries = await _countryService.GetAllCountriesAsync();
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): countries → {Countries}", stream, countries);
        var states = await _stateProvinceService.GetStateProvincesAsync();
		_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): states → {States}", stream, states);

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var customerGuid = manager.GetDefaultProperty("CustomerGuid").GuidValue;
			_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customerGuid → {CustomerGuid}", stream, customerGuid);
            var customer = await _customerService.GetCustomerByGuidAsync(customerGuid) ??
                           await _customerService.GetCustomerByEmailAsync(manager.GetDefaultProperty("Email").StringValue);

            int? avatarPictureId = null;
            string signature = null;
			_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): signature → {Signature}", stream, signature);
            string password = null;
			_logger.LogWarning("ImportCustomersFromXlsxAsync(stream={Stream}): password → {Password}", stream, password);
            string passwordSalt = null;
			_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): passwordSalt → {PasswordSalt}", stream, passwordSalt);

            var isNew = customer == null;

            if (isNew)
                customer = new Customer
                {
                    CustomerGuid = Guid.Empty.Equals(customerGuid) ? Guid.NewGuid() : customerGuid,
                    CreatedOnUtc = DateTime.UtcNow
                };

            var rolesToSave = new List<int>();
			_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): rolesToSave → {RolesToSave}", stream, rolesToSave);

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Email":
                        customer.Email = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Email → {Arg1}", stream, customer.Email);
                        break;
                    case "Username":
                        customer.Username = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Username → {Arg1}", stream, customer.Username);
                        break;
                    case "IsTaxExempt":
                        customer.IsTaxExempt = property.BooleanValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.IsTaxExempt → {Arg1}", stream, customer.IsTaxExempt);
                        break;
                    case "AffiliateId":
                        customer.AffiliateId = property.IntValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.AffiliateId → {Arg1}", stream, customer.AffiliateId);
                        break;
                    case "Vendor":
                        if (!string.IsNullOrEmpty(property.StringValue))
                            if (int.TryParse(property.StringValue, out var vendorId))
                                customer.VendorId = vendorId;
								_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.VendorId → {Arg1}", stream, customer.VendorId);
                            else
                            {
                                var vendors = await _vendorService.GetAllVendorsAsync(property.StringValue);
								_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): vendors → {Vendors}", stream, vendors);
                                customer.VendorId = vendors.FirstOrDefault()?.Id ?? 0;
                            }
                        else
                            customer.VendorId = 0;
                        break;
                    case "Active":
                        customer.Active = property.BooleanValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Active → {Arg1}", stream, customer.Active);
                        break;
                    case "CustomerRoles":
                        var roles = property.StringValue.Split(", ");
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): roles → {Roles}", stream, roles);

                        foreach (var role in roles)
                            if (int.TryParse(role, out var roleId))
                                rolesToSave.Add(roleId);
                            else
                            {
                                var currentRole = allRoles.FirstOrDefault(r =>
                                    r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));

                                if (currentRole != null)
                                    rolesToSave.Add(currentRole.Id);
                            }
                        break;
                    case "CreatedOnUtc":
                        if (DateTime.TryParse(property.StringValue, out var date))
                            customer.CreatedOnUtc = date;
							_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.CreatedOnUtc → {Arg1}", stream, customer.CreatedOnUtc);
                        break;
                    case "FirstName":
                        customer.FirstName = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.FirstName → {Arg1}", stream, customer.FirstName);
                        break;
                    case "LastName":
                        customer.LastName = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.LastName → {Arg1}", stream, customer.LastName);
                        break;
                    case "Gender":
                        customer.Gender = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Gender → {Arg1}", stream, customer.Gender);
                        break;
                    case "Company":
                        customer.Company = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Company → {Arg1}", stream, customer.Company);
                        break;
                    case "StreetAddress":
                        customer.StreetAddress = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.StreetAddress → {Arg1}", stream, customer.StreetAddress);
                        break;
                    case "StreetAddress2":
                        customer.StreetAddress2 = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.StreetAddress2 → {Arg1}", stream, customer.StreetAddress2);
                        break;
                    case "ZipPostalCode":
                        customer.ZipPostalCode = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.ZipPostalCode → {Arg1}", stream, customer.ZipPostalCode);
                        break;
                    case "City":
                        customer.City = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.City → {Arg1}", stream, customer.City);
                        break;
                    case "County":
                        customer.County = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.County → {Arg1}", stream, customer.County);
                        break;
                    case "Country":
                        if (int.TryParse(property.StringValue, out var countryId))
                            customer.CountryId = countryId;
							_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.CountryId → {Arg1}", stream, customer.CountryId);
                        else
                        {
                            var country = countries.FirstOrDefault(c =>
                                c.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (country != null)
                                customer.CountryId = country.Id;
                        }
                        break;
                    case "StateProvince":
                        if (int.TryParse(property.StringValue, out var stateId))
                            customer.StateProvinceId = stateId;
							_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.StateProvinceId → {Arg1}", stream, customer.StateProvinceId);
                        else
                        {
                            var state = states.FirstOrDefault(s =>
                                s.Name.Equals(property.StringValue, StringComparison.InvariantCultureIgnoreCase));

                            if (state != null)
                                customer.StateProvinceId = state.Id;
                        }
                        break;
                    case "Phone":
                        customer.Phone = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Phone → {Arg1}", stream, customer.Phone);
                        break;
                    case "Fax":
                        customer.Fax = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.Fax → {Arg1}", stream, customer.Fax);
                        break;
                    case "VatNumber":
                        customer.VatNumber = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.VatNumber → {Arg1}", stream, customer.VatNumber);
                        break;
                    case "VatNumberStatus":
                        customer.VatNumberStatusId = property.IntValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.VatNumberStatusId → {Arg1}", stream, customer.VatNumberStatusId);
                        break;
                    case "TimeZone":
                        customer.TimeZoneId = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.TimeZoneId → {Arg1}", stream, customer.TimeZoneId);
                        break;
                    case "AvatarPictureId":
                        avatarPictureId = property.IntValueNullable;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): avatarPictureId → {AvatarPictureId}", stream, avatarPictureId);
                        break;
                    case "Signature":
                        signature = property.StringValue;
                        break;
                    case "CustomCustomerAttributesXML":
                        customer.CustomCustomerAttributesXML = property.StringValue;
						_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customer.CustomCustomerAttributesXML → {Arg1}", stream, customer.CustomCustomerAttributesXML);
                        break;
                    case "Password":
                        password = property.StringValue;
                        break;
                    case "PasswordSalt":
                        passwordSalt = property.StringValue;
                        break;
                }
            }

            if (isNew)
                await _customerService.InsertCustomerAsync(customer);
            else
                await _customerService.UpdateCustomerAsync(customer);

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
			_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): customerRoles → {CustomerRoles}", stream, customerRoles);

            foreach (var roleId in rolesToSave)
            {
                var role = allRoles.FirstOrDefault(r => r.Id == roleId);
				_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): role → {Role}", stream, role);

                if (role == null || customerRoles.Any(cr => cr.Id == roleId))
                    continue;

                await _customerService.AddCustomerRoleMappingAsync(
                    new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = roleId });
            }

            if (!isNew && rolesToSave.Any())
                foreach (var customerRole in customerRoles.Where(cr => !rolesToSave.Contains(cr.Id)).ToList())
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);

            if (avatarPictureId.HasValue)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.AvatarPictureIdAttribute, avatarPictureId.Value);

            if (!string.IsNullOrEmpty(signature))
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SignatureAttribute,
                    signature);

            if (_securitySettings.AllowStoreOwnerExportImportCustomersWithHashedPassword &&
                !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(passwordSalt))
            {
                var lastPassword = isNew ? null : await _customerService.GetCurrentPasswordAsync(customer.Id);
				_logger.LogDebug("ImportCustomersFromXlsxAsync(stream={Stream}): lastPassword → {LastPassword}", stream, lastPassword);

                if (lastPassword == null || !(lastPassword.Password.Equals(password) && lastPassword.PasswordSalt.Equals(passwordSalt)))
                    await _customerService.InsertCustomerPasswordAsync(new CustomerPassword
                    {
                        CustomerId = customer.Id,
                        Password = password,
                        PasswordSalt = passwordSalt,
                        PasswordFormat = PasswordFormat.Hashed,
                        CreatedOnUtc = DateTime.UtcNow
                    });
            }

            iRow++;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportCustomers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCustomers"), iRow - 2));
			_logger.LogDebug("<<< Exiting ImportCustomersFromXlsxAsync(stream={Stream})", stream);
    }

    /// <summary>
    /// Import products from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportProductsFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportProductsFromXlsxAsync(stream={Stream})", stream);
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): languages → {Languages}", stream, languages);
        using var workbook = new XLWorkbook(stream);
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);
        var downloadedFiles = new List<string>();
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): downloadedFiles → {DownloadedFiles}", stream, downloadedFiles);

        var metadata = await PrepareImportProductDataAsync(workbook, languages);
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): metadata → {Metadata}", stream, metadata);
        var defaultWorksheet = metadata.DefaultWorksheet;

        if (_catalogSettings.ExportImportSplitProductsFile && metadata.CountProductsInFile > _catalogSettings.ExportImportProductsCountInOneFile)
        {
            await ImportProductsFromSplitedXlsxAsync(defaultWorksheet, metadata);
            return;
        }

        //performance optimization, load all products by SKU in one SQL request
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): currentVendor → {CurrentVendor}", stream, currentVendor);
        var allProductsBySku = await _productService.GetProductsBySkuAsync(metadata.AllSku.ToArray(), currentVendor?.Id ?? 0);
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allProductsBySku → {AllProductsBySku}", stream, allProductsBySku);

        //validate maximum number of products per vendor
        if (_vendorSettings.MaximumProductNumber > 0 &&
            currentVendor != null)
        {
            var newProductsCount = metadata.CountProductsInFile - allProductsBySku.Count;
            if (await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) + newProductsCount > _vendorSettings.MaximumProductNumber)
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"), _vendorSettings.MaximumProductNumber));
        }

        //performance optimization, load all categories IDs for products in one SQL request
        var allProductsCategoryIds = await _categoryService.GetProductCategoryIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allProductsCategoryIds → {AllProductsCategoryIds}", stream, allProductsCategoryIds);

        //performance optimization, load all categories in one SQL request
        Dictionary<CategoryKey, Category> allCategories = new();
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allCategories → {AllCategories}", stream, allCategories);
        try
        {
            var allCategoryList = await _categoryService.GetAllCategoriesAsync(showHidden: true);
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allCategoryList → {AllCategoryList}", stream, allCategoryList);

            allCategories = await allCategoryList
                .WhereAwait(async c => await _categoryService.CanVendorAddProductsAsync(c, allCategoryList))
                .ToDictionaryAwaitAsync(async c =>
                {
                    var keyName = await _categoryService.GetFormattedBreadCrumbAsync(c, allCategoryList);
					_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): keyName → {KeyName}", stream, keyName);
                    return new CategoryKey(keyName, c, c.LimitedToStores ? (await _storeMappingService.GetStoresIdsWithAccessAsync(c)).ToList() : new List<int>());
                });
        }
        catch (ArgumentException)
        {
            //categories with the same name are not supported in the same category level
            throw new ArgumentException(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.CategoriesWithSameNameNotSupported"));
        }

        //performance optimization, load all manufacturers IDs for products in one SQL request
        var allProductsManufacturerIds = await _manufacturerService.GetProductManufacturerIdsAsync(allProductsBySku.Select(p => p.Id).ToArray());
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allProductsManufacturerIds → {AllProductsManufacturerIds}", stream, allProductsManufacturerIds);

        //performance optimization, load all manufacturers in one SQL request
        var allManufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allManufacturers → {AllManufacturers}", stream, allManufacturers);

        //performance optimization, load all stores in one SQL request
        var allStores = await _storeService.GetAllStoresAsync();
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): allStores → {AllStores}", stream, allStores);

        //product to import images
        var productPictureMetadata = new List<ProductPictureMetadata>();
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): productPictureMetadata → {ProductPictureMetadata}", stream, productPictureMetadata);

        Product lastLoadedProduct = null;
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): lastLoadedProduct → {LastLoadedProduct}", stream, lastLoadedProduct);
        var typeOfExportedAttribute = ExportedAdditionalProductInfoType.NotSpecified;
		_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): typeOfExportedAttribute → {TypeOfExportedAttribute}", stream, typeOfExportedAttribute);

        for (var iRow = 2; iRow < metadata.EndRow; iRow++)
        {
            if (defaultWorksheet.Row(iRow).OutlineLevel != 0)
            {
                if (lastLoadedProduct == null)
                    continue;

                var newTypeOfExportedAttribute = GetTypeOfExportedAdditionalProductInfo(defaultWorksheet, metadata.LocalizedWorksheets, metadata.ProductAttributeManager, metadata.SpecificationAttributeManager, metadata.TierPriceManager, iRow);
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): newTypeOfExportedAttribute → {NewTypeOfExportedAttribute}", stream, newTypeOfExportedAttribute);

                //skip caption row
                if (newTypeOfExportedAttribute != ExportedAdditionalProductInfoType.NotSpecified &&
                    newTypeOfExportedAttribute != typeOfExportedAttribute)
                {
                    typeOfExportedAttribute = newTypeOfExportedAttribute;
                    continue;
                }

                switch (typeOfExportedAttribute)
                {
                    case ExportedAdditionalProductInfoType.ProductAttribute:
                        await ImportProductAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                        break;
                    case ExportedAdditionalProductInfoType.SpecificationAttribute:
                        await ImportSpecificationAttributeAsync(metadata, lastLoadedProduct, languages, iRow);
                        break;
                    case ExportedAdditionalProductInfoType.TierPrices:
                        await ImportTierPriceAsync(metadata, lastLoadedProduct, languages, iRow);
                        break;
                    case ExportedAdditionalProductInfoType.NotSpecified:
                    default:
                        continue;
                }

                continue;
            }

            metadata.Manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var product = metadata.SkuCellNum > 0 ? allProductsBySku.FirstOrDefault(p => p.Sku == metadata.Manager.GetDefaultProperty("SKU").StringValue) : null;
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product → {Product}", stream, product);

            var isNew = product == null;

            product ??= new Product();

            //some of previous values
            var previousStockQuantity = product.StockQuantity;
            var previousWarehouseId = product.WarehouseId;
            var prevTotalStockQuantity = await _productService.GetTotalStockQuantityAsync(product);
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): prevTotalStockQuantity → {PrevTotalStockQuantity}", stream, prevTotalStockQuantity);

            if (isNew)
                product.CreatedOnUtc = DateTime.UtcNow;
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.CreatedOnUtc → {Arg1}", stream, product.CreatedOnUtc);

            foreach (var property in metadata.Manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "ProductType":
                        product.ProductTypeId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ProductTypeId → {Arg1}", stream, product.ProductTypeId);
                        break;
                    case "ParentGroupedProductId":
                        product.ParentGroupedProductId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ParentGroupedProductId → {Arg1}", stream, product.ParentGroupedProductId);
                        break;
                    case "VisibleIndividually":
                        product.VisibleIndividually = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.VisibleIndividually → {Arg1}", stream, product.VisibleIndividually);
                        break;
                    case "Name":
                        product.Name = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Name → {Arg1}", stream, product.Name);
                        break;
                    case "ShortDescription":
                        product.ShortDescription = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ShortDescription → {Arg1}", stream, product.ShortDescription);
                        break;
                    case "FullDescription":
                        product.FullDescription = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.FullDescription → {Arg1}", stream, product.FullDescription);
                        break;
                    case "Vendor":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.VendorId = property.IntValue;
							_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.VendorId → {Arg1}", stream, product.VendorId);
                        break;
                    case "ProductTemplate":
                        product.ProductTemplateId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ProductTemplateId → {Arg1}", stream, product.ProductTemplateId);
                        break;
                    case "ShowOnHomepage":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.ShowOnHomepage = property.BooleanValue;
							_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ShowOnHomepage → {Arg1}", stream, product.ShowOnHomepage);
                        break;
                    case "DisplayOrder":
                        //vendor can't change this field
                        if (currentVendor == null)
                            product.DisplayOrder = property.IntValue;
							_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisplayOrder → {Arg1}", stream, product.DisplayOrder);
                        break;
                    case "MetaKeywords":
                        product.MetaKeywords = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MetaKeywords → {Arg1}", stream, product.MetaKeywords);
                        break;
                    case "MetaDescription":
                        product.MetaDescription = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MetaDescription → {Arg1}", stream, product.MetaDescription);
                        break;
                    case "MetaTitle":
                        product.MetaTitle = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MetaTitle → {Arg1}", stream, product.MetaTitle);
                        break;
                    case "AllowCustomerReviews":
                        product.AllowCustomerReviews = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AllowCustomerReviews → {Arg1}", stream, product.AllowCustomerReviews);
                        break;
                    case "Published":
                        product.Published = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Published → {Arg1}", stream, product.Published);
                        break;
                    case "SKU":
                        product.Sku = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Sku → {Arg1}", stream, product.Sku);
                        break;
                    case "ManufacturerPartNumber":
                        product.ManufacturerPartNumber = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ManufacturerPartNumber → {Arg1}", stream, product.ManufacturerPartNumber);
                        break;
                    case "Gtin":
                        product.Gtin = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Gtin → {Arg1}", stream, product.Gtin);
                        break;
                    case "IsGiftCard":
                        product.IsGiftCard = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsGiftCard → {Arg1}", stream, product.IsGiftCard);
                        break;
                    case "GiftCardType":
                        product.GiftCardTypeId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.GiftCardTypeId → {Arg1}", stream, product.GiftCardTypeId);
                        break;
                    case "OverriddenGiftCardAmount":
                        product.OverriddenGiftCardAmount = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.OverriddenGiftCardAmount → {Arg1}", stream, product.OverriddenGiftCardAmount);
                        break;
                    case "RequireOtherProducts":
                        product.RequireOtherProducts = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RequireOtherProducts → {Arg1}", stream, product.RequireOtherProducts);
                        break;
                    case "RequiredProductIds":
                        product.RequiredProductIds = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RequiredProductIds → {Arg1}", stream, product.RequiredProductIds);
                        break;
                    case "AutomaticallyAddRequiredProducts":
                        product.AutomaticallyAddRequiredProducts = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AutomaticallyAddRequiredProducts → {Arg1}", stream, product.AutomaticallyAddRequiredProducts);
                        break;
                    case "IsDownload":
                        product.IsDownload = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsDownload → {Arg1}", stream, product.IsDownload);
                        break;
                    case "DownloadId":
                        product.DownloadId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DownloadId → {Arg1}", stream, product.DownloadId);
                        break;
                    case "UnlimitedDownloads":
                        product.UnlimitedDownloads = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.UnlimitedDownloads → {Arg1}", stream, product.UnlimitedDownloads);
                        break;
                    case "MaxNumberOfDownloads":
                        product.MaxNumberOfDownloads = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MaxNumberOfDownloads → {Arg1}", stream, product.MaxNumberOfDownloads);
                        break;
                    case "DownloadActivationType":
                        product.DownloadActivationTypeId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DownloadActivationTypeId → {Arg1}", stream, product.DownloadActivationTypeId);
                        break;
                    case "HasSampleDownload":
                        product.HasSampleDownload = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.HasSampleDownload → {Arg1}", stream, product.HasSampleDownload);
                        break;
                    case "SampleDownloadId":
                        product.SampleDownloadId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.SampleDownloadId → {Arg1}", stream, product.SampleDownloadId);
                        break;
                    case "HasUserAgreement":
                        product.HasUserAgreement = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.HasUserAgreement → {Arg1}", stream, product.HasUserAgreement);
                        break;
                    case "UserAgreementText":
                        product.UserAgreementText = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.UserAgreementText → {Arg1}", stream, product.UserAgreementText);
                        break;
                    case "IsRecurring":
                        product.IsRecurring = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsRecurring → {Arg1}", stream, product.IsRecurring);
                        break;
                    case "RecurringCycleLength":
                        product.RecurringCycleLength = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RecurringCycleLength → {Arg1}", stream, product.RecurringCycleLength);
                        break;
                    case "RecurringCyclePeriod":
                        product.RecurringCyclePeriodId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RecurringCyclePeriodId → {Arg1}", stream, product.RecurringCyclePeriodId);
                        break;
                    case "RecurringTotalCycles":
                        product.RecurringTotalCycles = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RecurringTotalCycles → {Arg1}", stream, product.RecurringTotalCycles);
                        break;
                    case "IsRental":
                        product.IsRental = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsRental → {Arg1}", stream, product.IsRental);
                        break;
                    case "RentalPriceLength":
                        product.RentalPriceLength = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RentalPriceLength → {Arg1}", stream, product.RentalPriceLength);
                        break;
                    case "RentalPricePeriod":
                        product.RentalPricePeriodId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.RentalPricePeriodId → {Arg1}", stream, product.RentalPricePeriodId);
                        break;
                    case "IsShipEnabled":
                        product.IsShipEnabled = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsShipEnabled → {Arg1}", stream, product.IsShipEnabled);
                        break;
                    case "IsFreeShipping":
                        product.IsFreeShipping = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsFreeShipping → {Arg1}", stream, product.IsFreeShipping);
                        break;
                    case "ShipSeparately":
                        product.ShipSeparately = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ShipSeparately → {Arg1}", stream, product.ShipSeparately);
                        break;
                    case "AdditionalShippingCharge":
                        product.AdditionalShippingCharge = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AdditionalShippingCharge → {Arg1}", stream, product.AdditionalShippingCharge);
                        break;
                    case "DeliveryDate":
                        product.DeliveryDateId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DeliveryDateId → {Arg1}", stream, product.DeliveryDateId);
                        break;
                    case "IsTaxExempt":
                        product.IsTaxExempt = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.IsTaxExempt → {Arg1}", stream, product.IsTaxExempt);
                        break;
                    case "TaxCategory":
                        product.TaxCategoryId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.TaxCategoryId → {Arg1}", stream, product.TaxCategoryId);
                        break;
                    case "ManageInventoryMethod":
                        product.ManageInventoryMethodId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ManageInventoryMethodId → {Arg1}", stream, product.ManageInventoryMethodId);
                        break;
                    case "ProductAvailabilityRange":
                        product.ProductAvailabilityRangeId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ProductAvailabilityRangeId → {Arg1}", stream, product.ProductAvailabilityRangeId);
                        break;
                    case "UseMultipleWarehouses":
                        product.UseMultipleWarehouses = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.UseMultipleWarehouses → {Arg1}", stream, product.UseMultipleWarehouses);
                        break;
                    case "WarehouseId":
                        product.WarehouseId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.WarehouseId → {Arg1}", stream, product.WarehouseId);
                        break;
                    case "StockQuantity":
                        product.StockQuantity = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.StockQuantity → {Arg1}", stream, product.StockQuantity);
                        break;
                    case "DisplayStockAvailability":
                        product.DisplayStockAvailability = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisplayStockAvailability → {Arg1}", stream, product.DisplayStockAvailability);
                        break;
                    case "DisplayStockQuantity":
                        product.DisplayStockQuantity = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisplayStockQuantity → {Arg1}", stream, product.DisplayStockQuantity);
                        break;
                    case "MinStockQuantity":
                        product.MinStockQuantity = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MinStockQuantity → {Arg1}", stream, product.MinStockQuantity);
                        break;
                    case "LowStockActivity":
                        product.LowStockActivityId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.LowStockActivityId → {Arg1}", stream, product.LowStockActivityId);
                        break;
                    case "NotifyAdminForQuantityBelow":
                        product.NotifyAdminForQuantityBelow = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.NotifyAdminForQuantityBelow → {Arg1}", stream, product.NotifyAdminForQuantityBelow);
                        break;
                    case "BackorderMode":
                        product.BackorderModeId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BackorderModeId → {Arg1}", stream, product.BackorderModeId);
                        break;
                    case "AllowBackInStockSubscriptions":
                        product.AllowBackInStockSubscriptions = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AllowBackInStockSubscriptions → {Arg1}", stream, product.AllowBackInStockSubscriptions);
                        break;
                    case "OrderMinimumQuantity":
                        product.OrderMinimumQuantity = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.OrderMinimumQuantity → {Arg1}", stream, product.OrderMinimumQuantity);
                        break;
                    case "OrderMaximumQuantity":
                        product.OrderMaximumQuantity = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.OrderMaximumQuantity → {Arg1}", stream, product.OrderMaximumQuantity);
                        break;
                    case "AllowedQuantities":
                        product.AllowedQuantities = property.StringValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AllowedQuantities → {Arg1}", stream, product.AllowedQuantities);
                        break;
                    case "AllowAddingOnlyExistingAttributeCombinations":
                        product.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AllowAddingOnlyExistingAttributeCombinations → {Arg1}", stream, product.AllowAddingOnlyExistingAttributeCombinations);
                        break;
                    case "NotReturnable":
                        product.NotReturnable = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.NotReturnable → {Arg1}", stream, product.NotReturnable);
                        break;
                    case "DisableBuyButton":
                        product.DisableBuyButton = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisableBuyButton → {Arg1}", stream, product.DisableBuyButton);
                        break;
                    case "DisableWishlistButton":
                        product.DisableWishlistButton = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisableWishlistButton → {Arg1}", stream, product.DisableWishlistButton);
                        break;
                    case "AvailableForPreOrder":
                        product.AvailableForPreOrder = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AvailableForPreOrder → {Arg1}", stream, product.AvailableForPreOrder);
                        break;
                    case "PreOrderAvailabilityStartDateTimeUtc":
                        product.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.PreOrderAvailabilityStartDateTimeUtc → {Arg1}", stream, product.PreOrderAvailabilityStartDateTimeUtc);
                        break;
                    case "CallForPrice":
                        product.CallForPrice = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.CallForPrice → {Arg1}", stream, product.CallForPrice);
                        break;
                    case "Price":
                        product.Price = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Price → {Arg1}", stream, product.Price);
                        break;
                    case "OldPrice":
                        product.OldPrice = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.OldPrice → {Arg1}", stream, product.OldPrice);
                        break;
                    case "ProductCost":
                        product.ProductCost = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ProductCost → {Arg1}", stream, product.ProductCost);
                        break;
                    case "CustomerEntersPrice":
                        product.CustomerEntersPrice = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.CustomerEntersPrice → {Arg1}", stream, product.CustomerEntersPrice);
                        break;
                    case "MinimumCustomerEnteredPrice":
                        product.MinimumCustomerEnteredPrice = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MinimumCustomerEnteredPrice → {Arg1}", stream, product.MinimumCustomerEnteredPrice);
                        break;
                    case "MaximumCustomerEnteredPrice":
                        product.MaximumCustomerEnteredPrice = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MaximumCustomerEnteredPrice → {Arg1}", stream, product.MaximumCustomerEnteredPrice);
                        break;
                    case "BasepriceEnabled":
                        product.BasepriceEnabled = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BasepriceEnabled → {Arg1}", stream, product.BasepriceEnabled);
                        break;
                    case "BasepriceAmount":
                        product.BasepriceAmount = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BasepriceAmount → {Arg1}", stream, product.BasepriceAmount);
                        break;
                    case "BasepriceUnit":
                        product.BasepriceUnitId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BasepriceUnitId → {Arg1}", stream, product.BasepriceUnitId);
                        break;
                    case "BasepriceBaseAmount":
                        product.BasepriceBaseAmount = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BasepriceBaseAmount → {Arg1}", stream, product.BasepriceBaseAmount);
                        break;
                    case "BasepriceBaseUnit":
                        product.BasepriceBaseUnitId = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.BasepriceBaseUnitId → {Arg1}", stream, product.BasepriceBaseUnitId);
                        break;
                    case "MarkAsNew":
                        product.MarkAsNew = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MarkAsNew → {Arg1}", stream, product.MarkAsNew);
                        break;
                    case "MarkAsNewStartDateTimeUtc":
                        product.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MarkAsNewStartDateTimeUtc → {Arg1}", stream, product.MarkAsNewStartDateTimeUtc);
                        break;
                    case "MarkAsNewEndDateTimeUtc":
                        product.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MarkAsNewEndDateTimeUtc → {Arg1}", stream, product.MarkAsNewEndDateTimeUtc);
                        break;
                    case "Weight":
                        product.Weight = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Weight → {Arg1}", stream, product.Weight);
                        break;
                    case "Length":
                        product.Length = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Length → {Arg1}", stream, product.Length);
                        break;
                    case "Width":
                        product.Width = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Width → {Arg1}", stream, product.Width);
                        break;
                    case "Height":
                        product.Height = property.DecimalValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.Height → {Arg1}", stream, product.Height);
                        break;
                    case "IsLimitedToStores":
                        product.LimitedToStores = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.LimitedToStores → {Arg1}", stream, product.LimitedToStores);
                        break;
                    case "DisplayAttributeCombinationImagesOnly":
                        product.DisplayAttributeCombinationImagesOnly = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.DisplayAttributeCombinationImagesOnly → {Arg1}", stream, product.DisplayAttributeCombinationImagesOnly);
                        break;
                    case "AgeVerification":
                        product.AgeVerification = property.BooleanValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.AgeVerification → {Arg1}", stream, product.AgeVerification);
                        break;
                    case "MinimumAgeToPurchase":
                        product.MinimumAgeToPurchase = property.IntValue;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.MinimumAgeToPurchase → {Arg1}", stream, product.MinimumAgeToPurchase);
                        break;
                }
            }

            //set some default values if not specified
            if (isNew && metadata.Properties.All(p => p.PropertyName != "ProductType"))
                product.ProductType = ProductType.SimpleProduct;
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.ProductType → {Arg1}", stream, product.ProductType);
            if (isNew && metadata.Properties.All(p => p.PropertyName != "VisibleIndividually"))
                product.VisibleIndividually = true;
            if (isNew && metadata.Properties.All(p => p.PropertyName != "Published"))
                product.Published = true;

            //sets the current vendor for the new product
            if (isNew && currentVendor != null)
                product.VendorId = currentVendor.Id;

            product.UpdatedOnUtc = DateTime.UtcNow;
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): product.UpdatedOnUtc → {Arg1}", stream, product.UpdatedOnUtc);

            if (isNew)
                await _productService.InsertProductAsync(product);
            else
                await _productService.UpdateProductAsync(product);

            //quantity change history
            if (isNew || previousWarehouseId == product.WarehouseId)
            {
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                    product.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.Edit"));
            }
            //warehouse is changed 
            else
            {
                //compose a message
                var oldWarehouseMessage = string.Empty;
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): oldWarehouseMessage → {OldWarehouseMessage}", stream, oldWarehouseMessage);
                if (previousWarehouseId > 0)
                {
                    var oldWarehouse = await _warehouseService.GetWarehouseByIdAsync(previousWarehouseId);
					_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): oldWarehouse → {OldWarehouse}", stream, oldWarehouse);
                    if (oldWarehouse != null)
                        oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                }

                var newWarehouseMessage = string.Empty;
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): newWarehouseMessage → {NewWarehouseMessage}", stream, newWarehouseMessage);
                if (product.WarehouseId > 0)
                {
                    var newWarehouse = await _warehouseService.GetWarehouseByIdAsync(product.WarehouseId);
					_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): newWarehouse → {NewWarehouse}", stream, newWarehouse);
                    if (newWarehouse != null)
                        newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                }

                var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportProduct.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): message → {Message}", stream, message);

                //record history
                await _productService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
            }

            if (!isNew &&
                product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) > 0 &&
                prevTotalStockQuantity <= 0 &&
                product.Published &&
                !product.Deleted)
            {
                await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
            }

            var tempProperty = metadata.Manager.GetDefaultProperty("SeName");
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): tempProperty → {TempProperty}", stream, tempProperty);

            //search engine name
            var seName = tempProperty?.StringValue ?? (isNew ? string.Empty : await _urlRecordService.GetSeNameAsync(product, 0));
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): seName → {SeName}", stream, seName);
            await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, seName, product.Name, true), 0);

            //save product localized data
            await ImportProductLocalizedAsync(product, metadata, iRow, languages);

            tempProperty = metadata.Manager.GetDefaultProperty("Categories");

            if (tempProperty != null)
            {
                var categoryList = tempProperty.StringValue;

                //category mappings
                var categories = isNew || !allProductsCategoryIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsCategoryIds[product.Id];
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): categories → {Categories}", stream, categories);

                var storesIds = product.LimitedToStores
                    ? (await _storeMappingService.GetStoresIdsWithAccessAsync(product)).ToList()
                    : new List<int>();

                var importedCategories = await categoryList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(categoryName => new CategoryKey(categoryName, storesIds: storesIds))
                    .SelectAwait(async categoryKey =>
                    {
                        var rez = (allCategories.TryGetValue(categoryKey, out var value) ? value.Id : allCategories.Values.FirstOrDefault(c => c.Name == categoryKey.Key)?.Id) ??
                                  allCategories.FirstOrDefault(p =>
                                          p.Key.Key.Equals(categoryKey.Key, StringComparison.InvariantCultureIgnoreCase))
                                      .Value?.Id;

                        if (!rez.HasValue && int.TryParse(categoryKey.Key, out var id))
                            rez = id;

                        if (!rez.HasValue)
                            //database doesn't contain the imported category
                            //this can happen if the category was deleted during the import process
                            await _logger.WarningAsync(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.DatabaseNotContainCategory"), product.Name, categoryKey.Key));

                        return rez;
                    }).Where(id => id != null).ToListAsync();

				_logger.LogInformation("ImportProductsFromXlsxAsync(stream)={Stream}: database query executed in {Arg1} ms", stream, __logStart.ElapsedMilliseconds);
                foreach (var categoryId in importedCategories)
                {
                    if (categories.Any(c => c == categoryId))
                        continue;

                    var productCategory = new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId.Value,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    };
                    await _categoryService.InsertProductCategoryAsync(productCategory);
                }

                //delete product categories
                var deletedProductCategories = await categories.Where(categoryId => !importedCategories.Contains(categoryId))
                    .SelectAwait(async categoryId => (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true)).FirstOrDefault(pc => pc.CategoryId == categoryId)).Where(pc => pc != null).ToListAsync();

                await _categoryService.DeleteProductCategoriesAsync(deletedProductCategories);
            }

            tempProperty = metadata.Manager.GetDefaultProperty("Manufacturers");
            if (tempProperty != null)
            {
                var manufacturerList = tempProperty.StringValue;

                //manufacturer mappings
                var manufacturers = isNew || !allProductsManufacturerIds.ContainsKey(product.Id) ? Array.Empty<int>() : allProductsManufacturerIds[product.Id];
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): manufacturers → {Manufacturers}", stream, manufacturers);

                var importedManufacturers = await manufacturerList
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectAwait(async x =>
                    {
                        var id = allManufacturers.FirstOrDefault(m => m.Name == x.Trim())?.Id;
						_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): id → {Id}", stream, id);

                        if (id != null)
                            return id;

                        id = int.TryParse(x, out var parsedId) ? parsedId : null;

                        if (!id.HasValue)
                            //database doesn't contain the imported manufacturer
                            //this can happen if the manufacturer was deleted during the import process
                            await _logger.WarningAsync(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Import.DatabaseNotContainManufacturer"), product.Name, x));

                        return id;
                    }).Where(id => id.HasValue).ToListAsync();

                foreach (var manufacturerId in importedManufacturers)
                {
                    if (manufacturers.Any(c => c == manufacturerId))
                        continue;

                    var productManufacturer = new ProductManufacturer
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId.Value,
                        IsFeaturedProduct = false,
                        DisplayOrder = 1
                    };
                    await _manufacturerService.InsertProductManufacturerAsync(productManufacturer);
                }

                //delete product manufacturers
                var deletedProductsManufacturers = await manufacturers.Where(manufacturerId => !importedManufacturers.Contains(manufacturerId))
                    .SelectAwait(async manufacturerId => (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id)).FirstOrDefault(pc => pc.ManufacturerId == manufacturerId)).ToListAsync();
					_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): deletedProductsManufacturers → {DeletedProductsManufacturers}", stream, deletedProductsManufacturers);

                deletedProductsManufacturers = deletedProductsManufacturers.Where(m => m != null).ToList();
                await _manufacturerService.DeleteProductManufacturersAsync(deletedProductsManufacturers);
            }

            tempProperty = metadata.Manager.GetDefaultProperty("ProductTags");
            if (tempProperty != null)
            {
                var productTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): productTags → {ProductTags}", stream, productTags);

                //searching existing product tags by their id
                var productTagIds = productTags.Where(pt => int.TryParse(pt, out var _)).Select(int.Parse);
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): productTagIds → {ProductTagIds}", stream, productTagIds);

                var productTagsByIds = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Where(pt => productTagIds.Contains(pt.Id)).ToList();
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): productTagsByIds → {ProductTagsByIds}", stream, productTagsByIds);

                productTags.AddRange(productTagsByIds.Select(pt => pt.Name));
                var filter = productTagsByIds.Select(pt => pt.Id.ToString()).ToList();
				_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): filter → {Filter}", stream, filter);

                //product tag mappings
                await _productTagService.UpdateProductTagsAsync(product, productTags.Where(pt => !filter.Contains(pt)).ToArray());
            }

            tempProperty = metadata.Manager.GetDefaultProperty("LimitedToStores");
            if (tempProperty != null)
            {
                var limitedToStoresList = tempProperty.StringValue;

                var importedStores = product.LimitedToStores ? limitedToStoresList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => allStores.FirstOrDefault(store => store.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList() : new List<int>();

                await _storeMappingService.SaveStoreMappingsAsync(product, importedStores);
            }

            var pictureMetaData = new ProductPictureMetadata
            {
                ProductItem = product,
                IsNew = isNew
            };

            var pictureProperties = new List<string> { "Picture1", "Picture2", "Picture3" };
			_logger.LogDebug("ImportProductsFromXlsxAsync(stream={Stream}): pictureProperties → {PictureProperties}", stream, pictureProperties);

            if (currentVendor != null)
                pictureProperties = pictureProperties.Take(_vendorSettings.MaximumProductPicturesNumber).ToList();

            foreach (var propertyName in pictureProperties)
                pictureMetaData.PicturePaths.Add(await DownloadFileAsync(metadata.Manager.GetDefaultProperty(propertyName)?.StringValue, downloadedFiles));

            productPictureMetadata.Add(pictureMetaData);

            lastLoadedProduct = product;
        }

        if (_mediaSettings.ImportProductImagesUsingHash && await _pictureService.IsStoreInDbAsync())
            await ImportProductImagesUsingHashAsync(productPictureMetadata, allProductsBySku);
        else
            await ImportProductImagesUsingServicesAsync(productPictureMetadata);

        foreach (var downloadedFile in downloadedFiles)
        {
            if (!_fileProvider.FileExists(downloadedFile))
                continue;

            try
            {
                _fileProvider.DeleteFile(downloadedFile);
            }
            catch
            {
                // ignored
            }
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportProducts", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportProducts"), metadata.CountProductsInFile));
		_logger.LogDebug("<<< Exiting ImportProductsFromXlsxAsync(stream={Stream})", stream);
    }

    /// <summary>
    /// Import newsletter subscribers from TXT file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported subscribers
    /// </returns>
    public virtual async Task<int> ImportNewsLetterSubscribersFromTxtAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportNewsLetterSubscribersFromTxtAsync(stream={Stream})", stream);
        var store = await _storeContext.GetCurrentStoreAsync();
        var defaultLanguageId = store.DefaultLanguageId > 0 ? store.DefaultLanguageId : (await _workContext.GetWorkingLanguageAsync()).Id;
		_logger.LogDebug("ImportNewsLetterSubscribersFromTxtAsync(stream={Stream}): defaultLanguageId → {DefaultLanguageId}", stream, defaultLanguageId);
		_logger.LogDebug("ImportNewsLetterSubscribersFromTxtAsync(stream={Stream}): store → {Store}", stream, store);
        var defaultTypeId = ((await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id)).FirstOrDefault()
            ?? (await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync()).FirstOrDefault())
            .Id;
        var allSubscriptions = (await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync()).ToList();
		_logger.LogDebug("ImportNewsLetterSubscribersFromTxtAsync(stream={Stream}): allSubscriptions → {AllSubscriptions}", stream, allSubscriptions);

        var count = 0;
		_logger.LogDebug("ImportNewsLetterSubscribersFromTxtAsync(stream={Stream}): count → {Count}", stream, count);
        using (var reader = new StreamReader(stream))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var tmp = line.Split(',');
                if (tmp.Length > 5)
                    throw new NopException("Wrong file format");

                //"email" field specified
                var email = tmp[0].Trim();
                if (!CommonHelper.IsValidEmail(email))
                    continue;

                //"active" field specified
                var isActive = true;
                if (tmp.Length >= 2 && !bool.TryParse(tmp[1].Trim(), out isActive))
                    continue;

                //"typeId" field specified
                var typeId = defaultTypeId;
                if (tmp.Length >= 3 && !int.TryParse(tmp[2].Trim(), out typeId))
                    continue;

                //"storeId" field specified
                var storeId = store.Id;
                if (tmp.Length >= 4 && !int.TryParse(tmp[3].Trim(), out storeId))
                    continue;

                //"languageId" field specified
                var languageId = 0;
                if (tmp.Length == 5)
                    _ = int.TryParse(tmp[4].Trim(), out languageId);

                //import
                var subscription = allSubscriptions.FirstOrDefault(subscription =>
                    string.Equals(subscription.Email, email, StringComparison.InvariantCultureIgnoreCase) &&
                    subscription.StoreId == storeId &&
                    subscription.TypeId == typeId);
                if (subscription != null)
                {
                    subscription.Active = isActive;
                    subscription.LanguageId = languageId > 0 ? languageId : defaultLanguageId;
                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                }
                else
                {
                    subscription = new NewsLetterSubscription
                    {
                        Active = isActive,
                        CreatedOnUtc = DateTime.UtcNow,
                        Email = email,
                        StoreId = storeId,
                        LanguageId = languageId > 0 ? languageId : defaultLanguageId,
                        TypeId = typeId,
                        NewsLetterSubscriptionGuid = Guid.NewGuid()
                    };
                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                }

                count++;
            }
        }

        await _customerActivityService.InsertActivityAsync("ImportNewsLetterSubscriptions",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportNewsLetterSubscriptions"), count));

		_logger.LogDebug("<<< Exiting ImportNewsLetterSubscribersFromTxtAsync(stream={Stream})", stream);
        return count;
    }

    /// <summary>
    /// Import states from TXT file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="writeLog">Indicates whether to add logging</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of imported states
    /// </returns>
    public virtual async Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true)
    {
		_logger.LogDebug(">>> Entering ImportStatesFromTxtAsync(stream={Stream},writeLog={WriteLog})", stream, writeLog);
        var count = 0;
        using (var reader = new StreamReader(stream))
        {
		_logger.LogDebug("ImportStatesFromTxtAsync(stream={Stream},writeLog={WriteLog}): count → {Count}", stream, writeLog, count);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var tmp = line.Split(',');

                if (tmp.Length != 5)
                    throw new NopException("Wrong file format");

                //parse
                var countryTwoLetterIsoCode = tmp[0].Trim();
                var name = tmp[1].Trim();
                var abbreviation = tmp[2].Trim();
                var published = bool.Parse(tmp[3].Trim());
                var displayOrder = int.Parse(tmp[4].Trim());

                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryTwoLetterIsoCode);
                if (country == null)
                {
                    //country cannot be loaded. skip
                    continue;
                }

                //import
                var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true);
                var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (state != null)
                {
                    state.Abbreviation = abbreviation;
                    state.Published = published;
                    state.DisplayOrder = displayOrder;
                    await _stateProvinceService.UpdateStateProvinceAsync(state);
                }
                else
                {
                    state = new StateProvince
                    {
                        CountryId = country.Id,
                        Name = name,
                        Abbreviation = abbreviation,
                        Published = published,
                        DisplayOrder = displayOrder
                    };
                    await _stateProvinceService.InsertStateProvinceAsync(state);
                }

                count++;
            }
        }

        //activity log
        if (writeLog)
        {
            await _customerActivityService.InsertActivityAsync("ImportStates",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportStates"), count));
        }

		_logger.LogDebug("<<< Exiting ImportStatesFromTxtAsync(stream={Stream},writeLog={WriteLog})", stream, writeLog);
        return count;
    }

    /// <summary>
    /// Import manufacturers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportManufacturersFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportManufacturersFromXlsxAsync(stream={Stream})", stream);
        using var workbook = new XLWorkbook(stream);

		_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): languages → {Languages}", stream, languages);

        //the columns
        var metadata = GetWorkbookMetadata<Manufacturer>(workbook, languages);
		_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): metadata → {Metadata}", stream, metadata);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Manufacturer>(defaultProperties, _catalogSettings, localizedProperties, languages);
		_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manager → {Manager}", stream, manager);

        var iRow = 2;
        var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");
		_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): setSeName → {SetSeName}", stream, setSeName);

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manager.GetDefaultProperty("Id").IntValue);
			_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer → {Manufacturer}", stream, manufacturer);

            var isNew = manufacturer == null;

            manufacturer ??= new Manufacturer();

            if (isNew)
            {
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
				_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.CreatedOnUtc → {Arg1}", stream, manufacturer.CreatedOnUtc);

                //default values
                manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
				_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PageSize → {Arg1}", stream, manufacturer.PageSize);
				_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PageSizeOptions → {Arg1}", stream, manufacturer.PageSizeOptions);
				_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.Published → {Arg1}", stream, manufacturer.Published);
				_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.AllowCustomersToSelectPageSize → {Arg1}", stream, manufacturer.AllowCustomersToSelectPageSize);
                manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                manufacturer.Published = true;
                manufacturer.AllowCustomersToSelectPageSize = true;
            }

            var seName = string.Empty;
			_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): seName → {SeName}", stream, seName);

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        manufacturer.Name = property.StringValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.Name → {Arg1}", stream, manufacturer.Name);
                        break;
                    case "Description":
                        manufacturer.Description = property.StringValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.Description → {Arg1}", stream, manufacturer.Description);
                        break;
                    case "ManufacturerTemplateId":
                        manufacturer.ManufacturerTemplateId = property.IntValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.ManufacturerTemplateId → {Arg1}", stream, manufacturer.ManufacturerTemplateId);
                        break;
                    case "MetaKeywords":
                        manufacturer.MetaKeywords = property.StringValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.MetaKeywords → {Arg1}", stream, manufacturer.MetaKeywords);
                        break;
                    case "MetaDescription":
                        manufacturer.MetaDescription = property.StringValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.MetaDescription → {Arg1}", stream, manufacturer.MetaDescription);
                        break;
                    case "MetaTitle":
                        manufacturer.MetaTitle = property.StringValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.MetaTitle → {Arg1}", stream, manufacturer.MetaTitle);
                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): picture → {Picture}", stream, picture);

                        if (picture != null)
                            manufacturer.PictureId = picture.Id;
							_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PictureId → {Arg1}", stream, manufacturer.PictureId);

                        break;
                    case "PageSize":
                        manufacturer.PageSize = property.IntValue;
                        break;
                    case "AllowCustomersToSelectPageSize":
                        manufacturer.AllowCustomersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        manufacturer.PageSizeOptions = property.StringValue;
                        break;
                    case "PriceRangeFiltering":
                        manufacturer.PriceRangeFiltering = property.BooleanValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PriceRangeFiltering → {Arg1}", stream, manufacturer.PriceRangeFiltering);
                        break;
                    case "PriceFrom":
                        manufacturer.PriceFrom = property.DecimalValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PriceFrom → {Arg1}", stream, manufacturer.PriceFrom);
                        break;
                    case "PriceTo":
                        manufacturer.PriceTo = property.DecimalValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.PriceTo → {Arg1}", stream, manufacturer.PriceTo);
                        break;
                    case "AutomaticallyCalculatePriceRange":
                        manufacturer.ManuallyPriceRange = property.BooleanValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.ManuallyPriceRange → {Arg1}", stream, manufacturer.ManuallyPriceRange);
                        break;
                    case "Published":
                        manufacturer.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        manufacturer.DisplayOrder = property.IntValue;
						_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.DisplayOrder → {Arg1}", stream, manufacturer.DisplayOrder);
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            manufacturer.UpdatedOnUtc = DateTime.UtcNow;
			_logger.LogDebug("ImportManufacturersFromXlsxAsync(stream={Stream}): manufacturer.UpdatedOnUtc → {Arg1}", stream, manufacturer.UpdatedOnUtc);

            if (isNew)
                await _manufacturerService.InsertManufacturerAsync(manufacturer);
            else
                await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(manufacturer, await _urlRecordService.ValidateSeNameAsync(manufacturer, seName, manufacturer.Name, true), 0);

            //save manufacturer localized data
            await ImportManufaturerLocalizedAsync(manufacturer, metadata, manager, iRow, languages);

            iRow++;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportManufacturers",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportManufacturers"), iRow - 2));
			_logger.LogDebug("<<< Exiting ImportManufacturersFromXlsxAsync(stream={Stream})", stream);
    }

    /// <summary>
    /// Import categories from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportCategoriesFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportCategoriesFromXlsxAsync(stream={Stream})", stream);
        using var workbook = new XLWorkbook(stream);

		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);
        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): languages → {Languages}", stream, languages);

        //the columns
        var metadata = GetWorkbookMetadata<Category>(workbook, languages);
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): metadata → {Metadata}", stream, metadata);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<Category>(defaultProperties, _catalogSettings, localizedProperties, languages);
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): manager → {Manager}", stream, manager);

        var iRow = 2;
        var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): setSeName → {SetSeName}", stream, setSeName);

        //performance optimization, load all categories in one SQL request
        var allCategories = await (await _categoryService
                .GetAllCategoriesAsync(showHidden: true))
            .GroupByAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c))
            .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("ImportCategoriesFromXlsxAsync(stream)={Stream}: database query executed in {Arg1} ms", stream, __logStart.ElapsedMilliseconds);

        var saveNextTime = new List<int>();
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): saveNextTime → {SaveNextTime}", stream, saveNextTime);

        //performance optimization, load all stores in one SQL request
        var allStores = await _storeService.GetAllStoresAsync();
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): allStores → {AllStores}", stream, allStores);

        while (true)
        {
            var allColumnsAreEmpty = manager.GetDefaultProperties
                .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                .All(cell => string.IsNullOrEmpty(cell?.Value.ToString()));

            if (allColumnsAreEmpty)
                break;

            //get category by data in xlsx file if it possible, or create new category
            var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, iRow, allCategories);

            //update category by data in xlsx file
            var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, allStores, isNew);

            if (isParentCategoryExists)
            {
                //if parent category exists in database then save category into database
                await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                //save category localized data
                await ImportCategoryLocalizedAsync(category, metadata, manager, iRow, languages);
            }
            else
            {
                //if parent category doesn't exists in database then try save category into database next time
                saveNextTime.Add(iRow);
            }

            iRow++;
        }

        var needSave = saveNextTime.Any();
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): needSave → {NeedSave}", stream, needSave);

        while (needSave)
        {
            var remove = new List<int>();
			_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): remove → {Remove}", stream, remove);

            //try to save unsaved categories
            foreach (var rowId in saveNextTime)
            {
                //get category by data in xlsx file if it possible, or create new category
                var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, rowId, allCategories);
                //update category by data in xlsx file
                var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, allStores, isNew);

                if (!isParentCategoryExists)
                    continue;

                //if parent category exists in database then save category into database
                await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                //save category localized data
                await ImportCategoryLocalizedAsync(category, metadata, manager, rowId, languages);

                remove.Add(rowId);
            }

            saveNextTime.RemoveAll(remove.Contains);

            needSave = remove.Any() && saveNextTime.Any();
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportCategories",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

        if (!saveNextTime.Any())
            return;

        var categoriesName = new List<string>();
		_logger.LogDebug("ImportCategoriesFromXlsxAsync(stream={Stream}): categoriesName → {CategoriesName}", stream, categoriesName);

        foreach (var rowId in saveNextTime)
        {
            manager.ReadDefaultFromXlsx(defaultWorksheet, rowId);
            categoriesName.Add(manager.GetDefaultProperty("Name").StringValue);
        }

		_logger.LogDebug("<<< Exiting ImportCategoriesFromXlsxAsync(stream={Stream})", stream);
        throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", categoriesName)));
    }

    /// <summary>
    /// Import orders from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportOrdersFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportOrdersFromXlsxAsync(stream={Stream})", stream);
        using var workbook = new XLWorkbook(stream);

		_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);
        var (metadata, worksheet) = await PrepareImportOrderDataAsync(workbook);

        //performance optimization, load all orders by guid in one SQL request
        var allOrdersByGuids = await _orderService.GetOrdersByGuidsAsync(metadata.AllOrderGuids.ToArray());
		_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): allOrdersByGuids → {AllOrdersByGuids}", stream, allOrdersByGuids);

        //performance optimization, load all customers by guid in one SQL request
        var allCustomersByGuids = await _customerService.GetCustomersByGuidsAsync(metadata.AllCustomerGuids.ToArray());
		_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): allCustomersByGuids → {AllCustomersByGuids}", stream, allCustomersByGuids);

        Order lastLoadedOrder = null;
		_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): lastLoadedOrder → {LastLoadedOrder}", stream, lastLoadedOrder);

        for (var iRow = 2; iRow < metadata.EndRow; iRow++)
        {
            //imports product attributes
            if (worksheet.Row(iRow).OutlineLevel != 0)
            {
                if (lastLoadedOrder == null)
                    continue;

                metadata.OrderItemManager.ReadDefaultFromXlsx(worksheet, iRow, 2);

                //skip caption row
                if (!metadata.OrderItemManager.IsCaption)
                    await ImportOrderItemAsync(metadata.OrderItemManager, lastLoadedOrder);

                continue;
            }

            metadata.Manager.ReadDefaultFromXlsx(worksheet, iRow);

            var order = allOrdersByGuids.FirstOrDefault(p => p.OrderGuid == metadata.Manager.GetDefaultProperty("OrderGuid").GuidValue);
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order → {Order}", stream, order);

            var isNew = order == null;

            order ??= new Order();

            Address orderBillingAddress = null;
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress → {OrderBillingAddress}", stream, orderBillingAddress);
            Address orderAddress = null;
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress → {OrderAddress}", stream, orderAddress);

            if (isNew)
                order.CreatedOnUtc = DateTime.UtcNow;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CreatedOnUtc → {Arg1}", stream, order.CreatedOnUtc);
            else
            {
                orderBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                orderAddress = await _addressService.GetAddressByIdAsync((order.PickupInStore ? order.PickupAddressId : order.ShippingAddressId) ?? 0);
            }

            orderBillingAddress ??= new Address();
            orderAddress ??= new Address();

            var customer = allCustomersByGuids.FirstOrDefault(p => p.CustomerGuid.ToString() == metadata.Manager.GetDefaultProperty("CustomerGuid").StringValue);
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): customer → {Customer}", stream, customer);

            var billingStateProvinceAbbreviation = string.Empty;
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): billingStateProvinceAbbreviation → {BillingStateProvinceAbbreviation}", stream, billingStateProvinceAbbreviation);
            var shippingStateProvinceAbbreviation = string.Empty;
			_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): shippingStateProvinceAbbreviation → {ShippingStateProvinceAbbreviation}", stream, shippingStateProvinceAbbreviation);

            foreach (var property in metadata.Manager.GetDefaultProperties)
                switch (property.PropertyName)
                {
                    case "StoreId":
                        if (await _storeService.GetStoreByIdAsync(property.IntValue) is { })
                            order.StoreId = property.IntValue;
							_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.StoreId → {Arg1}", stream, order.StoreId);
                        else
                            order.StoreId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                        break;
                    case "OrderGuid":
                        order.OrderGuid = property.GuidValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderGuid → {Arg1}", stream, order.OrderGuid);
                        break;
                    case "CustomerId":
                        order.CustomerId = customer?.Id ?? 0;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomerId → {Arg1}", stream, order.CustomerId);
                        break;
                    case "OrderStatus":
                        order.OrderStatus = (OrderStatus)property.PropertyValue;
                        break;
                    case "PaymentStatus":
                        order.PaymentStatus = (PaymentStatus)property.PropertyValue;
                        break;
                    case "ShippingStatus":
                        order.ShippingStatus = (ShippingStatus)property.PropertyValue;
                        break;
                    case "OrderSubtotalInclTax":
                        order.OrderSubtotalInclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderSubtotalInclTax → {Arg1}", stream, order.OrderSubtotalInclTax);
                        break;
                    case "OrderSubtotalExclTax":
                        order.OrderSubtotalExclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderSubtotalExclTax → {Arg1}", stream, order.OrderSubtotalExclTax);
                        break;
                    case "OrderSubTotalDiscountInclTax":
                        order.OrderSubTotalDiscountInclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderSubTotalDiscountInclTax → {Arg1}", stream, order.OrderSubTotalDiscountInclTax);
                        break;
                    case "OrderSubTotalDiscountExclTax":
                        order.OrderSubTotalDiscountExclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderSubTotalDiscountExclTax → {Arg1}", stream, order.OrderSubTotalDiscountExclTax);
                        break;
                    case "OrderShippingInclTax":
                        order.OrderShippingInclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderShippingInclTax → {Arg1}", stream, order.OrderShippingInclTax);
                        break;
                    case "OrderShippingExclTax":
                        order.OrderShippingExclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderShippingExclTax → {Arg1}", stream, order.OrderShippingExclTax);
                        break;
                    case "PaymentMethodAdditionalFeeInclTax":
                        order.PaymentMethodAdditionalFeeInclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.PaymentMethodAdditionalFeeInclTax → {Arg1}", stream, order.PaymentMethodAdditionalFeeInclTax);
                        break;
                    case "PaymentMethodAdditionalFeeExclTax":
                        order.PaymentMethodAdditionalFeeExclTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.PaymentMethodAdditionalFeeExclTax → {Arg1}", stream, order.PaymentMethodAdditionalFeeExclTax);
                        break;
                    case "TaxRates":
                        order.TaxRates = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.TaxRates → {Arg1}", stream, order.TaxRates);
                        break;
                    case "OrderTax":
                        order.OrderTax = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderTax → {Arg1}", stream, order.OrderTax);
                        break;
                    case "OrderTotal":
                        order.OrderTotal = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderTotal → {Arg1}", stream, order.OrderTotal);
                        break;
                    case "RefundedAmount":
                        order.RefundedAmount = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.RefundedAmount → {Arg1}", stream, order.RefundedAmount);
                        break;
                    case "OrderDiscount":
                        order.OrderDiscount = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.OrderDiscount → {Arg1}", stream, order.OrderDiscount);
                        break;
                    case "CurrencyRate":
                        order.CurrencyRate = property.DecimalValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CurrencyRate → {Arg1}", stream, order.CurrencyRate);
                        break;
                    case "CustomerCurrencyCode":
                        order.CustomerCurrencyCode = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomerCurrencyCode → {Arg1}", stream, order.CustomerCurrencyCode);
                        break;
                    case "AffiliateId":
                        order.AffiliateId = property.IntValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.AffiliateId → {Arg1}", stream, order.AffiliateId);
                        break;
                    case "PaymentMethodSystemName":
                        order.PaymentMethodSystemName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.PaymentMethodSystemName → {Arg1}", stream, order.PaymentMethodSystemName);
                        break;
                    case "ShippingPickupInStore":
                        order.PickupInStore = property.BooleanValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.PickupInStore → {Arg1}", stream, order.PickupInStore);
                        break;
                    case "ShippingMethod":
                        order.ShippingMethod = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.ShippingMethod → {Arg1}", stream, order.ShippingMethod);
                        break;
                    case "ShippingRateComputationMethodSystemName":
                        order.ShippingRateComputationMethodSystemName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.ShippingRateComputationMethodSystemName → {Arg1}", stream, order.ShippingRateComputationMethodSystemName);
                        break;
                    case "CustomValuesXml":
                        order.CustomValuesXml = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomValuesXml → {Arg1}", stream, order.CustomValuesXml);
                        break;
                    case "VatNumber":
                        order.VatNumber = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.VatNumber → {Arg1}", stream, order.VatNumber);
                        break;
                    case "CreatedOnUtc":
                        order.CreatedOnUtc = DateTime.TryParse(property.StringValue, out var createdOnUtc) ? createdOnUtc : DateTime.UtcNow;
                        break;
                    case "BillingFirstName":
                        orderBillingAddress.FirstName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.FirstName → {Arg1}", stream, orderBillingAddress.FirstName);
                        break;
                    case "BillingLastName":
                        orderBillingAddress.LastName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.LastName → {Arg1}", stream, orderBillingAddress.LastName);
                        break;
                    case "BillingPhoneNumber":
                        orderBillingAddress.PhoneNumber = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.PhoneNumber → {Arg1}", stream, orderBillingAddress.PhoneNumber);
                        break;
                    case "BillingEmail":
                        orderBillingAddress.Email = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.Email → {Arg1}", stream, orderBillingAddress.Email);
                        break;
                    case "BillingFaxNumber":
                        orderBillingAddress.FaxNumber = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.FaxNumber → {Arg1}", stream, orderBillingAddress.FaxNumber);
                        break;
                    case "BillingCompany":
                        orderBillingAddress.Company = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.Company → {Arg1}", stream, orderBillingAddress.Company);
                        break;
                    case "BillingAddress1":
                        orderBillingAddress.Address1 = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.Address1 → {Arg1}", stream, orderBillingAddress.Address1);
                        break;
                    case "BillingAddress2":
                        orderBillingAddress.Address2 = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.Address2 → {Arg1}", stream, orderBillingAddress.Address2);
                        break;
                    case "BillingCity":
                        orderBillingAddress.City = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.City → {Arg1}", stream, orderBillingAddress.City);
                        break;
                    case "BillingCounty":
                        orderBillingAddress.County = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.County → {Arg1}", stream, orderBillingAddress.County);
                        break;
                    case "BillingStateProvinceAbbreviation":
                        billingStateProvinceAbbreviation = property.StringValue;

                        break;
                    case "BillingZipPostalCode":
                        orderBillingAddress.ZipPostalCode = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.ZipPostalCode → {Arg1}", stream, orderBillingAddress.ZipPostalCode);
                        break;
                    case "BillingCountryCode":
                        if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is { } billingCountry)
                            orderBillingAddress.CountryId = billingCountry.Id;
							_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.CountryId → {Arg1}", stream, orderBillingAddress.CountryId);
                        break;
                    case "ShippingFirstName":
                        orderAddress.FirstName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.FirstName → {Arg1}", stream, orderAddress.FirstName);
                        break;
                    case "ShippingLastName":
                        orderAddress.LastName = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.LastName → {Arg1}", stream, orderAddress.LastName);
                        break;
                    case "ShippingPhoneNumber":
                        orderAddress.PhoneNumber = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.PhoneNumber → {Arg1}", stream, orderAddress.PhoneNumber);
                        break;
                    case "ShippingEmail":
                        orderAddress.Email = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.Email → {Arg1}", stream, orderAddress.Email);
                        break;
                    case "ShippingFaxNumber":
                        orderAddress.FaxNumber = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.FaxNumber → {Arg1}", stream, orderAddress.FaxNumber);
                        break;
                    case "ShippingCompany":
                        orderAddress.Company = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.Company → {Arg1}", stream, orderAddress.Company);
                        break;
                    case "ShippingAddress1":
                        orderAddress.Address1 = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.Address1 → {Arg1}", stream, orderAddress.Address1);
                        break;
                    case "ShippingAddress2":
                        orderAddress.Address2 = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.Address2 → {Arg1}", stream, orderAddress.Address2);
                        break;
                    case "ShippingCity":
                        orderAddress.City = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.City → {Arg1}", stream, orderAddress.City);
                        break;
                    case "ShippingCounty":
                        orderAddress.County = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.County → {Arg1}", stream, orderAddress.County);
                        break;
                    case "ShippingStateProvinceAbbreviation":
                        shippingStateProvinceAbbreviation = property.StringValue;
                        break;
                    case "ShippingZipPostalCode":
                        orderAddress.ZipPostalCode = property.StringValue;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.ZipPostalCode → {Arg1}", stream, orderAddress.ZipPostalCode);
                        break;
                    case "ShippingCountryCode":
                        if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is { } shippingCountry)
                            orderAddress.CountryId = shippingCountry.Id;
							_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.CountryId → {Arg1}", stream, orderAddress.CountryId);
                        break;
                }

            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(billingStateProvinceAbbreviation, orderBillingAddress.CountryId) is { } billingState)
                orderBillingAddress.StateProvinceId = billingState.Id;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderBillingAddress.StateProvinceId → {Arg1}", stream, orderBillingAddress.StateProvinceId);

            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(shippingStateProvinceAbbreviation, orderAddress.CountryId) is { } shippingState)
                orderAddress.StateProvinceId = shippingState.Id;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): orderAddress.StateProvinceId → {Arg1}", stream, orderAddress.StateProvinceId);

            //check order address field values from excel
            if (string.IsNullOrWhiteSpace(orderAddress.FirstName) && string.IsNullOrWhiteSpace(orderAddress.LastName) && string.IsNullOrWhiteSpace(orderAddress.Email))
                orderAddress = null;

            //insert or update billing address
            if (orderBillingAddress.Id == 0)
            {
                await _addressService.InsertAddressAsync(orderBillingAddress);
                order.BillingAddressId = orderBillingAddress.Id;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.BillingAddressId → {Arg1}", stream, order.BillingAddressId);
            }
            else
                await _addressService.UpdateAddressAsync(orderBillingAddress);

            //insert or update shipping/pickup address
            if (orderAddress != null)
            {
                if (orderAddress.Id == 0)
                {
                    await _addressService.InsertAddressAsync(orderAddress);

                    if (order.PickupInStore)
                        order.PickupAddressId = orderAddress.Id;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.PickupAddressId → {Arg1}", stream, order.PickupAddressId);
                    else
                        order.ShippingAddressId = orderAddress.Id;
						_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.ShippingAddressId → {Arg1}", stream, order.ShippingAddressId);
                }
                else
                    await _addressService.UpdateAddressAsync(orderAddress);
            }
            else
                order.ShippingAddressId = null;

            //set some default values if not specified
            if (isNew)
            {
                //customer language
                var customerLanguage = await _languageService.GetLanguageByIdAsync(customer?.LanguageId ?? 0);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): customerLanguage → {CustomerLanguage}", stream, customerLanguage);
                if (customerLanguage == null || !customerLanguage.Published)
                    customerLanguage = await _workContext.GetWorkingLanguageAsync();
					_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomerLanguageId → {Arg1}", stream, order.CustomerLanguageId);
                order.CustomerLanguageId = customerLanguage.Id;

                //tax display type
                order.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomerTaxDisplayType → {Arg1}", stream, order.CustomerTaxDisplayType);

                //set other default values
                order.AllowStoringCreditCardNumber = false;
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.AllowStoringCreditCardNumber → {Arg1}", stream, order.AllowStoringCreditCardNumber);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.AuthorizationTransactionCode → {Arg1}", stream, order.AuthorizationTransactionCode);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.AuthorizationTransactionId → {Arg1}", stream, order.AuthorizationTransactionId);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.AuthorizationTransactionResult → {Arg1}", stream, order.AuthorizationTransactionResult);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CaptureTransactionId → {Arg1}", stream, order.CaptureTransactionId);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CaptureTransactionResult → {Arg1}", stream, order.CaptureTransactionResult);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardCvv2 → {Arg1}", stream, order.CardCvv2);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardExpirationMonth → {Arg1}", stream, order.CardExpirationMonth);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardExpirationYear → {Arg1}", stream, order.CardExpirationYear);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardName → {Arg1}", stream, order.CardName);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardNumber → {Arg1}", stream, order.CardNumber);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CardType → {Arg1}", stream, order.CardType);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomerIp → {Arg1}", stream, order.CustomerIp);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.CustomOrderNumber → {Arg1}", stream, order.CustomOrderNumber);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.MaskedCreditCardNumber → {Arg1}", stream, order.MaskedCreditCardNumber);
				_logger.LogDebug("ImportOrdersFromXlsxAsync(stream={Stream}): order.SubscriptionTransactionId → {Arg1}", stream, order.SubscriptionTransactionId);
                order.AuthorizationTransactionCode = string.Empty;
                order.AuthorizationTransactionId = string.Empty;
                order.AuthorizationTransactionResult = string.Empty;
                order.CaptureTransactionId = string.Empty;
                order.CaptureTransactionResult = string.Empty;
                order.CardCvv2 = string.Empty;
                order.CardExpirationMonth = string.Empty;
                order.CardExpirationYear = string.Empty;
                order.CardName = string.Empty;
                order.CardNumber = string.Empty;
                order.CardType = string.Empty;
                order.CustomerIp = string.Empty;
                order.CustomOrderNumber = string.Empty;
                order.MaskedCreditCardNumber = string.Empty;
                order.RefundedAmount = decimal.Zero;
                order.SubscriptionTransactionId = string.Empty;

                await _orderService.InsertOrderAsync(order);

                //generate and set custom order number
                order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
                await _orderService.UpdateOrderAsync(order);
            }
            else
                await _orderService.UpdateOrderAsync(order);

            lastLoadedOrder = order;
        }

        //activity log
        await _customerActivityService.InsertActivityAsync("ImportOrders", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportOrders"), metadata.CountOrdersInFile));
		_logger.LogDebug("<<< Exiting ImportOrdersFromXlsxAsync(stream={Stream})", stream);
    }

    /// <summary>
    /// Import filter level values from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ImportFilterLevelValuesFromXlsxAsync(Stream stream)
    {
		_logger.LogDebug(">>> Entering ImportFilterLevelValuesFromXlsxAsync(stream={Stream})", stream);
        ArgumentNullException.ThrowIfNull(stream);
        using var workbook = new XLWorkbook(stream);
		_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): workbook → {Workbook}", stream, workbook);

        var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
		_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): languages → {Languages}", stream, languages);

        //the columns
        var metadata = GetWorkbookMetadata<FilterLevelValue>(workbook, languages);
		_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): metadata → {Metadata}", stream, metadata);
        var defaultWorksheet = metadata.DefaultWorksheet;
        var defaultProperties = metadata.DefaultProperties;
        var localizedProperties = metadata.LocalizedProperties;

        var manager = new PropertyManager<FilterLevelValue>(defaultProperties, _catalogSettings, localizedProperties, languages);
		_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): manager → {Manager}", stream, manager);
        var iRow = 2;

        try
        {
            var filterLevelValues = await _filterLevelValueService.GetAllFilterLevelValuesAsync();
			_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValues → {FilterLevelValues}", stream, filterLevelValues);

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);
                var filterLevelValue = filterLevelValues.FirstOrDefault(f => f.Id == manager.GetDefaultProperty("Id").IntValue);
				_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValue → {FilterLevelValue}", stream, filterLevelValue);

                var isNew = filterLevelValue == null;
                filterLevelValue ??= new FilterLevelValue
                {
                    CreatedOnUtc = DateTime.UtcNow
                };

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case nameof(FilterLevelValue.FilterLevel1Value):
                            filterLevelValue.FilterLevel1Value = property.StringValue;
							_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValue.FilterLevel1Value → {Arg1}", stream, filterLevelValue.FilterLevel1Value);
                            break;
                        case nameof(FilterLevelValue.FilterLevel2Value):
                            filterLevelValue.FilterLevel2Value = property.StringValue;
							_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValue.FilterLevel2Value → {Arg1}", stream, filterLevelValue.FilterLevel2Value);
                            break;
                        case nameof(FilterLevelValue.FilterLevel3Value):
                            filterLevelValue.FilterLevel3Value = property.StringValue;
							_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValue.FilterLevel3Value → {Arg1}", stream, filterLevelValue.FilterLevel3Value);
                            break;
                    }
                }

                filterLevelValue.UpdatedOnUtc = DateTime.UtcNow;
				_logger.LogDebug("ImportFilterLevelValuesFromXlsxAsync(stream={Stream}): filterLevelValue.UpdatedOnUtc → {Arg1}", stream, filterLevelValue.UpdatedOnUtc);

                await (isNew
                    ? _filterLevelValueService.InsertFilterLevelValueAsync(filterLevelValue)
                    : _filterLevelValueService.UpdateFilterLevelValueAsync(filterLevelValue));

                iRow++;
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("ImportFilterLevelValues",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportFilterLevelValues"), iRow - 2));
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error while importing filter level values", ex);
		_logger.LogDebug("<<< Exiting ImportFilterLevelValuesFromXlsxAsync(stream={Stream})", stream);
            throw;
        }
    }

    #endregion

    #region Nested classes

    protected partial class ProductPictureMetadata
    {
        public Product ProductItem { get; set; }

        public HashSet<string> PicturePaths { get; set; } = new();

        public bool IsNew { get; set; }
    }

    protected partial class CategoryKey
    {
        public CategoryKey(string key, Category category = null, List<int> storesIds = null)
        {
			_logger.LogDebug(">>> Entering CategoryKey(key={Key},category={Category},storesIds={StoresIds})", key, category, storesIds);
            Key = key.Trim();
            StoresIds = storesIds ?? new List<int>();
			_logger.LogDebug("CategoryKey(key={Key},category={Category},storesIds={StoresIds}): StoresIds → {StoresIds1}", key, category, storesIds, StoresIds);
			_logger.LogWarning("CategoryKey(key={Key},category={Category},storesIds={StoresIds}): Key → {Key1}", key, category, storesIds, Key);
            Category = category;
			_logger.LogDebug("<<< Exiting CategoryKey(key={Key},category={Category},storesIds={StoresIds})", key, category, storesIds);
        }

        public List<int> StoresIds { get; }

        public Category Category { get; }

        public string Key { get; }

        public bool Equals(CategoryKey y)
        {
			_logger.LogDebug(">>> Entering Equals(y={Y})", y);
            if (y == null)
                return false;

            if (Category != null && y.Category != null)
                return Category.Id == y.Category.Id;

            if ((StoresIds.Any() || y.StoresIds.Any())
                && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                return false;

            return Key.Equals(y.Key);
        }

        public override int GetHashCode()
        {
			_logger.LogDebug(">>> Entering GetHashCode()");
            if (!StoresIds.Any())
                return Key.GetHashCode();

            var storesIds = StoresIds.Select(id => id.ToString())
                .Aggregate(string.Empty, (all, current) => all + current);

			_logger.LogDebug("<<< Exiting GetHashCode()");
            return $"{storesIds}_{Key}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
			_logger.LogDebug("<<< Exiting Equals(obj={Obj})", obj);
            return obj is CategoryKey other && other.Equals(this);
        }
    }

    #endregion
}