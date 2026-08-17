using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents the app settings model
/// </summary>
public partial record AppSettingsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AppSettingsModel()
    {
		_logger.LogDebug(">>> Entering AppSettingsModel()");
        DataConfigModel = new DataConfigModel();
        CacheConfigModel = new CacheConfigModel();
		_logger.LogDebug("AppSettingsModel(): CacheConfigModel → {CacheConfigModel}", CacheConfigModel);
		_logger.LogDebug("AppSettingsModel(): DataConfigModel → {DataConfigModel}", DataConfigModel);
        DistributedCacheConfigModel = new DistributedCacheConfigModel();
		_logger.LogDebug("AppSettingsModel(): DistributedCacheConfigModel → {DistributedCacheConfigModel}", DistributedCacheConfigModel);
        HostingConfigModel = new HostingConfigModel();
		_logger.LogDebug("AppSettingsModel(): HostingConfigModel → {HostingConfigModel}", HostingConfigModel);
        InstallationConfigModel = new InstallationConfigModel();
		_logger.LogDebug("AppSettingsModel(): InstallationConfigModel → {InstallationConfigModel}", InstallationConfigModel);
        PluginConfigModel = new PluginConfigModel();
		_logger.LogDebug("AppSettingsModel(): PluginConfigModel → {PluginConfigModel}", PluginConfigModel);
        CommonConfigModel = new CommonConfigModel();
		_logger.LogDebug("AppSettingsModel(): CommonConfigModel → {CommonConfigModel}", CommonConfigModel);
        WebOptimizerConfigModel = new WebOptimizerConfigModel();
		_logger.LogDebug("AppSettingsModel(): WebOptimizerConfigModel → {WebOptimizerConfigModel}", WebOptimizerConfigModel);
        EnvironmentVariables = new List<string>();
		_logger.LogDebug("<<< Exiting AppSettingsModel()");
		_logger.LogDebug("AppSettingsModel(): EnvironmentVariables → {EnvironmentVariables}", EnvironmentVariables);
    }

    #endregion

    #region Properties

    public DataConfigModel DataConfigModel { get; set; }

    public CacheConfigModel CacheConfigModel { get; set; }

    public HostingConfigModel HostingConfigModel { get; set; }

    public DistributedCacheConfigModel DistributedCacheConfigModel { get; set; }
    
    public InstallationConfigModel InstallationConfigModel { get; set; }

    public PluginConfigModel PluginConfigModel { get; set; }

    public CommonConfigModel CommonConfigModel { get; set; }

    public WebOptimizerConfigModel WebOptimizerConfigModel { get; set; }

    public List<string> EnvironmentVariables { get; set; }

    #endregion
}