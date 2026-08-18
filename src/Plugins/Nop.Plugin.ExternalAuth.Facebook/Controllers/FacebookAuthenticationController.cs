using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.ExternalAuth.Facebook.Controllers;

[AutoValidateAntiforgeryToken]
public class FacebookAuthenticationController : BasePluginController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOptionsMonitorCache<FacebookOptions> _optionsCache;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public FacebookAuthenticationController(FacebookExternalAuthSettings facebookExternalAuthSettings,
        IAuthenticationPluginManager authenticationPluginManager,
        IExternalAuthenticationService externalAuthenticationService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOptionsMonitorCache<FacebookOptions> optionsCache,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext})", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext);
        _facebookExternalAuthSettings = facebookExternalAuthSettings;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _facebookExternalAuthSettings → {_facebookExternalAuthSettings}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _facebookExternalAuthSettings);
        _authenticationPluginManager = authenticationPluginManager;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _authenticationPluginManager → {_authenticationPluginManager}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _authenticationPluginManager);
        _externalAuthenticationService = externalAuthenticationService;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _externalAuthenticationService → {_externalAuthenticationService}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _externalAuthenticationService);
        _localizationService = localizationService;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _localizationService → {_localizationService}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _localizationService);
        _notificationService = notificationService;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _notificationService → {_notificationService}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _notificationService);
        _optionsCache = optionsCache;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _optionsCache → {_optionsCache}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _optionsCache);
        _permissionService = permissionService;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _permissionService → {_permissionService}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _settingService → {_settingService}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _settingService);
        _storeContext = storeContext;
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _storeContext → {_storeContext}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _storeContext);
        _workContext = workContext;
		_logger.LogDebug("<<< Exiting FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext})", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext);
		_logger.LogDebug("FacebookAuthenticationController(facebookExternalAuthSettings={FacebookExternalAuthSettings},authenticationPluginManager={AuthenticationPluginManager},externalAuthenticationService={ExternalAuthenticationService},localizationService={LocalizationService},notificationService={NotificationService},optionsCache={OptionsCache},permissionService={PermissionService},settingService={SettingService},storeContext={StoreContext},workContext={WorkContext}): _workContext → {_workContext}", facebookExternalAuthSettings, authenticationPluginManager, externalAuthenticationService, localizationService, notificationService, optionsCache, permissionService, settingService, storeContext, workContext, _workContext);
    }

    #endregion

    #region Methods

    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public IActionResult Configure()
    {
		_logger.LogDebug(">>> Entering Configure()");
        var model = new ConfigurationModel
        {
            ClientId = _facebookExternalAuthSettings.ClientKeyIdentifier,
            ClientSecret = _facebookExternalAuthSettings.ClientSecret
        };

		_logger.LogDebug("<<< Exiting Configure()");
		_logger.LogDebug("<<< Exiting Configure(model={Model})", model);
        return View("~/Plugins/ExternalAuth.Facebook/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        //save settings
        _facebookExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
        _facebookExternalAuthSettings.ClientSecret = model.ClientSecret;
        await _settingService.SaveSettingAsync(_facebookExternalAuthSettings);

        //clear Facebook authentication options cache
        _optionsCache.TryRemove(FacebookDefaults.AuthenticationScheme);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    public async Task<IActionResult> Login(string returnUrl)
    {
		_logger.LogDebug(">>> Entering Login(returnUrl={ReturnUrl})", returnUrl);
        var store = await _storeContext.GetCurrentStoreAsync();
        var methodIsAvailable = await _authenticationPluginManager
            .IsPluginActiveAsync(FacebookAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), store.Id);
			_logger.LogDebug("Login(returnUrl={ReturnUrl}): store → {Store}", returnUrl, store);
        if (!methodIsAvailable)
			_logger.LogWarning("Throwing NopException from Login(returnUrl={ReturnUrl})", returnUrl);
            throw new NopException("Facebook authentication module cannot be loaded");

        if (string.IsNullOrEmpty(_facebookExternalAuthSettings.ClientKeyIdentifier) ||
            string.IsNullOrEmpty(_facebookExternalAuthSettings.ClientSecret))
        {
            throw new NopException("Facebook authentication module not configured");
        }

        //configure login callback action
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("LoginCallback", "FacebookAuthentication", new { returnUrl = returnUrl })
        };
        authenticationProperties.SetString(FacebookAuthenticationDefaults.ErrorCallback, Url.RouteUrl(NopRouteNames.General.LOGIN, new { returnUrl }));

		_logger.LogDebug("<<< Exiting Login(returnUrl={ReturnUrl})", returnUrl);
        return Challenge(authenticationProperties, FacebookDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> LoginCallback(string returnUrl)
    {
		_logger.LogDebug(">>> Entering LoginCallback(returnUrl={ReturnUrl})", returnUrl);
        //authenticate Facebook user
        var authenticateResult = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
		_logger.LogDebug("LoginCallback(returnUrl={ReturnUrl}): authenticateResult → {AuthenticateResult}", returnUrl, authenticateResult);
        if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
            return RedirectToRoute(NopRouteNames.General.LOGIN);

        //create external authentication parameters
        var authenticationParameters = new ExternalAuthenticationParameters
        {
            ProviderSystemName = FacebookAuthenticationDefaults.SystemName,
            AccessToken = await HttpContext.GetTokenAsync(FacebookDefaults.AuthenticationScheme, "access_token"),
            Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
            ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
            ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
            Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
        };

        //authenticate Nop user
		_logger.LogDebug("<<< Exiting LoginCallback(returnUrl={ReturnUrl})", returnUrl);
        return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
    }

    public async Task<IActionResult> DataDeletionStatusCheck(int earId)
    {
		_logger.LogDebug(">>> Entering DataDeletionStatusCheck(earId={EarId})", earId);
        var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(earId);
        if (externalAuthenticationRecord is not null)
			_logger.LogDebug("DataDeletionStatusCheck(earId={EarId}): externalAuthenticationRecord → {ExternalAuthenticationRecord}", earId, externalAuthenticationRecord);
            _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.ExternalAuth.Facebook.AuthenticationDataExist"));
        else
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.ExternalAuth.Facebook.AuthenticationDataDeletedSuccessfully"));

		_logger.LogDebug("<<< Exiting DataDeletionStatusCheck(earId={EarId})", earId);
        return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
    }

    #endregion
}