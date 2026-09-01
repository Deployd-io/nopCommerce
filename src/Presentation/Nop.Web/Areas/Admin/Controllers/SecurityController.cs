using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Security;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Filters;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SecurityController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAdminMenu _adminMenu;
    protected readonly ICustomerService _customerService;
    protected readonly ILogger _logger;
    protected readonly IPermissionService _permissionService;
    protected readonly ISecurityModelFactory _securityModelFactory;
    protected readonly IWorkContext _workContext;

    private static readonly Dictionary<string, string> _menuSystemNames = new();

    #endregion

    #region Ctor

    public SecurityController(IAdminMenu adminMenu,
        ICustomerService customerService,
        ILogger logger,
        IPermissionService permissionService,
        ISecurityModelFactory securityModelFactory,
        IWorkContext workContext)
    {
        _adminMenu = adminMenu;
		_logger.LogDebug("SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext}): _adminMenu → {_adminMenu}", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext, _adminMenu);
        _customerService = customerService;
		_logger.LogDebug("SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext}): _customerService → {_customerService}", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext, _customerService);
        _logger = logger;
		_logger.LogDebug(">>> Entering SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext})", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext);
        _permissionService = permissionService;
		_logger.LogDebug("SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext}): _permissionService → {_permissionService}", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext, _permissionService);
        _securityModelFactory = securityModelFactory;
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext})", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext);
		_logger.LogDebug("SecurityController(adminMenu={AdminMenu},customerService={CustomerService},logger={Logger},permissionService={PermissionService},securityModelFactory={SecurityModelFactory},workContext={WorkContext}): _securityModelFactory → {_securityModelFactory}", adminMenu, customerService, logger, permissionService, securityModelFactory, workContext, _securityModelFactory);
    }
    
    #endregion

    #region Methods

    public virtual async Task<IActionResult> AccessDenied(string pageUrl, string pageSystemNameKey)
    {
		_logger.LogDebug(">>> Entering AccessDenied(pageUrl={PageUrl},pageSystemNameKey={PageSystemNameKey})", pageUrl, pageSystemNameKey);
        if (!_menuSystemNames.Any())
        {
            void fillSystemNames(AdminMenuItem node)
            {
                if (!string.IsNullOrEmpty(node.ControllerName) && !string.IsNullOrEmpty(node.ActionName))
                {
                    var key = $"{node.ControllerName}.{node.ActionName}";
					_logger.LogWarning("AccessDenied(pageUrl={PageUrl},pageSystemNameKey={PageSystemNameKey}): key → {Key}", pageUrl, pageSystemNameKey, key);
                    _menuSystemNames[key] = node.SystemName;
                }

                foreach (var childNode in node.ChildNodes) 
                    fillSystemNames(childNode);
            }

            fillSystemNames(await _adminMenu.GetRootNodeAsync(true));
        }

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("AccessDenied(pageUrl={PageUrl},pageSystemNameKey={PageSystemNameKey}): currentCustomer → {CurrentCustomer}", pageUrl, pageSystemNameKey, currentCustomer);

        var menuSystemName = "Home";
		_logger.LogDebug("AccessDenied(pageUrl={PageUrl},pageSystemNameKey={PageSystemNameKey}): menuSystemName → {MenuSystemName}", pageUrl, pageSystemNameKey, menuSystemName);

        if (!string.IsNullOrEmpty(pageSystemNameKey))
        {
            if (_menuSystemNames.TryGetValue(pageSystemNameKey, out var value))
                menuSystemName = value;
            else
            {
                var systemName =
                    _menuSystemNames.FirstOrDefault(item => item.Key.StartsWith(pageSystemNameKey.Split('.')[0]));

                if (!string.IsNullOrEmpty(systemName.Value))
                    menuSystemName = systemName.Value;
            }
        }

        if (currentCustomer == null || await _customerService.IsGuestAsync(currentCustomer))
            await _logger.InformationAsync($"Access denied to anonymous request on {pageUrl}");
        else
            await _logger.InformationAsync($"Access denied to user #{currentCustomer.Id} '{currentCustomer.Email}' on {pageUrl}");

		_logger.LogDebug("<<< Exiting AccessDenied(pageUrl={PageUrl},pageSystemNameKey={PageSystemNameKey})", pageUrl, pageSystemNameKey);
        return View(model: menuSystemName);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionCategory(PermissionItemSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PermissionCategory(searchModel={SearchModel})", searchModel);
        var model = await _securityModelFactory.PreparePermissionItemListModelAsync(searchModel);

		_logger.LogDebug("PermissionCategory(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting PermissionCategory(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionEditPopup(int id)
    {
		_logger.LogDebug(">>> Entering PermissionEditPopup(id={Id})", id);
        var permissionRecord = await _permissionService.GetPermissionRecordByIdAsync(id);
        var model = await _securityModelFactory.PreparePermissionItemModelAsync(permissionRecord);
		_logger.LogDebug("PermissionEditPopup(id={Id}): model → {Model}", id, model);
		_logger.LogDebug("PermissionEditPopup(id={Id}): permissionRecord → {PermissionRecord}", id, permissionRecord);

		_logger.LogDebug("<<< Exiting PermissionEditPopup(id={Id})", id);
		_logger.LogDebug("<<< Exiting PermissionEditPopup(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionEditPopup(PermissionItemModel model)
    {
        if (ModelState.IsValid)
        {
            var mapping = await _permissionService.GetMappingByPermissionRecordIdAsync(model.Id);

            var rolesForDelete = mapping.Where(p => !model.SelectedCustomerRoleIds.Contains(p.CustomerRoleId))
                .Select(p => p.CustomerRoleId).ToList();

            var rolesToAdd = model.SelectedCustomerRoleIds.Where(p => mapping.All(m => m.CustomerRoleId != p)).ToList();

            foreach (var customerRoleId in rolesForDelete)
                await _permissionService.DeletePermissionRecordCustomerRoleMappingAsync(model.Id, customerRoleId);

            foreach (var customerRoleId in rolesToAdd)
                await _permissionService.InsertPermissionRecordCustomerRoleMappingAsync(new PermissionRecordCustomerRoleMapping
                {
                    PermissionRecordId = model.Id,
                    CustomerRoleId = customerRoleId
                });
            ViewBag.RefreshPage = true;

            var permissionRecord = await _permissionService.GetPermissionRecordByIdAsync(model.Id);

            if (rolesForDelete.Any() || rolesToAdd.Any())
                //for clear cache
                await _permissionService.UpdatePermissionRecordAsync(permissionRecord);


            model = await _securityModelFactory.PreparePermissionItemModelAsync(permissionRecord);

            return View(model);
        }

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> PermissionCategories(PermissionCategorySearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering PermissionCategories(searchModel={SearchModel})", searchModel);
        var model = await _securityModelFactory.PreparePermissionCategoryListModelAsync(searchModel);

		_logger.LogDebug("PermissionCategories(searchModel={SearchModel}): model → {Model}", searchModel, model);
		_logger.LogDebug("<<< Exiting PermissionCategories(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_ACL)]
    public virtual async Task<IActionResult> Permissions()
    {
		_logger.LogDebug(">>> Entering Permissions()");
        //prepare model
        var model = await _securityModelFactory.PreparePermissionConfigurationModelAsync(new PermissionConfigurationModel());
		_logger.LogDebug("Permissions(): model → {Model}", model);

		_logger.LogDebug("<<< Exiting Permissions()");
        return View(model);
    }

    #endregion
}