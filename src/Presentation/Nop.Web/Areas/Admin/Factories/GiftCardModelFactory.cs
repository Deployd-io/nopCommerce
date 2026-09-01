using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the gift card model factory implementation
/// </summary>
public partial class GiftCardModelFactory : IGiftCardModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderService _orderService;
    protected readonly IPriceFormatter _priceFormatter;

    #endregion

    #region Ctor

    public GiftCardModelFactory(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        IOrderService orderService,
        IPriceFormatter priceFormatter)
    {
		_logger.LogDebug(">>> Entering GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter})", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter);
        _currencySettings = currencySettings;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _currencySettings → {_currencySettings}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _currencySettings);
        _currencyService = currencyService;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _currencyService → {_currencyService}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _currencyService);
        _dateTimeHelper = dateTimeHelper;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _dateTimeHelper → {_dateTimeHelper}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _dateTimeHelper);
        _giftCardService = giftCardService;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _giftCardService → {_giftCardService}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _giftCardService);
        _localizationService = localizationService;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _localizationService → {_localizationService}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _localizationService);
        _orderService = orderService;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _orderService → {_orderService}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _orderService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("GiftCardModelFactory(currencySettings={CurrencySettings},currencyService={CurrencyService},dateTimeHelper={DateTimeHelper},giftCardService={GiftCardService},localizationService={LocalizationService},orderService={OrderService},priceFormatter={PriceFormatter}): _priceFormatter → {_priceFormatter}", currencySettings, currencyService, dateTimeHelper, giftCardService, localizationService, orderService, priceFormatter, _priceFormatter);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare gift card usage history search model
    /// </summary>
    /// <param name="searchModel">Gift card usage history search model</param>
    /// <param name="giftCard">Gift card</param>
    /// <returns>Gift card usage history search model</returns>
    protected virtual GiftCardUsageHistorySearchModel PrepareGiftCardUsageHistorySearchModel(GiftCardUsageHistorySearchModel searchModel,
        GiftCard giftCard)
    {
		_logger.LogDebug(">>> Entering PrepareGiftCardUsageHistorySearchModel(searchModel={SearchModel},giftCard={GiftCard})", searchModel, giftCard);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(giftCard);

        searchModel.GiftCardId = giftCard.Id;
		_logger.LogDebug("PrepareGiftCardUsageHistorySearchModel(searchModel={SearchModel},giftCard={GiftCard}): searchModel.GiftCardId → {Arg2}", searchModel, giftCard, searchModel.GiftCardId);

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareGiftCardUsageHistorySearchModel(searchModel={SearchModel},giftCard={GiftCard})", searchModel, giftCard);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare gift card search model
    /// </summary>
    /// <param name="searchModel">Gift card search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card search model
    /// </returns>
    public virtual async Task<GiftCardSearchModel> PrepareGiftCardSearchModelAsync(GiftCardSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareGiftCardSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare "activated" filter (0 - all; 1 - activated only; 2 - deactivated only)
        searchModel.ActivatedList.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.GiftCards.List.Activated.All")
        });
        searchModel.ActivatedList.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.GiftCards.List.Activated.ActivatedOnly")
        });
        searchModel.ActivatedList.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.GiftCards.List.Activated.DeactivatedOnly")
        });

        //prepare page parameters
        searchModel.SetGridPageSize();

		_logger.LogDebug("<<< Exiting PrepareGiftCardSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged gift card list model
    /// </summary>
    /// <param name="searchModel">Gift card search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card list model
    /// </returns>
    public virtual async Task<GiftCardListModel> PrepareGiftCardListModelAsync(GiftCardSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PrepareGiftCardListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter gift cards
        var isActivatedOnly = searchModel.ActivatedId == 0 ? null : searchModel.ActivatedId == 1 ? true : (bool?)false;
		_logger.LogDebug("PrepareGiftCardListModelAsync(searchModel={SearchModel}): isActivatedOnly → {IsActivatedOnly}", searchModel, isActivatedOnly);

        //get gift cards
        var giftCards = await _giftCardService.GetAllGiftCardsAsync(isGiftCardActivated: isActivatedOnly,
            giftCardCouponCode: searchModel.CouponCode,
            recipientName: searchModel.RecipientName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new GiftCardListModel().PrepareToGridAsync(searchModel, giftCards, () =>
        {
            return giftCards.SelectAwait(async giftCard =>
            {
                //fill in model values from the entity
                var giftCardModel = giftCard.ToModel<GiftCardModel>();
				_logger.LogDebug("PrepareGiftCardListModelAsync(searchModel={SearchModel}): giftCardModel → {GiftCardModel}", searchModel, giftCardModel);

                //convert dates to the user time
                giftCardModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                var giftAmount = await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard);
				_logger.LogDebug("PrepareGiftCardListModelAsync(searchModel={SearchModel}): giftAmount → {GiftAmount}", searchModel, giftAmount);
                giftCardModel.RemainingAmountStr = await _priceFormatter.FormatPriceAsync(giftAmount, true, false);
                giftCardModel.AmountStr = await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, false);

                return giftCardModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareGiftCardListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare gift card model
    /// </summary>
    /// <param name="model">Gift card model</param>
    /// <param name="giftCard">Gift card</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card model
    /// </returns>
    public virtual async Task<GiftCardModel> PrepareGiftCardModelAsync(GiftCardModel model, GiftCard giftCard, bool excludeProperties = false)
    {
		_logger.LogDebug(">>> Entering PrepareGiftCardModelAsync(model={Model},giftCard={GiftCard},excludeProperties={ExcludeProperties})", model, giftCard, excludeProperties);
        if (giftCard != null)
        {
            //fill in model values from the entity
            model ??= giftCard.ToModel<GiftCardModel>();

            var order = await _orderService.GetOrderByOrderItemAsync(giftCard.PurchasedWithOrderItemId ?? 0);
			_logger.LogDebug("PrepareGiftCardModelAsync(model={Model},giftCard={GiftCard},excludeProperties={ExcludeProperties}): order → {Order}", model, giftCard, excludeProperties, order);

            model.PurchasedWithOrderId = order?.Id;
			_logger.LogDebug("PrepareGiftCardModelAsync(model={Model},giftCard={GiftCard},excludeProperties={ExcludeProperties}): model.PurchasedWithOrderId → {Arg3}", model, giftCard, excludeProperties, model.PurchasedWithOrderId);
            model.RemainingAmountStr = await _priceFormatter.FormatPriceAsync(await _giftCardService.GetGiftCardRemainingAmountAsync(giftCard), true, false);
            model.AmountStr = await _priceFormatter.FormatPriceAsync(giftCard.Amount, true, false);
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(giftCard.CreatedOnUtc, DateTimeKind.Utc);
            model.PurchasedWithOrderNumber = order?.CustomOrderNumber;
			_logger.LogDebug("PrepareGiftCardModelAsync(model={Model},giftCard={GiftCard},excludeProperties={ExcludeProperties}): model.PurchasedWithOrderNumber → {Arg3}", model, giftCard, excludeProperties, model.PurchasedWithOrderNumber);

            //prepare nested search model
            PrepareGiftCardUsageHistorySearchModel(model.GiftCardUsageHistorySearchModel, giftCard);
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;

		_logger.LogDebug("<<< Exiting PrepareGiftCardModelAsync(model={Model},giftCard={GiftCard},excludeProperties={ExcludeProperties})", model, giftCard, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare paged gift usage history card list model
    /// </summary>
    /// <param name="searchModel">Gift card usage history search model</param>
    /// <param name="giftCard">Gift card</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gift card usage history list model
    /// </returns>
    public virtual async Task<GiftCardUsageHistoryListModel> PrepareGiftCardUsageHistoryListModelAsync(GiftCardUsageHistorySearchModel searchModel,
        GiftCard giftCard)
    {
		_logger.LogDebug(">>> Entering PrepareGiftCardUsageHistoryListModelAsync(searchModel={SearchModel},giftCard={GiftCard})", searchModel, giftCard);
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(giftCard);

        //get gift card usage history
        var usageHistory = (await _giftCardService.GetGiftCardUsageHistoryAsync(giftCard))
            .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new GiftCardUsageHistoryListModel().PrepareToGridAsync(searchModel, usageHistory, () =>
        {
            return usageHistory.SelectAwait(async historyEntry =>
            {
                //fill in model values from the entity
                var giftCardUsageHistoryModel = historyEntry.ToModel<GiftCardUsageHistoryModel>();
				_logger.LogDebug("PrepareGiftCardUsageHistoryListModelAsync(searchModel={SearchModel},giftCard={GiftCard}): giftCardUsageHistoryModel → {GiftCardUsageHistoryModel}", searchModel, giftCard, giftCardUsageHistoryModel);

                //convert dates to the user time
                giftCardUsageHistoryModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                giftCardUsageHistoryModel.OrderId = historyEntry.UsedWithOrderId;
				_logger.LogDebug("PrepareGiftCardUsageHistoryListModelAsync(searchModel={SearchModel},giftCard={GiftCard}): giftCardUsageHistoryModel.OrderId → {Arg2}", searchModel, giftCard, giftCardUsageHistoryModel.OrderId);
                giftCardUsageHistoryModel.CustomOrderNumber = (await _orderService.GetOrderByIdAsync(historyEntry.UsedWithOrderId))?.CustomOrderNumber;
                giftCardUsageHistoryModel.UsedValue = await _priceFormatter.FormatPriceAsync(historyEntry.UsedValue, true, false);

                return giftCardUsageHistoryModel;
            });
        });

		_logger.LogDebug("<<< Exiting PrepareGiftCardUsageHistoryListModelAsync(searchModel={SearchModel},giftCard={GiftCard})", searchModel, giftCard);
        return model;
    }

    #endregion
}