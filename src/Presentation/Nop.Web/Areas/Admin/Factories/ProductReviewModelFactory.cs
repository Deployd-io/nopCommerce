using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the product review model factory implementation
/// </summary>
public partial class ProductReviewModelFactory : IProductReviewModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IReviewTypeService _reviewTypeService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public ProductReviewModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IProductReviewService productReviewService,
        IProductService productService,
        IReviewTypeService reviewTypeService,
        IStoreService storeService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext})", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext);
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _baseAdminModelFactory → {_baseAdminModelFactory}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _baseAdminModelFactory);
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _catalogSettings → {_catalogSettings}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _catalogSettings);
    _logger.LogWarning("<<< Exiting ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext})", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext);
        _customerService = customerService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _customerService → {_customerService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _customerService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _dateTimeHelper);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _htmlFormatter → {_htmlFormatter}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _htmlFormatter);
        _localizationService = localizationService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _localizationService → {_localizationService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _localizationService);
        _productReviewService = productReviewService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _productReviewService → {_productReviewService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _productReviewService);
        _productService = productService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _productService → {_productService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _productService);
        _reviewTypeService = reviewTypeService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _reviewTypeService → {_reviewTypeService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _reviewTypeService);
        _storeService = storeService;
		_logger.LogDebug("ProductReviewModelFactory(catalogSettings={CatalogSettings},baseAdminModelFactory={BaseAdminModelFactory},customerService={CustomerService},dateTimeHelper={DateTimeHelper},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},productReviewService={ProductReviewService},productService={ProductService},reviewTypeService={ReviewTypeService},storeService={StoreService},workContext={WorkContext}): _storeService → {_storeService}", catalogSettings, baseAdminModelFactory, customerService, dateTimeHelper, htmlFormatter, localizationService, productReviewService, productService, reviewTypeService, storeService, workContext, _storeService);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare product review search model
    /// </summary>
    /// <param name="searchModel">Product review search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review search model
    /// </returns>
    public virtual async Task<ProductReviewSearchModel> PrepareProductReviewSearchModelAsync(ProductReviewSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductReviewSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.All"),
            Value = "0"
			_logger.LogDebug("PrepareProductReviewSearchModelAsync(searchModel={SearchModel}): Value → {Value}", searchModel, Value);
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly"),
            Value = "1"
        });
        searchModel.AvailableApprovedOptions.Add(new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly"),
            Value = "2"
        });

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareProductReviewSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product review list model
    /// </summary>
    /// <param name="searchModel">Product review search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review list model
    /// </returns>
    public virtual async Task<ProductReviewListModel> PrepareProductReviewListModelAsync(ProductReviewSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareProductReviewListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter reviews
        var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
		_logger.LogError("PrepareProductReviewListModelAsync(searchModel={SearchModel}): isApprovedOnly → {IsApprovedOnly}", searchModel, isApprovedOnly);
        var vendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogError("PrepareProductReviewListModelAsync(searchModel={SearchModel}): vendor → {Vendor}", searchModel, vendor);
        var vendorId = vendor?.Id ?? 0;

        //get product reviews
        var productReviews = await _productReviewService.GetAllProductReviewsAsync(showHidden: true,
            customerId: 0,
            approved: isApprovedOnly,
            fromUtc: createdOnFromValue,
            toUtc: createdToFromValue,
            message: searchModel.SearchText,
            storeId: searchModel.SearchStoreId,
            productId: searchModel.SearchProductId,
            vendorId: vendorId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new ProductReviewListModel().PrepareToGridAsync(searchModel, productReviews, () =>
        {
            return productReviews.SelectAwait(async productReview =>
            {
                //fill in model values from the entity
                var productReviewModel = productReview.ToModel<ProductReviewModel>();
				_logger.LogError("PrepareProductReviewListModelAsync(searchModel={SearchModel}): productReviewModel → {ProductReviewModel}", searchModel, productReviewModel);

                //convert dates to the user time
                productReviewModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                productReviewModel.StoreName = (await _storeService.GetStoreByIdAsync(productReview.StoreId))?.Name;
                productReviewModel.ProductName = (await _productService.GetProductByIdAsync(productReview.ProductId))?.Name;
                productReviewModel.CustomerInfo = (await _customerService.GetCustomerByIdAsync(productReview.CustomerId)) is Customer customer && (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");

                productReviewModel.ReviewText = _htmlFormatter.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                productReviewModel.ReplyText = _htmlFormatter.FormatText(productReview.ReplyText, false, true, false, false, false, false);

                return productReviewModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareProductReviewListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare product review model
    /// </summary>
    /// <param name="model">Product review model</param>
    /// <param name="productReview">Product review</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review model
    /// </returns>
    public virtual async Task<ProductReviewModel> PrepareProductReviewModelAsync(ProductReviewModel model,
        ProductReview productReview, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties})", model, productReview, excludeProperties);
        if (productReview != null)
        {
            var showStoreName = (await _storeService.GetAllStoresAsync()).Count > 1;
			_logger.LogError("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): showStoreName → {ShowStoreName}", model, productReview, excludeProperties, showStoreName);

            //fill in model values from the entity
            model ??= new ProductReviewModel
            {
                Id = productReview.Id,
                StoreName = showStoreName ? (await _storeService.GetStoreByIdAsync(productReview.StoreId))?.Name : string.Empty,
                ProductId = productReview.ProductId,
                ProductName = (await _productService.GetProductByIdAsync(productReview.ProductId))?.Name,
                CustomerId = productReview.CustomerId,
                Rating = productReview.Rating
            };

            model.ShowStoreName = showStoreName;
			_logger.LogDebug("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): model.ShowStoreName → {Arg3}", model, productReview, excludeProperties, model.ShowStoreName);

            model.CustomerInfo = await _customerService.GetCustomerByIdAsync(productReview.CustomerId) is Customer customer && await _customerService.IsRegisteredAsync(customer)
                ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");

            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

            if (!excludeProperties)
            {
                model.Title = productReview.Title;
				_logger.LogDebug("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): model.Title → {Arg3}", model, productReview, excludeProperties, model.Title);
				_logger.LogDebug("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): model.ReviewText → {Arg3}", model, productReview, excludeProperties, model.ReviewText);
				_logger.LogDebug("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): model.ReplyText → {Arg3}", model, productReview, excludeProperties, model.ReplyText);
				_logger.LogDebug("PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties}): model.IsApproved → {Arg3}", model, productReview, excludeProperties, model.IsApproved);
                model.ReviewText = productReview.ReviewText;
                model.ReplyText = productReview.ReplyText;
                model.IsApproved = productReview.IsApproved;
            }

            //prepare nested search model
            await PrepareProductReviewReviewTypeMappingSearchModelAsync(model.ProductReviewReviewTypeMappingSearchModel, productReview);
        }

        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

		_logger.LogWarning("<<< Exiting PrepareProductReviewModelAsync(model={Model},productReview={ProductReview},excludeProperties={ExcludeProperties})", model, productReview, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare product review mapping search model
    /// </summary>
    /// <param name="searchModel">Product review mapping search model</param>
    /// <param name="productReview">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review mapping search model
    /// </returns>
    public virtual async Task<ProductReviewReviewTypeMappingSearchModel> PrepareProductReviewReviewTypeMappingSearchModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel,
        ProductReview productReview)
    {
		_logger.LogError(">>> Entering PrepareProductReviewReviewTypeMappingSearchModelAsync(searchModel={SearchModel},productReview={ProductReview})", searchModel, productReview);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productReview);

        searchModel.ProductReviewId = productReview.Id;
		_logger.LogDebug("PrepareProductReviewReviewTypeMappingSearchModelAsync(searchModel={SearchModel},productReview={ProductReview}): searchModel.ProductReviewId → {Arg2}", searchModel, productReview, searchModel.ProductReviewId);

        searchModel.IsAnyReviewTypes = (await _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).Any();

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogWarning("<<< Exiting PrepareProductReviewReviewTypeMappingSearchModelAsync(searchModel={SearchModel},productReview={ProductReview})", searchModel, productReview);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product reviews mapping list model
    /// </summary>
    /// <param name="searchModel">Product review and review type mapping search model</param>
    /// <param name="productReview">Product review</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product review and review type mapping list model
    /// </returns>
    public virtual async Task<ProductReviewReviewTypeMappingListModel> PrepareProductReviewReviewTypeMappingListModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel, ProductReview productReview)
    {
		_logger.LogError(">>> Entering PrepareProductReviewReviewTypeMappingListModelAsync(searchModel={SearchModel},productReview={ProductReview})", searchModel, productReview);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(productReview);

        //get product review and review type mappings
        var productReviewReviewTypeMappings = (await _reviewTypeService
            .GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new ProductReviewReviewTypeMappingListModel().PrepareToGridAsync(searchModel, productReviewReviewTypeMappings, () =>
        {
            return productReviewReviewTypeMappings.SelectAwait(async productReviewReviewTypeMapping =>
            {
                //fill in model values from the entity
                var productReviewReviewTypeMappingModel = productReviewReviewTypeMapping
                    .ToModel<ProductReviewReviewTypeMappingModel>();

                //fill in additional values (not existing in the entity)
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(productReviewReviewTypeMapping.ReviewTypeId);
				_logger.LogError("PrepareProductReviewReviewTypeMappingListModelAsync(searchModel={SearchModel},productReview={ProductReview}): reviewType → {ReviewType}", searchModel, productReview, reviewType);

                productReviewReviewTypeMappingModel.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name);
                productReviewReviewTypeMappingModel.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description);
                productReviewReviewTypeMappingModel.VisibleToAllCustomers = reviewType.VisibleToAllCustomers;
				_logger.LogDebug("PrepareProductReviewReviewTypeMappingListModelAsync(searchModel={SearchModel},productReview={ProductReview}): productReviewReviewTypeMappingModel.VisibleToAllCustomers → {Arg2}", searchModel, productReview, productReviewReviewTypeMappingModel.VisibleToAllCustomers);

                return productReviewReviewTypeMappingModel;
            });
        });

		_logger.LogWarning("<<< Exiting PrepareProductReviewReviewTypeMappingListModelAsync(searchModel={SearchModel},productReview={ProductReview})", searchModel, productReview);
        return model;
    }

    #endregion
}