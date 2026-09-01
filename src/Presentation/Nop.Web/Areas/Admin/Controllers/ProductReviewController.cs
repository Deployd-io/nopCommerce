using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class ProductReviewController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductReviewModelFactory _productReviewModelFactory;
    protected readonly IProductReviewService _productReviewService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion Fields

    #region Ctor

    public ProductReviewController(CatalogSettings catalogSettings,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductReviewModelFactory productReviewModelFactory,
        IProductReviewService productReviewService,
        IProductService productService,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService)
    {
		_logger.LogDebug(">>> Entering ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService})", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _catalogSettings → {_catalogSettings}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _catalogSettings);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _customerActivityService → {_customerActivityService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _customerActivityService);
        _customerService = customerService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _customerService → {_customerService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _customerService);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _eventPublisher → {_eventPublisher}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _eventPublisher);
        _localizationService = localizationService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _localizationService → {_localizationService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _notificationService → {_notificationService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _permissionService → {_permissionService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _permissionService);
        _productReviewModelFactory = productReviewModelFactory;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _productReviewModelFactory → {_productReviewModelFactory}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _productReviewModelFactory);
        _productReviewService = productReviewService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _productReviewService → {_productReviewService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _productReviewService);
        _productService = productService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _productService → {_productService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _productService);
        _workContext = workContext;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _workContext → {_workContext}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _workContext);
        _workflowMessageService = workflowMessageService;
		_logger.LogDebug("ProductReviewController(catalogSettings={CatalogSettings},customerActivityService={CustomerActivityService},customerService={CustomerService},eventPublisher={EventPublisher},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},productReviewModelFactory={ProductReviewModelFactory},productReviewService={ProductReviewService},productService={ProductService},workContext={WorkContext},workflowMessageService={WorkflowMessageService}): _workflowMessageService → {_workflowMessageService}", catalogSettings, customerActivityService, customerService, eventPublisher, localizationService, notificationService, permissionService, productReviewModelFactory, productReviewService, productService, workContext, workflowMessageService, _workflowMessageService);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewSearchModelAsync(new ProductReviewSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> List(ProductReviewSearchModel searchModel)
    {
        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a product review with the specified id
        var productReview = await _productReviewService.GetProductReviewByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): productReview → {ProductReview}", id, productReview);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("Edit(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        if (currentVendor != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewModelAsync(null, productReview);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(ProductReviewModel model, bool continueEditing)
    {
        //try to get a product review with the specified id
        var productReview = await _productReviewService.GetProductReviewByIdAsync(model.Id);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && (await _productService.GetProductByIdAsync(productReview.ProductId)).VendorId != currentVendor.Id)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var previousIsApproved = productReview.IsApproved;

            //vendor can edit "Reply text" only
            var isLoggedInAsVendor = currentVendor != null;
            if (!isLoggedInAsVendor)
            {
                productReview.Title = model.Title;
                productReview.ReviewText = model.ReviewText;
                productReview.IsApproved = model.IsApproved;
            }

            productReview.ReplyText = model.ReplyText;

            //notify customer about reply
            if (productReview.IsApproved &&
                !string.IsNullOrEmpty(productReview.ReplyText) &&
                _catalogSettings.NotifyCustomerAboutProductReviewReply && !productReview.CustomerNotifiedOfReply)
            {
                var customer = await _customerService.GetCustomerByIdAsync(productReview.CustomerId);
                var customerLanguageId = customer?.LanguageId ?? 0;

                var queuedEmailIds = await _workflowMessageService.SendProductReviewReplyCustomerNotificationMessageAsync(productReview, customerLanguageId);
                if (queuedEmailIds.Any())
                    productReview.CustomerNotifiedOfReply = true;
            }

            await _productReviewService.UpdateProductReviewAsync(productReview);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditProductReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditProductReview"), productReview.Id), productReview);

            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                var product = await _productService.GetProductByIdAsync(productReview.ProductId);
                //update product totals
                await _productReviewService.UpdateProductReviewTotalsAsync(product);

                //raise event (only if it wasn't approved before and is approved now)
                if (!previousIsApproved && productReview.IsApproved)
                    await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Updated"));

            return continueEditing ? RedirectToAction("Edit", new { id = productReview.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _productReviewModelFactory.PrepareProductReviewModelAsync(model, productReview, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a product review with the specified id
        var productReview = await _productReviewService.GetProductReviewByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): productReview → {ProductReview}", id, productReview);
        if (productReview == null)
            return RedirectToAction("List");

        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        await _productReviewService.DeleteProductReviewAsync(productReview);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteProductReview",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteProductReview"), productReview.Id), productReview);

        var product = await _productService.GetProductByIdAsync(productReview.ProductId);
		_logger.LogDebug("Delete(id={Id}): product → {Product}", id, product);

        //update product totals
        await _productReviewService.UpdateProductReviewTotalsAsync(product);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.ProductReviews.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter not approved reviews
        var productReviews = (await _productReviewService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);
		_logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): productReviews → {ProductReviews}", selectedIds, productReviews);

        foreach (var productReview in productReviews)
        {
            productReview.IsApproved = true;
            await _productReviewService.UpdateProductReviewAsync(productReview);

            var product = await _productService.GetProductByIdAsync(productReview.ProductId);
			_logger.LogDebug("ApproveSelected(selectedIds={SelectedIds}): product → {Product}", selectedIds, product);

            //update product totals
            await _productReviewService.UpdateProductReviewTotalsAsync(product);

            //raise event 
            await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));
        }

		_logger.LogDebug("<<< Exiting ApproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        //filter approved reviews
        var productReviews = (await _productReviewService.GetProductReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);
		_logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): productReviews → {ProductReviews}", selectedIds, productReviews);

        foreach (var productReview in productReviews)
        {
            productReview.IsApproved = false;
            await _productReviewService.UpdateProductReviewAsync(productReview);

            var product = await _productService.GetProductByIdAsync(productReview.ProductId);
			_logger.LogDebug("DisapproveSelected(selectedIds={SelectedIds}): product → {Product}", selectedIds, product);

            //update product totals
            await _productReviewService.UpdateProductReviewTotalsAsync(product);
        }

		_logger.LogDebug("<<< Exiting DisapproveSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
		_logger.LogDebug(">>> Entering DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        //a vendor does not have access to this functionality
        if (await _workContext.GetCurrentVendorAsync() != null)
            return RedirectToAction("List");

        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var productReviews = await _productReviewService.GetProductReviewsByIdsAsync(selectedIds.ToArray());
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): productReviews → {ProductReviews}", selectedIds, productReviews);
        var products = await _productService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): products → {Products}", selectedIds, products);

        await _productReviewService.DeleteProductReviewsAsync(productReviews);

        //activity log
        var activityLogFormat = await _localizationService.GetResourceAsync("ActivityLog.DeleteProductReview");
		_logger.LogDebug("DeleteSelected(selectedIds={SelectedIds}): activityLogFormat → {ActivityLogFormat}", selectedIds, activityLogFormat);
        await _customerActivityService.InsertActivitiesAsync("DeleteProductReview", productReviews, productReview => string.Format(activityLogFormat, productReview.Id));

        //update product totals
        foreach (var product in products)
        {
            await _productReviewService.UpdateProductReviewTotalsAsync(product);
        }

		_logger.LogDebug("<<< Exiting DeleteSelected(selectedIds={SelectedIds})", selectedIds);
        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Catalog.PRODUCT_REVIEWS_VIEW)]
    public virtual async Task<IActionResult> ProductReviewReviewTypeMappingList(ProductReviewReviewTypeMappingSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductReviewReviewTypeMappingList(searchModel={SearchModel})", searchModel);
        var productReview = await _productReviewService.GetProductReviewByIdAsync(searchModel.ProductReviewId)
            ?? throw new ArgumentException("No product review found with the specified id");

        //prepare model
        var model = await _productReviewModelFactory.PrepareProductReviewReviewTypeMappingListModelAsync(searchModel, productReview);
		_logger.LogDebug("ProductReviewReviewTypeMappingList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ProductReviewReviewTypeMappingList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    #endregion
}