using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Core.Configuration;

/// <summary>
/// Represents the app settings helper
/// </summary>
public partial class AppSettingsHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected static Dictionary<string, int> _configurationOrder;

    #endregion

    #region Methods

    /// <summary>
    /// Create app settings with the passed configurations and save it to the file
    /// </summary>
    /// <param name="configurations">Configurations to save</param>
    /// <param name="fileProvider">File provider</param>
    /// <param name="overwrite">Whether to overwrite appsettings file</param>
    /// <returns>App settings</returns>
    public static AppSettings SaveAppSettings(IList<IConfig> configurations, INopFileProvider fileProvider, bool overwrite = true)
    {
		_logger.LogDebug(">>> Entering SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite})", configurations, fileProvider, overwrite);
        ArgumentNullException.ThrowIfNull(configurations);

        _configurationOrder = configurations.ToDictionary(config => config.Name, config => config.GetOrder());

        //create app settings
        var appSettings = Singleton<AppSettings>.Instance ?? new AppSettings();
		_logger.LogDebug("SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite}): appSettings → {AppSettings}", configurations, fileProvider, overwrite, appSettings);
        appSettings.Update(configurations);
        Singleton<AppSettings>.Instance = appSettings;

        //create file if not exists
        var filePath = fileProvider.MapPath(NopConfigurationDefaults.AppSettingsFilePath);
		_logger.LogDebug("SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite}): filePath → {FilePath}", configurations, fileProvider, overwrite, filePath);
        var fileExists = fileProvider.FileExists(filePath);
		_logger.LogDebug("SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite}): fileExists → {FileExists}", configurations, fileProvider, overwrite, fileExists);
        fileProvider.CreateFile(filePath);

        //get raw configuration parameters
        var configuration = JsonConvert.DeserializeObject<AppSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))
                                ?.Configuration
                            ?? new();
        foreach (var config in configurations)
        {
            configuration[config.Name] = JToken.FromObject(config);
        }

        //sort configurations for display by order (e.g. data configuration with 0 will be the first)
        appSettings.Configuration = configuration
			_logger.LogDebug("SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite}): appSettings.Configuration → {Arg3}", configurations, fileProvider, overwrite, appSettings.Configuration);
            .SelectMany(outConfig => _configurationOrder.Where(inConfig => inConfig.Key == outConfig.Key).DefaultIfEmpty(),
                (outConfig, inConfig) => new { OutConfig = outConfig, InConfig = inConfig })
            .OrderBy(config => config.InConfig.Value)
            .Select(config => config.OutConfig)
            .ToDictionary(config => config.Key, config => config.Value);

        //save app settings to the file
        if (!fileExists || overwrite)
        {
            var text = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
			_logger.LogDebug("SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite}): text → {Text}", configurations, fileProvider, overwrite, text);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

		_logger.LogDebug("<<< Exiting SaveAppSettings(configurations={Configurations},fileProvider={FileProvider},overwrite={Overwrite})", configurations, fileProvider, overwrite);
        return appSettings;
    }

    #endregion
}