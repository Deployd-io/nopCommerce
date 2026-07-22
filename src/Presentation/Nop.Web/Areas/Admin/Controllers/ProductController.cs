using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.FilterLevels;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ProductController : BaseAdminController
{
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly IAclService _aclService;
    protected readonly IArtificialIntelligenceService _artificialIntelligenceService;
    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICopyProductService _copyProductService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDiscountService _discountService;
    protected readonly IDownloadService _downloadService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExportManager _exportManager;
    protected readonly IFilterLevelValueModelFactory _filterLevelValueModelFactory;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly IImportManager _importManager;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPdfService _pdfService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly ISettingService _settingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITranslationModelFactory _translationModelFactory;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVideoService _videoService;
    protected readonly IWarehouseService _warehouseService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly CurrencySettings _currencySettings;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ProductController(AdminAreaSettings adminAreaSettings,
        CustomerSettings customerSettings,
        IAclService aclService,
        IArtificialIntelligenceService artificialIntelligenceService,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICategoryService categoryService,
        ICopyProductService copyProductService,
        ICurrencyService currencyService,
        ICustomerActivityService customerActivityService,
        IDiscountService discountService,
        IDownloadService downloadService,
        IEventPublisher eventPublisher,
        IExportManager exportManager,
        IFilterLevelValueModelFactory filterLevelValueModelFactory,
        IFilterLevelValueService filterLevelValueService,
        IHttpClientFactory httpClientFactory,
        IImportManager importManager,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocalizedEntityService localizedEntityService,
        IManufacturerService manufacturerService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IPdfService pdfService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IProductTagService productTagService,
        ISettingService settingService,
        IShoppingCartService shoppingCartService,
        ISpecificationAttributeService specificationAttributeService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITranslationModelFactory translationModelFactory,
        IUrlRecordService urlRecordService,
        IVideoService videoService,
        IWarehouseService warehouseService,
        IWebHelper webHelper,
        IWorkContext workContext,
        CurrencySettings currencySettings,
        LocalizationSettings localizationSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _adminAreaSettings = adminAreaSettings;
        _customerSettings = customerSettings;
        _aclService = aclService;
        _artificialIntelligenceService = artificialIntelligenceService;
        _backInStockSubscriptionService = backInStockSubscriptionService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _categoryService = categoryService;
        _copyProductService = copyProductService;
        _currencyService = currencyService;
        _customerActivityService = customerActivityService;
        _discountService = discountService;
        _downloadService = downloadService;
        _eventPublisher = eventPublisher;
        _exportManager = exportManager;
        _filterLevelValueModelFactory = filterLevelValueModelFactory;
        _filterLevelValueService = filterLevelValueService;
        _httpClientFactory = httpClientFactory;
        _importManager = importManager;
        _languageService = languageService;
        _localizationService = localizationService;
        _localizedEntityService = localizedEntityService;
        _manufacturerService = manufacturerService;
        _fileProvider = fileProvider;
        _notificationService = notificationService;
        _pdfService = pdfService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _productAttributeFormatter = productAttributeFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _productTagService = productTagService;
        _settingService = settingService;
        _shoppingCartService = shoppingCartService;
        _specificationAttributeService = specificationAttributeService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _translationModelFactory = translationModelFactory;
        _urlRecordService = urlRecordService;
        _videoService = videoService;
        _warehouseService = warehouseService;
        _webHelper = webHelper;
        _workContext = workContext;
        _currencySettings = currencySettings;
        _localizationSettings = localizationSettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task UpdateLocalesAsync(Product product, ProductModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.ShortDescription,
                localized.ShortDescription,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.FullDescription,
                localized.FullDescription,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(product,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            //search engine name
            var seName = await _urlRecordService.ValidateSeNameAsync(product, localized.SeName, localized.Name, false);
            await _urlRecordService.SaveSlugAsync(product, seName, localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(ProductTag productTag, ProductTagModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(productTag,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(productTag,
                x => x.MetaKeywords,
                localized.MetaKeywords,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(productTag,
                x => x.MetaDescription,
                localized.MetaDescription,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(productTag,
                x => x.MetaTitle,
                localized.MetaTitle,
                localized.LanguageId);

            var seName = await _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, localized.Name, false);
            await _urlRecordService.SaveSlugAsync(productTag, seName, localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(ProductAttributeMapping pam, ProductAttributeMappingModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(pam,
                x => x.TextPrompt,
                localized.TextPrompt,
                localized.LanguageId);
            await _localizedEntityService.SaveLocalizedValueAsync(pam,
                x => x.DefaultValue,
                localized.DefaultValue,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdateLocalesAsync(ProductAttributeValue pav, ProductAttributeValueModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(pav,
                x => x.Name,
                localized.Name,
                localized.LanguageId);
        }
    }

    protected virtual async Task UpdatePictureSeoNamesAsync(Product product)
    {
        foreach (var pp in await _productService.GetProductPicturesByProductIdAsync(product.Id))
            await _pictureService.SetSeoFilenameAsync(pp.PictureId, await _pictureService.GetPictureSeNameAsync(product.Name));
    }

    protected virtual async Task SaveCategoryMappingsAsync(Product product, ProductModel model)
    {
        var existingProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

        //delete categories
        var productCategoriesToDelete = existingProductCategories.Where(pc => !model.SelectedCategoryIds.Contains(pc.CategoryId)).ToList();
        await _categoryService.DeleteProductCategoriesAsync(productCategoriesToDelete);

        //add categories
        foreach (var categoryId in model.SelectedCategoryIds)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category is null)
                continue;

            if (!await _categoryService.CanVendorAddProductsAsync(category))
                continue;

            if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
            {
                //find next display order
                var displayOrder = 1;
                var existingCategoryMapping = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                if (existingCategoryMapping.Any())
                    displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                await _categoryService.InsertProductCategoryAsync(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId,
                    DisplayOrder = displayOrder
                });
            }
        }
    }

    protected virtual async Task SaveManufacturerMappingsAsync(Product product, ProductModel model)
    {
        var existingProductManufacturers = await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true);

        //delete manufacturers
        var productManufacturersToDelete = existingProductManufacturers.Where(pm => !model.SelectedManufacturerIds.Contains(pm.ManufacturerId)).ToList();
        await _manufacturerService.DeleteProductManufacturersAsync(productManufacturersToDelete);

        //add manufacturers
        foreach (var manufacturerId in model.SelectedManufacturerIds)
        {
            if (_manufacturerService.FindProductManufacturer(existingProductManufacturers, product.Id, manufacturerId) == null)
            {
                //find next display order
                var displayOrder = 1;
                var existingManufacturerMapping = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(manufacturerId, showHidden: true);
                if (existingManufacturerMapping.Any())
                    displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                await _manufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
                {
                    ProductId = product.Id,
                    ManufacturerId = manufacturerId,
                    DisplayOrder = displayOrder
                });
            }
        }
    }

    protected virtual async Task SaveDiscountMappingsAsync(Product product, ProductModel model)
    {
        var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);

        foreach (var discount in allDiscounts)
        {
            if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
            {
                //new discount
                if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                    await _productService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });
            }
            else
            {
                //remove discount
                if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
                    await _productService.DeleteDiscountProductMappingAsync(discountProductMapping);
            }
        }

        await _productService.UpdateProductAsync(product);
    }

    protected virtual async Task<string> GetAttributesXmlForProductAttributeCombinationAsync(IFormCollection form, List<string> warnings, int productId)
    {
        var attributesXml = string.Empty;

        //get product attribute mappings (exclude non-combinable attributes)
        var attributes = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId))
            .Where(productAttributeMapping => !productAttributeMapping.IsNonCombinable()).ToList();

        foreach (var attribute in attributes)
        {
            var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
            StringValues ctrlAttributes;

            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                    ctrlAttributes = form[controlId];
                    if (!string.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
                        if (selectedAttributeId > 0)
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.Checkboxes:
                    var cblAttributes = form[controlId].ToString();
                    if (!string.IsNullOrEmpty(cblAttributes))
                    {
                        foreach (var item in cblAttributes.Split(_separator,
                                     StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }

                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    ctrlAttributes = form[controlId];
                    if (!string.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                            attribute, enteredText);
                    }

                    break;
                case AttributeControlType.Datepicker:
                    var date = form[controlId + "_day"];
                    var month = form[controlId + "_month"];
                    var year = form[controlId + "_year"];
                    DateTime? selectedDate = null;
                    try
                    {
                        selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                    }
                    catch
                    {
                        //ignore any exception
                    }

                    if (selectedDate.HasValue)
                    {
                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                            attribute, selectedDate.Value.ToString("D"));
                    }

                    break;
                case AttributeControlType.FileUpload:
                    var requestForm = await Request.ReadFormAsync();
                    var httpPostedFile = requestForm.Files[controlId];
                    if (!string.IsNullOrEmpty(httpPostedFile?.FileName))
                    {
                        var fileSizeOk = true;
                        if (attribute.ValidationFileMaximumSize.HasValue)
                        {
                            //compare in bytes
                            var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                            if (httpPostedFile.Length > maxFileSizeBytes)
                            {
                                warnings.Add(string.Format(
                                    await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"),
                                    attribute.ValidationFileMaximumSize.Value));
                                fileSizeOk = false;
                            }
                        }

                        if (fileSizeOk)
                        {
                            //save an uploaded file
                            var download = new Download
                            {
                                DownloadGuid = Guid.NewGuid(),
                                UseDownloadUrl = false,
                                DownloadUrl = string.Empty,
                                DownloadBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile),
                                ContentType = httpPostedFile.ContentType,
                                Filename = _fileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                Extension = _fileProvider.GetFileExtension(httpPostedFile.FileName),
                                IsNew = true
                            };
                            await _downloadService.InsertDownloadAsync(download);

                            //save attribute
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, download.DownloadGuid.ToString());
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        //validate conditional attributes (if specified)
        foreach (var attribute in attributes)
        {
            var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributesXml);
            if (conditionMet.HasValue && !conditionMet.Value)
            {
                attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
            }
        }

        return attributesXml;
    }

    protected virtual async Task SaveProductWarehouseInventoryAsync(Product product, ProductModel model)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (model.ManageInventoryMethodId != (int)ManageInventoryMethod.ManageStock)
            return;

        if (!model.UseMultipleWarehouses)
            return;

        var warehouses = await _warehouseService.GetAllWarehousesAsync();

        var form = await Request.ReadFormAsync();
        var formData = form.ToDictionary(x => x.Key, x => x.Value.ToString());

        foreach (var warehouse in warehouses)
        {
            //parse stock quantity
            var stockQuantity = 0;
            foreach (var formKey in formData.Keys)
            {
                if (!formKey.Equals($"warehouse_qty_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                _ = int.TryParse(formData[formKey], out stockQuantity);
                break;
            }

            //parse reserved quantity
            var reservedQuantity = 0;
            foreach (var formKey in formData.Keys)
                if (formKey.Equals($"warehouse_reserved_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(formData[formKey], out reservedQuantity);
                    break;
                }

            //parse "used" field
            var used = false;
            foreach (var formKey in formData.Keys)
                if (formKey.Equals($"warehouse_used_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(formData[formKey], out var tmp);
                    used = tmp == warehouse.Id;
                    break;
                }

            //quantity change history message
            var message = $"{await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit")}";

            var existingPwI = (await _productService.GetAllProductWarehouseInventoryRecordsAsync(product.Id)).FirstOrDefault(x => x.WarehouseId == warehouse.Id);
            if (existingPwI != null)
            {
                if (used)
                {
                    var previousStockQuantity = existingPwI.StockQuantity;

                    //update existing record
                    existingPwI.StockQuantity = stockQuantity;
                    existingPwI.ReservedQuantity = reservedQuantity;
                    await _productService.UpdateProductWarehouseInventoryAsync(existingPwI);

                    //quantity change history
                    await _productService.AddStockQuantityHistoryEntryAsync(product, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                        existingPwI.WarehouseId, message);
                }
                else
                {
                    //delete. no need to store record for qty 0
                    await _productService.DeleteProductWarehouseInventoryAsync(existingPwI);

                    //quantity change history
                    await _productService.AddStockQuantityHistoryEntryAsync(product, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
                }
            }
            else
            {
                if (!used)
                    continue;

                //no need to insert a record for qty 0
                existingPwI = new ProductWarehouseInventory
                {
                    WarehouseId = warehouse.Id,
                    ProductId = product.Id,
                    StockQuantity = stockQuantity,
                    ReservedQuantity = reservedQuantity
                };

                await _productService.InsertProductWarehouseInventoryAsync(existingPwI);

                //quantity change history
                await _productService.AddStockQuantityHistoryEntryAsync(product, existingPwI.StockQuantity, existingPwI.StockQuantity,
                    existingPwI.WarehouseId, message);
            }
        }
		logger.LogDebug("<<< Exiting SaveProductWarehouseInventoryAsync(product={Product},model={Model})", product, model);
    }

    protected virtual async Task SaveConditionAttributesAsync(ProductAttributeMapping productAttributeMapping,
        ProductAttributeConditionModel model, IFormCollection form)
    {
        string attributesXml = null;
        if (model.EnableCondition)
        {
            var attribute = await _productAttributeService.GetProductAttributeMappingByIdAsync(model.SelectedProductAttributeId);
            if (attribute != null)
            {
                var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        var ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            //for conditions we should empty values save even when nothing is selected
                            //otherwise "attributesXml" will be empty
                            //hence we won't be able to find a selected attribute
                            attributesXml = _productAttributeParser.AddProductAttribute(null, attribute,
                                selectedAttributeId > 0 ? selectedAttributeId.ToString() : string.Empty);
                        }
                        else
                        {
                            //for conditions we should empty values save even when nothing is selected
                            //otherwise "attributesXml" will be empty
                            //hence we won't be able to find a selected attribute
                            attributesXml = _productAttributeParser.AddProductAttribute(null,
                                attribute, string.Empty);
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            var anyValueSelected = false;
                            foreach (var item in cblAttributes.ToString()
                                         .Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId <= 0)
                                    continue;

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                                anyValueSelected = true;
                            }

                            if (!anyValueSelected)
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(null,
                                    attribute, string.Empty);
                            }
                        }
                        else
                        {
                            //for conditions we should save empty values even when nothing is selected
                            //otherwise "attributesXml" will be empty
                            //hence we won't be able to find a selected attribute
                            attributesXml = _productAttributeParser.AddProductAttribute(null,
                                attribute, string.Empty);
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //these attribute types are supported as conditions
                        break;
                }
            }
        }

        productAttributeMapping.ConditionAttributeXml = attributesXml;
        await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
    }

    protected virtual async Task GenerateAttributeCombinationsAsync(Product product, IList<int> allowedAttributeIds = null)
    {
        var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true, allowedAttributeIds);
        foreach (var attributesXml in allAttributesXml)
        {
            var existingCombination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);

            //already exists?
            if (existingCombination != null)
                continue;

            //new one
            var warnings = new List<string>();
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true, true));
            if (warnings.Any())
                continue;

            //save combination
            var combination = new ProductAttributeCombination
            {
                ProductId = product.Id,
                AttributesXml = attributesXml,
                StockQuantity = 0,
                AllowOutOfStockOrders = false,
                Sku = null,
                ManufacturerPartNumber = null,
                Gtin = null,
                OverriddenPrice = null,
                NotifyAdminForQuantityBelow = 1
            };
            await _productAttributeService.InsertProductAttributeCombinationAsync(combination);
        }
    }

    protected virtual async Task PingVideoUrlAsync(string videoUrl)
    {
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var path = videoUrl.StartsWith('/')
            ? $"{_webHelper.GetStoreLocation()}{videoUrl.TrimStart('/')}"
            : videoUrl;

        var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
        await client.GetStringAsync(path);
    }

    protected virtual async Task SaveAttributeCombinationPicturesAsync(Product product, ProductAttributeCombination combination, ProductAttributeCombinationModel model)
    {
        var existingCombinationPictures = await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id);
        var productPictureIds = (await _pictureService.GetPicturesByProductIdAsync(product.Id)).Select(p => p.Id).ToList();

        //delete manufacturers
        foreach (var existingCombinationPicture in existingCombinationPictures)
            if (!model.PictureIds.Contains(existingCombinationPicture.PictureId) || !productPictureIds.Contains(existingCombinationPicture.PictureId))
                await _productAttributeService.DeleteProductAttributeCombinationPictureAsync(existingCombinationPicture);

        //add manufacturers
        foreach (var pictureId in model.PictureIds)
        {
            if (!productPictureIds.Contains(pictureId))
                continue;

            if (_productAttributeService.FindProductAttributeCombinationPicture(existingCombinationPictures, combination.Id, pictureId) == null)
            {
                await _productAttributeService.InsertProductAttributeCombinationPictureAsync(new ProductAttributeCombinationPicture
                {
                    ProductAttributeCombinationId = combination.Id,
                    PictureId = pictureId
                });
            }
        }
    }

    protected virtual async Task SaveAttributeValuePicturesAsync(Product product, ProductAttributeValue value, ProductAttributeValueModel model)
    {
        var existingValuePictures = await _productAttributeService.GetProductAttributeValuePicturesAsync(value.Id);
        var productPictureIds = (await _pictureService.GetPicturesByProductIdAsync(product.Id)).Select(p => p.Id).ToList();

        //delete product attribute value picture
        var existingPicturesToDelete = existingValuePictures.Where(pavp => !model.PictureIds.Contains(pavp.PictureId) || !productPictureIds.Contains(pavp.PictureId)).ToList();
        await _productAttributeService.DeleteProductAttributeValuePicturesAsync(existingPicturesToDelete);

        //add product attribute value picture
        foreach (var pictureId in model.PictureIds)
        {
            if (!productPictureIds.Contains(pictureId))
                continue;

            if (_productAttributeService.FindProductAttributeValuePicture(existingValuePictures, value.Id, pictureId) == null)
            {
                await _productAttributeService.InsertProductAttributeValuePictureAsync(new ProductAttributeValuePicture
                {
                    ProductAttributeValueId = value.Id,
                    PictureId = pictureId
                });
            }
        }
    }

    protected virtual async Task<List<BulkEditData>> ParseBulkEditDataAsync()
    {
		logger.LogDebug(">>> Entering ParseBulkEditDataAsync()");
        var rez = new Dictionary<int, BulkEditData>();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		logger.LogDebug("ParseBulkEditDataAsync(): rez → {Rez}", rez);
		logger.LogDebug("ParseBulkEditDataAsync(): currentVendor → {CurrentVendor}", currentVendor);

        foreach (var item in Request.Form)
        {
            if (getData(item, "product-select-", out var productId))
            {
                setData(productId, data =>
                {
                    data.IsSelected = true;
					logger.LogDebug("ParseBulkEditDataAsync(): data.IsSelected → {Arg0}", data.IsSelected);
                });
            }

            if (getData(item, "name-", out productId))
            {
                setData(productId, data =>
                {
                    data.Name = item.Value;
					logger.LogDebug("ParseBulkEditDataAsync(): data.Name → {Arg0}", data.Name);
                });
            }

            if (getData(item, "sku-", out productId))
            {
                setData(productId, data =>
                {
                    data.Sku = item.Value;
					logger.LogDebug("ParseBulkEditDataAsync(): data.Sku → {Arg0}", data.Sku);
                });
            }

            if (getData(item, "price-", out productId))
            {
                setData(productId, data =>
                {
                    data.Price = decimal.Parse(item.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                });
            }

            if (getData(item, "old-price-", out productId))
            {
                setData(productId, data =>
                {
                    data.OldPrice = decimal.Parse(item.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                });
            }

            if (getData(item, "quantity-", out productId))
            {
                setData(productId, data =>
                {
                    data.Quantity = int.Parse(item.Value);
                });
            }

            if (getData(item, "published-", out productId))
            {
                setData(productId, data =>
                {
                    data.IsPublished = true;
					logger.LogDebug("ParseBulkEditDataAsync(): data.IsPublished → {Arg0}", data.IsPublished);
                });
            }
        }

        var productIds = rez.Select(p => p.Key).ToArray();

		logger.LogDebug("ParseBulkEditDataAsync(): productIds → {ProductIds}", productIds);
        var products = await _productService.GetProductsByIdsAsync(productIds);

		logger.LogDebug("ParseBulkEditDataAsync(): products → {Products}", products);
        foreach (var product in products)
            rez[product.Id].Product = product;

        return rez.Values.ToList();

        bool getData(KeyValuePair<string, StringValues> item, string selector, out int productId)
        {
            var key = item.Key;
            productId = 0;
			logger.LogDebug("ParseBulkEditDataAsync(): productId → {ProductId}", productId);

            if (!key.StartsWith(selector))
                return false;

            productId = int.Parse(key.Replace(selector, string.Empty));

            return true;
        }

        void setData(int productId, Action<BulkEditData> action)
        {
            if (!rez.ContainsKey(productId))
                rez.Add(productId, new BulkEditData(_taxSettings.DefaultTaxCategoryId, currentVendor?.Id ?? 0));

            action(rez[productId]);
        }
		logger.LogDebug("<<< Exiting ParseBulkEditDataAsync()");
    }

    #endregion

    #region Methods

    #region Product list / create / edit / delete

    public virtual IActionResult Index()
    {
		logger.LogDebug(">>> Entering Index()");
		logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _productModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());

		logger.LogDebug("List(): model → {Model}", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> BulkEdit()
    {
		logger.LogDebug(">>> Entering BulkEdit()");
        //prepare model
        var model = await _productModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());
        model.Length = _adminAreaSettings.ProductsBulkEditGridPageSize;
		logger.LogDebug("BulkEdit(): model → {Model}", model);
		logger.LogDebug("BulkEdit(): model.Length → {Arg0}", model.Length);

		logger.LogDebug(">>> Entering Edit(id={Id})", id);
		logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ActionName("BulkEdit"), ParameterBasedOnFormName("bulk-edit-save-selected", "selected")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> BulkEditSave(ProductSearchModel searchModel, bool selected)
    {
		logger.LogDebug(">>> Entering BulkEditSave(searchModel={SearchModel},selected={Selected})", searchModel, selected);
        var data = await ParseBulkEditDataAsync();

		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): data → {Data}", searchModel, selected, data);
        var productsToUpdate = data.Where(d => d.NeedToUpdate(selected));
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): productsToUpdate → {ProductsToUpdate}", searchModel, selected, productsToUpdate);
		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): currentVendor → {CurrentVendor}", searchModel, selected, currentVendor);
        
        if (currentVendor != null)
            productsToUpdate = productsToUpdate.Where(p => p.Product.VendorId == currentVendor.Id);

        await _productService.UpdateProductsAsync(productsToUpdate.Select(d => d.UpdateProduct(selected)).ToList());

        var productsToInsert = data.Where(d => d.NeedToCreate(selected));
        await _productService.InsertProductsAsync(productsToInsert.Select(d => d.CreateProduct(selected)).ToList());
		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): productsToInsert → {ProductsToInsert}", searchModel, selected, productsToInsert);

        //prepare model
        var model = await _productModelFactory.PrepareProductSearchModelAsync(searchModel);
        model.Length = _adminAreaSettings.ProductsBulkEditGridPageSize;
		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): model → {Model}", searchModel, selected, model);
		logger.LogDebug("BulkEditSave(searchModel={SearchModel},selected={Selected}): model.Length → {Arg2}", searchModel, selected, model.Length);

		logger.LogDebug("<<< Exiting BulkEditSave(searchModel={SearchModel},selected={Selected})", searchModel, selected);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductList(ProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareProductListModelAsync(searchModel);

		logger.LogDebug("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> BulkEditProducts(ProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering BulkEditProducts(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareProductListModelAsync(searchModel);
        var html = await RenderPartialViewToStringAsync("_BulkEdit.Products", model.Data.ToList());
		logger.LogDebug("BulkEditProducts(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("BulkEditProducts(searchModel={SearchModel}): html → {Html}", searchModel, html);

		logger.LogDebug("<<< Exiting BulkEditProducts(searchModel={SearchModel})", searchModel);
        return Json(new Dictionary<string, object> { { "Html", html }, { "Products", model } });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> BulkEditNewProduct(int id)
    {
		logger.LogDebug(">>> Entering BulkEditNewProduct(id={Id})", id);
        var primaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

		logger.LogDebug("BulkEditNewProduct(id={Id}): primaryStoreCurrencyCode → {PrimaryStoreCurrencyCode}", id, primaryStoreCurrencyCode);
        //prepare model
        var model = new List<ProductModel> { new()
        {
            Id = id,
            PrimaryStoreCurrencyCode = primaryStoreCurrencyCode,
            Published = true
			logger.LogDebug("BulkEditNewProduct(id={Id}): PrimaryStoreCurrencyCode → {PrimaryStoreCurrencyCode}", id, PrimaryStoreCurrencyCode);
			logger.LogDebug("BulkEditNewProduct(id={Id}): Published → {Published}", id, Published);
        } };

        var html = await RenderPartialViewToStringAsync("_BulkEdit.Products", model);

		logger.LogDebug("BulkEditNewProduct(id={Id}): html → {Html}", id, html);
		logger.LogDebug("<<< Exiting BulkEditNewProduct(id={Id})", id);
        return Json(html);
    }

    [HttpPost, ActionName("List")]
    [FormValueRequired("go-to-product-by-sku")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> GoToSku(ProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering GoToSku(searchModel={SearchModel})", searchModel);
        //try to load a product entity, if not found, then try to load a product attribute combination
        var productId = (await _productService.GetProductBySkuAsync(searchModel.GoDirectlyToSku))?.Id
            ?? (await _productAttributeService.GetProductAttributeCombinationBySkuAsync(searchModel.GoDirectlyToSku))?.ProductId;

        if (productId != null)
		logger.LogDebug("<<< Exiting GoToSku(searchModel={SearchModel})", searchModel);
            return RedirectToAction("Edit", "Product", new { id = productId });

        //not found
        return await List();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		logger.LogDebug(">>> Entering Create()");
        //validate maximum number of products per vendor
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (_vendorSettings.MaximumProductNumber > 0 &&
			logger.LogDebug("Create(): currentVendor → {CurrentVendor}", currentVendor);
            currentVendor != null &&
            await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumProductNumber)
        {
            _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
                _vendorSettings.MaximumProductNumber));
            return RedirectToAction("List");
        }

        //prepare model
        var model = await _productModelFactory.PrepareProductModelAsync(new ProductModel(), null);

		logger.LogDebug("Create(): model → {Model}", model);
		logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(ProductModel model, bool continueEditing)
    {
        //validate maximum number of products per vendor
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (_vendorSettings.MaximumProductNumber > 0 &&
            currentVendor != null &&
            await _productService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumProductNumber)
        {
            _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
                _vendorSettings.MaximumProductNumber));
            return RedirectToAction("List");
        }

        if (ModelState.IsValid)
        {
            //a vendor should have access only to his products
            if (currentVendor != null)
                model.VendorId = currentVendor.Id;

            //vendors cannot edit "Show on home page" property
            if (currentVendor != null && model.ShowOnHomepage)
                model.ShowOnHomepage = false;

            //product
            var product = model.ToEntity<Product>();
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.InsertProductAsync(product);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(product, model);

            //categories
            await SaveCategoryMappingsAsync(product, model);

            //manufacturers
            await SaveManufacturerMappingsAsync(product, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(product, model.SelectedStoreIds);

            //discounts
            await SaveDiscountMappingsAsync(product, model);

            //tags
            await _productTagService.UpdateProductTagsAsync(product, model.SelectedProductTags.ToArray());

            //warehouses
            await SaveProductWarehouseInventoryAsync(product, model);

            //quantity change history
            await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
                await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewProduct",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewProduct"), product.Name), product);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = product.Id });
        }

        //prepare model
        model = await _productModelFactory.PrepareProductModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null || product.Deleted)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //prepare model
        var model = await _productModelFactory.PrepareProductModelAsync(null, product);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslate(int itemId)
    {
		logger.LogDebug(">>> Entering PreTranslate(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		logger.LogDebug("PreTranslate(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(itemId);
        if (product == null || product.Deleted)
			logger.LogDebug("PreTranslate(itemId={ItemId}): product → {Product}", itemId, product);
            return Json(translationModel);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("PreTranslate(itemId={ItemId}): currentVendor → {CurrentVendor}", itemId, currentVendor);
            return Json(translationModel);

        //prepare model
        var model = await _productModelFactory.PrepareProductModelAsync(null, product);

		logger.LogDebug("PreTranslate(itemId={ItemId}): model → {Model}", itemId, model);
        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model,
            (nameof(ProductLocalizedModel.Name), false),
            (nameof(ProductLocalizedModel.ShortDescription), false),
            (nameof(ProductLocalizedModel.FullDescription), true));

		logger.LogDebug("<<< Exiting PreTranslate(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ProductModel model, bool continueEditing)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(model.Id);
        if (product == null || product.Deleted)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //check if the product quantity has been changed while we were editing the product
        //and if it has been changed then we show error notification
        //and redirect on the editing page without data saving
        if (product.StockQuantity != model.LastStockQuantity)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
            return RedirectToAction("Edit", new { id = product.Id });
        }

        if (ModelState.IsValid)
        {
            //a vendor should have access only to his products
            if (currentVendor != null)
                model.VendorId = currentVendor.Id;

            //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
            //vendors cannot edit "Show on home page" property
            if (currentVendor != null && model.ShowOnHomepage != product.ShowOnHomepage)
                model.ShowOnHomepage = product.ShowOnHomepage;

            //some previously used values
            var prevTotalStockQuantity = await _productService.GetTotalStockQuantityAsync(product);
            var prevDownloadId = product.DownloadId;
            var prevSampleDownloadId = product.SampleDownloadId;
            var previousStockQuantity = product.StockQuantity;
            var previousWarehouseId = product.WarehouseId;
            var previousProductType = product.ProductType;

            //product
            product = model.ToEntity(product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.UpdateProductAsync(product);

            //remove associated products
            if (previousProductType == ProductType.GroupedProduct && product.ProductType == ProductType.SimpleProduct)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var storeId = store?.Id ?? 0;
                var vendorId = currentVendor?.Id ?? 0;

                var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, storeId, vendorId);
                foreach (var associatedProduct in associatedProducts)
                {
                    associatedProduct.ParentGroupedProductId = 0;
                    await _productService.UpdateProductAsync(associatedProduct);
                }
            }

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
            await _urlRecordService.SaveSlugAsync(product, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(product, model);

            //tags
            await _productTagService.UpdateProductTagsAsync(product, model.SelectedProductTags.ToArray());

            //warehouses
            await SaveProductWarehouseInventoryAsync(product, model);

            //categories
            await SaveCategoryMappingsAsync(product, model);

            //manufacturers
            await SaveManufacturerMappingsAsync(product, model);

            //stores
            await _storeMappingService.SaveStoreMappingsAsync(product, model.SelectedStoreIds);

            //discounts
            await SaveDiscountMappingsAsync(product, model);

            //picture seo names
            await UpdatePictureSeoNamesAsync(product);

            //back in stock notifications
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                await _productService.GetTotalStockQuantityAsync(product) > 0 &&
                prevTotalStockQuantity <= 0 &&
                product.Published &&
                !product.Deleted)
            {
                await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
            }

            //delete an old "download" file (if deleted or updated)
            if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
            {
                var prevDownload = await _downloadService.GetDownloadByIdAsync(prevDownloadId);
                if (prevDownload != null)
                    await _downloadService.DeleteDownloadAsync(prevDownload);
            }

            //delete an old "sample download" file (if deleted or updated)
            if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
            {
                var prevSampleDownload = await _downloadService.GetDownloadByIdAsync(prevSampleDownloadId);
                if (prevSampleDownload != null)
                    await _downloadService.DeleteDownloadAsync(prevSampleDownload);
            }

            //quantity change history
            if (previousWarehouseId != product.WarehouseId)
            {
                //warehouse is changed 
                //compose a message
                var oldWarehouseMessage = string.Empty;
                if (previousWarehouseId > 0)
                {
                    var oldWarehouse = await _warehouseService.GetWarehouseByIdAsync(previousWarehouseId);
                    if (oldWarehouse != null)
                        oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                }

                var newWarehouseMessage = string.Empty;
                if (product.WarehouseId > 0)
                {
                    var newWarehouse = await _warehouseService.GetWarehouseByIdAsync(product.WarehouseId);
                    if (newWarehouse != null)
                        newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                }

                var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                //record history
                await _productService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
            }
            else
            {
                await _productService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                    product.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("EditProduct",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProduct"), product.Name), product);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = product.Id });
        }

        //prepare model
        model = await _productModelFactory.PrepareProductModelAsync(model, product, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
			logger.LogDebug("Delete(id={Id}): product → {Product}", id, product);
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("Delete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return RedirectToAction("List");

        await _productService.DeleteProductAsync(product);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteProduct",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProduct"), product.Name), product);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Deleted"));

		logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var currentVendor = await _workContext.GetCurrentVendorAsync();

		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): currentVendor → {CurrentVendor}", selectedIds, currentVendor);
        var products = (await _productService.GetProductsByIdsAsync(selectedIds.ToArray()))
            .Where(p => currentVendor == null || p.VendorId == currentVendor.Id).ToList();

        await _productService.DeleteProductsAsync(products);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteProduct");

		logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        foreach (var product in products)
            await _customerActivityService.InsertActivityAsync("DeleteProduct",
                string.Format(activityLogFormat, product.Name), product);

		logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CopyProduct(ProductModel model)
    {
        var copyModel = model.CopyProductModel;
        try
        {
            var originalProduct = await _productService.GetProductByIdAsync(copyModel.Id);

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && originalProduct.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            var newProduct = await _copyProductService.CopyProductAsync(originalProduct, copyModel.Name, copyModel.Published, copyModel.CopyMultimedia);

            //publishing post copy product event
            await _eventPublisher.PublishAsync(new PostCopyProductEvent(originalProduct, newProduct));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Copied"));

            return RedirectToAction("Edit", new { id = newProduct.Id });
        }
        catch (Exception exc)
        {
            _notificationService.ErrorNotification(exc.Message);
            return RedirectToAction("Edit", new { id = copyModel.Id });
        }
    }

    //action displaying notification (warning) to a store owner that entered SKU already exists
    public virtual async Task<IActionResult> SkuReservedWarning(int productId, string sku)
    {
        string message;

        //check whether product with passed SKU already exists
        var productBySku = await _productService.GetProductBySkuAsync(sku);
        if (productBySku != null)
        {
            if (productBySku.Id == productId)
                return Json(new { Result = string.Empty });

            message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Sku.Reserved"), productBySku.Name);
            return Json(new { Result = message });
        }

        //check whether combination with passed SKU already exists
        var combinationBySku = await _productAttributeService.GetProductAttributeCombinationBySkuAsync(sku);
        if (combinationBySku == null)
            return Json(new { Result = string.Empty });

        message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved"),
            (await _productService.GetProductByIdAsync(combinationBySku.ProductId))?.Name);

        return Json(new { Result = message });
    }

    //action displaying notification (warning) to a store owner that 'Date of Birth' is disabled
    public virtual async Task<IActionResult> CustomersDateOfBirthDisabledWarning()
    {
		logger.LogDebug(">>> Entering CustomersDateOfBirthDisabledWarning()");
        if (_customerSettings.DateOfBirthEnabled)
            return Json(new { Result = string.Empty });

        var warning = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.AgeVerification.DateOfBirthDisabled"),
            Url.Action("CustomerUser", "Setting"));

		logger.LogDebug("<<< Exiting CustomersDateOfBirthDisabledWarning()");
        return Json(new { Result = warning });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FullDescriptionGeneratorPopup(int languageId, string productName)
    {
		logger.LogDebug(">>> Entering FullDescriptionGeneratorPopup(languageId={LanguageId},productName={ProductName})", languageId, productName);
        var model = new ArtificialIntelligenceFullDescriptionModel
        {
            ProductName = productName,
            LanguageId = languageId,
            TargetLanguageId = languageId == 0 ? _localizationSettings.DefaultAdminLanguageId : languageId
        };

        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

		logger.LogDebug("<<< Exiting FullDescriptionGeneratorPopup(languageId={LanguageId},productName={ProductName})", languageId, productName);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FullDescriptionGeneratorPopup(ArtificialIntelligenceFullDescriptionModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.SaveButtonClicked)
        {
            ViewBag.SaveDescription = true;
        }
        else
        {
            try
            {
                model.GeneratedDescription = await _artificialIntelligenceService.CreateProductDescriptionAsync(
                    model.ProductName, model.Keywords, (ToneOfVoiceType)model.ToneOfVoiceId, model.Instructions,
                    model.CustomToneOfVoice, model.TargetLanguageId);
            }
            catch (NopException ex)
            {
                model.GeneratedDescription = ex.Message;
            }
        }

        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        return View(model);
    }

    #endregion

    #region Required products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> LoadProductFriendlyNames(string productIds)
    {
        var result = string.Empty;

        if (string.IsNullOrWhiteSpace(productIds))
            return Json(new { Text = result });

        var ids = new List<int>();
        var rangeArray = productIds
            .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();

        foreach (var str1 in rangeArray)
        {
            if (int.TryParse(str1, out var tmp1))
                ids.Add(tmp1);
        }

        var products = await _productService.GetProductsByIdsAsync(ids.ToArray());
        for (var i = 0; i <= products.Count - 1; i++)
        {
            result += products[i].Name;
            if (i != products.Count - 1)
                result += ", ";
        }

        return Json(new { Text = result });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RequiredProductAddPopup()
    {
		logger.LogDebug(">>> Entering RequiredProductAddPopup()");
        //prepare model
        var model = await _productModelFactory.PrepareAddRequiredProductSearchModelAsync(new AddRequiredProductSearchModel());

		logger.LogDebug("RequiredProductAddPopup(): model → {Model}", model);
		logger.LogDebug("<<< Exiting RequiredProductAddPopup()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RequiredProductAddPopupList(AddRequiredProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering RequiredProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareAddRequiredProductListModelAsync(searchModel);

		logger.LogDebug("RequiredProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting RequiredProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Related products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> RelatedProductList(RelatedProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering RelatedProductList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("RelatedProductList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareRelatedProductListModelAsync(searchModel, product);

		logger.LogDebug("RelatedProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting RelatedProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RelatedProductUpdate(RelatedProductModel model)
    {
        //try to get a related product with the specified id
        var relatedProduct = await _productService.GetRelatedProductByIdAsync(model.Id)
            ?? throw new ArgumentException("No related product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            var product = await _productService.GetProductByIdAsync(relatedProduct.ProductId1);
            if (product != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");
        }

        relatedProduct.DisplayOrder = model.DisplayOrder;
        await _productService.UpdateRelatedProductAsync(relatedProduct);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RelatedProductDelete(int id)
    {
		logger.LogDebug(">>> Entering RelatedProductDelete(id={Id})", id);
        //try to get a related product with the specified id
        var relatedProduct = await _productService.GetRelatedProductByIdAsync(id)
            ?? throw new ArgumentException("No related product found with the specified id");

        var productId = relatedProduct.ProductId1;

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("RelatedProductDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null && product.VendorId != currentVendor.Id)
				logger.LogDebug("RelatedProductDelete(id={Id}): product → {Product}", id, product);
                return Content("This is not your product");
        }

        await _productService.DeleteRelatedProductAsync(relatedProduct);

		logger.LogDebug("<<< Exiting RelatedProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RelatedProductAddPopup(int productId)
    {
        //prepare model
        var model = await _productModelFactory.PrepareAddRelatedProductSearchModelAsync(new AddRelatedProductSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RelatedProductAddPopupList(AddRelatedProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering RelatedProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareAddRelatedProductListModelAsync(searchModel);

		logger.LogDebug("RelatedProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting RelatedProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RelatedProductAddPopup(AddRelatedProductModel model)
    {
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingRelatedProducts = await _productService.GetRelatedProductsByProductId1Async(model.ProductId, showHidden: true);
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            foreach (var product in selectedProducts)
            {
                //a vendor should have access only to his products
                if (currentVendor != null && product.VendorId != currentVendor.Id)
                    continue;

                if (_productService.FindRelatedProduct(existingRelatedProducts, model.ProductId, product.Id) != null)
                    continue;

                await _productService.InsertRelatedProductAsync(new RelatedProduct
                {
                    ProductId1 = model.ProductId,
                    ProductId2 = product.Id,
                    DisplayOrder = 1
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddRelatedProductSearchModel());
    }

    #endregion

    #region Cross-sell products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> CrossSellProductList(CrossSellProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CrossSellProductList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("CrossSellProductList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareCrossSellProductListModelAsync(searchModel, product);

		logger.LogDebug("CrossSellProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting CrossSellProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CrossSellProductDelete(int id)
    {
		logger.LogDebug(">>> Entering CrossSellProductDelete(id={Id})", id);
        //try to get a cross-sell product with the specified id
        var crossSellProduct = await _productService.GetCrossSellProductByIdAsync(id)
            ?? throw new ArgumentException("No cross-sell product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("CrossSellProductDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        {
            var product = await _productService.GetProductByIdAsync(crossSellProduct.ProductId1);
            if (product != null && product.VendorId != currentVendor.Id)
				logger.LogDebug("CrossSellProductDelete(id={Id}): product → {Product}", id, product);
                return Content("This is not your product");
        }

        await _productService.DeleteCrossSellProductAsync(crossSellProduct);

		logger.LogDebug("<<< Exiting CrossSellProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CrossSellProductAddPopup(int productId)
    {
        //prepare model
        var model = await _productModelFactory.PrepareAddCrossSellProductSearchModelAsync(new AddCrossSellProductSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CrossSellProductAddPopupList(AddCrossSellProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering CrossSellProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareAddCrossSellProductListModelAsync(searchModel);

		logger.LogDebug("CrossSellProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting CrossSellProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CrossSellProductAddPopup(AddCrossSellProductModel model)
    {
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            var existingCrossSellProducts = await _productService.GetCrossSellProductsByProductId1Async(model.ProductId, showHidden: true);
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            foreach (var product in selectedProducts)
            {
                //a vendor should have access only to his products
                if (currentVendor != null && product.VendorId != currentVendor.Id)
                    continue;

                if (_productService.FindCrossSellProduct(existingCrossSellProducts, model.ProductId, product.Id) != null)
                    continue;

                await _productService.InsertCrossSellProductAsync(new CrossSellProduct
                {
                    ProductId1 = model.ProductId,
                    ProductId2 = product.Id
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddCrossSellProductSearchModel());
    }

    #endregion

    #region Filter level values

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_VIEW)]
    public virtual async Task<IActionResult> FilterLevelValueList(FilterLevelValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering FilterLevelValueList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("FilterLevelValueList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareFilterLevelValueListModelAsync(searchModel, product);

		logger.LogDebug("FilterLevelValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting FilterLevelValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FilterLevelValueDelete(int productId, int id)
    {
        //try to get a filter level value mapping with the specified id
        var existingProductFilterLevelValues = await _filterLevelValueService.GetFilterLevelValueProductsByFilterLevelValueIdAsync(id);

        var filterLevelValueMapping = existingProductFilterLevelValues.FirstOrDefault(pc => pc.ProductId == productId && pc.FilterLevelValueId == id)
            ?? throw new ArgumentException("No filter level value mapping found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            var product = await _productService.GetProductByIdAsync(filterLevelValueMapping.ProductId);
            if (product != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");
        }

        await _filterLevelValueService.DeleteFilterLevelValueProductAsync(filterLevelValueMapping);

        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FilterLevelValuesAddPopup(int productId)
    {
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueSearchModelAsync(new FilterLevelValueSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FilterLevelValuesAddPopupList(FilterLevelValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering FilterLevelValuesAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _filterLevelValueModelFactory.PrepareFilterLevelValueListModelAsync(searchModel);

		logger.LogDebug("FilterLevelValuesAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting FilterLevelValuesAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.FILTER_LEVEL_VALUE_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> FilterLevelValuesAddPopup(AddFilterLevelValueModel model)
    {
        var selectedFilterLevelValues = await _filterLevelValueService.GetFilterLevelValuesByIdsAsync(model.SelectedFilterLevelValueIds.ToArray());
        if (selectedFilterLevelValues.Any())
        {
            foreach (var filterLevelValue in selectedFilterLevelValues)
            {
                //whether product filter level value with such parameters already exists
                var existingProductFilterLevelValues = await _filterLevelValueService.GetFilterLevelValueProductsByFilterLevelValueIdAsync(filterLevelValue.Id);
                if (existingProductFilterLevelValues.FirstOrDefault(pc => pc.ProductId == model.ProductId && pc.FilterLevelValueId == filterLevelValue.Id) != null)
                    continue;

                await _filterLevelValueService.InsertProductFilterLevelValueAsync(new FilterLevelValueProductMapping
                {
                    FilterLevelValueId = filterLevelValue.Id,
                    ProductId = model.ProductId
                });
            }
        }

        ViewBag.RefreshPage = true;

        return View(new FilterLevelValueSearchModel());
    }

    #endregion

    #region Associated products

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> AssociatedProductList(AssociatedProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering AssociatedProductList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("AssociatedProductList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareAssociatedProductListModelAsync(searchModel, product);

		logger.LogDebug("AssociatedProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting AssociatedProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociatedProductUpdate(AssociatedProductModel model)
    {
        //try to get an associated product with the specified id
        var associatedProduct = await _productService.GetProductByIdAsync(model.Id)
            ?? throw new ArgumentException("No associated product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
            return Content("This is not your product");

        associatedProduct.DisplayOrder = model.DisplayOrder;
        await _productService.UpdateProductAsync(associatedProduct);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociatedProductDelete(int id)
    {
		logger.LogDebug(">>> Entering AssociatedProductDelete(id={Id})", id);
        //try to get an associated product with the specified id
        var product = await _productService.GetProductByIdAsync(id)
            ?? throw new ArgumentException("No associated product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("AssociatedProductDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return Content("This is not your product");

        product.ParentGroupedProductId = 0;
		logger.LogDebug("AssociatedProductDelete(id={Id}): product.ParentGroupedProductId → {Arg1}", id, product.ParentGroupedProductId);
        await _productService.UpdateProductAsync(product);

		logger.LogDebug("<<< Exiting AssociatedProductDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociatedProductAddPopup(int productId)
    {
        //prepare model
        var model = await _productModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociatedProductAddPopupList(AddAssociatedProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering AssociatedProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareAddAssociatedProductListModelAsync(searchModel);

		logger.LogDebug("AssociatedProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting AssociatedProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociatedProductAddPopup(AddAssociatedProductModel model)
    {
        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());

        var tryToAddSelfGroupedProduct = selectedProducts
            .Select(p => p.Id)
            .Contains(model.ProductId);

        if (selectedProducts.Any())
        {
            foreach (var product in selectedProducts)
            {
                if (product.Id == model.ProductId)
                    continue;

                //a vendor should have access only to his products
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor != null && product.VendorId != currentVendor.Id)
                    continue;

                product.ParentGroupedProductId = model.ProductId;
                await _productService.UpdateProductAsync(product);
            }
        }

        if (tryToAddSelfGroupedProduct)
        {
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct"));

            var addAssociatedProductSearchModel = await _productModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());
            //set current product id
            addAssociatedProductSearchModel.ProductId = model.ProductId;

            ViewBag.RefreshPage = true;

            return View(addAssociatedProductSearchModel);
        }

        ViewBag.RefreshPage = true;

        ViewBag.ClosePage = true;

        return View(new AddAssociatedProductSearchModel());
    }

    #endregion

    #region Product pictures

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductPictureAdd(int productId, IFormCollection form)
    {
        if (productId == 0)
            throw new ArgumentException();

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        var files = form.Files.ToList();
        if (!files.Any())
            return Json(new { success = false });

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            if (product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            var existingPictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            if (existingPictures.Count >= _vendorSettings.MaximumProductPicturesNumber)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Pictures.Alert.VendorNumberPicturesLimit"),
                });
            }
        }

        try
        {
            foreach (var file in files)
            {
                //insert picture
                var picture = await _pictureService.InsertPictureAsync(file);

                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(product.Name));

                await _productService.InsertProductPictureAsync(new ProductPicture
                {
                    PictureId = picture.Id,
                    ProductId = product.Id,
                    DisplayOrder = 0
                });
            }
        }
        catch (Exception exc)
        {
            return Json(new
            {
                success = false,
                message = $"{await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Pictures.Alert.PictureAdd")} {exc.Message}",
            });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductPictureList(ProductPictureSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductPictureList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductPictureList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductPictureListModelAsync(searchModel, product);

		logger.LogDebug("ProductPictureList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductPictureList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductPictureUpdate(ProductPictureModel model)
    {
        //try to get a product picture with the specified id
        var productPicture = await _productService.GetProductPictureByIdAsync(model.Id)
            ?? throw new ArgumentException("No product picture found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            var product = await _productService.GetProductByIdAsync(productPicture.ProductId);
            if (product != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");
        }

        //try to get a picture with the specified id
        var picture = await _pictureService.GetPictureByIdAsync(productPicture.PictureId)
            ?? throw new ArgumentException("No picture found with the specified id");

        await _pictureService.UpdatePictureAsync(picture.Id,
            await _pictureService.LoadPictureBinaryAsync(picture),
            picture.MimeType,
            picture.SeoFilename,
            model.OverrideAltAttribute,
            model.OverrideTitleAttribute);

        productPicture.DisplayOrder = model.DisplayOrder;
        await _productService.UpdateProductPictureAsync(productPicture);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductPictureDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductPictureDelete(id={Id})", id);
        //try to get a product picture with the specified id
        var productPicture = await _productService.GetProductPictureByIdAsync(id)
            ?? throw new ArgumentException("No product picture found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("ProductPictureDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        {
            var product = await _productService.GetProductByIdAsync(productPicture.ProductId);
            if (product != null && product.VendorId != currentVendor.Id)
				logger.LogDebug("ProductPictureDelete(id={Id}): product → {Product}", id, product);
                return Content("This is not your product");
        }

        var pictureId = productPicture.PictureId;
        await _productService.DeleteProductPictureAsync(productPicture);

        //try to get a picture with the specified id
        var picture = await _pictureService.GetPictureByIdAsync(pictureId)
            ?? throw new ArgumentException("No picture found with the specified id");

        await _pictureService.DeletePictureAsync(picture);

		logger.LogDebug("<<< Exiting ProductPictureDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Product videos

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductVideoAdd(int productId, [Validate] ProductVideoModel model)
    {
        if (productId == 0)
            throw new ArgumentException();

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        if (string.IsNullOrEmpty(model.VideoUrl))
            ModelState.AddModelError(string.Empty,
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Videos.Alert.VideoAdd.EmptyUrl"));

        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var videoUrl = model.VideoUrl.TrimStart('~');

        try
        {
            await PingVideoUrlAsync(videoUrl);
        }
        catch (Exception exc)
        {
            return Json(new
            {
                success = false,
                error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Videos.Alert.VideoAdd")} {exc.Message}",
            });
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List");
        try
        {
            var video = new Video
            {
                VideoUrl = videoUrl
            };

            //insert video
            await _videoService.InsertVideoAsync(video);

            await _productService.InsertProductVideoAsync(new ProductVideo
            {
                VideoId = video.Id,
                ProductId = product.Id,
                DisplayOrder = model.DisplayOrder
            });
        }
        catch (Exception exc)
        {
            return Json(new
            {
                success = false,
                error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Videos.Alert.VideoAdd")} {exc.Message}",
            });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductVideoList(ProductVideoSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductVideoList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductVideoList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductVideoListModelAsync(searchModel, product);

		logger.LogDebug("ProductVideoList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductVideoList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductVideoUpdate([Validate] ProductVideoModel model)
    {
        //try to get a product picture with the specified id
        var productVideo = await _productService.GetProductVideoByIdAsync(model.Id)
            ?? throw new ArgumentException("No product video found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            var product = await _productService.GetProductByIdAsync(productVideo.ProductId);
            if (product != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");
        }

        //try to get a video with the specified id
        var video = await _videoService.GetVideoByIdAsync(productVideo.VideoId)
            ?? throw new ArgumentException("No video found with the specified id");

        var videoUrl = model.VideoUrl.TrimStart('~');

        try
        {
            await PingVideoUrlAsync(videoUrl);
        }
        catch (Exception exc)
        {
            return Json(new
            {
                success = false,
                error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.Products.Multimedia.Videos.Alert.VideoUpdate")} {exc.Message}",
            });
        }

        video.VideoUrl = videoUrl;

        await _videoService.UpdateVideoAsync(video);

        productVideo.DisplayOrder = model.DisplayOrder;
        await _productService.UpdateProductVideoAsync(productVideo);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductVideoDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductVideoDelete(id={Id})", id);
        //try to get a product video with the specified id
        var productVideo = await _productService.GetProductVideoByIdAsync(id)
            ?? throw new ArgumentException("No product video found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("ProductVideoDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        {
            var product = await _productService.GetProductByIdAsync(productVideo.ProductId);
            if (product != null && product.VendorId != currentVendor.Id)
				logger.LogDebug("ProductVideoDelete(id={Id}): product → {Product}", id, product);
                return Content("This is not your product");
        }

        var videoId = productVideo.VideoId;
        await _productService.DeleteProductVideoAsync(productVideo);

        //try to get a video with the specified id
        var video = await _videoService.GetVideoByIdAsync(videoId)
            ?? throw new ArgumentException("No video found with the specified id");

        await _videoService.DeleteVideoAsync(video);

		logger.LogDebug("<<< Exiting ProductVideoDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Product specification attributes

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductSpecificationAttributeAdd(AddSpecificationAttributeModel model, bool continueEditing)
    {
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        if (product == null)
        {
            _notificationService.ErrorNotification("No product found with the specified id");
            return RedirectToAction("List");
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
        {
            return RedirectToAction("List");
        }

        //we allow filtering only for "Option" attribute type
        if (model.AttributeTypeId != (int)SpecificationAttributeType.Option)
            model.AllowFiltering = false;

        //we don't allow CustomValue for "Option" attribute type
        if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
            model.ValueRaw = null;

        //store raw html if field allow this
        if (model.AttributeTypeId == (int)SpecificationAttributeType.CustomText || model.AttributeTypeId == (int)SpecificationAttributeType.Hyperlink)
            model.ValueRaw = model.Value;

        var psa = model.ToEntity<ProductSpecificationAttribute>();
        psa.CustomValue = model.ValueRaw;
        await _specificationAttributeService.InsertProductSpecificationAttributeAsync(psa);

        switch (psa.AttributeType)
        {
            case SpecificationAttributeType.CustomText:
                foreach (var localized in model.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(psa,
                        x => x.CustomValue,
                        localized.Value,
                        localized.LanguageId);
                }

                break;
            case SpecificationAttributeType.CustomHtmlText:
                foreach (var localized in model.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(psa,
                        x => x.CustomValue,
                        localized.ValueRaw,
                        localized.LanguageId);
                }

                break;
            case SpecificationAttributeType.Option:
                break;
            case SpecificationAttributeType.Hyperlink:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (continueEditing)
            return RedirectToAction("ProductSpecAttributeAddOrEdit",
                new { productId = psa.ProductId, specificationId = psa.Id });

        //select an appropriate card
        SaveSelectedCardName("product-specification-attributes");
        return RedirectToAction("Edit", new { id = model.ProductId });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductSpecAttrList(ProductSpecificationAttributeSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductSpecAttrList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductSpecAttrList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductSpecificationAttributeListModelAsync(searchModel, product);

		logger.LogDebug("ProductSpecAttrList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductSpecAttrList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
    {
        //try to get a product specification attribute with the specified id
        var psa = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
        if (psa == null)
        {
            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");
            _notificationService.ErrorNotification("No product specification attribute found with the specified id");

            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null
            && (await _productService.GetProductByIdAsync(psa.ProductId)).VendorId != currentVendor.Id)
        {
            _notificationService.ErrorNotification("This is not your product");

            return RedirectToAction("List");
        }

        //we allow filtering and change option only for "Option" attribute type
        //save localized values for CustomHtmlText and CustomText
        switch (model.AttributeTypeId)
        {
            case (int)SpecificationAttributeType.Option:
                psa.AllowFiltering = model.AllowFiltering;
                psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;

                break;
            case (int)SpecificationAttributeType.CustomHtmlText:
                psa.CustomValue = model.ValueRaw;
                foreach (var localized in model.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(psa,
                        x => x.CustomValue,
                        localized.ValueRaw,
                        localized.LanguageId);
                }

                break;
            case (int)SpecificationAttributeType.CustomText:
                psa.CustomValue = model.Value;
                foreach (var localized in model.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(psa,
                        x => x.CustomValue,
                        localized.Value,
                        localized.LanguageId);
                }

                break;
            default:
                psa.CustomValue = model.Value;

                break;
        }

        psa.ShowOnProductPage = model.ShowOnProductPage;
        psa.DisplayOrder = model.DisplayOrder;
        await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(psa);

        if (continueEditing)
        {
            return RedirectToAction("ProductSpecAttributeAddOrEdit",
                new { productId = psa.ProductId, specificationId = model.SpecificationId });
        }

        //select an appropriate card
        SaveSelectedCardName("product-specification-attributes");

        return RedirectToAction("Edit", new { id = psa.ProductId });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductSpecAttributeAddOrEdit(int productId, int? specificationId)
    {
        if (!specificationId.HasValue && !await _permissionService.AuthorizeAsync(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE))
            return AccessDeniedView();

        if (await _productService.GetProductByIdAsync(productId) == null)
        {
            _notificationService.ErrorNotification("No product found with the specified id");
            return RedirectToAction("List");
        }

        //try to get a product specification attribute with the specified id
        try
        {
            var model = await _productModelFactory.PrepareAddSpecificationAttributeModelAsync(productId, specificationId);
            return View(model);
        }
        catch (Exception ex)
        {
            await _notificationService.ErrorNotificationAsync(ex);

            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");
            return RedirectToAction("Edit", new { id = productId });
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductSpecAttrDelete(AddSpecificationAttributeModel model)
    {
        //try to get a product specification attribute with the specified id
        var psa = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
        if (psa == null)
        {
            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");
            _notificationService.ErrorNotification("No product specification attribute found with the specified id");
            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && (await _productService.GetProductByIdAsync(psa.ProductId)).VendorId != currentVendor.Id)
        {
            _notificationService.ErrorNotification("This is not your product");
            return RedirectToAction("List", new { id = model.ProductId });
        }

        await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(psa);

        //select an appropriate card
        SaveSelectedCardName("product-specification-attributes");

        return RedirectToAction("Edit", new { id = psa.ProductId });
    }

    #endregion

    #region Product tags

    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_VIEW)]
    public virtual async Task<IActionResult> ProductTags()
    {
		logger.LogDebug(">>> Entering ProductTags()");
        //prepare model
        var model = await _productModelFactory.PrepareProductTagSearchModelAsync(new ProductTagSearchModel());

		logger.LogDebug("ProductTags(): model → {Model}", model);
		logger.LogDebug("<<< Exiting ProductTags()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_VIEW)]
    public virtual async Task<IActionResult> ProductTags(ProductTagSearchModel searchModel)
    {
        //prepare model
        var model = await _productModelFactory.PrepareProductTagListModelAsync(searchModel);

		logger.LogDebug("<<< Exiting ProductTags(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductTagDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductTagDelete(id={Id})", id);
        //try to get a product tag with the specified id
        var tag = await _productTagService.GetProductTagByIdAsync(id)
            ?? throw new ArgumentException("No product tag found with the specified id");

        await _productTagService.DeleteProductTagAsync(tag);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductTags.Deleted"));

		logger.LogDebug("<<< Exiting ProductTagDelete(id={Id})", id);
        return RedirectToAction("ProductTags");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductTagsDelete(ICollection<int> selectedIds)
    {
		logger.LogDebug(">>> Entering ProductTagsDelete(selectedIds={SelectedIds})", selectedIds);
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var tags = await _productTagService.GetProductTagsByIdsAsync(selectedIds.ToArray());
        await _productTagService.DeleteProductTagsAsync(tags);
		logger.LogDebug("ProductTagsDelete(selectedIds={SelectedIds}): tags → {Tags}", selectedIds, tags);

		logger.LogDebug("<<< Exiting ProductTagsDelete(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_VIEW)]
    public virtual async Task<IActionResult> EditProductTag(int id)
    {
		logger.LogDebug(">>> Entering EditProductTag(id={Id})", id);
        //try to get a product tag with the specified id
        var productTag = await _productTagService.GetProductTagByIdAsync(id);
        if (productTag == null)
			logger.LogDebug("EditProductTag(id={Id}): productTag → {ProductTag}", id, productTag);
            return RedirectToAction("List");

        //prepare tag model
        var model = await _productModelFactory.PrepareProductTagModelAsync(null, productTag);

		logger.LogDebug("EditProductTag(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting EditProductTag(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditProductTag(ProductTagModel model, bool continueEditing)
    {
        //try to get a product tag with the specified id
        var productTag = await _productTagService.GetProductTagByIdAsync(model.Id);
        if (productTag == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            productTag.Name = model.Name;
            productTag.MetaDescription = model.MetaDescription;
            productTag.MetaKeywords = model.MetaKeywords;
            productTag.MetaTitle = model.MetaTitle;
            await _productTagService.UpdateProductTagAsync(productTag);

            //locales
            await UpdateLocalesAsync(productTag, model);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductTags.Updated"));

            return continueEditing ? RedirectToAction("EditProductTag", new { id = productTag.Id }) : RedirectToAction("ProductTags");
        }

        //prepare model
        model = await _productModelFactory.PrepareProductTagModelAsync(model, productTag, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting EditProductTag(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_TAGS_VIEW)]
    public virtual async Task<IActionResult> TaggedProducts(ProductTagProductSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering TaggedProducts(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareTaggedProductListModelAsync(searchModel);

		logger.LogDebug("TaggedProducts(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting TaggedProducts(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Purchased with order

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> PurchasedWithOrders(ProductOrderSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PurchasedWithOrders(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("PurchasedWithOrders(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductOrderListModelAsync(searchModel, product);

		logger.LogDebug("PurchasedWithOrders(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting PurchasedWithOrders(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #region Export / Import

    [HttpPost, ActionName("DownloadCatalogPDF")]
    [FormValueRequired("download-catalog-pdf")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> DownloadCatalogAsPdf(ProductSearchModel model)
    {
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.SearchVendorId = currentVendor.Id;
        }

        var categoryIds = new List<int> { model.SearchCategoryId };
        //include subcategories
        if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
            categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

        //0 - all (according to "ShowHidden" parameter)
        //1 - published only
        //2 - unpublished only
        bool? overridePublished = null;
        if (model.SearchPublishedId == 1)
            overridePublished = true;
        else if (model.SearchPublishedId == 2)
            overridePublished = false;

        var products = await _productService.SearchProductsAsync(0,
            categoryIds: categoryIds,
            manufacturerIds: new List<int> { model.SearchManufacturerId },
            storeId: model.SearchStoreId,
            vendorId: model.SearchVendorId,
            warehouseId: model.SearchWarehouseId,
            productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
            keywords: model.SearchProductName,
            showHidden: true,
            overridePublished: overridePublished);

        try
        {
            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintProductsToPdfAsync(stream, products);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost, ActionName("ExportToXml")]
    [FormValueRequired("exportxml-all")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlAll(ProductSearchModel model)
    {
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.SearchVendorId = currentVendor.Id;
        }

        var categoryIds = new List<int> { model.SearchCategoryId };
        //include subcategories
        if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
            categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

        //0 - all (according to "ShowHidden" parameter)
        //1 - published only
        //2 - unpublished only
        bool? overridePublished = null;
        if (model.SearchPublishedId == 1)
            overridePublished = true;
        else if (model.SearchPublishedId == 2)
            overridePublished = false;

        var products = await _productService.SearchProductsAsync(0,
            categoryIds: categoryIds,
            manufacturerIds: new List<int> { model.SearchManufacturerId },
            storeId: model.SearchStoreId,
            vendorId: model.SearchVendorId,
            warehouseId: model.SearchWarehouseId,
            productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
            keywords: model.SearchProductName,
            showHidden: true,
            overridePublished: overridePublished);

        try
        {
            var xml = await _exportManager.ExportProductsToXmlAsync(products);

            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
    {
		logger.LogDebug(">>> Entering ExportXmlSelected(selectedIds={SelectedIds})", selectedIds);
        var products = new List<Product>();
        if (selectedIds != null)
		logger.LogDebug("ExportXmlSelected(selectedIds={SelectedIds}): products → {Products}", selectedIds, products);
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            products.AddRange(await _productService.GetProductsByIdsAsync(ids));
        }
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("ExportXmlSelected(selectedIds={SelectedIds}): currentVendor → {CurrentVendor}", selectedIds, currentVendor);
        {
            products = products.Where(p => p.VendorId == currentVendor.Id).ToList();
        }

        try
        {
            var xml = await _exportManager.ExportProductsToXmlAsync(products);
            return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ExportXmlSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ExportXmlSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("List");
        }
    }

    [HttpPost, ActionName("ExportToExcel")]
    [FormValueRequired("exportexcel-all")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelAll(ProductSearchModel model)
    {
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
        {
            model.SearchVendorId = currentVendor.Id;
        }

        var categoryIds = new List<int> { model.SearchCategoryId };
        //include subcategories
        if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
            categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

        //0 - all (according to "ShowHidden" parameter)
        //1 - published only
        //2 - unpublished only
        bool? overridePublished = null;
        if (model.SearchPublishedId == 1)
            overridePublished = true;
        else if (model.SearchPublishedId == 2)
            overridePublished = false;

        var products = await _productService.SearchProductsAsync(0,
            categoryIds: categoryIds,
            manufacturerIds: new List<int> { model.SearchManufacturerId },
            storeId: model.SearchStoreId,
            vendorId: model.SearchVendorId,
            warehouseId: model.SearchWarehouseId,
            productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
            keywords: model.SearchProductName,
            showHidden: true,
            overridePublished: overridePublished);

        try
        {
            var bytes = await _exportManager.ExportProductsToXlsxAsync(products);

            return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        }
        catch (Exception exc)
        {
            await _notificationService.ErrorNotificationAsync(exc);

            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
    {
		logger.LogDebug(">>> Entering ExportExcelSelected(selectedIds={SelectedIds})", selectedIds);
        var products = new List<Product>();
        if (selectedIds != null)
		logger.LogDebug("ExportExcelSelected(selectedIds={SelectedIds}): products → {Products}", selectedIds, products);
        {
            var ids = selectedIds
                .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            products.AddRange(await _productService.GetProductsByIdsAsync(ids));
        }
        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
		logger.LogDebug("ExportExcelSelected(selectedIds={SelectedIds}): currentVendor → {CurrentVendor}", selectedIds, currentVendor);
        {
            products = products.Where(p => p.VendorId == currentVendor.Id).ToList();
        }

        try
        {
            var bytes = await _exportManager.ExportProductsToXlsxAsync(products);

			logger.LogDebug("ExportExcelSelected(selectedIds={SelectedIds}): bytes → {Bytes}", selectedIds, bytes);
            return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ExportExcelSelected(selectedIds={SelectedIds}): {Arg1}", selectedIds, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);
			logger.LogDebug("<<< Exiting ExportExcelSelected(selectedIds={SelectedIds})", selectedIds);
            return RedirectToAction("List");
        }
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_IMPORT_EXPORT)]
    public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
    {
		logger.LogDebug(">>> Entering ImportExcel(importexcelfile={Importexcelfile})", importexcelfile);
        if (await _workContext.GetCurrentVendorAsync() != null && !_vendorSettings.AllowVendorsToImportProducts)
            //a vendor can not import products
            return AccessDeniedView();

        try
        {
            if (importexcelfile != null && importexcelfile.Length > 0)
            {
                await _importManager.ImportProductsFromXlsxAsync(importexcelfile.OpenReadStream());
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Imported"));

            return RedirectToAction("List");
        }
        catch (Exception exc)
        {
			logger.LogError("Exception in ImportExcel(importexcelfile={Importexcelfile}): {Arg1}", importexcelfile, ex.Message);
            await _notificationService.ErrorNotificationAsync(exc);

			logger.LogDebug("<<< Exiting ImportExcel(importexcelfile={Importexcelfile})", importexcelfile);
            return RedirectToAction("List");
        }
    }

    #endregion

    #region Tier prices

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> TierPriceList(TierPriceSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering TierPriceList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("TierPriceList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareTierPriceListModelAsync(searchModel, product);

		logger.LogDebug("TierPriceList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting TierPriceList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TierPriceCreatePopup(int productId)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //prepare model
        var model = await _productModelFactory.PrepareTierPriceModelAsync(new TierPriceModel(), product, null);

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TierPriceCreatePopup(TierPriceModel model)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(model.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        if (ModelState.IsValid)
        {
            //fill entity from model
            var tierPrice = model.ToEntity<TierPrice>();
            tierPrice.ProductId = product.Id;
            tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;

            await _productService.InsertTierPriceAsync(tierPrice);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareTierPriceModelAsync(model, product, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting TierPriceCreatePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> TierPriceEditPopup(int id)
    {
		logger.LogDebug(">>> Entering TierPriceEditPopup(id={Id})", id);
        //try to get a tier price with the specified id
        var tierPrice = await _productService.GetTierPriceByIdAsync(id);
        if (tierPrice == null)
			logger.LogDebug("TierPriceEditPopup(id={Id}): tierPrice → {TierPrice}", id, tierPrice);
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(tierPrice.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("TierPriceEditPopup(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareTierPriceModelAsync(null, product, tierPrice);

		logger.LogDebug("TierPriceEditPopup(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting TierPriceEditPopup(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TierPriceEditPopup(TierPriceModel model)
    {
        //try to get a tier price with the specified id
        var tierPrice = await _productService.GetTierPriceByIdAsync(model.Id);
        if (tierPrice == null)
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(tierPrice.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        if (ModelState.IsValid)
        {
            //fill entity from model
            tierPrice = model.ToEntity(tierPrice);
            tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;
            await _productService.UpdateTierPriceAsync(tierPrice);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareTierPriceModelAsync(model, product, tierPrice, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting TierPriceEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> TierPriceDelete(int id)
    {
		logger.LogDebug(">>> Entering TierPriceDelete(id={Id})", id);
        //try to get a tier price with the specified id
        var tierPrice = await _productService.GetTierPriceByIdAsync(id)
            ?? throw new ArgumentException("No tier price found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(tierPrice.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("TierPriceDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return Content("This is not your product");

        await _productService.DeleteTierPriceAsync(tierPrice);

		logger.LogDebug("<<< Exiting TierPriceDelete(id={Id})", id);
        return new NullJsonResult();
    }

    #endregion

    #region Product attributes

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeMappingList(ProductAttributeMappingSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductAttributeMappingList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeMappingList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeMappingListModelAsync(searchModel, product);

		logger.LogDebug("ProductAttributeMappingList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductAttributeMappingList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your product"));
            return RedirectToAction("List");
        }

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeMappingModelAsync(new ProductAttributeMappingModel(), product, null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeMappingCreate(ProductAttributeMappingModel model, bool continueEditing)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(model.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your product"));
            return RedirectToAction("List");
        }

        //ensure this attribute is not mapped yet
        if ((await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
            .Any(x => x.ProductAttributeId == model.ProductAttributeId))
        {
            //redisplay form
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

            model = await _productModelFactory.PrepareProductAttributeMappingModelAsync(model, product, null, true);

            return View(model);
        }

        //insert mapping
        var productAttributeMapping = model.ToEntity<ProductAttributeMapping>();

        await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);
        await UpdateLocalesAsync(productAttributeMapping, model);

        //predefined values
        var predefinedValues = await _productAttributeService.GetPredefinedProductAttributeValuesAsync(model.ProductAttributeId);
        foreach (var predefinedValue in predefinedValues)
        {
            var pav = new ProductAttributeValue
            {
                ProductAttributeMappingId = productAttributeMapping.Id,
                AttributeValueType = AttributeValueType.Simple,
                Name = predefinedValue.Name,
                PriceAdjustment = predefinedValue.PriceAdjustment,
                PriceAdjustmentUsePercentage = predefinedValue.PriceAdjustmentUsePercentage,
                WeightAdjustment = predefinedValue.WeightAdjustment,
                Cost = predefinedValue.Cost,
                IsPreSelected = predefinedValue.IsPreSelected,
                DisplayOrder = predefinedValue.DisplayOrder
            };
            await _productAttributeService.InsertProductAttributeValueAsync(pav);

            //locales
            var languages = await _languageService.GetAllLanguagesAsync(true);

            //localization
            foreach (var lang in languages)
            {
                var name = await _localizationService.GetLocalizedAsync(predefinedValue, x => x.Name, lang.Id, false, false);
                if (!string.IsNullOrEmpty(name))
                    await _localizedEntityService.SaveLocalizedValueAsync(pav, x => x.Name, name, lang.Id);
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Added"));

        if (!continueEditing)
        {
            //select an appropriate card
            SaveSelectedCardName("product-product-attributes");
            return RedirectToAction("Edit", new { id = product.Id });
        }

		logger.LogDebug("<<< Exiting ProductAttributeMappingCreate(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeMappingEdit(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeMappingEdit(id={Id})", id);
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(id)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
		logger.LogDebug("ProductAttributeMappingEdit(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your product"));
            return RedirectToAction("List");
        }

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeMappingModelAsync(null, product, productAttributeMapping);

		logger.LogDebug("ProductAttributeMappingEdit(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting ProductAttributeMappingEdit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeMappingEdit(ProductAttributeMappingModel model, bool continueEditing, IFormCollection form)
    {
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(model.Id)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your product"));
            return RedirectToAction("List");
        }

        //ensure this attribute is not mapped yet
        if ((await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
            .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
        {
            //redisplay form
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

            model = await _productModelFactory.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping, true);

            return View(model);
        }

        //fill entity from model
        productAttributeMapping = model.ToEntity(productAttributeMapping);
        await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);

        await UpdateLocalesAsync(productAttributeMapping, model);

        await SaveConditionAttributesAsync(productAttributeMapping, model.ConditionModel, form);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Updated"));

        if (!continueEditing)
        {
            //select an appropriate card
            SaveSelectedCardName("product-product-attributes");
            return RedirectToAction("Edit", new { id = product.Id });
        }

        return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> PreTranslateProductAttribute(int itemId)
    {
		logger.LogDebug(">>> Entering PreTranslateProductAttribute(itemId={ItemId})", itemId);
        var translationModel = new TranslationModel();

		logger.LogDebug("PreTranslateProductAttribute(itemId={ItemId}): translationModel → {TranslationModel}", itemId, translationModel);
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(itemId);
        if (productAttributeMapping == null)
			logger.LogDebug("PreTranslateProductAttribute(itemId={ItemId}): productAttributeMapping → {ProductAttributeMapping}", itemId, productAttributeMapping);
            return Json(translationModel);

        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId);

		logger.LogDebug("PreTranslateProductAttribute(itemId={ItemId}): product → {Product}", itemId, product);
        if (product == null)
            return Json(translationModel);

        var model = await _productModelFactory.PrepareProductAttributeMappingModelAsync(null, product, productAttributeMapping);

		logger.LogDebug("PreTranslateProductAttribute(itemId={ItemId}): model → {Model}", itemId, model);
        translationModel = await _translationModelFactory.PrepareTranslationModelAsync(model, nameof(ProductAttributeMappingModel.TextPrompt));

		logger.LogDebug("<<< Exiting PreTranslateProductAttribute(itemId={ItemId})", itemId);
        return Json(translationModel);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeMappingDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeMappingDelete(id={Id})", id);
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(id)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeMappingDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return Content("This is not your product");

        //check if existed combinations contains the specified attribute
        var existedCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        if (existedCombinations?.Any() == true)
		logger.LogDebug("ProductAttributeMappingDelete(id={Id}): existedCombinations → {ExistedCombinations}", id, existedCombinations);
        {
            foreach (var combination in existedCombinations)
            {
                var mappings = await _productAttributeParser
                    .ParseProductAttributeMappingsAsync(combination.AttributesXml);

                if (mappings?.Any(m => m.Id == productAttributeMapping.Id) == true)
                {
                    _notificationService.ErrorNotification(
                        string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExistsInCombination"),
                            await _productAttributeFormatter.FormatAttributesAsync(product, combination.AttributesXml, await _workContext.GetCurrentCustomerAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));

                    return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
                }
            }
        }

        await _productAttributeService.DeleteProductAttributeMappingAsync(productAttributeMapping);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Deleted"));

        //select an appropriate card
        SaveSelectedCardName("product-product-attributes");
		logger.LogDebug("<<< Exiting ProductAttributeMappingDelete(id={Id})", id);
        return RedirectToAction("Edit", new { id = productAttributeMapping.ProductId });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeValueList(ProductAttributeValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductAttributeValueList(searchModel={SearchModel})", searchModel);
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(searchModel.ProductAttributeMappingId)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeValueList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeValueListModelAsync(searchModel, productAttributeMapping);

		logger.LogDebug("ProductAttributeValueList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductAttributeValueList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(int productAttributeMappingId)
    {
		logger.LogDebug(">>> Entering ProductAttributeValueCreatePopup(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeValueCreatePopup(productAttributeMappingId={ProductAttributeMappingId}): currentVendor → {CurrentVendor}", productAttributeMappingId, currentVendor);
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeValueModelAsync(new ProductAttributeValueModel(), productAttributeMapping, null);

		logger.LogDebug("ProductAttributeValueCreatePopup(productAttributeMappingId={ProductAttributeMappingId}): model → {Model}", productAttributeMappingId, model);
		logger.LogDebug("<<< Exiting ProductAttributeValueCreatePopup(productAttributeMappingId={ProductAttributeMappingId})", productAttributeMappingId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(ProductAttributeValueModel model)
    {
        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(model.ProductAttributeMappingId);
        if (productAttributeMapping == null)
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        if (productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
        {
            //ensure valid color is chosen/entered
            if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                ModelState.AddModelError(string.Empty, "Color is required");
            try
            {
                //ensure color is valid (can be instantiated)
                System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError(string.Empty, exc.Message);
            }
        }

        //ensure a picture is uploaded
        if (productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
        {
            ModelState.AddModelError(string.Empty, "Image is required");
        }

        if (ModelState.IsValid)
        {
            //fill entity from model
            var pav = model.ToEntity<ProductAttributeValue>();

            pav.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;

            await _productAttributeService.InsertProductAttributeValueAsync(pav);
            await UpdateLocalesAsync(pav, model);
            await SaveAttributeValuePicturesAsync(product, pav, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, null, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting ProductAttributeValueCreatePopup(model={Model})", model);
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeValueEditPopup(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeValueEditPopup(id={Id})", id);
        //try to get a product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(id);
        if (productAttributeValue == null)
			logger.LogDebug("ProductAttributeValueEditPopup(id={Id}): productAttributeValue → {ProductAttributeValue}", id, productAttributeValue);
            return RedirectToAction("List", "Product");

        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId);
        if (productAttributeMapping == null)
			logger.LogDebug("ProductAttributeValueEditPopup(id={Id}): productAttributeMapping → {ProductAttributeMapping}", id, productAttributeMapping);
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeValueEditPopup(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeValueModelAsync(null, productAttributeMapping, productAttributeValue);

		logger.LogDebug("ProductAttributeValueEditPopup(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting ProductAttributeValueEditPopup(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeValueEditPopup(ProductAttributeValueModel model)
    {
        //try to get a product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(model.Id);
        if (productAttributeValue == null)
            return RedirectToAction("List", "Product");

        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId);
        if (productAttributeMapping == null)
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        if (productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
        {
            //ensure valid color is chosen/entered
            if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                ModelState.AddModelError(string.Empty, "Color is required");
            try
            {
                //ensure color is valid (can be instantiated)
                System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError(string.Empty, exc.Message);
            }
        }

        //ensure a picture is uploaded
        if (productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
        {
            ModelState.AddModelError(string.Empty, "Image is required");
        }

        if (ModelState.IsValid)
        {
            //fill entity from model
            productAttributeValue = model.ToEntity(productAttributeValue);
            productAttributeValue.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
            await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);

            await UpdateLocalesAsync(productAttributeValue, model);
            await SaveAttributeValuePicturesAsync(product, productAttributeValue, model);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, productAttributeValue, true);

        //if we got this far, something failed, redisplay form
		logger.LogDebug("<<< Exiting ProductAttributeValueEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeValueDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeValueDelete(id={Id})", id);
        //try to get a product attribute value with the specified id
        var productAttributeValue = await _productAttributeService.GetProductAttributeValueByIdAsync(id)
            ?? throw new ArgumentException("No product attribute value found with the specified id");

        //try to get a product attribute mapping with the specified id
        var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId)
            ?? throw new ArgumentException("No product attribute mapping found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productAttributeMapping.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeValueDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return Content("This is not your product");

        //check if existed combinations contains the specified attribute value
        var existedCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        if (existedCombinations?.Any() == true)
		logger.LogDebug("ProductAttributeValueDelete(id={Id}): existedCombinations → {ExistedCombinations}", id, existedCombinations);
        {
            foreach (var combination in existedCombinations)
            {
                var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.AttributesXml);

				logger.LogDebug("ProductAttributeValueDelete(id={Id}): attributeValues → {AttributeValues}", id, attributeValues);
                if (attributeValues.Where(attribute => attribute.Id == id).Any())
                {
                    return Conflict(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.AlreadyExistsInCombination"),
                        await _productAttributeFormatter.FormatAttributesAsync(product, combination.AttributesXml, await _workContext.GetCurrentCustomerAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));
                }
            }
        }

        await _productAttributeService.DeleteProductAttributeValueAsync(productAttributeValue);

		logger.LogDebug("<<< Exiting ProductAttributeValueDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup()
    {
		logger.LogDebug(">>> Entering AssociateProductToAttributeValuePopup()");
        //prepare model
        var model = await _productModelFactory.PrepareAssociateProductToAttributeValueSearchModelAsync(new AssociateProductToAttributeValueSearchModel());

		logger.LogDebug("AssociateProductToAttributeValuePopup(): model → {Model}", model);
		logger.LogDebug("<<< Exiting AssociateProductToAttributeValuePopup()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociateProductToAttributeValuePopupList(AssociateProductToAttributeValueSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering AssociateProductToAttributeValuePopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _productModelFactory.PrepareAssociateProductToAttributeValueListModelAsync(searchModel);

		logger.LogDebug("AssociateProductToAttributeValuePopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting AssociateProductToAttributeValuePopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup([Bind(Prefix = nameof(AssociateProductToAttributeValueModel))] AssociateProductToAttributeValueModel model)
    {
        //try to get a product with the specified id
        var associatedProduct = await _productService.GetProductByIdAsync(model.AssociatedToProductId);
        if (associatedProduct == null)
            return Content("Cannot load a product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
            return Content("This is not your product");

        ViewBag.RefreshPage = true;
        ViewBag.productId = associatedProduct.Id;
        ViewBag.productName = associatedProduct.Name;

        return View(new AssociateProductToAttributeValueSearchModel());
    }

    //action displaying notification (warning) to a store owner when associating some product
    public virtual async Task<IActionResult> AssociatedProductGetWarnings(int productId)
    {
        var associatedProduct = await _productService.GetProductByIdAsync(productId);
        if (associatedProduct == null)
            return Json(new { Result = string.Empty });

        //attributes
        if (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(associatedProduct.Id) is IList<ProductAttributeMapping> mapping && mapping.Any())
        {
            if (mapping.Any(attribute => attribute.IsRequired))
                return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasRequiredAttributes") });

            return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasAttributes") });
        }

        //gift card
        if (associatedProduct.IsGiftCard)
        {
            return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.GiftCard") });
        }

        //downloadable product
        if (associatedProduct.IsDownload)
        {
            return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable") });
        }

        return Json(new { Result = string.Empty });
    }

    #endregion

    #region Product attribute combinations

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeCombinationList(ProductAttributeCombinationSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering ProductAttributeCombinationList(searchModel={SearchModel})", searchModel);
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeCombinationList(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeCombinationListModelAsync(searchModel, product);

		logger.LogDebug("ProductAttributeCombinationList(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting ProductAttributeCombinationList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeCombinationDelete(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeCombinationDelete(id={Id})", id);
        //try to get a combination with the specified id
        var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(id)
            ?? throw new ArgumentException("No product attribute combination found with the specified id");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(combination.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeCombinationDelete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return Content("This is not your product");

        await _productAttributeService.DeleteProductAttributeCombinationAsync(combination);

		logger.LogDebug("<<< Exiting ProductAttributeCombinationDelete(id={Id})", id);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(new ProductAttributeCombinationModel(), product, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId, ProductAttributeCombinationModel model, IFormCollection form)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        //attributes
        var warnings = new List<string>();
        var attributesXml = await GetAttributesXmlForProductAttributeCombinationAsync(form, warnings, product.Id);

        //check whether the attribute value is specified
        if (string.IsNullOrEmpty(attributesXml))
            warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(),
            ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

        //check whether the same attribute combination already exists
        var existingCombination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        if (existingCombination != null)
            warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

        if (!warnings.Any())
        {
            //save combination
            var combination = model.ToEntity<ProductAttributeCombination>();

            //fill attributes
            combination.AttributesXml = attributesXml;

            await _productAttributeService.InsertProductAttributeCombinationAsync(combination);

            await SaveAttributeCombinationPicturesAsync(product, combination, model);

            //quantity change history
            await _productService.AddStockQuantityHistoryEntryAsync(product, combination.StockQuantity, combination.StockQuantity,
                message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, null, true);
        model.Warnings = warnings;

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(int productId)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(new ProductAttributeCombinationModel(), product, null);

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(IFormCollection form, ProductAttributeCombinationModel model)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(model.ProductId);
        
        if (product == null)
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
            .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

        var requiredAttributeNames = await (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
            .Where(pam => pam.IsRequired)
            .Where(pam => !pam.IsNonCombinable())
            .WhereAwait(async pam => !(await _productAttributeService.GetProductAttributeValuesAsync(pam.Id)).Any(v => allowedAttributeIds.Any(id => id == v.Id)))
            .SelectAwait(async pam => (await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name).ToListAsync();

        if (requiredAttributeNames.Any())
        {
            model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, null, true);
            var pavModels = model.ProductAttributes.SelectMany(pa => pa.Values)
                .Where(v => allowedAttributeIds.Any(id => id == v.Id))
                .ToList();
            foreach (var pavModel in pavModels)
            {
                pavModel.Checked = "checked";
            }

            model.Warnings.Add(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.SelectRequiredAttributes"), string.Join(", ", requiredAttributeNames)));

            return View(model);
        }

        await GenerateAttributeCombinationsAsync(product, allowedAttributeIds);

        ViewBag.RefreshPage = true;

        return View(new ProductAttributeCombinationModel());
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(int id)
    {
		logger.LogDebug(">>> Entering ProductAttributeCombinationEditPopup(id={Id})", id);
        //try to get a combination with the specified id
        var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(id);
        if (combination == null)
			logger.LogDebug("ProductAttributeCombinationEditPopup(id={Id}): combination → {Combination}", id, combination);
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(combination.ProductId);
        if (product == null)
			logger.LogDebug("ProductAttributeCombinationEditPopup(id={Id}): product → {Product}", id, product);
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("ProductAttributeCombinationEditPopup(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
            return RedirectToAction("List", "Product");

        //prepare model
        var model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(null, product, combination);

		logger.LogDebug("ProductAttributeCombinationEditPopup(id={Id}): model → {Model}", id, model);
		logger.LogDebug("<<< Exiting ProductAttributeCombinationEditPopup(id={Id})", id);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(ProductAttributeCombinationModel model, IFormCollection form)
    {
        //try to get a combination with the specified id
        var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(model.Id);
        if (combination == null)
            return RedirectToAction("List", "Product");

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(combination.ProductId);
        if (product == null)
            return RedirectToAction("List", "Product");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return RedirectToAction("List", "Product");

        //attributes
        var warnings = new List<string>();
        var attributesXml = await GetAttributesXmlForProductAttributeCombinationAsync(form, warnings, product.Id);

        //check whether the attribute value is specified
        if (string.IsNullOrEmpty(attributesXml))
            warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

        warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(),
            ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

        //check whether the same attribute combination already exists
        var existingCombination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
        if (existingCombination != null && existingCombination.Id != model.Id && existingCombination.AttributesXml.Equals(attributesXml))
            warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

        if (!warnings.Any() && ModelState.IsValid)
        {
            var previousStockQuantity = combination.StockQuantity;

            //save combination
            //fill entity from model
            combination = model.ToEntity(combination);
            combination.AttributesXml = attributesXml;

            await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

            await SaveAttributeCombinationPicturesAsync(product, combination, model);

            //quantity change history
            await _productService.AddStockQuantityHistoryEntryAsync(product, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

            ViewBag.RefreshPage = true;

            return View(model);
        }

        //prepare model
        model = await _productModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, combination, true);
        model.Warnings = warnings;

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_CREATE_EDIT_DELETE)]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> GenerateAllAttributeCombinations(int productId)
    {
        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
            return Content("This is not your product");

        await GenerateAttributeCombinationsAsync(product);

        return Json(new { Success = true });
    }

    #endregion

    #region Product editor settings

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> SaveProductEditorSettings(ProductModel model, string returnUrl = "")
    {
		logger.LogDebug(">>> Entering SaveProductEditorSettings(model={Model},returnUrl={ReturnUrl})", model, returnUrl);
        //vendors cannot manage these settings
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        var productEditorSettings = await _settingService.LoadSettingAsync<ProductEditorSettings>();
        productEditorSettings = model.ProductEditorSettingsModel.ToSettings(productEditorSettings);
		logger.LogDebug("SaveProductEditorSettings(model={Model},returnUrl={ReturnUrl}): productEditorSettings → {ProductEditorSettings}", model, returnUrl, productEditorSettings);
        await _settingService.SaveSettingAsync(productEditorSettings);

        //product list
        if (string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("List");

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
		logger.LogDebug("<<< Exiting SaveProductEditorSettings(model={Model},returnUrl={ReturnUrl})", model, returnUrl);
            return RedirectToAction("List");

        return Redirect(returnUrl);
    }

    #endregion

    #region Stock quantity history

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCTS_VIEW)]
    public virtual async Task<IActionResult> StockQuantityHistory(StockQuantityHistorySearchModel searchModel)
    {
		logger.LogDebug(">>> Entering StockQuantityHistory(searchModel={SearchModel})", searchModel);
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
            ?? throw new ArgumentException("No product found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && product.VendorId != currentVendor.Id)
			logger.LogDebug("StockQuantityHistory(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
            return Content("This is not your product");

        //prepare model
        var model = await _productModelFactory.PrepareStockQuantityHistoryListModelAsync(searchModel, product);

		logger.LogDebug("StockQuantityHistory(searchModel={SearchModel}): model → {Model}", searchModel, model);
		logger.LogDebug("<<< Exiting StockQuantityHistory(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion

    #endregion

    #region Nested class

    protected class BulkEditData
    {
        protected bool _updated;
        protected bool _created;
        protected int _defaultTaxCategoryId;
        protected int _vendorId;

        public BulkEditData(int defaultTaxCategoryId, int vendorId)
        {
            _defaultTaxCategoryId = defaultTaxCategoryId;
            _vendorId = vendorId;
			logger.LogDebug("<<< Exiting BulkEditData(defaultTaxCategoryId={DefaultTaxCategoryId},vendorId={VendorId})", defaultTaxCategoryId, vendorId);
			logger.LogDebug("BulkEditData(defaultTaxCategoryId={DefaultTaxCategoryId},vendorId={VendorId}): _vendorId → {_vendorId}", defaultTaxCategoryId, vendorId, _vendorId);
			logger.LogDebug("BulkEditData(defaultTaxCategoryId={DefaultTaxCategoryId},vendorId={VendorId}): _defaultTaxCategoryId → {_defaultTaxCategoryId}", defaultTaxCategoryId, vendorId, _defaultTaxCategoryId);
        }

        public bool IsSelected { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsPublished { get; set; }
        public Product Product { get; set; }

        public bool NeedToUpdate(bool selected)
        {
			logger.LogDebug(">>> Entering NeedToUpdate(selected={Selected})", selected);
            if (selected && !IsSelected)
                return false;

            if (Product == null)
                return false;

            if (_updated)
                return true;

            return isStringValueChanged(Product.Name, Name) ||
                isStringValueChanged(Product.Sku, Sku) ||
                !Product.Price.Equals(Price) ||
                !Product.OldPrice.Equals(OldPrice) ||
                !Product.StockQuantity.Equals(Quantity) ||
                !Product.Published.Equals(IsPublished);

            bool isStringValueChanged(string oldValue, string newValue)
            {
                if (string.IsNullOrEmpty(oldValue))
				logger.LogDebug("<<< Exiting NeedToUpdate(selected={Selected})", selected);
                    return !string.IsNullOrEmpty(newValue);

                return !oldValue.Equals(newValue);
            }
        }

        public bool NeedToCreate(bool selected)
        {
			logger.LogDebug(">>> Entering NeedToCreate(selected={Selected})", selected);
            if (selected && !IsSelected)
                return false;

            if (Product != null)
			logger.LogDebug("<<< Exiting NeedToCreate(selected={Selected})", selected);
                return false;

            return true;
        }

        public Product UpdateProduct(bool selected)
        {
			logger.LogDebug(">>> Entering UpdateProduct(selected={Selected})", selected);
            if (!NeedToUpdate(selected) || _updated)
                return Product;

            Product.Name = Name;
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.Name → {Arg1}", selected, Product.Name);
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.Sku → {Arg1}", selected, Product.Sku);
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.Price → {Arg1}", selected, Product.Price);
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.OldPrice → {Arg1}", selected, Product.OldPrice);
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.StockQuantity → {Arg1}", selected, Product.StockQuantity);
			logger.LogDebug("UpdateProduct(selected={Selected}): Product.Published → {Arg1}", selected, Product.Published);
            Product.Sku = Sku;
            Product.Price = Price;
            Product.OldPrice = OldPrice;
            Product.StockQuantity = Quantity;
            Product.Published = IsPublished;

            _updated = true;
			logger.LogDebug("UpdateProduct(selected={Selected}): _updated → {_updated}", selected, _updated);

			logger.LogDebug("<<< Exiting UpdateProduct(selected={Selected})", selected);
            return Product;
        }

        public Product CreateProduct(bool selected)
        {
			logger.LogDebug(">>> Entering CreateProduct(selected={Selected})", selected);
            if (!NeedToCreate(selected) || _created)
                return Product;

            Product = new Product
			logger.LogDebug("CreateProduct(selected={Selected}): Product → {Product}", selected, Product);
            {
                Name = Name,
                Sku = Sku,
                Price = Price,
                OldPrice = OldPrice,
                StockQuantity = Quantity,
                Published = IsPublished,
                //set default values for the new model
                MaximumCustomerEnteredPrice = 1000,
                MaxNumberOfDownloads = 10,
                RecurringCycleLength = 100,
                RecurringTotalCycles = 10,
                RentalPriceLength = 1,
                NotifyAdminForQuantityBelow = 1,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                TaxCategoryId = _defaultTaxCategoryId,
                UnlimitedDownloads = true,
                IsShipEnabled = true,
                AllowCustomerReviews = true,
                VisibleIndividually = true,
                ProductType = ProductType.SimpleProduct,
                VendorId = _vendorId
            };

            _created = true;
			logger.LogDebug("CreateProduct(selected={Selected}): _created → {_created}", selected, _created);

			logger.LogDebug("<<< Exiting CreateProduct(selected={Selected})", selected);
            return Product;
        }
    }

    #endregion
}
