using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.CustomerRoles.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class DiscountRulesCustomerRolesController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IDiscountService _discountService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public DiscountRulesCustomerRolesController(ICustomerService customerService,
        IDiscountService discountService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        ISettingService settingService)
    {
		_logger.LogError(">>> Entering DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService})", customerService, discountService, localizationService, permissionService, settingService);
        _customerService = customerService;
        _discountService = discountService;
		_logger.LogDebug("DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService}): _discountService → {_discountService}", customerService, discountService, localizationService, permissionService, settingService, _discountService);
		_logger.LogDebug("DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService}): _customerService → {_customerService}", customerService, discountService, localizationService, permissionService, settingService, _customerService);
    _logger.LogWarning("<<< Exiting DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService})", customerService, discountService, localizationService, permissionService, settingService);
        _localizationService = localizationService;
		_logger.LogDebug("DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService}): _localizationService → {_localizationService}", customerService, discountService, localizationService, permissionService, settingService, _localizationService);
        _permissionService = permissionService;
		_logger.LogDebug("DiscountRulesCustomerRolesController(customerService={CustomerService},discountService={DiscountService},localizationService={LocalizationService},permissionService={PermissionService},settingService={SettingService}): _permissionService → {_permissionService}", customerService, discountService, localizationService, permissionService, settingService, _permissionService);
        _settingService = settingService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get errors message from model state
    /// </summary>
    /// <param name="modelState">Model state</param>
    /// <returns>Errors message</returns>
    protected IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
    {
		_logger.LogError(">>> Entering GetErrorsFromModelState(modelState={ModelState})", modelState);
		_logger.LogWarning("<<< Exiting GetErrorsFromModelState(modelState={ModelState})", modelState);
        return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
    public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
    {
		_logger.LogError(">>> Entering Configure(discountId={DiscountId},discountRequirementId={DiscountRequirementId})", discountId, discountRequirementId);
        //load the discount
        var discount = await _discountService.GetDiscountByIdAsync(discountId)
                       ?? throw new ArgumentException("Discount could not be loaded");

        //check whether the discount requirement exists
        if (discountRequirementId.HasValue && await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.Value) is null)
            return Content("Failed to load requirement.");

        //try to get previously saved restricted customer role identifier
        var restrictedRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirementId ?? 0));
		_logger.LogError("Configure(discountId={DiscountId},discountRequirementId={DiscountRequirementId}): restrictedRoleId → {RestrictedRoleId}", discountId, discountRequirementId, restrictedRoleId);

        var model = new RequirementModel
        {
            RequirementId = discountRequirementId ?? 0,
            DiscountId = discountId,
            CustomerRoleId = restrictedRoleId,
            //set available customer roles
            AvailableCustomerRoles = (await _customerService.GetAllCustomerRolesAsync(true)).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = role.Id == restrictedRoleId
            }).ToList()
        };
        model.AvailableCustomerRoles.Insert(0, new SelectListItem
        {
            Text = await _localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole.Select"),
            Value = "0"
			_logger.LogDebug("Configure(discountId={DiscountId},discountRequirementId={DiscountRequirementId}): Value → {Value}", discountId, discountRequirementId, Value);
        });

        //set the HTML field prefix
        ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(DiscountRequirementDefaults.HtmlFieldPrefix, discountRequirementId ?? 0);

		_logger.LogWarning("<<< Exiting Configure(discountId={DiscountId},discountRequirementId={DiscountRequirementId})", discountId, discountRequirementId);
		_logger.LogWarning("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/DiscountRules.CustomerRoles/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
    public async Task<IActionResult> Configure(RequirementModel model)
    {
        if (ModelState.IsValid)
        {
            //load the discount
            var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
            if (discount == null)
                return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

            //get the discount requirement
            var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(model.RequirementId);

            //the discount requirement does not exist, so create a new one
            if (discountRequirement == null)
            {
                discountRequirement = new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName
                };

                await _discountService.InsertDiscountRequirementAsync(discountRequirement);
            }

            //save restricted customer role identifier
            await _settingService.SetSettingAsync(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id), model.CustomerRoleId);

            return Ok(new { NewRequirementId = discountRequirement.Id });
        }

        return Ok(new { Errors = GetErrorsFromModelState(ModelState) });
    }

    #endregion
}