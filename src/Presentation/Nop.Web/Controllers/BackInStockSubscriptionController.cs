using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Http;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Infrastructure;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class BackInStockSubscriptionController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IProductService _productService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public BackInStockSubscriptionController(CatalogSettings catalogSettings,
        CustomerSettings customerSettings,
        IBackInStockSubscriptionService backInStockSubscriptionService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IProductService productService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext})", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext);
        _catalogSettings = catalogSettings;
        _customerSettings = customerSettings;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _customerSettings → {_customerSettings}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _customerSettings);
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _catalogSettings → {_catalogSettings}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _catalogSettings);
    _logger.LogWarning("<<< Exiting BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext})", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext);
        _backInStockSubscriptionService = backInStockSubscriptionService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _backInStockSubscriptionService → {_backInStockSubscriptionService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _backInStockSubscriptionService);
        _customerService = customerService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _customerService → {_customerService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _customerService);
        _localizationService = localizationService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _localizationService → {_localizationService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _notificationService → {_notificationService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _notificationService);
        _productService = productService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _productService → {_productService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _productService);
        _storeContext = storeContext;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _storeContext → {_storeContext}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _storeContext);
        _urlRecordService = urlRecordService;
		_logger.LogDebug("BackInStockSubscriptionController(catalogSettings={CatalogSettings},customerSettings={CustomerSettings},backInStockSubscriptionService={BackInStockSubscriptionService},customerService={CustomerService},localizationService={LocalizationService},notificationService={NotificationService},productService={ProductService},storeContext={StoreContext},urlRecordService={UrlRecordService},workContext={WorkContext}): _urlRecordService → {_urlRecordService}", catalogSettings, customerSettings, backInStockSubscriptionService, customerService, localizationService, notificationService, productService, storeContext, urlRecordService, workContext, _urlRecordService);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    // Product details page > back in stock subscribe
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> SubscribePopup(int productId)
    {
		_logger.LogError(">>> Entering SubscribePopup(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted)
			_logger.LogError("Throwing ArgumentException from SubscribePopup(productId={ProductId})", productId);
			_logger.LogError("SubscribePopup(productId={ProductId}): product → {Product}", productId, product);
            throw new ArgumentException("No product found with the specified id");

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("SubscribePopup(productId={ProductId}): customer → {Customer}", productId, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("SubscribePopup(productId={ProductId}): store → {Store}", productId, store);
        var model = new BackInStockSubscribeModel
        {
            ProductId = product.Id,
            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
            IsCurrentCustomerRegistered = await _customerService.IsRegisteredAsync(customer),
            MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions,
            CurrentNumberOfBackInStockSubscriptions = (await _backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                .TotalCount
        };
        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
            product.BackorderMode == BackorderMode.NoBackorders &&
            product.AllowBackInStockSubscriptions &&
            await _productService.GetTotalStockQuantityAsync(product) <= 0)
        {
            //out of stock
            model.SubscriptionAllowed = true;
			_logger.LogDebug("SubscribePopup(productId={ProductId}): model.SubscriptionAllowed → {Arg1}", productId, model.SubscriptionAllowed);
            model.AlreadySubscribed = await _backInStockSubscriptionService
                .FindSubscriptionAsync(customer.Id, product.Id, store.Id) != null;
				_logger.LogDebug("SubscribePopup(productId={ProductId}): model.AlreadySubscribed → {Arg1}", productId, model.AlreadySubscribed);
        }

		_logger.LogWarning("<<< Exiting SubscribePopup(productId={ProductId})", productId);
        return PartialView(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> SubscribePopupPOST(int productId)
    {
		_logger.LogError(">>> Entering SubscribePopupPOST(productId={ProductId})", productId);
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || product.Deleted)
			_logger.LogError("Throwing ArgumentException from SubscribePopupPOST(productId={ProductId})", productId);
			_logger.LogError("SubscribePopupPOST(productId={ProductId}): product → {Product}", productId, product);
            throw new ArgumentException("No product found with the specified id");

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("SubscribePopupPOST(productId={ProductId}): customer → {Customer}", productId, customer);
        if (!await _customerService.IsRegisteredAsync(customer))
            return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.OnlyRegistered"));

        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
            product.BackorderMode == BackorderMode.NoBackorders &&
            product.AllowBackInStockSubscriptions &&
            await _productService.GetTotalStockQuantityAsync(product) <= 0)
        {
            //out of stock
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogError("SubscribePopupPOST(productId={ProductId}): store → {Store}", productId, store);
            var subscription = await _backInStockSubscriptionService
                .FindSubscriptionAsync(customer.Id, product.Id, store.Id);
            if (subscription != null)
            {
                //subscription already exists
                //unsubscribe
                await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Unsubscribed"));
                return new OkResult();
            }

            //subscription does not exist
            //subscribe
            if ((await _backInStockSubscriptionService
                    .GetAllSubscriptionsByCustomerIdAsync(customer.Id, store.Id, 0, 1))
                .TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
            {
                return Json(new
                {
                    result = string.Format(await _localizationService.GetResourceAsync("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions)
                });
            }
            subscription = new BackInStockSubscription
            {
                CustomerId = customer.Id,
                ProductId = product.Id,
                StoreId = store.Id,
                CreatedOnUtc = DateTime.UtcNow
            };
            await _backInStockSubscriptionService.InsertSubscriptionAsync(subscription);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("BackInStockSubscriptions.Notification.Subscribed"));
		_logger.LogWarning("<<< Exiting SubscribePopupPOST(productId={ProductId})", productId);
            return new OkResult();
        }

        //subscription not possible
        return Content(await _localizationService.GetResourceAsync("BackInStockSubscriptions.NotAllowed"));
    }

    // My account / Back in stock subscriptions
    public virtual async Task<IActionResult> CustomerSubscriptions(int? pageNumber)
    {
		_logger.LogError(">>> Entering CustomerSubscriptions(pageNumber={PageNumber})", pageNumber);
        if (_customerSettings.HideBackInStockSubscriptionsTab)
        {
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        var pageIndex = 0;
		_logger.LogError("CustomerSubscriptions(pageNumber={PageNumber}): pageIndex → {PageIndex}", pageNumber, pageIndex);
        if (pageNumber > 0)
        {
            pageIndex = pageNumber.Value - 1;
        }
        var pageSize = 10;

        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogError("CustomerSubscriptions(pageNumber={PageNumber}): customer → {Customer}", pageNumber, customer);
        var store = await _storeContext.GetCurrentStoreAsync();
		_logger.LogError("CustomerSubscriptions(pageNumber={PageNumber}): store → {Store}", pageNumber, store);
        var list = await _backInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id,
            store.Id, pageIndex, pageSize);

        var model = new CustomerBackInStockSubscriptionsModel();
		_logger.LogError("CustomerSubscriptions(pageNumber={PageNumber}): model → {Model}", pageNumber, model);

        foreach (var subscription in list)
        {
            var product = await _productService.GetProductByIdAsync(subscription.ProductId);
			_logger.LogError("CustomerSubscriptions(pageNumber={PageNumber}): product → {Product}", pageNumber, product);

            if (product != null)
            {
                var subscriptionModel = new CustomerBackInStockSubscriptionsModel.BackInStockSubscriptionModel
                {
                    Id = subscription.Id,
                    ProductId = product.Id,
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                };
                model.Subscriptions.Add(subscriptionModel);
            }
        }

        model.PagerModel = new PagerModel(_localizationService)
        {
            PageSize = list.PageSize,
            TotalRecords = list.TotalCount,
            PageIndex = list.PageIndex,
            ShowTotalSummary = false,
            RouteActionName = NopRouteNames.Standard.CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS,
            UseRouteLinks = true,
            RouteValues = new BackInStockSubscriptionsRouteValues { PageNumber = pageIndex }
        };

		_logger.LogWarning("<<< Exiting CustomerSubscriptions(pageNumber={PageNumber})", pageNumber);
        return View(model);
    }

    [HttpPost, ActionName("CustomerSubscriptions")]
    public virtual async Task<IActionResult> CustomerSubscriptionsPOST(IFormCollection formCollection)
    {
		_logger.LogError(">>> Entering CustomerSubscriptionsPOST(formCollection={FormCollection})", formCollection);
        foreach (var key in formCollection.Keys)
        {
            var value = formCollection[key];

            if (value.Equals("on") && key.StartsWith("biss", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = key.Replace("biss", "").Trim();
				_logger.LogError("CustomerSubscriptionsPOST(formCollection={FormCollection}): id → {Id}", formCollection, id);
                if (int.TryParse(id, out var subscriptionId))
                {
                    var subscription = await _backInStockSubscriptionService.GetSubscriptionByIdAsync(subscriptionId);
					_logger.LogError("CustomerSubscriptionsPOST(formCollection={FormCollection}): subscription → {Subscription}", formCollection, subscription);
                    var customer = await _workContext.GetCurrentCustomerAsync();
					_logger.LogError("CustomerSubscriptionsPOST(formCollection={FormCollection}): customer → {Customer}", formCollection, customer);
                    if (subscription != null && subscription.CustomerId == customer.Id)
                    {
                        await _backInStockSubscriptionService.DeleteSubscriptionAsync(subscription);
                    }
                }
            }
        }

		_logger.LogWarning("<<< Exiting CustomerSubscriptionsPOST(formCollection={FormCollection})", formCollection);
        return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_BACK_IN_STOCK_SUBSCRIPTIONS);
    }

    #endregion

    #region Nested class

    /// <summary>
    /// record that has only page for route value. Used for (My Account) Back in stock subscriptions pagination
    /// </summary>
    public partial record BackInStockSubscriptionsRouteValues : BaseRouteValues;

    #endregion
}