using Microsoft.Extensions.Logging;
namespace Nop.Core.Configuration;

/// <summary>
/// Represents the event that is raised when App Settings are saving
/// </summary>
public partial class AppSettingsSavingEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IList<IConfig> _configurations;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="configurations">List of configuration to save</param>
    public AppSettingsSavingEvent(IList<IConfig> configurations)
    {
		_logger.LogError(">>> Entering AppSettingsSavingEvent(configurations={Configurations})", configurations);
        _configurations = configurations;
		_logger.LogDebug("AppSettingsSavingEvent(configurations={Configurations}): _configurations → {_configurations}", configurations, _configurations);
    _logger.LogWarning("<<< Exiting AppSettingsSavingEvent(configurations={Configurations})", configurations);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add configuration to save
    /// </summary>
    /// <param name="config">Configuration to save</param>
    public virtual void AddConfig<TConfig>(TConfig config) where TConfig : class, IConfig
    {
		_logger.LogError(">>> Entering AddConfig(config={Config})", config);
        if (_configurations.OfType<TConfig>().FirstOrDefault() is { } currentConfig)
            _configurations[_configurations.IndexOf(currentConfig)] = config;
        else
            _configurations.Add(config);
			_logger.LogWarning("<<< Exiting AddConfig(config={Config})", config);
    }

    #endregion
}