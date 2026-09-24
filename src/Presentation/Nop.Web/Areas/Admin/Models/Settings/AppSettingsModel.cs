using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

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
		_logger.LogError(">>> Entering AppSettingsModel()");
        DataConfigModel = new DataConfigModel();
        CacheConfigModel = new CacheConfigModel();
		_logger.LogError("AppSettingsModel(): CacheConfigModel → {CacheConfigModel}", CacheConfigModel);
        DistributedCacheConfigModel = new DistributedCacheConfigModel();
		_logger.LogError("AppSettingsModel(): DistributedCacheConfigModel → {DistributedCacheConfigModel}", DistributedCacheConfigModel);
        HostingConfigModel = new HostingConfigModel();
		_logger.LogError("AppSettingsModel(): HostingConfigModel → {HostingConfigModel}", HostingConfigModel);
        InstallationConfigModel = new InstallationConfigModel();
		_logger.LogError("AppSettingsModel(): InstallationConfigModel → {InstallationConfigModel}", InstallationConfigModel);
        PluginConfigModel = new PluginConfigModel();
		_logger.LogError("AppSettingsModel(): PluginConfigModel → {PluginConfigModel}", PluginConfigModel);
        CommonConfigModel = new CommonConfigModel();
		_logger.LogError("AppSettingsModel(): CommonConfigModel → {CommonConfigModel}", CommonConfigModel);
        WebOptimizerConfigModel = new WebOptimizerConfigModel();
		_logger.LogError("AppSettingsModel(): WebOptimizerConfigModel → {WebOptimizerConfigModel}", WebOptimizerConfigModel);
        EnvironmentVariables = new List<string>();
		_logger.LogError("AppSettingsModel(): EnvironmentVariables → {EnvironmentVariables}", EnvironmentVariables);
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