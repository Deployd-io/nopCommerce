using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the discount model factory implementation
/// </summary>
public partial class DiscountModelFactory : IDiscountModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountPluginManager _discountPluginManager;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public DiscountModelFactory(CurrencySettings currencySettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IDiscountPluginManager discountPluginManager,
        IDiscountService discountService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        IOrderService orderService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext})", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext);
        _currencySettings = currencySettings;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _currencySettings → {_currencySettings}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _currencySettings);
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _baseAdminModelFactory → {_baseAdminModelFactory}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _baseAdminModelFactory);
        _categoryService = categoryService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _categoryService → {_categoryService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _categoryService);
        _currencyService = currencyService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _currencyService → {_currencyService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _currencyService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _dateTimeHelper → {_dateTimeHelper}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _dateTimeHelper);
        _discountPluginManager = discountPluginManager;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _discountPluginManager → {_discountPluginManager}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _discountPluginManager);
        _discountService = discountService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _discountService → {_discountService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _discountService);
        _localizationService = localizationService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _localizationService → {_localizationService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _localizationService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _manufacturerService → {_manufacturerService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _manufacturerService);
        _orderService = orderService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _orderService → {_orderService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _orderService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _priceFormatter → {_priceFormatter}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _priceFormatter);
        _productService = productService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _productService → {_productService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _productService);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _urlRecordService → {_urlRecordService}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _urlRecordService);
        _webHelper = webHelper;
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext})", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext);
		_logger.LogDebug("DiscountModelFactory(currencySettings={CurrencySettings},baseAdminModelFactory={BaseAdminModelFactory},categoryService={CategoryService},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},orderService={OrderService},priceFormatter={PriceFormatter},productService={ProductService},urlRecordService={UrlRecordService},webHelper={WebHelper},workContext={WorkContext}): _workContext → {_workContext}", currencySettings, baseAdminModelFactory, categoryService, currencyService, dateTimeHelper, discountPluginManager, discountService, localizationService, manufacturerService, orderService, priceFormatter, productService, urlRecordService, webHelper, workContext, _workContext);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare discount usage history search model
    /// </summary>
    /// <param name="searchModel">Discount usage history search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>Discount usage history search model</returns>
    protected virtual DiscountUsageHistorySearchModel PrepareDiscountUsageHistorySearchModel(DiscountUsageHistorySearchModel searchModel,
        Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountUsageHistorySearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        searchModel.DiscountId = discount.Id;
		_logger.LogDebug("PrepareDiscountUsageHistorySearchModel(searchModel={SearchModel},discount={Discount}): searchModel.DiscountId → {Arg2}", searchModel, discount, searchModel.DiscountId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareDiscountUsageHistorySearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return searchModel;
    }

    /// <summary>
    /// Prepare discount product search model
    /// </summary>
    /// <param name="searchModel">Discount product search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>Discount product search model</returns>
    protected virtual DiscountProductSearchModel PrepareDiscountProductSearchModel(DiscountProductSearchModel searchModel, Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountProductSearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        searchModel.DiscountId = discount.Id;
		_logger.LogDebug("PrepareDiscountProductSearchModel(searchModel={SearchModel},discount={Discount}): searchModel.DiscountId → {Arg2}", searchModel, discount, searchModel.DiscountId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareDiscountProductSearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return searchModel;
    }

    /// <summary>
    /// Prepare discount category search model
    /// </summary>
    /// <param name="searchModel">Discount category search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>Discount category search model</returns>
    protected virtual DiscountCategorySearchModel PrepareDiscountCategorySearchModel(DiscountCategorySearchModel searchModel, Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountCategorySearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        searchModel.DiscountId = discount.Id;
		_logger.LogDebug("PrepareDiscountCategorySearchModel(searchModel={SearchModel},discount={Discount}): searchModel.DiscountId → {Arg2}", searchModel, discount, searchModel.DiscountId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareDiscountCategorySearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return searchModel;
    }

    /// <summary>
    /// Prepare discount manufacturer search model
    /// </summary>
    /// <param name="searchModel">Discount manufacturer search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>Discount manufacturer search model</returns>
    protected virtual DiscountManufacturerSearchModel PrepareDiscountManufacturerSearchModel(DiscountManufacturerSearchModel searchModel,
        Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountManufacturerSearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        searchModel.DiscountId = discount.Id;
		_logger.LogDebug("PrepareDiscountManufacturerSearchModel(searchModel={SearchModel},discount={Discount}): searchModel.DiscountId → {Arg2}", searchModel, discount, searchModel.DiscountId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareDiscountManufacturerSearchModel(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare discount search model
    /// </summary>
    /// <param name="searchModel">Discount search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount search model
    /// </returns>
    public virtual async Task<DiscountSearchModel> PrepareDiscountSearchModelAsync(DiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available discount types
        await _baseAdminModelFactory.PrepareDiscountTypesAsync(searchModel.AvailableDiscountTypes);

        //prepare available vendors
        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        if (!searchModel.IsLoggedInAsVendor)
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.All")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.ActiveOnly")
        });
        searchModel.AvailableActiveOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.InactiveOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged discount list model
    /// </summary>
    /// <param name="searchModel">Discount search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount list model
    /// </returns>
    public virtual async Task<DiscountListModel> PrepareDiscountListModelAsync(DiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter discounts
        var discountType = searchModel.SearchDiscountTypeId > 0 ? (DiscountType?)searchModel.SearchDiscountTypeId : null;
		_logger.LogDebug("PrepareDiscountListModelAsync(searchModel={SearchModel}): discountType → {DiscountType}", searchModel, discountType);
        var startDateUtc = searchModel.SearchStartDate.HasValue ?
            (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
        var endDateUtc = searchModel.SearchEndDate.HasValue ?
            (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;
        var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);
		_logger.LogDebug("PrepareDiscountListModelAsync(searchModel={SearchModel}): isActive → {IsActive}", searchModel, isActive);

        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareDiscountListModelAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
        {
            searchModel.SearchVendorId = currentVendor.Id;
			_logger.LogDebug("PrepareDiscountListModelAsync(searchModel={SearchModel}): searchModel.SearchVendorId → {Arg1}", searchModel, searchModel.SearchVendorId);
            discountType = DiscountType.AssignedToSkus;
        }

        //get discounts
        var discounts = (await _discountService.GetAllDiscountsAsync(showHidden: true,
            discountType: discountType,
            couponCode: searchModel.SearchDiscountCouponCode,
            discountName: searchModel.SearchDiscountName,
            startDateUtc: startDateUtc,
            endDateUtc: endDateUtc,
            isActive: isActive,
            vendorId: searchModel.SearchVendorId)).ToPagedList(searchModel);

        //prepare list model
        var model = await new DiscountListModel().PrepareToGridAsync(searchModel, discounts, () =>
        {
            return discounts.SelectAwait(async discount =>
            {
                //fill in model values from the entity
                var discountModel = discount.ToModel<DiscountModel>();
				_logger.LogDebug("PrepareDiscountListModelAsync(searchModel={SearchModel}): discountModel → {DiscountModel}", searchModel, discountModel);

                //fill in additional values (not existing in the entity)
                discountModel.DiscountTypeName = await _localizationService.GetLocalizedEnumAsync(discount.DiscountType);
                discountModel.PrimaryStoreCurrencyCode = (await _currencyService
                    .GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                discountModel.TimesUsed = (await _discountService.GetAllDiscountUsageHistoryAsync(discount.Id, pageSize: 1)).TotalCount;

                return discountModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare discount model
    /// </summary>
    /// <param name="model">Discount model</param>
    /// <param name="discount">Discount</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount model
    /// </returns>
    public virtual async Task<DiscountModel> PrepareDiscountModelAsync(DiscountModel model, Discount discount, bool excludeProperties = false)
    {
        if (discount != null)
        {
            //fill in model values from the entity
            model ??= discount.ToModel<DiscountModel>();

            //prepare available discount requirement rules
            var discountRules = await _discountPluginManager.LoadAllPluginsAsync();
            foreach (var discountRule in discountRules)
            {
                model.AvailableDiscountRequirementRules.Add(new SelectListItem
                {
                    Text = discountRule.PluginDescriptor.FriendlyName,
                    Value = discountRule.PluginDescriptor.SystemName
                });
            }

            model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.AddGroup"),
                Value = "AddGroup"
            });

            model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"),
                Value = string.Empty
            });

            //prepare available requirement groups
            var requirementGroups = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);
            model.AvailableRequirementGroups = requirementGroups.Select(requirement =>
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

            //prepare nested search models
            PrepareDiscountUsageHistorySearchModel(model.DiscountUsageHistorySearchModel, discount);
            PrepareDiscountProductSearchModel(model.DiscountProductSearchModel, discount);
            PrepareDiscountCategorySearchModel(model.DiscountCategorySearchModel, discount);
            PrepareDiscountManufacturerSearchModel(model.DiscountManufacturerSearchModel, discount);
        }

        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        if (model.IsLoggedInAsVendor)
        {
            model.DiscountTypeId = (int)DiscountType.AssignedToSkus;
        }
        else
        {
            //prepare available vendors
            model.AvailableVendors.Add(new SelectListItem { Value = string.Empty, Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Fields.Vendor.None") });
            await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors, withSpecialDefaultItem: false);

        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //get URL of discount with coupon code
        if (model.RequiresCouponCode && !string.IsNullOrEmpty(model.CouponCode))
        {
            model.DiscountUrl = QueryHelpers.AddQueryString((_webHelper.GetStoreLocation()).TrimEnd('/'),
                NopDiscountDefaults.DiscountCouponQueryParameter, model.CouponCode);
        }

        //set default values for the new model
        if (discount == null)
        {
            model.IsActive = true;
            model.LimitationTimes = 1;
        }

        return model;
    }

    /// <summary>
    /// Prepare discount requirement rule models
    /// </summary>
    /// <param name="requirements">Collection of discount requirements</param>
    /// <param name="discount">Discount</param>
    /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of discount requirement rule models
    /// </returns>
    public virtual async Task<IList<DiscountRequirementRuleModel>> PrepareDiscountRequirementRuleModelsAsync
        (ICollection<DiscountRequirement> requirements, Discount discount, RequirementGroupInteractionType groupInteractionType)
    {
        ArgumentNullException.ThrowIfNull(requirements);

        ArgumentNullException.ThrowIfNull(discount);

        var lastRequirement = requirements.LastOrDefault();

        return await requirements.SelectAwait(async requirement =>
        {
            //set common properties
            var requirementModel = new DiscountRequirementRuleModel
            {
                DiscountRequirementId = requirement.Id,
                ParentId = requirement.ParentId,
                IsGroup = requirement.IsGroup,
                RuleName = requirement.DiscountRequirementRuleSystemName,
                IsLastInGroup = lastRequirement == null || lastRequirement.Id == requirement.Id,
                InteractionType = groupInteractionType.ToString().ToUpperInvariant()
            };

            var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
            requirementModel.AvailableInteractionTypes = await interactionType.ToSelectListAsync();

            if (requirement.IsGroup)
            {
                //get child requirements for the group
                var childRequirements = await _discountService.GetDiscountRequirementsByParentAsync(requirement);

                requirementModel.ChildRequirements = await PrepareDiscountRequirementRuleModelsAsync(childRequirements, discount, interactionType);

                return requirementModel;
            }

            //or try to get name and configuration URL for the requirement
            var requirementRule = await _discountPluginManager.LoadPluginBySystemNameAsync(requirement.DiscountRequirementRuleSystemName);
            if (requirementRule == null)
                return null;

            requirementModel.RuleName = requirementRule.PluginDescriptor.FriendlyName;
            requirementModel
                .ConfigurationUrl = requirementRule.GetConfigurationUrl(discount.Id, requirement.Id);

            return requirementModel;
        }).ToListAsync();
    }

    /// <summary>
    /// Prepare paged discount usage history list model
    /// </summary>
    /// <param name="searchModel">Discount usage history search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount usage history list model
    /// </returns>
    public virtual async Task<DiscountUsageHistoryListModel> PrepareDiscountUsageHistoryListModelAsync(DiscountUsageHistorySearchModel searchModel,
        Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountUsageHistoryListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        //get discount usage history
        var history = await _discountService.GetAllDiscountUsageHistoryAsync(discountId: discount.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new DiscountUsageHistoryListModel().PrepareToGridAsync(searchModel, history, () =>
        {
            return history.SelectAwait(async historyEntry =>
            {
                //fill in model values from the entity
                var discountUsageHistoryModel = historyEntry.ToModel<DiscountUsageHistoryModel>();
				_logger.LogDebug("PrepareDiscountUsageHistoryListModelAsync(searchModel={SearchModel},discount={Discount}): discountUsageHistoryModel → {DiscountUsageHistoryModel}", searchModel, discount, discountUsageHistoryModel);

                //convert dates to the user time
                discountUsageHistoryModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                var order = await _orderService.GetOrderByIdAsync(historyEntry.OrderId);
				_logger.LogDebug("PrepareDiscountUsageHistoryListModelAsync(searchModel={SearchModel},discount={Discount}): order → {Order}", searchModel, discount, order);
                if (order != null)
                {
                    discountUsageHistoryModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);
                    discountUsageHistoryModel.CustomOrderNumber = order.CustomOrderNumber;
					_logger.LogDebug("PrepareDiscountUsageHistoryListModelAsync(searchModel={SearchModel},discount={Discount}): discountUsageHistoryModel.CustomOrderNumber → {Arg2}", searchModel, discount, discountUsageHistoryModel.CustomOrderNumber);
                }

                return discountUsageHistoryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareDiscountUsageHistoryListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return model;
    }

    /// <summary>
    /// Prepare paged discount product list model
    /// </summary>
    /// <param name="searchModel">Discount product search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount product list model
    /// </returns>
    public virtual async Task<DiscountProductListModel> PrepareDiscountProductListModelAsync(DiscountProductSearchModel searchModel, Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        //get products with applied discount
        var discountProducts = await _productService.GetProductsWithAppliedDiscountAsync(discountId: discount.Id,
            showHidden: false,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = new DiscountProductListModel().PrepareToGrid(searchModel, discountProducts, () =>
        {
            //fill in model values from the entity
            return discountProducts.Select(product =>
            {
                var discountProductModel = product.ToModel<DiscountProductModel>();
				_logger.LogDebug("PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount}): discountProductModel → {DiscountProductModel}", searchModel, discount, discountProductModel);
				_logger.LogDebug("PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount}): discountProductModel.ProductId → {Arg2}", searchModel, discount, discountProductModel.ProductId);
				_logger.LogDebug("PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount}): discountProductModel.ProductName → {Arg2}", searchModel, discount, discountProductModel.ProductName);
				_logger.LogDebug("PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount}): discountProductModel.VendorId → {Arg2}", searchModel, discount, discountProductModel.VendorId);
                discountProductModel.ProductId = product.Id;
                discountProductModel.ProductName = product.Name;
                discountProductModel.VendorId = product.VendorId;

                return discountProductModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareDiscountProductListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the discount
    /// </summary>
    /// <param name="searchModel">Product search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the discount
    /// </returns>
    public virtual async Task<AddProductToDiscountSearchModel> PrepareAddProductToDiscountSearchModelAsync(AddProductToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

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

		_logger.LogDebug("<<< Exiting PrepareAddProductToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the discount
    /// </summary>
    /// <param name="searchModel">Product search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the discount
    /// </returns>
    public virtual async Task<AddProductToDiscountListModel> PrepareAddProductToDiscountListModelAsync(AddProductToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddProductToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("PrepareAddProductToDiscountListModelAsync(searchModel={SearchModel}): currentVendor → {CurrentVendor}", searchModel, currentVendor);
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;
			_logger.LogDebug("PrepareAddProductToDiscountListModelAsync(searchModel={SearchModel}): searchModel.SearchVendorId → {Arg1}", searchModel, searchModel.SearchVendorId);

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
        var model = await new AddProductToDiscountListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();
				_logger.LogDebug("PrepareAddProductToDiscountListModelAsync(searchModel={SearchModel}): productModel → {ProductModel}", searchModel, productModel);
                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddProductToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged discount category list model
    /// </summary>
    /// <param name="searchModel">Discount category search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount category list model
    /// </returns>
    public virtual async Task<DiscountCategoryListModel> PrepareDiscountCategoryListModelAsync(DiscountCategorySearchModel searchModel, Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountCategoryListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        //get categories with applied discount
        var discountCategories = await _categoryService.GetCategoriesByAppliedDiscountAsync(discountId: discount.Id,
            showHidden: false,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new DiscountCategoryListModel().PrepareToGridAsync(searchModel, discountCategories, () =>
        {
            //fill in model values from the entity
            return discountCategories.SelectAwait(async category =>
            {
                var discountCategoryModel = category.ToModel<DiscountCategoryModel>();
				_logger.LogDebug("PrepareDiscountCategoryListModelAsync(searchModel={SearchModel},discount={Discount}): discountCategoryModel → {DiscountCategoryModel}", searchModel, discount, discountCategoryModel);

                discountCategoryModel.CategoryName = await _categoryService.GetFormattedBreadCrumbAsync(category);
                discountCategoryModel.CategoryId = category.Id;
				_logger.LogDebug("PrepareDiscountCategoryListModelAsync(searchModel={SearchModel},discount={Discount}): discountCategoryModel.CategoryId → {Arg2}", searchModel, discount, discountCategoryModel.CategoryId);

                return discountCategoryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareDiscountCategoryListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return model;
    }

    /// <summary>
    /// Prepare category search model to add to the discount
    /// </summary>
    /// <param name="searchModel">Category search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category search model to add to the discount
    /// </returns>
    public virtual Task<AddCategoryToDiscountSearchModel> PrepareAddCategoryToDiscountSearchModelAsync(AddCategoryToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddCategoryToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddCategoryToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged category list model to add to the discount
    /// </summary>
    /// <param name="searchModel">Category search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category list model to add to the discount
    /// </returns>
    public virtual async Task<AddCategoryToDiscountListModel> PrepareAddCategoryToDiscountListModelAsync(AddCategoryToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddCategoryToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get categories
        var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true,
            categoryName: searchModel.SearchCategoryName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddCategoryToDiscountListModel().PrepareToGridAsync(searchModel, categories, () =>
        {
            return categories.SelectAwait(async category =>
            {
                //fill in model values from the entity
                var categoryModel = category.ToModel<CategoryModel>();
				_logger.LogDebug("PrepareAddCategoryToDiscountListModelAsync(searchModel={SearchModel}): categoryModel → {CategoryModel}", searchModel, categoryModel);

                //fill in additional values (not existing in the entity)
                categoryModel.Breadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
                categoryModel.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);

                return categoryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddCategoryToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged discount manufacturer list model
    /// </summary>
    /// <param name="searchModel">Discount manufacturer search model</param>
    /// <param name="discount">Discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the discount manufacturer list model
    /// </returns>
    public virtual async Task<DiscountManufacturerListModel> PrepareDiscountManufacturerListModelAsync(DiscountManufacturerSearchModel searchModel,
        Discount discount)
    {
		_logger.LogDebug(">>> Entering PrepareDiscountManufacturerListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(discount);

        //get manufacturers with applied discount
        var discountManufacturers = await _manufacturerService.GetManufacturersWithAppliedDiscountAsync(discountId: discount.Id,
            showHidden: false,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = new DiscountManufacturerListModel().PrepareToGrid(searchModel, discountManufacturers, () =>
        {
            //fill in model values from the entity
            return discountManufacturers.Select(manufacturer =>
            {
                var discountManufacturerModel = manufacturer.ToModel<DiscountManufacturerModel>();
				_logger.LogDebug("PrepareDiscountManufacturerListModelAsync(searchModel={SearchModel},discount={Discount}): discountManufacturerModel → {DiscountManufacturerModel}", searchModel, discount, discountManufacturerModel);
				_logger.LogDebug("PrepareDiscountManufacturerListModelAsync(searchModel={SearchModel},discount={Discount}): discountManufacturerModel.ManufacturerId → {Arg2}", searchModel, discount, discountManufacturerModel.ManufacturerId);
				_logger.LogDebug("PrepareDiscountManufacturerListModelAsync(searchModel={SearchModel},discount={Discount}): discountManufacturerModel.ManufacturerName → {Arg2}", searchModel, discount, discountManufacturerModel.ManufacturerName);
                discountManufacturerModel.ManufacturerId = manufacturer.Id;
                discountManufacturerModel.ManufacturerName = manufacturer.Name;

                return discountManufacturerModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareDiscountManufacturerListModelAsync(searchModel={SearchModel},discount={Discount})", searchModel, discount);
        return model;
    }

    /// <summary>
    /// Prepare manufacturer search model to add to the discount
    /// </summary>
    /// <param name="searchModel">Manufacturer search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer search model to add to the discount
    /// </returns>
    public virtual Task<AddManufacturerToDiscountSearchModel> PrepareAddManufacturerToDiscountSearchModelAsync(AddManufacturerToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddManufacturerToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareAddManufacturerToDiscountSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged manufacturer list model to add to the discount
    /// </summary>
    /// <param name="searchModel">Manufacturer search model to add to the discount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer list model to add to the discount
    /// </returns>
    public virtual async Task<AddManufacturerToDiscountListModel> PrepareAddManufacturerToDiscountListModelAsync(AddManufacturerToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareAddManufacturerToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get manufacturers
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true,
            manufacturerName: searchModel.SearchManufacturerName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddManufacturerToDiscountListModel().PrepareToGridAsync(searchModel, manufacturers, () =>
        {
            return manufacturers.SelectAwait(async manufacturer =>
            {
                var manufacturerModel = manufacturer.ToModel<ManufacturerModel>();
				_logger.LogDebug("PrepareAddManufacturerToDiscountListModelAsync(searchModel={SearchModel}): manufacturerModel → {ManufacturerModel}", searchModel, manufacturerModel);
                manufacturerModel.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);

                return manufacturerModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareAddManufacturerToDiscountListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}