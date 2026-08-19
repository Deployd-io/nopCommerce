using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Web.Areas.Admin.Models.MultiFactorAuthentication;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class AuthenticationController : BaseAdminController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IExternalAuthenticationMethodModelFactory _externalAuthenticationMethodModelFactory;
    protected readonly IMultiFactorAuthenticationMethodModelFactory _multiFactorAuthenticationMethodModelFactory;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;

    #endregion

    #region Ctor

    public AuthenticationController(ExternalAuthenticationSettings externalAuthenticationSettings,
        IAuthenticationPluginManager authenticationPluginManager,
        IEventPublisher eventPublisher,
        IExternalAuthenticationMethodModelFactory externalAuthenticationMethodModelFactory,
        IMultiFactorAuthenticationMethodModelFactory multiFactorAuthenticationMethodModelFactory,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        IPermissionService permissionService,
        ISettingService settingService,
        MultiFactorAuthenticationSettings multiFactorAuthenticationSettings)
    {
		_logger.LogDebug(">>> Entering AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings})", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings);
        _externalAuthenticationSettings = externalAuthenticationSettings;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _externalAuthenticationSettings → {_externalAuthenticationSettings}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _externalAuthenticationSettings);
        _authenticationPluginManager = authenticationPluginManager;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _authenticationPluginManager → {_authenticationPluginManager}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _authenticationPluginManager);
        _eventPublisher = eventPublisher;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _eventPublisher → {_eventPublisher}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _eventPublisher);
        _externalAuthenticationMethodModelFactory = externalAuthenticationMethodModelFactory;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _externalAuthenticationMethodModelFactory → {_externalAuthenticationMethodModelFactory}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _externalAuthenticationMethodModelFactory);
        _multiFactorAuthenticationMethodModelFactory = multiFactorAuthenticationMethodModelFactory;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _multiFactorAuthenticationMethodModelFactory → {_multiFactorAuthenticationMethodModelFactory}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _multiFactorAuthenticationMethodModelFactory);
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _multiFactorAuthenticationPluginManager → {_multiFactorAuthenticationPluginManager}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _multiFactorAuthenticationPluginManager);
        _permissionService = permissionService;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _permissionService → {_permissionService}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _permissionService);
        _settingService = settingService;
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _settingService → {_settingService}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _settingService);
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
		_logger.LogDebug("<<< Exiting AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings})", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings);
		_logger.LogDebug("AuthenticationController(externalAuthenticationSettings={ExternalAuthenticationSettings},authenticationPluginManager={AuthenticationPluginManager},eventPublisher={EventPublisher},externalAuthenticationMethodModelFactory={ExternalAuthenticationMethodModelFactory},multiFactorAuthenticationMethodModelFactory={MultiFactorAuthenticationMethodModelFactory},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},permissionService={PermissionService},settingService={SettingService},multiFactorAuthenticationSettings={MultiFactorAuthenticationSettings}): _multiFactorAuthenticationSettings → {_multiFactorAuthenticationSettings}", externalAuthenticationSettings, authenticationPluginManager, eventPublisher, externalAuthenticationMethodModelFactory, multiFactorAuthenticationMethodModelFactory, multiFactorAuthenticationPluginManager, permissionService, settingService, multiFactorAuthenticationSettings, _multiFactorAuthenticationSettings);
    }

    #endregion

    #region External Authentication

    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public virtual IActionResult ExternalMethods()
    {
		_logger.LogDebug(">>> Entering ExternalMethods()");
        //prepare model
        var model = _externalAuthenticationMethodModelFactory
            .PrepareExternalAuthenticationMethodSearchModel(new ExternalAuthenticationMethodSearchModel());

		_logger.LogDebug("<<< Exiting ExternalMethods()");
		_logger.LogDebug("<<< Exiting ExternalMethods(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public virtual async Task<IActionResult> ExternalMethods(ExternalAuthenticationMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _externalAuthenticationMethodModelFactory.PrepareExternalAuthenticationMethodListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public virtual async Task<IActionResult> ExternalMethodUpdate(ExternalAuthenticationMethodModel model)
    {
		_logger.LogDebug(">>> Entering ExternalMethodUpdate(model={Model})", model);
        var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_authenticationPluginManager.IsPluginActive(method))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
            }
        }

        var pluginDescriptor = method.PluginDescriptor;
        pluginDescriptor.DisplayOrder = model.DisplayOrder;
		_logger.LogDebug("ExternalMethodUpdate(model={Model}): pluginDescriptor.DisplayOrder → {Arg1}", model, pluginDescriptor.DisplayOrder);

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

		_logger.LogDebug("<<< Exiting ExternalMethodUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion

    #region Multi-factor Authentication

    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public virtual IActionResult MultiFactorMethods()
    {
		_logger.LogDebug(">>> Entering MultiFactorMethods()");
        //prepare model
        var model = _multiFactorAuthenticationMethodModelFactory
            .PrepareMultiFactorAuthenticationMethodSearchModel(new MultiFactorAuthenticationMethodSearchModel());

		_logger.LogDebug("<<< Exiting MultiFactorMethods()");
		_logger.LogDebug("<<< Exiting MultiFactorMethods(searchModel={SearchModel})", searchModel);
        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public virtual async Task<IActionResult> MultiFactorMethods(MultiFactorAuthenticationMethodSearchModel searchModel)
    {
        //prepare model
        var model = await _multiFactorAuthenticationMethodModelFactory.PrepareMultiFactorAuthenticationMethodListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_MULTIFACTOR_AUTHENTICATION_METHODS)]
    public virtual async Task<IActionResult> MultiFactorMethodUpdate(MultiFactorAuthenticationMethodModel model)
    {
		_logger.LogDebug(">>> Entering MultiFactorMethodUpdate(model={Model})", model);
        var method = await _multiFactorAuthenticationPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
        if (_multiFactorAuthenticationPluginManager.IsPluginActive(method))
        {
            if (!model.IsActive)
            {
                //mark as disabled
                _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Remove(method.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
            }
        }
        else
        {
            if (model.IsActive)
            {
                //mark as active
                _multiFactorAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(method.PluginDescriptor.SystemName);
                await _settingService.SaveSettingAsync(_multiFactorAuthenticationSettings);
            }
        }

        var pluginDescriptor = method.PluginDescriptor;
        pluginDescriptor.DisplayOrder = model.DisplayOrder;
		_logger.LogDebug("MultiFactorMethodUpdate(model={Model}): pluginDescriptor.DisplayOrder → {Arg1}", model, pluginDescriptor.DisplayOrder);

        //update the description file
        pluginDescriptor.Save();

        //raise event
        await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

		_logger.LogDebug("<<< Exiting MultiFactorMethodUpdate(model={Model})", model);
        return new NullJsonResult();
    }

    #endregion
}