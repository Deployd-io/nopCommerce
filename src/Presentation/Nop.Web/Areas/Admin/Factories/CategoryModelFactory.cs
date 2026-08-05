using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the category model factory implementation
/// </summary>
public partial class CategoryModelFactory : ICategoryModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly IDiscountService _discountService;
    protected readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IProductService _productService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public CategoryModelFactory(CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICategoryService categoryService,
        IDiscountService discountService,
        IDiscountSupportedModelFactory discountSupportedModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IProductService productService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IUrlRecordService urlRecordService)
    {
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _categoryService = categoryService;
        _discountService = discountService;
        _discountSupportedModelFactory = discountSupportedModelFactory;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _productService = productService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare category product search model
    /// </summary>
    /// <param name="searchModel">Category product search model</param>
    /// <param name="category">Category</param>
    /// <returns>Category product search model</returns>
    protected virtual CategoryProductSearchModel PrepareCategoryProductSearchModel(CategoryProductSearchModel searchModel, Category category)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryProductSearchModel(searchModel={SearchModel},category={Category})", searchModel, category);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(category);

        searchModel.CategoryId = category.Id;
		_logger.LogDebug("PrepareCategoryProductSearchModel(searchModel={SearchModel},category={Category}): searchModel.CategoryId → {Arg2}", searchModel, category, searchModel.CategoryId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCategoryProductSearchModel(searchModel={SearchModel},category={Category})", searchModel, category);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare category search model
    /// </summary>
    /// <param name="searchModel">Category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category search model
    /// </returns>
    public virtual async Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.All")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.PublishedOnly")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged category list model
    /// </summary>
    /// <param name="searchModel">Category search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category list model
    /// </returns>
    public virtual async Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);
        //get categories
        var categories = await _categoryService.GetAllCategoriesAsync(categoryName: searchModel.SearchCategoryName,
            showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

        //prepare grid model
        var model = await new CategoryListModel().PrepareToGridAsync(searchModel, categories, () =>
        {
            return categories.SelectAwait(async category =>
            {
                //fill in model values from the entity
                var categoryModel = category.ToModel<CategoryModel>();

				_logger.LogDebug("PrepareCategoryListModelAsync(searchModel={SearchModel}): categoryModel → {CategoryModel}", searchModel, categoryModel);
                //fill in additional values (not existing in the entity)
                categoryModel.Breadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
                categoryModel.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);

                return categoryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareCategoryListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare category model
    /// </summary>
    /// <param name="model">Category model</param>
    /// <param name="category">Category</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category model
    /// </returns>
    public virtual async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false)
    {
        Func<CategoryLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (category != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = category.ToModel<CategoryModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);
				_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model → {Model1}", model, category, excludeProperties, model);
            }

            //prepare nested search model
            PrepareCategoryProductSearchModel(model.CategoryProductSearchModel, category);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(category, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(category, entity => entity.Description, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(category, languageId, false, false);
            };
        }

        //set default values for the new model
        if (category == null)
        {
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.PageSize → {Arg3}", model, category, excludeProperties, model.PageSize);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.PageSizeOptions → {Arg3}", model, category, excludeProperties, model.PageSizeOptions);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.Published → {Arg3}", model, category, excludeProperties, model.Published);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.AllowCustomersToSelectPageSize → {Arg3}", model, category, excludeProperties, model.AllowCustomersToSelectPageSize);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.PriceRangeFiltering → {Arg3}", model, category, excludeProperties, model.PriceRangeFiltering);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.ManuallyPriceRange → {Arg3}", model, category, excludeProperties, model.ManuallyPriceRange);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.PriceFrom → {Arg3}", model, category, excludeProperties, model.PriceFrom);
			_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): model.PriceTo → {Arg3}", model, category, excludeProperties, model.PriceTo);
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.Published = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available category templates
        await _baseAdminModelFactory.PrepareCategoryTemplatesAsync(model.AvailableCategoryTemplates, false);

        //prepare available parent categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Parent.None"));

        //prepare model discounts
        var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
        await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, category, availableDiscounts, excludeProperties);
		_logger.LogDebug("PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties}): availableDiscounts → {AvailableDiscounts}", model, category, excludeProperties, availableDiscounts);

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, category, excludeProperties);

        await _baseAdminModelFactory.PreparePreTranslationSupportModelAsync(model);

		_logger.LogDebug("<<< Exiting PrepareCategoryModelAsync(model={Model},category={Category},excludeProperties={ExcludeProperties})", model, category, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged category product list model
    /// </summary>
    /// <param name="searchModel">Category product search model</param>
    /// <param name="category">Category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category product list model
    /// </returns>
    public virtual async Task<CategoryProductListModel> PrepareCategoryProductListModelAsync(CategoryProductSearchModel searchModel, Category category)
    {
		_logger.LogDebug(">>> Entering PrepareCategoryProductListModelAsync(searchModel={SearchModel},category={Category})", searchModel, category);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(category);

        //get product categories
        var productCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(category.Id,
            showHidden: true,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new CategoryProductListModel().PrepareToGridAsync(searchModel, productCategories, () =>
        {
            return productCategories.SelectAwait(async productCategory =>
            {
                //fill in model values from the entity
                var categoryProductModel = productCategory.ToModel<CategoryProductModel>();

				_logger.LogDebug("PrepareCategoryProductListModelAsync(searchModel={SearchModel},category={Category}): categoryProductModel → {CategoryProductModel}", searchModel, category, categoryProductModel);
                //fill in additional values (not existing in the entity)
                categoryProductModel.ProductName = (await _productService.GetProductByIdAsync(productCategory.ProductId))?.Name;

                return categoryProductModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareCategoryProductListModelAsync(searchModel={SearchModel},category={Category})", searchModel, category);
        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the category
    /// </summary>
    /// <param name="searchModel">Product search model to add to the category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the category
    /// </returns>
    public virtual async Task<AddProductToCategorySearchModel> PrepareAddProductToCategorySearchModelAsync(AddProductToCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddProductToCategorySearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the category
    /// </summary>
    /// <param name="searchModel">Product search model to add to the category</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the category
    /// </returns>
    public virtual async Task<AddProductToCategoryListModel> PrepareAddProductToCategoryListModelAsync(AddProductToCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToCategoryListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddProductToCategoryListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

				_logger.LogDebug("PrepareAddProductToCategoryListModelAsync(searchModel={SearchModel}): productModel → {ProductModel}", searchModel, productModel);
                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddProductToCategoryListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}