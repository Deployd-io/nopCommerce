using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Discounts;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class DiscountController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICategoryService _categoryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly IDiscountModelFactory _discountModelFactory;
    protected readonly IDiscountPluginManager _discountPluginManager;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IProductService _productService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public DiscountController(CatalogSettings catalogSettings,
        ICategoryService categoryService,
        ICustomerActivityService customerActivityService,
        IDiscountModelFactory discountModelFactory,
        IDiscountPluginManager discountPluginManager,
        IDiscountService discountService,
        ILocalizationService localizationService,
        IManufacturerService manufacturerService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IProductService productService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _catalogSettings → {_catalogSettings}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _catalogSettings);
        _categoryService = categoryService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _categoryService → {_categoryService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _categoryService);
        _customerActivityService = customerActivityService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _customerActivityService → {_customerActivityService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _customerActivityService);
        _discountModelFactory = discountModelFactory;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _discountModelFactory → {_discountModelFactory}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _discountModelFactory);
        _discountPluginManager = discountPluginManager;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _discountPluginManager → {_discountPluginManager}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _discountPluginManager);
        _discountService = discountService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _discountService → {_discountService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _discountService);
        _localizationService = localizationService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _localizationService → {_localizationService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _localizationService);
        _manufacturerService = manufacturerService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _manufacturerService → {_manufacturerService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _manufacturerService);
        _notificationService = notificationService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _notificationService → {_notificationService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _permissionService → {_permissionService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _permissionService);
        _productService = productService;
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _productService → {_productService}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _productService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext})", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext);
		_logger.LogDebug("DiscountController(catalogSettings={CatalogSettings},categoryService={CategoryService},customerActivityService={CustomerActivityService},discountModelFactory={DiscountModelFactory},discountPluginManager={DiscountPluginManager},discountService={DiscountService},localizationService={LocalizationService},manufacturerService={ManufacturerService},notificationService={NotificationService},permissionService={PermissionService},productService={ProductService},workContext={WorkContext}): _workContext → {_workContext}", catalogSettings, categoryService, customerActivityService, discountModelFactory, discountPluginManager, discountService, localizationService, manufacturerService, notificationService, permissionService, productService, workContext, _workContext);
    }

    #endregion

    #region Methods

    #region Discounts

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
		_logger.LogDebug("<<< Exiting Index()");
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
		_logger.LogDebug(">>> Entering List()");
        //whether discounts are ignored
        if (_catalogSettings.IgnoreDiscounts)
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.IgnoreDiscounts.Warning"));

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountSearchModelAsync(new DiscountSearchModel());
		_logger.LogDebug("List(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting List()");
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> List(DiscountSearchModel searchModel)
    {
        //prepare model
        var model = await _discountModelFactory.PrepareDiscountListModelAsync(searchModel);

		_logger.LogDebug("<<< Exiting List(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
		_logger.LogDebug(">>> Entering Create()");
        //prepare model
        var model = await _discountModelFactory.PrepareDiscountModelAsync(new DiscountModel(), null);
		_logger.LogDebug("Create(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Create()");
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(DiscountModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                model.VendorId = currentVendor.Id;

            var discount = model.ToEntity<Discount>();
            await _discountService.InsertDiscountAsync(discount);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewDiscount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewDiscount"), discount.Name), discount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = discount.Id });
        }

        //prepare model
        model = await _discountModelFactory.PrepareDiscountModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Create(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
		_logger.LogDebug(">>> Entering Edit(id={Id})", id);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(id);
		_logger.LogDebug("Edit(id={Id}): discount → {Discount}", id, discount);
        if (discount == null)
            return RedirectToAction("List");

        //a vendor should have access only to his discounts
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("Edit(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        if (currentVendor != null && discount.VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountModelAsync(null, discount);
		_logger.LogDebug("Edit(id={Id}): model → {Model}", id, model);

		_logger.LogDebug("<<< Exiting Edit(id={Id})", id);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(DiscountModel model, bool continueEditing)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.Id);
        if (discount == null)
            return RedirectToAction("List");

        //a vendor should have access only to his discounts
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && discount.VendorId != currentVendor.Id)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var prevDiscountType = discount.DiscountType;
            discount = model.ToEntity(discount);
            await _discountService.UpdateDiscountAsync(discount);

            //clean up old references (if changed) 
            if (prevDiscountType != discount.DiscountType)
            {
                switch (prevDiscountType)
                {
                    case DiscountType.AssignedToSkus:
                        await _productService.ClearDiscountProductMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToCategories:
                        await _categoryService.ClearDiscountCategoryMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToManufacturers:
                        await _manufacturerService.ClearDiscountManufacturerMappingAsync(discount);
                        break;
                    default:
                        break;
                }
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("EditDiscount",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditDiscount"), discount.Name), discount);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = discount.Id });
        }

        //prepare model
        model = await _discountModelFactory.PrepareDiscountModelAsync(model, discount, true);

        //if we got this far, something failed, redisplay form
		_logger.LogDebug("<<< Exiting Edit(model={Model},continueEditing={ContinueEditing})", model, continueEditing);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
		_logger.LogDebug(">>> Entering Delete(id={Id})", id);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(id);
		_logger.LogDebug("Delete(id={Id}): discount → {Discount}", id, discount);
        if (discount == null)
            return RedirectToAction("List");

        //a vendor should have access only to his discounts
        var currentVendor = await _workContext.GetCurrentVendorAsync();
		_logger.LogDebug("Delete(id={Id}): currentVendor → {CurrentVendor}", id, currentVendor);
        if (currentVendor != null && discount.VendorId != currentVendor.Id)
            return RedirectToAction("List");

        //applied to products
        var products = await _productService.GetProductsWithAppliedDiscountAsync(discount.Id, true);
		_logger.LogDebug("Delete(id={Id}): products → {Products}", id, products);

        await _discountService.DeleteDiscountAsync(discount);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteDiscount",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteDiscount"), discount.Name), discount);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Deleted"));

		_logger.LogDebug("<<< Exiting Delete(id={Id})", id);
        return RedirectToAction("List");
    }

    #endregion

    #region Discount requirements

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
    {
		_logger.LogDebug(">>> Entering GetDiscountRequirementConfigurationUrl(systemName={SystemName},discountId={DiscountId},discountRequirementId={DiscountRequirementId})", systemName, discountId, discountRequirementId);
        ArgumentException.ThrowIfNullOrEmpty(systemName);

        var discountRequirementRule = await _discountPluginManager.LoadPluginBySystemNameAsync(systemName)
            ?? throw new ArgumentException("Discount requirement rule could not be loaded");

        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("Discount could not be loaded");

        var url = discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId);
		_logger.LogDebug("GetDiscountRequirementConfigurationUrl(systemName={SystemName},discountId={DiscountId},discountRequirementId={DiscountRequirementId}): url → {Url}", systemName, discountId, discountRequirementId, url);

		_logger.LogDebug("<<< Exiting GetDiscountRequirementConfigurationUrl(systemName={SystemName},discountId={DiscountId},discountRequirementId={DiscountRequirementId})", systemName, discountId, discountRequirementId);
        return Json(new { url });
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> GetDiscountRequirements(int discountId, int discountRequirementId,
        int? parentId, int? interactionTypeId, bool deleteRequirement)
    {
		_logger.LogDebug(">>> Entering GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement})", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement);
        var requirements = new List<DiscountRequirementRuleModel>();

		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): requirements → {Requirements}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, requirements);
        var discount = await _discountService.GetDiscountByIdAsync(discountId);
		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): discount → {Discount}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, discount);
        if (discount == null)
            return Json(requirements);

        var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId);
		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): discountRequirement → {DiscountRequirement}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, discountRequirement);
        if (discountRequirement != null)
        {
            //delete
            if (deleteRequirement)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, true);

                var discountRequirements = await _discountService.GetAllDiscountRequirementsAsync(discount.Id);
				_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): discountRequirements → {DiscountRequirements}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, discountRequirements);

                //delete default group if there are no any requirements
                if (!discountRequirements.Any(requirement => requirement.ParentId.HasValue))
                {
                    foreach (var dr in discountRequirements)
                        await _discountService.DeleteDiscountRequirementAsync(dr, true);
                }
            }
            //or update the requirement
            else
            {
                var defaultGroupId = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup)?.Id ?? 0;
				_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): defaultGroupId → {DefaultGroupId}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, defaultGroupId);
                if (defaultGroupId == 0)
                {
                    //add default requirement group
                    var defaultGroup = new DiscountRequirement
                    {
                        IsGroup = true,
                        DiscountId = discount.Id,
                        InteractionType = RequirementGroupInteractionType.And,
                        DiscountRequirementRuleSystemName = await _localizationService
                            .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
                    };

                    await _discountService.InsertDiscountRequirementAsync(defaultGroup);

                    defaultGroupId = defaultGroup.Id;
                }

                //set parent identifier if specified
                if (parentId.HasValue)
                    discountRequirement.ParentId = parentId.Value;
					_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): discountRequirement.ParentId → {Arg5}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, discountRequirement.ParentId);
                else
                {
                    //or default group identifier
                    if (defaultGroupId != discountRequirement.Id)
                        discountRequirement.ParentId = defaultGroupId;
                }

                //set interaction type
                if (interactionTypeId.HasValue)
                    discountRequirement.InteractionTypeId = interactionTypeId;
					_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): discountRequirement.InteractionTypeId → {Arg5}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, discountRequirement.InteractionTypeId);

                await _discountService.UpdateDiscountRequirementAsync(discountRequirement);
            }
        }

        //get current requirements
        var topLevelRequirements = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).Where(requirement => requirement.IsGroup).ToList();
		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): topLevelRequirements → {TopLevelRequirements}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, topLevelRequirements);

        //get interaction type of top-level group
        var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;
		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): interactionType → {InteractionType}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, interactionType);

        if (interactionType.HasValue)
        {
            requirements = (await _discountModelFactory
                .PrepareDiscountRequirementRuleModelsAsync(topLevelRequirements, discount, interactionType.Value)).ToList();
        }

        //get available groups
        var requirementGroups = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);
		_logger.LogDebug("GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement}): requirementGroups → {RequirementGroups}", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement, requirementGroups);

        var availableRequirementGroups = requirementGroups.Select(requirement =>
            new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

		_logger.LogDebug("<<< Exiting GetDiscountRequirements(discountId={DiscountId},discountRequirementId={DiscountRequirementId},parentId={ParentId},interactionTypeId={InteractionTypeId},deleteRequirement={DeleteRequirement})", discountId, discountRequirementId, parentId, interactionTypeId, deleteRequirement);
        return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddNewGroup(int discountId, string name)
    {
		_logger.LogDebug(">>> Entering AddNewGroup(discountId={DiscountId},name={Name})", discountId, name);
        var discount = await _discountService.GetDiscountByIdAsync(discountId) ?? throw new ArgumentException("Discount could not be loaded");

		_logger.LogDebug("AddNewGroup(discountId={DiscountId},name={Name}): discount → {Discount}", discountId, name, discount);
        var defaultGroup = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id, true)).FirstOrDefault(requirement => requirement.IsGroup);
		_logger.LogDebug("AddNewGroup(discountId={DiscountId},name={Name}): defaultGroup → {DefaultGroup}", discountId, name, defaultGroup);
        if (defaultGroup == null)
        {
            //add default requirement group
            await _discountService.InsertDiscountRequirementAsync(new DiscountRequirement
            {
                DiscountId = discount.Id,
                IsGroup = true,
                InteractionType = RequirementGroupInteractionType.And,
                DiscountRequirementRuleSystemName = await _localizationService
                    .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
            });
        }

        //save new requirement group
        var discountRequirementGroup = new DiscountRequirement
        {
            DiscountId = discount.Id,
            IsGroup = true,
            DiscountRequirementRuleSystemName = name,
            InteractionType = RequirementGroupInteractionType.And
        };

        await _discountService.InsertDiscountRequirementAsync(discountRequirementGroup);

        if (!string.IsNullOrEmpty(name))
            return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });

        //set identifier as group name (if not specified)
        discountRequirementGroup.DiscountRequirementRuleSystemName = $"#{discountRequirementGroup.Id}";
		_logger.LogDebug("AddNewGroup(discountId={DiscountId},name={Name}): discountRequirementGroup.DiscountRequirementRuleSystemName → {Arg2}", discountId, name, discountRequirementGroup.DiscountRequirementRuleSystemName);
        await _discountService.UpdateDiscountRequirementAsync(discountRequirementGroup);

        await _discountService.UpdateDiscountAsync(discount);

		_logger.LogDebug("<<< Exiting AddNewGroup(discountId={DiscountId},name={Name})", discountId, name);
        return Json(new { Result = true, NewRequirementId = discountRequirementGroup.Id });
    }

    //action displaying notification (warning) to a store owner that entered coupon code already exists
    public virtual async Task<IActionResult> CouponCodeReservedWarning(int discountId, string couponCode)
    {
		_logger.LogDebug(">>> Entering CouponCodeReservedWarning(discountId={DiscountId},couponCode={CouponCode})", discountId, couponCode);
        if (string.IsNullOrEmpty(couponCode))
            return Json(new { Result = string.Empty });

        //check whether discount with passed coupon code exists
        var discounts = (await _discountService.GetAllDiscountsAsync(couponCode: couponCode, showHidden: true))
            .Where(discount => discount.Id != discountId);
        if (!discounts.Any())
            return Json(new { Result = string.Empty });

        var message = string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Fields.CouponCode.Reserved"), discounts.FirstOrDefault().Name);
		_logger.LogDebug("CouponCodeReservedWarning(discountId={DiscountId},couponCode={CouponCode}): message → {Message}", discountId, couponCode, message);
		_logger.LogDebug("<<< Exiting CouponCodeReservedWarning(discountId={DiscountId},couponCode={CouponCode})", discountId, couponCode);
        return Json(new { Result = message });
    }

    #endregion

    #region Applied to products

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> ProductList(DiscountProductSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductList(searchModel={SearchModel})", searchModel);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountProductListModelAsync(searchModel, discount);
		_logger.LogDebug("ProductList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ProductList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductDelete(int discountId, int productId)
    {
		_logger.LogDebug(">>> Entering ProductDelete(discountId={DiscountId},productId={ProductId})", discountId, productId);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a product with the specified id
        var product = await _productService.GetProductByIdAsync(productId)
            ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

        //remove discount
        if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
            await _productService.DeleteDiscountProductMappingAsync(discountProductMapping);

        await _productService.UpdateProductAsync(product);

		_logger.LogDebug("<<< Exiting ProductDelete(discountId={DiscountId},productId={ProductId})", discountId, productId);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(int discountId)
    {
		_logger.LogDebug(">>> Entering ProductAddPopup(discountId={DiscountId})", discountId);
        //prepare model
        var model = await _discountModelFactory.PrepareAddProductToDiscountSearchModelAsync(new AddProductToDiscountSearchModel());
		_logger.LogDebug("ProductAddPopup(discountId={DiscountId}): model → {Model}", discountId, model);

		_logger.LogDebug("<<< Exiting ProductAddPopup(discountId={DiscountId})", discountId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopupList(AddProductToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ProductAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _discountModelFactory.PrepareAddProductToDiscountListModelAsync(searchModel);
		_logger.LogDebug("ProductAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ProductAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ProductAddPopup(AddProductToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        if (selectedProducts.Any())
        {
            foreach (var product in selectedProducts)
            {
                if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                    await _productService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });

                await _productService.UpdateProductAsync(product);
            }
        }

        ViewBag.RefreshPage = true;

        return View(new AddProductToDiscountSearchModel());
    }

    #endregion

    #region Applied to categories

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> CategoryList(DiscountCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering CategoryList(searchModel={SearchModel})", searchModel);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountCategoryListModelAsync(searchModel, discount);
		_logger.LogDebug("CategoryList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting CategoryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryDelete(int discountId, int categoryId)
    {
		_logger.LogDebug(">>> Entering CategoryDelete(discountId={DiscountId},categoryId={CategoryId})", discountId, categoryId);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a category with the specified id
        var category = await _categoryService.GetCategoryByIdAsync(categoryId)
            ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

        //remove discount
        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
            await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);

        await _categoryService.UpdateCategoryAsync(category);

		_logger.LogDebug("<<< Exiting CategoryDelete(discountId={DiscountId},categoryId={CategoryId})", discountId, categoryId);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopup(int discountId)
    {
		_logger.LogDebug(">>> Entering CategoryAddPopup(discountId={DiscountId})", discountId);
        //prepare model
        var model = await _discountModelFactory.PrepareAddCategoryToDiscountSearchModelAsync(new AddCategoryToDiscountSearchModel());
		_logger.LogDebug("CategoryAddPopup(discountId={DiscountId}): model → {Model}", discountId, model);

		_logger.LogDebug("<<< Exiting CategoryAddPopup(discountId={DiscountId})", discountId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopupList(AddCategoryToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering CategoryAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _discountModelFactory.PrepareAddCategoryToDiscountListModelAsync(searchModel);
		_logger.LogDebug("CategoryAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting CategoryAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CategoryAddPopup(AddCategoryToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        foreach (var id in model.SelectedCategoryIds)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                continue;

            if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });

            await _categoryService.UpdateCategoryAsync(category);
        }

        ViewBag.RefreshPage = true;

        return View(new AddCategoryToDiscountSearchModel());
    }

    #endregion

    #region Applied to manufacturers

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> ManufacturerList(DiscountManufacturerSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ManufacturerList(searchModel={SearchModel})", searchModel);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountManufacturerListModelAsync(searchModel, discount);
		_logger.LogDebug("ManufacturerList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ManufacturerList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerDelete(int discountId, int manufacturerId)
    {
		_logger.LogDebug(">>> Entering ManufacturerDelete(discountId={DiscountId},manufacturerId={ManufacturerId})", discountId, manufacturerId);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a manufacturer with the specified id
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId)
            ?? throw new ArgumentException("No manufacturer found with the specified id", nameof(manufacturerId));

        //remove discount
        if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
            await _manufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);

        await _manufacturerService.UpdateManufacturerAsync(manufacturer);

		_logger.LogDebug("<<< Exiting ManufacturerDelete(discountId={DiscountId},manufacturerId={ManufacturerId})", discountId, manufacturerId);
        return new NullJsonResult();
    }

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopup(int discountId)
    {
		_logger.LogDebug(">>> Entering ManufacturerAddPopup(discountId={DiscountId})", discountId);
        //prepare model
        var model = await _discountModelFactory.PrepareAddManufacturerToDiscountSearchModelAsync(new AddManufacturerToDiscountSearchModel());
		_logger.LogDebug("ManufacturerAddPopup(discountId={DiscountId}): model → {Model}", discountId, model);

		_logger.LogDebug("<<< Exiting ManufacturerAddPopup(discountId={DiscountId})", discountId);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopupList(AddManufacturerToDiscountSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering ManufacturerAddPopupList(searchModel={SearchModel})", searchModel);
        //prepare model
        var model = await _discountModelFactory.PrepareAddManufacturerToDiscountListModelAsync(searchModel);
		_logger.LogDebug("ManufacturerAddPopupList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting ManufacturerAddPopupList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> ManufacturerAddPopup(AddManufacturerToDiscountModel model)
    {
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        foreach (var id in model.SelectedManufacturerIds)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                continue;

            if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

            await _manufacturerService.UpdateManufacturerAsync(manufacturer);
        }

        ViewBag.RefreshPage = true;

        return View(new AddManufacturerToDiscountSearchModel());
    }

    #endregion

    #region Discount usage history

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public virtual async Task<IActionResult> UsageHistoryList(DiscountUsageHistorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering UsageHistoryList(searchModel={SearchModel})", searchModel);
        //try to get a discount with the specified id
        var discount = await _discountService.GetDiscountByIdAsync(searchModel.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

        //prepare model
        var model = await _discountModelFactory.PrepareDiscountUsageHistoryListModelAsync(searchModel, discount);
		_logger.LogDebug("UsageHistoryList(searchModel={SearchModel}): model → {Model}", searchModel, model);

		_logger.LogDebug("<<< Exiting UsageHistoryList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> UsageHistoryDelete(int discountId, int id)
    {
		_logger.LogDebug(">>> Entering UsageHistoryDelete(discountId={DiscountId},id={Id})", discountId, id);
        //try to get a discount with the specified id
        _ = await _discountService.GetDiscountByIdAsync(discountId)
            ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

        //try to get a discount usage history entry with the specified id
        var discountUsageHistoryEntry = await _discountService.GetDiscountUsageHistoryByIdAsync(id)
            ?? throw new ArgumentException("No discount usage history entry found with the specified id", nameof(id));

        await _discountService.DeleteDiscountUsageHistoryAsync(discountUsageHistoryEntry);

		_logger.LogDebug("<<< Exiting UsageHistoryDelete(discountId={DiscountId},id={Id})", discountId, id);
        return new NullJsonResult();
    }

    #endregion

    #endregion
}