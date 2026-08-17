using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the product attribute model factory implementation
/// </summary>
public partial class ProductAttributeModelFactory : IProductAttributeModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor

    public ProductAttributeModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IProductAttributeService productAttributeService,
        IProductService productService)
    {
		_logger.LogDebug(">>> Entering ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService})", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService}): _baseAdminModelFactory → {_baseAdminModelFactory}", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService, _baseAdminModelFactory);
        _localizationService = localizationService;
		_logger.LogDebug("ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService}): _localizationService → {_localizationService}", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService}): _localizedModelFactory → {_localizedModelFactory}", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService, _localizedModelFactory);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService}): _productAttributeService → {_productAttributeService}", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService, _productAttributeService);
        _productService = productService;
		_logger.LogDebug("<<< Exiting ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService})", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService);
		_logger.LogDebug("ProductAttributeModelFactory(baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},localizedModelFactory={LocalizedModelFactory},productAttributeService={ProductAttributeService},productService={ProductService}): _productService → {_productService}", baseAdminModelFactory, localizationService, localizedModelFactory, productAttributeService, productService, _productService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare predefined product attribute value search model
    /// </summary>
    /// <param name="searchModel">Predefined product attribute value search model</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>Predefined product attribute value search model</returns>
    protected virtual PredefinedProductAttributeValueSearchModel PreparePredefinedProductAttributeValueSearchModel(
        PredefinedProductAttributeValueSearchModel searchModel, ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering PreparePredefinedProductAttributeValueSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productAttribute);

        searchModel.ProductAttributeId = productAttribute.Id;
		_logger.LogDebug("PreparePredefinedProductAttributeValueSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute}): searchModel.ProductAttributeId → {Arg2}", searchModel, productAttribute, searchModel.ProductAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PreparePredefinedProductAttributeValueSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        return searchModel;
    }

    /// <summary>
    /// Prepare search model of products that use the product attribute
    /// </summary>
    /// <param name="searchModel">Search model of products that use the product attribute</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>Search model of products that use the product attribute</returns>
    protected virtual ProductAttributeProductSearchModel PrepareProductAttributeProductSearchModel(ProductAttributeProductSearchModel searchModel,
        ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeProductSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productAttribute);

        searchModel.ProductAttributeId = productAttribute.Id;
		_logger.LogDebug("PrepareProductAttributeProductSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute}): searchModel.ProductAttributeId → {Arg2}", searchModel, productAttribute, searchModel.ProductAttributeId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareProductAttributeProductSearchModel(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare product attribute search model
    /// </summary>
    /// <param name="searchModel">Product attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute search model
    /// </returns>
    public virtual Task<ProductAttributeSearchModel> PrepareProductAttributeSearchModelAsync(ProductAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareProductAttributeSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged product attribute list model
    /// </summary>
    /// <param name="searchModel">Product attribute search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute list model
    /// </returns>
    public virtual async Task<ProductAttributeListModel> PrepareProductAttributeListModelAsync(ProductAttributeSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product attributes
        var productAttributes = await _productAttributeService.GetAllProductAttributesAsync(
            name: searchModel.SearchProductAttributeName,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare list model
        var model = new ProductAttributeListModel().PrepareToGrid(searchModel, productAttributes, () =>
        {
            //fill in model values from the entity
            return productAttributes.Select(attribute => attribute.ToModel<ProductAttributeModel>());

        });

		_logger.LogDebug("<<< Exiting PrepareProductAttributeListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare product attribute model
    /// </summary>
    /// <param name="model">Product attribute model</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute model
    /// </returns>
    public virtual async Task<ProductAttributeModel> PrepareProductAttributeModelAsync(ProductAttributeModel model,
        ProductAttribute productAttribute, bool excludeProperties = false)
    {
        Func<ProductAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (productAttribute != null)
        {
            //fill in model values from the entity
            model ??= productAttribute.ToModel<ProductAttributeModel>();

            //prepare nested search models
            PreparePredefinedProductAttributeValueSearchModel(model.PredefinedProductAttributeValueSearchModel, productAttribute);
            PrepareProductAttributeProductSearchModel(model.ProductAttributeProductSearchModel, productAttribute);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(productAttribute, entity => entity.Name, languageId, false, false);
                locale.Description = await _localizationService.GetLocalizedAsync(productAttribute, entity => entity.Description, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        await _baseAdminModelFactory.PreparePreTranslationSupportModelAsync(model);

        return model;
    }

    /// <summary>
    /// Prepare paged predefined product attribute value list model
    /// </summary>
    /// <param name="searchModel">Predefined product attribute value search model</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the predefined product attribute value list model
    /// </returns>
    public virtual async Task<PredefinedProductAttributeValueListModel> PreparePredefinedProductAttributeValueListModelAsync(
        PredefinedProductAttributeValueSearchModel searchModel, ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering PreparePredefinedProductAttributeValueListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productAttribute);

        //get predefined product attribute values
        var values = (await _productAttributeService.GetPredefinedProductAttributeValuesAsync(productAttribute.Id)).ToPagedList(searchModel);
		_logger.LogDebug("PreparePredefinedProductAttributeValueListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute}): values → {Values}", searchModel, productAttribute, values);

        //prepare list model
        var model = new PredefinedProductAttributeValueListModel().PrepareToGrid(searchModel, values, () =>
        {
            return values.Select(value =>
            {
                //fill in model values from the entity
                var predefinedProductAttributeValueModel = value.ToModel<PredefinedProductAttributeValueModel>();
				_logger.LogDebug("PreparePredefinedProductAttributeValueListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute}): predefinedProductAttributeValueModel → {PredefinedProductAttributeValueModel}", searchModel, productAttribute, predefinedProductAttributeValueModel);

                //fill in additional values (not existing in the entity)
                predefinedProductAttributeValueModel.WeightAdjustmentStr = value.WeightAdjustment.ToString("G29");
                predefinedProductAttributeValueModel.PriceAdjustmentStr = value.PriceAdjustment
					_logger.LogDebug("PreparePredefinedProductAttributeValueListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute}): predefinedProductAttributeValueModel.PriceAdjustmentStr → {Arg2}", searchModel, productAttribute, predefinedProductAttributeValueModel.PriceAdjustmentStr);
                    .ToString("G29") + (value.PriceAdjustmentUsePercentage ? " %" : string.Empty);

                return predefinedProductAttributeValueModel;
            });
        });

		_logger.LogDebug("<<< Exiting PreparePredefinedProductAttributeValueListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        return model;
    }

    /// <summary>
    /// Prepare predefined product attribute value model
    /// </summary>
    /// <param name="model">Predefined product attribute value model</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <param name="productAttributeValue">Predefined product attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the predefined product attribute value model
    /// </returns>
    public virtual async Task<PredefinedProductAttributeValueModel> PreparePredefinedProductAttributeValueModelAsync(PredefinedProductAttributeValueModel model,
        ProductAttribute productAttribute, PredefinedProductAttributeValue productAttributeValue, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(productAttribute);

        Func<PredefinedProductAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (productAttributeValue != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = productAttributeValue.ToModel<PredefinedProductAttributeValueModel>();
				_logger.LogDebug("PreparePredefinedProductAttributeValueModelAsync(model={Model},productAttribute={ProductAttribute},productAttributeValue={ProductAttributeValue},excludeProperties={ExcludeProperties}): model → {Model1}", model, productAttribute, productAttributeValue, excludeProperties, model);
            }

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(productAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.ProductAttributeId = productAttribute.Id;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare paged list model of products that use the product attribute
    /// </summary>
    /// <param name="searchModel">Search model of products that use the product attribute</param>
    /// <param name="productAttribute">Product attribute</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list model of products that use the product attribute
    /// </returns>
    public virtual async Task<ProductAttributeProductListModel> PrepareProductAttributeProductListModelAsync(ProductAttributeProductSearchModel searchModel,
        ProductAttribute productAttribute)
    {
		_logger.LogDebug(">>> Entering PrepareProductAttributeProductListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productAttribute);

        //get products
        var products = await _productService.GetProductsByProductAttributeIdAsync(productAttributeId: productAttribute.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new ProductAttributeProductListModel().PrepareToGrid(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.Select(product =>
            {
                var productAttributeProductModel = product.ToModel<ProductAttributeProductModel>();
				_logger.LogDebug("PrepareProductAttributeProductListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute}): productAttributeProductModel → {ProductAttributeProductModel}", searchModel, productAttribute, productAttributeProductModel);
				_logger.LogDebug("PrepareProductAttributeProductListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute}): productAttributeProductModel.ProductName → {Arg2}", searchModel, productAttribute, productAttributeProductModel.ProductName);
                productAttributeProductModel.ProductName = product.Name;
                return productAttributeProductModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareProductAttributeProductListModelAsync(searchModel={SearchModel},productAttribute={ProductAttribute})", searchModel, productAttribute);
        return model;
    }

    #endregion
}