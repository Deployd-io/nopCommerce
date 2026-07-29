using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents the app settings model
/// </summary>
public partial record AppSettingsModel : BaseNopModel
{
    #region Ctor

    public AppSettingsModel()
    {
		logger.LogDebug(">>> Entering AppSettingsModel()");
        DataConfigModel = new DataConfigModel();
        CacheConfigModel = new CacheConfigModel();
		logger.LogDebug("AppSettingsModel(): DataConfigModel → {DataConfigModel}", DataConfigModel);
        DistributedCacheConfigModel = new DistributedCacheConfigModel();
		logger.LogDebug("AppSettingsModel(): CacheConfigModel → {CacheConfigModel}", CacheConfigModel);
		logger.LogDebug("AppSettingsModel(): DistributedCacheConfigModel → {DistributedCacheConfigModel}", DistributedCacheConfigModel);
        HostingConfigModel = new HostingConfigModel();
        InstallationConfigModel = new InstallationConfigModel();
		logger.LogDebug("AppSettingsModel(): HostingConfigModel → {HostingConfigModel}", HostingConfigModel);
		logger.LogDebug("AppSettingsModel(): InstallationConfigModel → {InstallationConfigModel}", InstallationConfigModel);
        PluginConfigModel = new PluginConfigModel();
        CommonConfigModel = new CommonConfigModel();
		logger.LogDebug("AppSettingsModel(): PluginConfigModel → {PluginConfigModel}", PluginConfigModel);
		logger.LogDebug("AppSettingsModel(): CommonConfigModel → {CommonConfigModel}", CommonConfigModel);
        WebOptimizerConfigModel = new WebOptimizerConfigModel();
        EnvironmentVariables = new List<string>();
		logger.LogDebug("<<< Exiting AppSettingsModel()");
		logger.LogDebug("AppSettingsModel(): WebOptimizerConfigModel → {WebOptimizerConfigModel}", WebOptimizerConfigModel);
		logger.LogDebug("AppSettingsModel(): EnvironmentVariables → {EnvironmentVariables}", EnvironmentVariables);
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