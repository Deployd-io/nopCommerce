using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.FilterLevels;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Factories;

public partial class CatalogModelFactory : ICatalogModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICategoryTemplateService _categoryTemplateService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IFilterLevelValueService _filterLevelValueService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IJsonLdModelFactory _jsonLdModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IManufacturerTemplateService _manufacturerTemplateService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPictureService _pictureService;
    protected readonly IProductModelFactory _productModelFactory;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly ISearchTermService _searchTermService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly SeoSettings _seoSettings;
    protected readonly VendorSettings _vendorSettings;
    private static readonly char[] _separator = [',', ' '];

    #endregion

    #region Ctor

    public CatalogModelFactory(CatalogSettings catalogSettings,
        CustomerSettings customerSettings,
        ForumSettings forumSettings,
        ICategoryService categoryService,
        ICategoryTemplateService categoryTemplateService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IFilterLevelValueService filterLevelValueService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IJsonLdModelFactory jsonLdModelFactory,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IManufacturerTemplateService manufacturerTemplateService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductModelFactory productModelFactory,
        IProductReviewService productReviewService,
        IProductService productService,
        IProductTagService productTagService,
        ISearchTermService searchTermService,
        ISpecificationAttributeService specificationAttributeService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IVendorService vendorService,
        IWebHelper webHelper,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        SeoSettings seoSettings,
        VendorSettings vendorSettings)
    {
        _catalogSettings = catalogSettings;
        _customerSettings = customerSettings;
        _forumSettings = forumSettings;
        _categoryService = categoryService;
        _categoryTemplateService = categoryTemplateService;
        _currencyService = currencyService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _filterLevelValueService = filterLevelValueService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _jsonLdModelFactory = jsonLdModelFactory;
        _localizationService = localizationService;
        _manufacturerService = manufacturerService;
        _manufacturerTemplateService = manufacturerTemplateService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _productModelFactory = productModelFactory;
        _productReviewService = productReviewService;
        _productService = productService;
        _productTagService = productTagService;
        _searchTermService = searchTermService;
        _specificationAttributeService = specificationAttributeService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _vendorService = vendorService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _seoSettings = seoSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets the price range converted to primary store currency
    /// </summary>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="Task"/> containing the price range converted to primary store currency
    /// </returns>
    protected virtual async Task<PriceRangeModel> GetConvertedPriceRangeAsync(CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering GetConvertedPriceRangeAsync(command={Command})", command);
        var result = new PriceRangeModel();

		_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): result → {Result}", command, result);
        if (string.IsNullOrWhiteSpace(command.Price))
            return result;

        var fromTo = command.Price.Trim().Split(['-']);
        if (fromTo.Length == 2)
		_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): fromTo → {FromTo}", command, fromTo);
        {
            var rawFromPrice = fromTo[0]?.Trim();
            if (!string.IsNullOrEmpty(rawFromPrice) && decimal.TryParse(rawFromPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var from))
				_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): rawFromPrice → {RawFromPrice}", command, rawFromPrice);
				_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): result.From → {Arg1}", command, result.From);
                result.From = from;

            var rawToPrice = fromTo[1]?.Trim();
            if (!string.IsNullOrEmpty(rawToPrice) && decimal.TryParse(rawToPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var to))
				_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): rawToPrice → {RawToPrice}", command, rawToPrice);
				_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): result.To → {Arg1}", command, result.To);
                result.To = to;

            if (result.From > result.To)
                result.From = result.To;

            var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

			_logger.LogDebug("GetConvertedPriceRangeAsync(command={Command}): workingCurrency → {WorkingCurrency}", command, workingCurrency);
            if (result.From.HasValue)
                result.From = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.From.Value, workingCurrency);

            if (result.To.HasValue)
                result.To = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.To.Value, workingCurrency);
        }

		_logger.LogDebug("<<< Exiting GetConvertedPriceRangeAsync(command={Command})", command);
        return result;
    }

    /// <summary>
    /// Prepares the specification filter model
    /// </summary>
    /// <param name="selectedOptions">The selected options to filter the products</param>
    /// <param name="availableOptions">The available options to filter the products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification filter model
    /// </returns>
    protected virtual async Task<SpecificationFilterModel> PrepareSpecificationFilterModel(IList<int> selectedOptions, IList<SpecificationAttributeOption> availableOptions)
    {
		_logger.LogDebug(">>> Entering PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions})", selectedOptions, availableOptions);
        var model = new SpecificationFilterModel();

		_logger.LogDebug("PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions}): model → {Model}", selectedOptions, availableOptions, model);
        if (availableOptions?.Any() == true)
        {
            model.Enabled = true;
			_logger.LogDebug("PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions}): model.Enabled → {Arg2}", selectedOptions, availableOptions, model.Enabled);

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

			_logger.LogDebug("PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions}): workingLanguage → {WorkingLanguage}", selectedOptions, availableOptions, workingLanguage);
            foreach (var option in availableOptions)
            {
                var attributeFilter = model.Attributes.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                if (attributeFilter == null)
				_logger.LogDebug("PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions}): attributeFilter → {AttributeFilter}", selectedOptions, availableOptions, attributeFilter);
                {
                    var attribute = await _specificationAttributeService
                        .GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    attributeFilter = new SpecificationAttributeFilterModel
                    {
                        Id = attribute.Id,
                        Name = await _localizationService
                            .GetLocalizedAsync(attribute, x => x.Name, workingLanguage.Id)
                    };
                    model.Attributes.Add(attributeFilter);
                }

                attributeFilter.Values.Add(new SpecificationAttributeValueFilterModel
                {
                    Id = option.Id,
                    Name = await _localizationService
                        .GetLocalizedAsync(option, x => x.Name, workingLanguage.Id),
                    Selected = selectedOptions?.Any(optionId => optionId == option.Id) == true,
                    ColorSquaresRgb = option.ColorSquaresRgb
                });
            }
        }

		_logger.LogDebug("<<< Exiting PrepareSpecificationFilterModel(selectedOptions={SelectedOptions},availableOptions={AvailableOptions})", selectedOptions, availableOptions);
        return model;
    }

    /// <summary>
    /// Prepares the manufacturer filter model
    /// </summary>
    /// <param name="selectedManufacturers">The selected manufacturers to filter the products</param>
    /// <param name="availableManufacturers">The available manufacturers to filter the products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the specification filter model
    /// </returns>
    protected virtual async Task<ManufacturerFilterModel> PrepareManufacturerFilterModel(IList<int> selectedManufacturers, IList<Manufacturer> availableManufacturers)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerFilterModel(selectedManufacturers={SelectedManufacturers},availableManufacturers={AvailableManufacturers})", selectedManufacturers, availableManufacturers);
        var model = new ManufacturerFilterModel();

		_logger.LogDebug("PrepareManufacturerFilterModel(selectedManufacturers={SelectedManufacturers},availableManufacturers={AvailableManufacturers}): model → {Model}", selectedManufacturers, availableManufacturers, model);
        if (availableManufacturers?.Any() == true)
        {
            model.Enabled = true;
			_logger.LogDebug("PrepareManufacturerFilterModel(selectedManufacturers={SelectedManufacturers},availableManufacturers={AvailableManufacturers}): model.Enabled → {Arg2}", selectedManufacturers, availableManufacturers, model.Enabled);

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

			_logger.LogDebug("PrepareManufacturerFilterModel(selectedManufacturers={SelectedManufacturers},availableManufacturers={AvailableManufacturers}): workingLanguage → {WorkingLanguage}", selectedManufacturers, availableManufacturers, workingLanguage);
            foreach (var manufacturer in availableManufacturers)
            {
                model.Manufacturers.Add(new SelectListItem
                {
                    Value = manufacturer.Id.ToString(),
                    Text = await _localizationService
                        .GetLocalizedAsync(manufacturer, x => x.Name, workingLanguage.Id),
                    Selected = selectedManufacturers?
                        .Any(manufacturerId => manufacturerId == manufacturer.Id) == true
                });
            }
        }

		_logger.LogDebug("<<< Exiting PrepareManufacturerFilterModel(selectedManufacturers={SelectedManufacturers},availableManufacturers={AvailableManufacturers})", selectedManufacturers, availableManufacturers);
        return model;
    }

    /// <summary>
    /// Prepares the price range filter
    /// </summary>
    /// <param name="selectedPriceRange">The selected price range to filter the products</param>
    /// <param name="availablePriceRange">The available price range to filter the products</param>
    /// <returns>The price range filter</returns>
    protected virtual async Task<PriceRangeFilterModel> PreparePriceRangeFilterAsync(PriceRangeModel selectedPriceRange, PriceRangeModel availablePriceRange)
    {
        var model = new PriceRangeFilterModel();

        if (!availablePriceRange.To.HasValue || availablePriceRange.To <= 0
                                             || availablePriceRange.To == availablePriceRange.From)
        {
            // filter by price isn't available
            selectedPriceRange.From = null;
            selectedPriceRange.To = null;

            return model;
        }

        if (selectedPriceRange.From < availablePriceRange.From)
            selectedPriceRange.From = availablePriceRange.From;

        if (selectedPriceRange.To > availablePriceRange.To)
            selectedPriceRange.To = availablePriceRange.To;

        var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

        Task<decimal> toWorkingCurrencyAsync(decimal? price)
            => _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price.Value, workingCurrency);

        model.Enabled = true;
        model.AvailablePriceRange.From = availablePriceRange.From > decimal.Zero
            ? Math.Floor(await toWorkingCurrencyAsync(availablePriceRange.From))
            : decimal.Zero;
        model.AvailablePriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(availablePriceRange.To));

        if (!selectedPriceRange.From.HasValue || availablePriceRange.From == selectedPriceRange.From)
        {
            //already converted
            model.SelectedPriceRange.From = model.AvailablePriceRange.From;
        }
        else if (selectedPriceRange.From > decimal.Zero)
            model.SelectedPriceRange.From = Math.Floor(await toWorkingCurrencyAsync(selectedPriceRange.From));

        if (!selectedPriceRange.To.HasValue || availablePriceRange.To == selectedPriceRange.To)
        {
            //already converted
            model.SelectedPriceRange.To = model.AvailablePriceRange.To;
        }
        else if (selectedPriceRange.To > decimal.Zero)
            model.SelectedPriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(selectedPriceRange.To));

        return model;
    }

    /// <summary>
    /// Prepares catalog products
    /// </summary>
    /// <param name="model">Catalog products model</param>
    /// <param name="products">The products</param>
    /// <param name="isFiltering">A value indicating that filtering has been applied</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareCatalogProductsAsync(CatalogProductsModel model, IPagedList<Product> products, bool isFiltering = false)
    {
        if (!string.IsNullOrEmpty(model.WarningMessage))
            return;

        if (!products.Any() && isFiltering)
            model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.Products.NoResult");
        else
        {
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();
            model.LoadPagedList(products);
        }
		_logger.LogDebug("<<< Exiting PrepareCatalogProductsAsync(model={Model},products={Products},isFiltering={IsFiltering})", model, products, isFiltering);
    }

    /// <summary>
    /// Prepare category picture model
    /// </summary>
    /// <param name="category">Category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PrepareCategoryPictureModelAsync(Category category)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryPictureModelAsync(category={Category})", category);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var pictureSize = _mediaSettings.CategoryThumbPictureSize;
        var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey,
            category, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

        return await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
        {
            var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
            string fullSizeImageUrl, imageUrl;

            (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
            (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

            var titleLocale = await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat");
            var altLocale = await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat");
            var localizedName = await _localizationService.GetLocalizedAsync(category, x => x.Name);

            return new PictureModel
            {
                FullSizeImageUrl = fullSizeImageUrl,
                ImageUrl = imageUrl,
                Title = string.Format(titleLocale, localizedName),
                AlternateText = string.Format(altLocale, localizedName)
            };
        });
		_logger.LogInformation("PrepareCategoryPictureModelAsync(category)={Category}: external service call took {Arg1} ms", category, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareCategoryPictureModelAsync(category={Category})", category);
    }

    /// <summary>
    /// Prepare manufacturer picture model
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PrepareManufacturerPictureModelAsync(Manufacturer manufacturer)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerPictureModelAsync(manufacturer={Manufacturer})", manufacturer);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
        var manufacturerPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerPictureModelKey,
            manufacturer, pictureSize, true, await _workContext.GetWorkingLanguageAsync(),
            _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

        return await _staticCacheManager.GetAsync(manufacturerPictureCacheKey, async () =>
        {
            var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
            string fullSizeImageUrl, imageUrl;

            (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
            (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

            var localizedName = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name);

            var pictureModel = new PictureModel
            {
                FullSizeImageUrl = fullSizeImageUrl,
                ImageUrl = imageUrl,
                Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), localizedName),
                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), localizedName)
            };

            return pictureModel;
        });
		_logger.LogInformation("PrepareManufacturerPictureModelAsync(manufacturer)={Manufacturer}: external service call took {Arg1} ms", manufacturer, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareManufacturerPictureModelAsync(manufacturer={Manufacturer})", manufacturer);
    }

    /// <summary>
    /// Prepare vendor picture model
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PrepareVendorPictureModelAsync(Vendor vendor)
    {
		_logger.LogDebug(">>> Entering PrepareVendorPictureModelAsync(vendor={Vendor})", vendor);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var pictureSize = _mediaSettings.VendorThumbPictureSize;
        var pictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.VendorPictureModelKey,
            vendor, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

        return await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
        {
            var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
            string fullSizeImageUrl, imageUrl;

            (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
            (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

            var localizedName = await _localizationService.GetLocalizedAsync(vendor, x => x.Name);

            var pictureModel = new PictureModel
            {
                FullSizeImageUrl = fullSizeImageUrl,
                ImageUrl = imageUrl,
                Title = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageLinkTitleFormat"), localizedName),
                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageAlternateTextFormat"), localizedName)
            };

            return pictureModel;
        });
		_logger.LogInformation("PrepareVendorPictureModelAsync(vendor)={Vendor}: external service call took {Arg1} ms", vendor, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareVendorPictureModelAsync(vendor={Vendor})", vendor);
    }

    /// <summary>
    /// Prepare category (simple) models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of category (simple) models
    /// </returns>
    protected virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync()
    {
		_logger.LogDebug(">>> Entering PrepareCategorySimpleModelsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //load and cache them
        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareCategorySimpleModelsAsync(): language → {Language}", language);
		_logger.LogDebug("PrepareCategorySimpleModelsAsync(): customer → {Customer}", customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareCategorySimpleModelsAsync(): customerRoleIds → {CustomerRoleIds}", customerRoleIds);
		_logger.LogDebug("PrepareCategorySimpleModelsAsync(): store → {Store}", store);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryAllModelKey,
            language, customerRoleIds, store);

		_logger.LogInformation("PrepareCategorySimpleModelsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareCategorySimpleModelsAsync()");
        return await _staticCacheManager.GetAsync(cacheKey, async () => await PrepareCategorySimpleModelsAsync(0));
    }

    /// <summary>
    /// Prepare category (simple) models
    /// </summary>
    /// <param name="rootCategoryId">Root category identifier</param>
    /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of category (simple) models
    /// </returns>
    protected virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, bool loadSubCategories = true)
    {
        var result = new List<CategorySimpleModel>();

        //little hack for performance optimization
        //we know that this method is used to load top and left menu for categories.
        //it'll load all categories anyway.
        //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
        //so we load all categories at once (we know they are cached)
        var store = await _storeContext.GetCurrentStoreAsync();
        var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
        var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).OrderBy(c => c.DisplayOrder).ToList();
        foreach (var category in categories)
        {
            var categoryModel = new CategorySimpleModel
            {
                Id = category.Id,
                Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                SeName = await _urlRecordService.GetSeNameAsync(category)
            };

            //number of products in each category
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                var categoryIds = new List<int> { category.Id };
                //include subcategories
                if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                    categoryIds.AddRange(
                        await _categoryService.GetChildCategoryIdsAsync(category.Id, store.Id));

                categoryModel.NumberOfProducts =
                    await _productService.GetNumberOfProductsInCategoryAsync(categoryIds, store.Id);
            }

            if (loadSubCategories)
            {
                var subCategories = await PrepareCategorySimpleModelsAsync(category.Id);
                categoryModel.SubCategories.AddRange(subCategories);
            }

            result.Add(categoryModel);
        }

        return result;
    }


    #endregion

    #region Categories

    /// <summary>
    /// Prepare category model
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category model
    /// </returns>
    public virtual async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryModelAsync(category={Category},command={Command})", category, command);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(category);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CategoryModel
        {
            Id = category.Id,
            Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
            Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
            MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(category),
            CatalogProductsModel = await PrepareCategoryProductsModelAsync(category, command),
            PictureModel = await PrepareCategoryPictureModelAsync(category)
        };

        //category breadcrumb
        if (_catalogSettings.CategoryBreadcrumbEnabled)
        {
            model.DisplayCategoryBreadcrumb = true;
			_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): model.DisplayCategoryBreadcrumb → {Arg2}", category, command, model.DisplayCategoryBreadcrumb);

            model.CategoryBreadcrumb = await (await _categoryService.GetCategoryBreadCrumbAsync(category)).SelectAwait(async catBr =>
                new CategoryModel
                {
                    Id = catBr.Id,
                    Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(catBr)
                }).ToListAsync();
				_logger.LogInformation("PrepareCategoryModelAsync(category,command)={Category},{Command}: database query executed in {Arg2} ms", category, command, __logStart.ElapsedMilliseconds);

            if (_seoSettings.MicrodataEnabled)
            {
                var categoryBreadcrumb = model.CategoryBreadcrumb.Select(c => new CategorySimpleModel { Id = c.Id, Name = c.Name, SeName = c.SeName }).ToList();
                var jsonLdModel = await _jsonLdModelFactory.PrepareJsonLdCategoryBreadcrumbAsync(categoryBreadcrumb);
				_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): categoryBreadcrumb → {CategoryBreadcrumb}", category, command, categoryBreadcrumb);
				_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): jsonLdModel → {JsonLdModel}", category, command, jsonLdModel);
				_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): model.JsonLd → {Arg2}", category, command, model.JsonLd);
                model.JsonLd = JsonConvert
                    .SerializeObject(jsonLdModel, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        //subcategories
        model.SubCategories = await (await _categoryService.GetAllCategoriesByParentCategoryIdAsync(category.Id))
            .SelectAwait(async curCategory =>
            {
                return new CategoryModel.SubCategoryModel
                {
                    Id = curCategory.Id,
                    Name = await _localizationService.GetLocalizedAsync(curCategory, y => y.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                    Description = await _localizationService.GetLocalizedAsync(curCategory, y => y.Description),
                    PictureModel = await PrepareCategoryPictureModelAsync(curCategory)
                };
            }).ToListAsync();

        //featured products
        if (!_catalogSettings.IgnoreFeaturedProducts)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var featuredProducts = await _productService.GetCategoryFeaturedProductsAsync(category.Id, currentStore.Id);
			_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): currentStore → {CurrentStore}", category, command, currentStore);
			_logger.LogDebug("PrepareCategoryModelAsync(category={Category},command={Command}): featuredProducts → {FeaturedProducts}", category, command, featuredProducts);
            if (featuredProducts != null)
                model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
        }

		_logger.LogDebug("<<< Exiting PrepareCategoryModelAsync(category={Category},command={Command})", category, command);
        return model;
    }

    /// <summary>
    /// Prepare category template view path
    /// </summary>
    /// <param name="templateId">Template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template view path
    /// </returns>
    public virtual async Task<string> PrepareCategoryTemplateViewPathAsync(int templateId)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryTemplateViewPathAsync(templateId={TemplateId})", templateId);
        var template = (await _categoryTemplateService.GetCategoryTemplateByIdAsync(templateId) ??
                        (await _categoryTemplateService.GetAllCategoryTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

		_logger.LogDebug("<<< Exiting PrepareCategoryTemplateViewPathAsync(templateId={TemplateId})", templateId);
        return template.ViewPath;
    }

    /// <summary>
    /// Prepare category navigation model
    /// </summary>
    /// <param name="currentCategoryId">Current category identifier</param>
    /// <param name="currentProductId">Current product identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category navigation model
    /// </returns>
    public virtual async Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId, int currentProductId)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryNavigationModelAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        //get active category
        var activeCategoryId = 0;
        if (currentCategoryId > 0)
		_logger.LogDebug("PrepareCategoryNavigationModelAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): activeCategoryId → {ActiveCategoryId}", currentCategoryId, currentProductId, activeCategoryId);
        {
            //category details page
            activeCategoryId = currentCategoryId;
        }
        else if (currentProductId > 0)
        {
            //product details page
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(currentProductId);
            if (productCategories.Any())
				_logger.LogDebug("PrepareCategoryNavigationModelAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): productCategories → {ProductCategories}", currentCategoryId, currentProductId, productCategories);
                activeCategoryId = productCategories[0].CategoryId;
        }

        var cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();
        var model = new CategoryNavigationModel
		_logger.LogDebug("PrepareCategoryNavigationModelAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId}): cachedCategoriesModel → {CachedCategoriesModel}", currentCategoryId, currentProductId, cachedCategoriesModel);
        {
            CurrentCategoryId = activeCategoryId,
            Categories = cachedCategoriesModel
        };

		_logger.LogDebug("<<< Exiting PrepareCategoryNavigationModelAsync(currentCategoryId={CurrentCategoryId},currentProductId={CurrentProductId})", currentCategoryId, currentProductId);
        return model;
    }

    /// <summary>
    /// Prepare homepage category models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of homepage category models
    /// </returns>
    public virtual async Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync()
    {
		_logger.LogDebug(">>> Entering PrepareHomepageCategoryModelsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
		_logger.LogDebug("PrepareHomepageCategoryModelsAsync(): language → {Language}", language);
		_logger.LogDebug("PrepareHomepageCategoryModelsAsync(): customer → {Customer}", customer);
		_logger.LogDebug("PrepareHomepageCategoryModelsAsync(): customerRoleIds → {CustomerRoleIds}", customerRoleIds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var store = await _storeContext.GetCurrentStoreAsync();
        var pictureSize = _mediaSettings.CategoryThumbPictureSize;
        var categoriesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryHomepageKey,
            store, customerRoleIds, pictureSize, language, _webHelper.IsCurrentConnectionSecured());
			_logger.LogDebug("PrepareHomepageCategoryModelsAsync(): store → {Store}", store);

        var model = await _staticCacheManager.GetAsync(categoriesCacheKey, async () =>
        {
            var homepageCategories = await _categoryService.GetAllCategoriesDisplayedOnHomepageAsync();
            return await homepageCategories.SelectAwait(async category =>
            {
			_logger.LogDebug("PrepareHomepageCategoryModelsAsync(): homepageCategories → {HomepageCategories}", homepageCategories);
                var catModel = new CategoryModel
                {
                    Id = category.Id,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                    MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                    MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                    MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    PictureModel = await PrepareCategoryPictureModelAsync(category)
                };

                return catModel;
            }).ToListAsync();
			_logger.LogInformation("PrepareHomepageCategoryModelsAsync(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        });

		_logger.LogInformation("PrepareHomepageCategoryModelsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareHomepageCategoryModelsAsync()");
        return model;
    }

    /// <summary>
    /// Prepares the category products model
    /// </summary>
    /// <param name="category">Category</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareCategoryProductsModelAsync(Category category, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryProductsModelAsync(category={Category},command={Command})", category, command);
        ArgumentNullException.ThrowIfNull(category);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        var currentStore = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PrepareCategoryProductsModelAsync(category={Category},command={Command}): currentStore → {CurrentStore}", category, command, currentStore);
        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, category.AllowCustomersToSelectPageSize,
            category.PageSizeOptions, category.PageSize);

        var categoryIds = new List<int> { category.Id };

        //include subcategories
        if (_catalogSettings.ShowProductsFromSubcategories)
            categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, currentStore.Id));

        //price range
        PriceRangeModel selectedPriceRange = null;
        if (_catalogSettings.EnablePriceRangeFiltering && category.PriceRangeFiltering)
		_logger.LogDebug("PrepareCategoryProductsModelAsync(category={Category},command={Command}): selectedPriceRange → {SelectedPriceRange}", category, command, selectedPriceRange);
        {
            selectedPriceRange = await GetConvertedPriceRangeAsync(command);

            PriceRangeModel availablePriceRange = null;
            if (!category.ManuallyPriceRange)
            {
                async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                {
                    var products = await _productService.SearchProductsAsync(0, 1,
                        categoryIds: categoryIds,
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                        orderBy: orderBy);

					_logger.LogDebug("<<< Exiting getProductPriceAsync(orderBy={OrderBy})", orderBy);
                    return products?.FirstOrDefault()?.Price ?? 0;
                }

                availablePriceRange = new PriceRangeModel
                {
                    From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                    To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                };
            }
            else
            {
                availablePriceRange = new PriceRangeModel
                {
                    From = category.PriceFrom,
                    To = category.PriceTo
                };
            }

            model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
        }

        //filterable options
        var filterableOptions = await _specificationAttributeService
            .GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(category.Id);

        if (_catalogSettings.EnableSpecificationAttributeFiltering)
        {
            model.SpecificationFilter = await PrepareSpecificationFilterModel(command.Specs, filterableOptions);
        }

        //filterable manufacturers
        if (_catalogSettings.EnableManufacturerFiltering)
        {
            var manufacturers = await _manufacturerService.GetManufacturersByCategoryIdAsync(category.Id);

            model.ManufacturerFilter = await PrepareManufacturerFilterModel(command.Ms, manufacturers);
        }

        var filteredSpecs = command.Specs is null ? null : filterableOptions.Where(fo => command.Specs.Contains(fo.Id)).ToList();

        //products
        var products = await _productService.SearchProductsAsync(
            command.PageNumber - 1,
            command.PageSize,
            categoryIds: categoryIds,
            storeId: currentStore.Id,
            visibleIndividuallyOnly: true,
            excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
            priceMin: selectedPriceRange?.From,
            priceMax: selectedPriceRange?.To,
            manufacturerIds: command.Ms,
            filteredSpecOptions: filteredSpecs,
            orderBy: (ProductSortingEnum)command.OrderBy);

        var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
        await PrepareCatalogProductsAsync(model, products, isFiltering);

		_logger.LogDebug("<<< Exiting PrepareCategoryProductsModelAsync(category={Category},command={Command})", category, command);
        return model;
    }

    #endregion

    #region Manufacturers

    /// <summary>
    /// Prepare manufacturer model
    /// </summary>
    /// <param name="manufacturer">Manufacturer identifier</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer model
    /// </returns>
    public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerModelAsync(manufacturer={Manufacturer},command={Command})", manufacturer, command);
        ArgumentNullException.ThrowIfNull(manufacturer);

        ArgumentNullException.ThrowIfNull(command);

        var model = new ManufacturerModel
        {
            Id = manufacturer.Id,
            Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
            Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
            MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
            CatalogProductsModel = await PrepareManufacturerProductsModelAsync(manufacturer, command),
            PictureModel = await PrepareManufacturerPictureModelAsync(manufacturer)
        };

        var store = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PrepareManufacturerModelAsync(manufacturer={Manufacturer},command={Command}): store → {Store}", manufacturer, command, store);
        //featured products
        if (!_catalogSettings.IgnoreFeaturedProducts)
        {
            var featuredProducts = await _productService.GetManufacturerFeaturedProductsAsync(manufacturer.Id, store.Id);
            if (featuredProducts != null)
				_logger.LogDebug("PrepareManufacturerModelAsync(manufacturer={Manufacturer},command={Command}): featuredProducts → {FeaturedProducts}", manufacturer, command, featuredProducts);
                model.FeaturedProducts = (await _productModelFactory.PrepareProductOverviewModelsAsync(featuredProducts)).ToList();
        }

		_logger.LogDebug("<<< Exiting PrepareManufacturerModelAsync(manufacturer={Manufacturer},command={Command})", manufacturer, command);
        return model;
    }

    /// <summary>
    /// Prepares the manufacturer products model
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareManufacturerProductsModelAsync(Manufacturer manufacturer, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerProductsModelAsync(manufacturer={Manufacturer},command={Command})", manufacturer, command);
        ArgumentNullException.ThrowIfNull(manufacturer);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        var manufacturerIds = new List<int> { manufacturer.Id };
        var currentStore = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PrepareManufacturerProductsModelAsync(manufacturer={Manufacturer},command={Command}): currentStore → {CurrentStore}", manufacturer, command, currentStore);
        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, manufacturer.AllowCustomersToSelectPageSize,
            manufacturer.PageSizeOptions, manufacturer.PageSize);

        //price range
        PriceRangeModel selectedPriceRange = null;
        if (_catalogSettings.EnablePriceRangeFiltering && manufacturer.PriceRangeFiltering)
		_logger.LogDebug("PrepareManufacturerProductsModelAsync(manufacturer={Manufacturer},command={Command}): selectedPriceRange → {SelectedPriceRange}", manufacturer, command, selectedPriceRange);
        {
            selectedPriceRange = await GetConvertedPriceRangeAsync(command);

            PriceRangeModel availablePriceRange = null;
            if (!manufacturer.ManuallyPriceRange)
            {
                async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                {
                    var products = await _productService.SearchProductsAsync(0, 1,
                        manufacturerIds: manufacturerIds,
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
                        orderBy: orderBy);

                    return products?.FirstOrDefault()?.Price ?? 0;
                }

                availablePriceRange = new PriceRangeModel
                {
                    From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                    To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                };
            }
            else
            {
                availablePriceRange = new PriceRangeModel
                {
                    From = manufacturer.PriceFrom,
                    To = manufacturer.PriceTo
                };
            }

            model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
        }

        // filterable options
        var filterableOptions = await _specificationAttributeService
            .GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturer.Id);

        if (_catalogSettings.EnableSpecificationAttributeFiltering)
        {
            model.SpecificationFilter = await PrepareSpecificationFilterModel(command.Specs, filterableOptions);
        }

        var filteredSpecs = command.Specs is null ? null : filterableOptions.Where(fo => command.Specs.Contains(fo.Id)).ToList();

        //products
        var products = await _productService.SearchProductsAsync(
            command.PageNumber - 1,
            command.PageSize,
            manufacturerIds: manufacturerIds,
            storeId: currentStore.Id,
            visibleIndividuallyOnly: true,
            excludeFeaturedProducts: !_catalogSettings.IgnoreFeaturedProducts && !_catalogSettings.IncludeFeaturedProductsInNormalLists,
            priceMin: selectedPriceRange?.From,
            priceMax: selectedPriceRange?.To,
            filteredSpecOptions: filteredSpecs,
            orderBy: (ProductSortingEnum)command.OrderBy);

        var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
        await PrepareCatalogProductsAsync(model, products, isFiltering);

		_logger.LogDebug("<<< Exiting PrepareManufacturerProductsModelAsync(manufacturer={Manufacturer},command={Command})", manufacturer, command);
        return model;
    }

    /// <summary>
    /// Prepare manufacturer template view path
    /// </summary>
    /// <param name="templateId">Template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template view path
    /// </returns>
    public virtual async Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerTemplateViewPathAsync(templateId={TemplateId})", templateId);
        var template = (await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(templateId) ??
                        (await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).FirstOrDefault()) ?? throw new Exception("No default template could be loaded");

		_logger.LogDebug("<<< Exiting PrepareManufacturerTemplateViewPathAsync(templateId={TemplateId})", templateId);
        return template.ViewPath;
    }

    /// <summary>
    /// Prepare manufacturer all models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of manufacturer models
    /// </returns>
    public virtual async Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync()
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerAllModelsAsync()");
        var model = new List<ManufacturerModel>();

		_logger.LogDebug("PrepareManufacturerAllModelsAsync(): model → {Model}", model);
        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
		_logger.LogDebug("PrepareManufacturerAllModelsAsync(): currentStore → {CurrentStore}", currentStore);
		_logger.LogDebug("PrepareManufacturerAllModelsAsync(): manufacturers → {Manufacturers}", manufacturers);
        foreach (var manufacturer in manufacturers)
        {
            var modelMan = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                //prepare picture model
                PictureModel = await PrepareManufacturerPictureModelAsync(manufacturer)
            };

            model.Add(modelMan);
        }

		_logger.LogDebug("<<< Exiting PrepareManufacturerAllModelsAsync()");
        return model;
    }

    /// <summary>
    /// Prepare manufacturer navigation model
    /// </summary>
    /// <param name="currentManufacturerId">Current manufacturer identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer navigation model
    /// </returns>
    public virtual async Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId)
    {
		_logger.LogDebug(">>> Entering PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId})", currentManufacturerId);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId}): language → {Language}", currentManufacturerId, language);
		_logger.LogDebug("PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId}): customer → {Customer}", currentManufacturerId, customer);
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId}): customerRoleIds → {CustomerRoleIds}", currentManufacturerId, customerRoleIds);
		_logger.LogDebug("PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId}): store → {Store}", currentManufacturerId, store);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerNavigationModelKey,
            currentManufacturerId, language, customerRoleIds, store);
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentManufacturer = await _manufacturerService.GetManufacturerByIdAsync(currentManufacturerId);

			_logger.LogDebug("PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId}): currentManufacturer → {CurrentManufacturer}", currentManufacturerId, currentManufacturer);
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id,
                pageSize: _catalogSettings.ManufacturersBlockItemsToDisplay);
            var model = new ManufacturerNavigationModel
            {
                TotalManufacturers = manufacturers.TotalCount
            };

            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new ManufacturerBriefInfoModel
                {
                    Id = manufacturer.Id,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                    IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                };
                model.Manufacturers.Add(modelMan);
            }

            return model;
        });

		_logger.LogInformation("PrepareManufacturerNavigationModelAsync(currentManufacturerId)={CurrentManufacturerId}: external service call took {Arg1} ms", currentManufacturerId, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareManufacturerNavigationModelAsync(currentManufacturerId={CurrentManufacturerId})", currentManufacturerId);
        return cachedModel;
    }

    #endregion

    #region Vendors

    /// <summary>
    /// Prepare vendor model
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor model
    /// </returns>
    public virtual async Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareVendorModelAsync(vendor={Vendor},command={Command})", vendor, command);
        ArgumentNullException.ThrowIfNull(vendor);

        ArgumentNullException.ThrowIfNull(command);

        var model = new VendorModel
        {
            Id = vendor.Id,
            Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
            Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
            MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
            SeName = await _urlRecordService.GetSeNameAsync(vendor),
            AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors,
            CatalogProductsModel = await PrepareVendorProductsModelAsync(vendor, command),
            PictureModel = await PrepareVendorPictureModelAsync(vendor),
            ProductReviews = await PrepareVendorProductReviewsModelAsync(vendor, new VendorReviewsPagingFilteringModel())
        };

        if (_forumSettings.AllowPrivateMessages)
            model.PmCustomerId = vendor.PmCustomerId;
			_logger.LogDebug("PrepareVendorModelAsync(vendor={Vendor},command={Command}): model.PmCustomerId → {Arg2}", vendor, command, model.PmCustomerId);

		_logger.LogDebug("<<< Exiting PrepareVendorModelAsync(vendor={Vendor},command={Command})", vendor, command);
        return model;
    }

    /// <summary>
    /// Prepares the vendor products model
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareVendorProductsModelAsync(Vendor vendor, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareVendorProductsModelAsync(vendor={Vendor},command={Command})", vendor, command);
        ArgumentNullException.ThrowIfNull(vendor);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, vendor.AllowCustomersToSelectPageSize,
            vendor.PageSizeOptions, vendor.PageSize);

        //price range
        PriceRangeModel selectedPriceRange = null;
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareVendorProductsModelAsync(vendor={Vendor},command={Command}): selectedPriceRange → {SelectedPriceRange}", vendor, command, selectedPriceRange);
		_logger.LogDebug("PrepareVendorProductsModelAsync(vendor={Vendor},command={Command}): store → {Store}", vendor, command, store);
        if (_catalogSettings.EnablePriceRangeFiltering && vendor.PriceRangeFiltering)
        {
            selectedPriceRange = await GetConvertedPriceRangeAsync(command);

            PriceRangeModel availablePriceRange;
            if (!vendor.ManuallyPriceRange)
            {
                async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                {
                    var products = await _productService.SearchProductsAsync(0, 1,
                        vendorId: vendor.Id,
                        storeId: store.Id,
                        visibleIndividuallyOnly: true,
                        orderBy: orderBy);

                    return products?.FirstOrDefault()?.Price ?? 0;
                }

                availablePriceRange = new PriceRangeModel
                {
                    From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                    To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                };
            }
            else
            {
                availablePriceRange = new PriceRangeModel
                {
                    From = vendor.PriceFrom,
                    To = vendor.PriceTo
                };
            }

            model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
        }

        //products
        var products = await _productService.SearchProductsAsync(
            command.PageNumber - 1,
            command.PageSize,
            vendorId: vendor.Id,
            priceMin: selectedPriceRange?.From,
            priceMax: selectedPriceRange?.To,
            storeId: store.Id,
            visibleIndividuallyOnly: true,
            orderBy: (ProductSortingEnum)command.OrderBy);

        var isFiltering = selectedPriceRange?.From is not null;
        await PrepareCatalogProductsAsync(model, products, isFiltering);

		_logger.LogDebug("<<< Exiting PrepareVendorProductsModelAsync(vendor={Vendor},command={Command})", vendor, command);
        return model;
    }

    /// <summary>
    /// Prepare vendor all models
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of vendor models
    /// </returns>
    public virtual async Task<List<VendorModel>> PrepareVendorAllModelsAsync()
    {
		_logger.LogDebug(">>> Entering PrepareVendorAllModelsAsync()");
        var model = new List<VendorModel>();
        var vendors = await _vendorService.GetAllVendorsAsync();
		_logger.LogDebug("PrepareVendorAllModelsAsync(): model → {Model}", model);
		_logger.LogDebug("PrepareVendorAllModelsAsync(): vendors → {Vendors}", vendors);
        foreach (var vendor in vendors)
        {
            var vendorModel = new VendorModel
            {
                Id = vendor.Id,
                Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(vendor),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors,
                PictureModel = await PrepareVendorPictureModelAsync(vendor)
            };

            model.Add(vendorModel);
        }

		_logger.LogDebug("<<< Exiting PrepareVendorAllModelsAsync()");
        return model;
    }

    /// <summary>
    /// Prepare vendor navigation model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendor navigation model
    /// </returns>
    public virtual async Task<VendorNavigationModel> PrepareVendorNavigationModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareVendorNavigationModelAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var cacheKey = NopModelCacheDefaults.VendorNavigationModelKey;
        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var vendors = await _vendorService.GetAllVendorsAsync(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
            var model = new VendorNavigationModel
			_logger.LogDebug("PrepareVendorNavigationModelAsync(): vendors → {Vendors}", vendors);
            {
                TotalVendors = vendors.TotalCount
            };

            foreach (var vendor in vendors)
            {
                model.Vendors.Add(new VendorBriefInfoModel
                {
                    Id = vendor.Id,
                    Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(vendor),
                });
            }

            return model;
        });

		_logger.LogInformation("PrepareVendorNavigationModelAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareVendorNavigationModelAsync()");
        return cachedModel;
    }


    /// <summary>
    /// Prepare review models for vendor products
    /// </summary>
    /// <returns>
    /// <param name="vendor">Vendor</param>
    /// <param name="pagingModel">Model to filter product reviews</param>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of product reviews
    /// </returns>
    public virtual async Task<VendorProductReviewsListModel> PrepareVendorProductReviewsModelAsync(Vendor vendor, VendorReviewsPagingFilteringModel pagingModel)
    {
		_logger.LogDebug(">>> Entering PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel})", vendor, pagingModel);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        ArgumentNullException.ThrowIfNull(vendor);
        ArgumentNullException.ThrowIfNull(pagingModel);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();

        if (pagingModel.PageSize <= 0)
            pagingModel.PageSize = _catalogSettings.VendorProductReviewsPageSize;
			_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): pagingModel.PageSize → {Arg2}", vendor, pagingModel, pagingModel.PageSize);
        if (pagingModel.PageNumber <= 0)
            pagingModel.PageNumber = 1;
			_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): pagingModel.PageNumber → {Arg2}", vendor, pagingModel, pagingModel.PageNumber);

        var model = new VendorProductReviewsListModel
        {
            VendorId = vendor.Id,
            VendorName = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
            VendorUrl = await _nopUrlHelper.RouteGenericUrlAsync(vendor)
        };

        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.VendorReviewsModelKey, vendor, currentStore);
		_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): currentStore → {CurrentStore}", vendor, pagingModel, currentStore);
		_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): cacheKey → {CacheKey}", vendor, pagingModel, cacheKey);
        var vendorReviewModels = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var vendorReviews = await _productReviewService.GetAllProductReviewsAsync(
                vendorId: vendor.Id,
                approved: true,
                storeId: currentStore.Id);

            return await vendorReviews.SelectAwait(async pr =>
            {
                var customer = await _customerService.GetCustomerByIdAsync(pr.CustomerId);
                var product = await _productService.GetProductByIdAsync(pr.ProductId);
				_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): customer → {Customer}", vendor, pagingModel, customer);
				_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): product → {Product}", vendor, pagingModel, product);

                var model = new VendorProductReviewModel
                {
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    CustomerId = pr.CustomerId,
                    CustomerName = await _customerService.FormatUsernameAsync(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !await _customerService.IsGuestAsync(customer),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    CreatedOnUtc = pr.CreatedOnUtc,
                };

                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                        await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                return model;
            })
            .OrderBy(m => m.CreatedOnUtc)
            .ToListAsync();
			_logger.LogInformation("PrepareVendorProductReviewsModelAsync(vendor,pagingModel)={Vendor},{PagingModel}: database query executed in {Arg2} ms", vendor, pagingModel, __logStart.ElapsedMilliseconds);
        });

        var pagedVendorReviews = new PagedList<VendorProductReviewModel>(vendorReviewModels, pagingModel.PageNumber - 1, pagingModel.PageSize);

		_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): pagedVendorReviews → {PagedVendorReviews}", vendor, pagingModel, pagedVendorReviews);
        //re-init pager
        model.PagingFilteringContext.LoadPagedList(pagedVendorReviews);

        model.Reviews = pagedVendorReviews;
		_logger.LogDebug("PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel}): model.Reviews → {Arg2}", vendor, pagingModel, model.Reviews);

		_logger.LogInformation("PrepareVendorProductReviewsModelAsync(vendor,pagingModel)={Vendor},{PagingModel}: external service call took {Arg2} ms", vendor, pagingModel, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareVendorProductReviewsModelAsync(vendor={Vendor},pagingModel={PagingModel})", vendor, pagingModel);
        return model;
    }

    #endregion

    #region Product tags

    /// <summary>
    /// Prepare popular product tags model
    /// </summary>
    /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tags model
    /// </returns>
    public virtual async Task<PopularProductTagsModel> PreparePopularProductTagsModelAsync(int numberTagsToReturn = 0)
    {
		_logger.LogDebug(">>> Entering PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn})", numberTagsToReturn);
        var model = new PopularProductTagsModel();

		_logger.LogDebug("PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn}): model → {Model}", numberTagsToReturn, model);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var currentStore = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn}): currentStore → {CurrentStore}", numberTagsToReturn, currentStore);
        var tagStats = await _productTagService.GetProductCountAsync(currentStore.Id);

		_logger.LogDebug("PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn}): tagStats → {TagStats}", numberTagsToReturn, tagStats);
		_logger.LogDebug("PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn}): model.TotalTags → {Arg1}", numberTagsToReturn, model.TotalTags);
        model.TotalTags = tagStats.Count;

        model.Tags.AddRange(await tagStats
            //Take the most popular tags if specified
            .OrderByDescending(x => x.Value).Take(numberTagsToReturn > 0 ? numberTagsToReturn : tagStats.Count)
            .SelectAwait(async tagStat =>
            {
                var tag = await _productTagService.GetProductTagByIdAsync(tagStat.Key);

				_logger.LogDebug("PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn}): tag → {Tag}", numberTagsToReturn, tag);
                return new ProductTagModel
                {
                    Id = tag.Id,
                    Name = await _localizationService.GetLocalizedAsync(tag, t => t.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(tag),
                    ProductCount = tagStat.Value
                };
            })
            //sorting result
            .OrderBy(x => x.Name)
            .ToListAsync());
			_logger.LogInformation("PreparePopularProductTagsModelAsync(numberTagsToReturn)={NumberTagsToReturn}: database query executed in {Arg1} ms", numberTagsToReturn, __logStart.ElapsedMilliseconds);

		_logger.LogDebug("<<< Exiting PreparePopularProductTagsModelAsync(numberTagsToReturn={NumberTagsToReturn})", numberTagsToReturn);
        return model;
    }

    /// <summary>
    /// Prepare products by tag model
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the products by tag model
    /// </returns>
    public virtual async Task<ProductsByTagModel> PrepareProductsByTagModelAsync(ProductTag productTag, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareProductsByTagModelAsync(productTag={ProductTag},command={Command})", productTag, command);
        ArgumentNullException.ThrowIfNull(productTag);

        ArgumentNullException.ThrowIfNull(command);

        var model = new ProductsByTagModel
        {
            Id = productTag.Id,
            MetaKeywords = await _localizationService.GetLocalizedAsync(productTag, x => x.MetaKeywords),
            MetaDescription = await _localizationService.GetLocalizedAsync(productTag, x => x.MetaDescription),
            MetaTitle = await _localizationService.GetLocalizedAsync(productTag, x => x.MetaTitle),
            TagName = await _localizationService.GetLocalizedAsync(productTag, y => y.Name),
            TagSeName = await _urlRecordService.GetSeNameAsync(productTag),
            CatalogProductsModel = await PrepareTagProductsModelAsync(productTag, command)
        };

		_logger.LogDebug("<<< Exiting PrepareProductsByTagModelAsync(productTag={ProductTag},command={Command})", productTag, command);
        return model;
    }

    /// <summary>
    /// Prepares the tag products model
    /// </summary>
    /// <param name="productTag">Product tag</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ag products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareTagProductsModelAsync(ProductTag productTag, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareTagProductsModelAsync(productTag={ProductTag},command={Command})", productTag, command);
        ArgumentNullException.ThrowIfNull(productTag);

        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, _catalogSettings.ProductsByTagAllowCustomersToSelectPageSize,
            _catalogSettings.ProductsByTagPageSizeOptions, _catalogSettings.ProductsByTagPageSize);

        //price range
        PriceRangeModel selectedPriceRange = null;
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("PrepareTagProductsModelAsync(productTag={ProductTag},command={Command}): selectedPriceRange → {SelectedPriceRange}", productTag, command, selectedPriceRange);
		_logger.LogDebug("PrepareTagProductsModelAsync(productTag={ProductTag},command={Command}): store → {Store}", productTag, command, store);
        if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.ProductsByTagPriceRangeFiltering)
        {
            selectedPriceRange = await GetConvertedPriceRangeAsync(command);

            PriceRangeModel availablePriceRange;
            if (!_catalogSettings.ProductsByTagManuallyPriceRange)
            {
                async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                {
                    var products = await _productService.SearchProductsAsync(0, 1,
                        storeId: store.Id,
                        productTagId: productTag.Id,
                        visibleIndividuallyOnly: true,
                        orderBy: orderBy);

                    return products?.FirstOrDefault()?.Price ?? 0;
                }

                availablePriceRange = new PriceRangeModel
                {
                    From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                    To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                };
            }
            else
            {
                availablePriceRange = new PriceRangeModel
                {
                    From = _catalogSettings.ProductsByTagPriceFrom,
                    To = _catalogSettings.ProductsByTagPriceTo
                };
				_logger.LogDebug(">>> Entering getProductPriceAsync(orderBy={OrderBy})", orderBy);
            }

            model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
        }

        //products
        var products = await _productService.SearchProductsAsync(
            command.PageNumber - 1,
            command.PageSize,
            priceMin: selectedPriceRange?.From,
            priceMax: selectedPriceRange?.To,
            storeId: store.Id,
            productTagId: productTag.Id,
            visibleIndividuallyOnly: true,
            orderBy: (ProductSortingEnum)command.OrderBy);

        var isFiltering = selectedPriceRange?.From is not null;
        await PrepareCatalogProductsAsync(model, products, isFiltering);

		_logger.LogDebug("<<< Exiting PrepareTagProductsModelAsync(productTag={ProductTag},command={Command})", productTag, command);
        return model;
    }

    #endregion

    #region New products

    /// <summary>
    /// Prepare new products model
    /// </summary>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the new products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareNewProductsModelAsync(CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareNewProductsModelAsync(command={Command})", command);
        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        var currentStore = await _storeContext.GetCurrentStoreAsync();

		_logger.LogDebug("PrepareNewProductsModelAsync(command={Command}): currentStore → {CurrentStore}", command, currentStore);
        //page size
        await PreparePageSizeOptionsAsync(model, command, _catalogSettings.NewProductsAllowCustomersToSelectPageSize,
            _catalogSettings.NewProductsPageSizeOptions, _catalogSettings.NewProductsPageSize);

        //products
        var products = await _productService.GetProductsMarkedAsNewAsync(storeId: currentStore.Id,
            pageIndex: command.PageNumber - 1,
            pageSize: command.PageSize);

        await PrepareCatalogProductsAsync(model, products);

		_logger.LogDebug("<<< Exiting PrepareNewProductsModelAsync(command={Command})", command);
        return model;
    }

    #endregion

    #region Searching

    /// <summary>
    /// Prepare search model
    /// </summary>
    /// <param name="model">Search model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search model
    /// </returns>
    public virtual async Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogProductsCommand command)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(command);

        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var categoriesModels = new List<SearchModel.CategoryModel>();
        //all categories
        var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: currentStore.Id);
        foreach (var c in allCategories)
        {
            //generate full category name (breadcrumb)
            var categoryBreadcrumb = string.Empty;
            var breadcrumb = await _categoryService.GetCategoryBreadCrumbAsync(c, allCategories);
            for (var i = 0; i <= breadcrumb.Count - 1; i++)
            {
                categoryBreadcrumb += await _localizationService.GetLocalizedAsync(breadcrumb[i], x => x.Name);
                if (i != breadcrumb.Count - 1)
                    categoryBreadcrumb += " >> ";
            }

            categoriesModels.Add(new SearchModel.CategoryModel
            {
                Id = c.Id,
                Breadcrumb = categoryBreadcrumb
            });
        }

        if (categoriesModels.Any())
        {
            //first empty entry
            model.AvailableCategories.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Common.All")
            });
            //all other categories
            foreach (var c in categoriesModels)
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Breadcrumb,
                    Selected = model.cid == c.Id
                });
            }
        }

        var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
        if (manufacturers.Any())
        {
            model.AvailableManufacturers.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Common.All")
            });
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = await _localizationService.GetLocalizedAsync(m, x => x.Name),
                    Selected = model.mid == m.Id
                });
        }

        model.asv = _vendorSettings.AllowSearchByVendor;
        if (model.asv)
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            if (vendors.Any())
            {
                model.AvailableVendors.Add(new SelectListItem
                {
                    Value = "0",
                    Text = await _localizationService.GetResourceAsync("Common.All")
                });
                foreach (var vendor in vendors)
                    model.AvailableVendors.Add(new SelectListItem
                    {
                        Value = vendor.Id.ToString(),
                        Text = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                        Selected = model.vid == vendor.Id
                    });
            }
        }

        model.CatalogProductsModel = await PrepareSearchProductsModelAsync(model, command);

        return model;
    }

    /// <summary>
    /// Prepares the search products model
    /// </summary>
    /// <param name="model">Search model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command})", searchModel, command);
        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
            _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageProductsPerPage);

        var searchTerms = searchModel.q == null
            ? string.Empty
            : searchModel.q.Trim();

        IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
        //only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
		_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): products → {Products}", searchModel, command, products);
        //we don't use "!string.IsNullOrEmpty(searchTerms)" in cases of "ProductSearchTermMinimumLength" set to 0 but searching by other parameters (e.g. category or price filter)
        var request = _httpContextAccessor.HttpContext.Request;

        var isSearchTermSpecified = request.Query.ContainsKey("q");

		_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): isSearchTermSpecified → {IsSearchTermSpecified}", searchModel, command, isSearchTermSpecified);
        if (!isSearchTermSpecified && request.HasFormContentType)
            isSearchTermSpecified = request.Form.ContainsKey("q");

        if (isSearchTermSpecified)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();

			_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): currentStore → {CurrentStore}", searchModel, command, currentStore);
            if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
            {
                model.WarningMessage =
                    string.Format(await _localizationService.GetResourceAsync("Search.SearchTermMinimumLengthIsNCharacters"),
                        _catalogSettings.ProductSearchTermMinimumLength);
            }
            else
            {
                var categoryIds = new List<int>();
                var manufacturerId = 0;
				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): categoryIds → {CategoryIds}", searchModel, command, categoryIds);
				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): manufacturerId → {ManufacturerId}", searchModel, command, manufacturerId);
                var searchInDescriptions = false;
                var searchInProductTags = false;
				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): searchInDescriptions → {SearchInDescriptions}", searchModel, command, searchInDescriptions);
				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): searchInProductTags → {SearchInProductTags}", searchModel, command, searchInProductTags);
                var vendorId = 0;

				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): vendorId → {VendorId}", searchModel, command, vendorId);
                if (searchModel.advs)
                {
                    //advanced search
                    var categoryId = searchModel.cid;
                    if (categoryId > 0)
                    {
                        categoryIds.Add(categoryId);
                        if (searchModel.isc)
                        {
                            //include subcategories
                            categoryIds.AddRange(
                                await _categoryService.GetChildCategoryIdsAsync(categoryId, currentStore.Id));
                        }
                    }

                    manufacturerId = searchModel.mid;

                    if (searchModel.asv)
                        vendorId = searchModel.vid;

                    searchInDescriptions = searchModel.sid;
                    searchInProductTags = searchModel.sit;
                }

                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): workingLanguage → {WorkingLanguage}", searchModel, command, workingLanguage);
                //price range
                PriceRangeModel selectedPriceRange = null;
                if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.SearchPagePriceRangeFiltering)
				_logger.LogDebug("PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command}): selectedPriceRange → {SelectedPriceRange}", searchModel, command, selectedPriceRange);
                {
                    selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                    PriceRangeModel availablePriceRange;
                    async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                    {
                        var products = await _productService.SearchProductsAsync(0, 1,
                            categoryIds: categoryIds,
                            manufacturerIds: new List<int> { manufacturerId },
                            storeId: currentStore.Id,
                            visibleIndividuallyOnly: true,
                            keywords: searchTerms,
                            searchDescriptions: searchInDescriptions,
                            searchProductTags: searchInProductTags,
                            languageId: workingLanguage.Id,
                            vendorId: vendorId,
                            orderBy: orderBy);

                        return products?.FirstOrDefault()?.Price ?? 0;
                    }

                    if (_catalogSettings.SearchPageManuallyPriceRange)
                    {
                        var to = await getProductPriceAsync(ProductSortingEnum.PriceDesc);

                        availablePriceRange = new PriceRangeModel
                        {
                            From = _catalogSettings.SearchPagePriceFrom,
                            To = to == 0 ? 0 : _catalogSettings.SearchPagePriceTo
                        };
                    }
                    else
                        availablePriceRange = new PriceRangeModel
                        {
                            From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                            To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                        };

                    model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
                }

                //products
                products = await _productService.SearchProductsAsync(
                    command.PageNumber - 1,
                    command.PageSize,
                    categoryIds: categoryIds,
                    manufacturerIds: new List<int> { manufacturerId },
                    storeId: currentStore.Id,
                    visibleIndividuallyOnly: true,
                    keywords: searchTerms,
                    priceMin: selectedPriceRange?.From,
                    priceMax: selectedPriceRange?.To,
                    searchDescriptions: searchInDescriptions,
                    searchProductTags: searchInProductTags,
                    languageId: workingLanguage.Id,
                    orderBy: (ProductSortingEnum)command.OrderBy,
                    vendorId: vendorId);

                //search term statistics
                if (!string.IsNullOrEmpty(searchTerms))
                {
                    var searchTerm =
                        await _searchTermService.GetSearchTermByKeywordAsync(searchTerms, currentStore.Id);
                    if (searchTerm != null)
                    {
                        searchTerm.Count++;
                        await _searchTermService.UpdateSearchTermAsync(searchTerm);
                    }
                    else
                    {
                        searchTerm = new SearchTerm
                        {
                            Keyword = searchTerms,
                            StoreId = currentStore.Id,
                            Count = 1
                        };
                        await _searchTermService.InsertSearchTermAsync(searchTerm);
                    }
                }

                //event
                await _eventPublisher.PublishAsync(new ProductSearchEvent
                {
                    SearchTerm = searchTerms,
                    SearchInDescriptions = searchInDescriptions,
                    CategoryIds = categoryIds,
                    ManufacturerId = manufacturerId,
                    WorkingLanguageId = workingLanguage.Id,
                    VendorId = vendorId
                });
            }
        }

        var isFiltering = !string.IsNullOrEmpty(searchTerms);
        await PrepareCatalogProductsAsync(model, products, isFiltering);

		_logger.LogDebug("<<< Exiting PrepareSearchProductsModelAsync(searchModel={SearchModel},command={Command})", searchModel, command);
        return model;
    }

    /// <summary>
    /// Prepares the search products by filter level values model
    /// </summary>
    /// <param name="searchModel">Search filter level values model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search products model
    /// </returns>
    public virtual async Task<CatalogProductsModel> PrepareSearchProductsByFilterLevelValuesModelAsync(SearchFilterLevelValueModel searchModel, CatalogProductsCommand command)
    {
		_logger.LogDebug(">>> Entering PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel={SearchModel},command={Command})", searchModel, command);
        ArgumentNullException.ThrowIfNull(command);

        var model = new CatalogProductsModel
        {
            UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
        };

        //sorting
        await PrepareSortingOptionsAsync(model, command);
        //view mode
        await PrepareViewModesAsync(model, command);
        //page size
        await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
            _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageProductsPerPage);

        var flv1 = searchModel.fl1id == "0" || string.IsNullOrEmpty(searchModel.fl1id)
            ? string.Empty
            : searchModel.fl1id;

        var flv2 = searchModel.fl2id == "0" || string.IsNullOrEmpty(searchModel.fl2id)
            ? string.Empty
            : searchModel.fl2id;

        var flv3 = searchModel.fl3id == "0" || string.IsNullOrEmpty(searchModel.fl3id)
            ? string.Empty
            : searchModel.fl3id;

        IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);

		_logger.LogDebug("PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel={SearchModel},command={Command}): products → {Products}", searchModel, command, products);
        var (filterLevel1Disabled, _, _) = _filterLevelValueService.IsFilterLevelDisabled();

        if (!string.IsNullOrEmpty(flv1) && !filterLevel1Disabled)
        {
            var filterLevelValue = (await _filterLevelValueService.GetAllFilterLevelValuesAsync(flv1, flv2, flv3)).FirstOrDefault();
            if (filterLevelValue == null)
			_logger.LogDebug("PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel={SearchModel},command={Command}): filterLevelValue → {FilterLevelValue}", searchModel, command, filterLevelValue);
            {
                return model;
            }

            var store = await _storeContext.GetCurrentStoreAsync();

			_logger.LogDebug("PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel={SearchModel},command={Command}): store → {Store}", searchModel, command, store);
            //products
            products = await _filterLevelValueService.GetProductsByFilterLevelValueIdAsync(filterLevelValue.Id,
            pageIndex: command.PageNumber - 1,
            pageSize: command.PageSize,
            storeId: store.Id,
            orderBy: (ProductSortingEnum)command.OrderBy);

            await PrepareCatalogProductsAsync(model, products, true);
        }

		_logger.LogDebug("<<< Exiting PrepareSearchProductsByFilterLevelValuesModelAsync(searchModel={SearchModel},command={Command})", searchModel, command);
        return model;
    }

    /// <summary>
    /// Prepare search box model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search box model
    /// </returns>
    public virtual async Task<SearchBoxModel> PrepareSearchBoxModelAsync()
    {
		_logger.LogDebug(">>> Entering PrepareSearchBoxModelAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var model = new SearchBoxModel
        {
            AutoCompleteEnabled = _catalogSettings.ProductSearchAutoCompleteEnabled,
            AutoCompleteSearchThumbPictureSize = _mediaSettings.AutoCompleteSearchThumbPictureSize,
            ShowProductImagesInSearchAutoComplete = _catalogSettings.ShowProductImagesInSearchAutoComplete,
            SearchTermMinimumLength = _catalogSettings.ProductSearchTermMinimumLength,
            ShowSearchBox = _catalogSettings.ProductSearchEnabled,
            ShowSearchBoxCategories = _catalogSettings.ShowSearchBoxCategories,
        };

        if (_catalogSettings.ShowSearchBoxCategories)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var language = await _workContext.GetWorkingLanguageAsync();
			_logger.LogDebug("PrepareSearchBoxModelAsync(): store → {Store}", store);
			_logger.LogDebug("PrepareSearchBoxModelAsync(): language → {Language}", language);
            var categoriesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SearchBoxCategoryListModelKey, store, language);

			_logger.LogDebug("PrepareSearchBoxModelAsync(): categoriesCacheKey → {CategoriesCacheKey}", categoriesCacheKey);
            model.AvailableCategories = await _staticCacheManager.GetAsync(categoriesCacheKey, async () =>
            {
                var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
                var result = new List<SelectListItem>
				_logger.LogDebug("PrepareSearchBoxModelAsync(): allCategories → {AllCategories}", allCategories);
                {
                    //empty entry
                    new()
                    {
                        Value = "0",
                        Text = await _localizationService.GetResourceAsync("Search.SearchBox.AllCategories")
                    }
                };

                //add top categories
                foreach (var c in allCategories.Where(c => c.ParentCategoryId == 0).OrderBy(c => c.DisplayOrder).ToList())
                {
                    result.Add(new()
                    {
                        Value = c.Id.ToString(),
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name, language.Id),
                        Selected = model.SearchCategoryId == c.Id
						_logger.LogDebug("PrepareSearchBoxModelAsync(): Value → {Value}", Value);
						_logger.LogDebug("PrepareSearchBoxModelAsync(): Text → {Text}", Text);
                    });
                }

                return result;
            });
        }

		_logger.LogInformation("PrepareSearchBoxModelAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting PrepareSearchBoxModelAsync()");
        return model;
    }

    #endregion

    #region Common

    /// <summary>
    /// Prepare sorting options
    /// </summary>
    /// <param name="model">Catalog products model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareSortingOptionsAsync(CatalogProductsModel model, CatalogProductsCommand command)
    {
        //get active sorting options
        var activeSortingOptionsIds = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
            .Except(_catalogSettings.ProductSortingEnumDisabled).ToList();

        //order sorting options
        var orderedActiveSortingOptions = activeSortingOptionsIds
            .Select(id => new { Id = id, Order = _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(id, out var order) ? order : id })
            .OrderBy(option => option.Order).ToList();

        //set the default option
        model.OrderBy = command.OrderBy;
        command.OrderBy = orderedActiveSortingOptions.FirstOrDefault()?.Id ?? (int)ProductSortingEnum.Position;

        //ensure that product sorting is enabled
        if (!_catalogSettings.AllowProductSorting)
            return;

        model.AllowProductSorting = true;
        command.OrderBy = model.OrderBy ?? command.OrderBy;

        //prepare available model sorting options
        foreach (var option in orderedActiveSortingOptions)
        {
            model.AvailableSortOptions.Add(new SelectListItem
            {
                Text = await _localizationService.GetLocalizedEnumAsync((ProductSortingEnum)option.Id),
                Value = option.Id.ToString(),
                Selected = option.Id == command.OrderBy
            });
        }
		_logger.LogDebug("<<< Exiting PrepareSortingOptionsAsync(model={Model},command={Command})", model, command);
    }

    /// <summary>
    /// Prepare view modes
    /// </summary>
    /// <param name="model">Catalog products model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareViewModesAsync(CatalogProductsModel model, CatalogProductsCommand command)
    {
        model.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

        var viewMode = !string.IsNullOrEmpty(command.ViewMode)
            ? command.ViewMode
            : _catalogSettings.DefaultViewMode;
        model.ViewMode = viewMode;
        if (model.AllowProductViewModeChanging)
        {
            //grid
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.Grid"),
                Value = "grid",
                Selected = viewMode == "grid"
            });
            //list
            model.AvailableViewModes.Add(new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.List"),
                Value = "list",
                Selected = viewMode == "list"
            });
        }
    }

    /// <summary>
    /// Prepare page size options
    /// </summary>
    /// <param name="model">Catalog products model</param>
    /// <param name="command">Model to get the catalog products</param>
    /// <param name="allowCustomersToSelectPageSize">Are customers allowed to select page size?</param>
    /// <param name="pageSizeOptions">Page size options</param>
    /// <param name="fixedPageSize">Fixed page size</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task PreparePageSizeOptionsAsync(CatalogProductsModel model, CatalogProductsCommand command,
        bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
    {
        if (command.PageNumber <= 0)
            command.PageNumber = 1;

        model.AllowCustomersToSelectPageSize = false;
        if (allowCustomersToSelectPageSize && pageSizeOptions != null)
        {
            var pageSizes = pageSizeOptions.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

            if (pageSizes.Any())
            {
                // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                {
                    if (int.TryParse(pageSizes.FirstOrDefault(), out var temp))
                    {
                        if (temp > 0)
                            command.PageSize = temp;
                    }
                }

                foreach (var pageSize in pageSizes)
                {
                    if (!int.TryParse(pageSize, out var temp))
                        continue;

                    if (temp <= 0)
                        continue;

                    model.PageSizeOptions.Add(new SelectListItem
                    {
                        Text = pageSize,
                        Value = pageSize,
                        Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    });
                }

                if (model.PageSizeOptions.Any())
                {
                    model.PageSizeOptions = model.PageSizeOptions.OrderBy(x => int.Parse(x.Value)).ToList();
                    model.AllowCustomersToSelectPageSize = true;

                    if (command.PageSize <= 0)
                        command.PageSize = int.Parse(model.PageSizeOptions.First().Value);
                }
            }
        }
        else
        {
            //customer is not allowed to select a page size
            command.PageSize = fixedPageSize;
        }

        //ensure pge size is specified
        if (command.PageSize <= 0)
        {
            command.PageSize = fixedPageSize;
        }

        return Task.CompletedTask;
    }

    #endregion
}
