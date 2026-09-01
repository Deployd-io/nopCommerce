using System.Reflection;
using Nop.Core.Events;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Themes;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Represents AppStarted event consumer
/// </summary>
public partial class AppStartedConsumer : IConsumer<AppStartedEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ILogger _logger;
    protected readonly IMigrationManager _migrationManager;
    protected readonly IPermissionService _permissionService;
    protected readonly IPluginService _pluginService;
    protected readonly ISettingService _settingService;
    protected readonly ITaskScheduler _taskScheduler;
    protected readonly IThemeProvider _themeProvider;

    #endregion

    #region Ctor

    public AppStartedConsumer(ILogger logger,
        IMigrationManager migrationManager,
        IPermissionService permissionService,
        IPluginService pluginService,
        ISettingService settingService,
        ITaskScheduler taskScheduler,
        IThemeProvider themeProvider)
    {
        _logger = logger;
		_logger.LogDebug(">>> Entering AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider})", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider);
        _migrationManager = migrationManager;
		_logger.LogDebug("AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider}): _migrationManager → {_migrationManager}", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider, _migrationManager);
        _permissionService = permissionService;
        _pluginService = pluginService;
		_logger.LogDebug("AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider}): _permissionService → {_permissionService}", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider, _permissionService);
		_logger.LogDebug("AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider}): _pluginService → {_pluginService}", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider, _pluginService);
        _settingService = settingService;
        _taskScheduler = taskScheduler;
		_logger.LogDebug("AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider}): _settingService → {_settingService}", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider, _settingService);
		_logger.LogDebug("AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider}): _taskScheduler → {_taskScheduler}", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider, _taskScheduler);
		_logger.LogDebug("<<< Exiting AppStartedConsumer(logger={Logger},migrationManager={MigrationManager},permissionService={PermissionService},pluginService={PluginService},settingService={SettingService},taskScheduler={TaskScheduler},themeProvider={ThemeProvider})", logger, migrationManager, permissionService, pluginService, settingService, taskScheduler, themeProvider);
        _themeProvider = themeProvider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(AppStartedEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //log application start
        await _logger.InformationAsync("Application started");

        //init theme provider
        await _themeProvider.InitializeAsync();

        //install and update plugins
        await _pluginService.InstallPluginsAsync();
        await _pluginService.UpdatePluginsAsync();

        //insert new ACL permission if exists
        await _permissionService.InsertPermissionsAsync();

        //update nopCommerce core and db
        var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): assembly → {Assembly}", eventMessage, assembly);
        _migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
        assembly = Assembly.GetAssembly(typeof(IMigrationManager));
        _migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

        //run scheduler
        await _taskScheduler.InitializeAsync();
        await _taskScheduler.StartSchedulerAsync();

        await _settingService.TryGetLicenseAsync();
		_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}