using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class GoogleAuthenticatorController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly GoogleAuthenticatorService _googleAuthenticatorService;
    protected readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor 

    public GoogleAuthenticatorController(GoogleAuthenticatorService googleAuthenticatorService,
        GoogleAuthenticatorSettings googleAuthenticatorSettings,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IWorkContext workContext
		_logger.LogDebug(">>> Entering GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext})", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext);
    )
    {
        _googleAuthenticatorService = googleAuthenticatorService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _googleAuthenticatorService → {_googleAuthenticatorService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _googleAuthenticatorService);
        _googleAuthenticatorSettings = googleAuthenticatorSettings;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _googleAuthenticatorSettings → {_googleAuthenticatorSettings}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _googleAuthenticatorSettings);
        _customerService = customerService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _customerService → {_customerService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _customerService);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _genericAttributeService → {_genericAttributeService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _genericAttributeService);
        _localizationService = localizationService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _localizationService → {_localizationService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _notificationService → {_notificationService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _notificationService);
        _permissionService = permissionService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _permissionService → {_permissionService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _settingService → {_settingService}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _settingService);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext})", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext);
		_logger.LogDebug("GoogleAuthenticatorController(googleAuthenticatorService={GoogleAuthenticatorService},googleAuthenticatorSettings={GoogleAuthenticatorSettings},customerService={CustomerService},genericAttributeService={GenericAttributeService},localizationService={LocalizationService},notificationService={NotificationService},permissionService={PermissionService},settingService={SettingService},workContext={WorkContext}): _workContext → {_workContext}", googleAuthenticatorService, googleAuthenticatorSettings, customerService, genericAttributeService, localizationService, notificationService, permissionService, settingService, workContext, _workContext);
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public async Task<IActionResult> Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        //prepare model
        var model = new ConfigurationModel
        {
            QRPixelsPerModule = _googleAuthenticatorSettings.QRPixelsPerModule,
            BusinessPrefix = _googleAuthenticatorSettings.BusinessPrefix
        };
        model.GoogleAuthenticatorSearchModel.HideSearchBlock = await _genericAttributeService
            .GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), GoogleAuthenticatorDefaults.HideSearchBlockAttribute);

		_logger.LogDebug("<<< Exiting Configure()");
        return View("~/Plugins/MultiFactorAuth.GoogleAuthenticator/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //set new settings values
        _googleAuthenticatorSettings.QRPixelsPerModule = model.QRPixelsPerModule;
        _googleAuthenticatorSettings.BusinessPrefix = model.BusinessPrefix;
        await _settingService.SaveSettingAsync(_googleAuthenticatorSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public async Task<IActionResult> GoogleAuthenticatorList(GoogleAuthenticatorSearchModel searchModel)
    {
		_logger.LogDebug(">>> Entering GoogleAuthenticatorList(searchModel={SearchModel})", searchModel);
        //get GoogleAuthenticator configuration records
        var configurations = await _googleAuthenticatorService.GetPagedConfigurationsAsync(searchModel.SearchEmail,
            searchModel.Page - 1, searchModel.PageSize);
        var model = new GoogleAuthenticatorListModel().PrepareToGrid(searchModel, configurations, () =>
        {
            //fill in model values from the configuration
            return configurations.Select(configuration => new GoogleAuthenticatorModel
            {
                Id = configuration.Id,
                Customer = configuration.Customer,
                SecretKey = configuration.SecretKey
            });
        });

		_logger.LogDebug("<<< Exiting GoogleAuthenticatorList(searchModel={SearchModel})", searchModel);
        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> GoogleAuthenticatorDelete(GoogleAuthenticatorModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //delete configuration
        var configuration = await _googleAuthenticatorService.GetConfigurationByIdAsync(model.Id);

        if (configuration == null)
            return new NullJsonResult();

        await _googleAuthenticatorService.DeleteConfigurationAsync(configuration);

        var customer = await _customerService.GetCustomerByEmailAsync(configuration.Customer) ??
            await _customerService.GetCustomerByUsernameAsync(configuration.Customer);

        if (customer != null)
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

        return new NullJsonResult();
    }

    #endregion
}