namespace Nop.Core.Configuration;

/// <summary>
/// Represents the event that is raised when App Settings are saving
/// </summary>
public partial class AppSettingsSavingEvent
{
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
		logger.LogDebug(">>> Entering AppSettingsSavingEvent(configurations={Configurations})", configurations);
        _configurations = configurations;
		logger.LogDebug("<<< Exiting AppSettingsSavingEvent(configurations={Configurations})", configurations);
		logger.LogDebug("AppSettingsSavingEvent(configurations={Configurations}): _configurations → {_configurations}", configurations, _configurations);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add configuration to save
    /// </summary>
    /// <param name="config">Configuration to save</param>
    public virtual void AddConfig<TConfig>(TConfig config) where TConfig : class, IConfig
    {
		logger.LogDebug(">>> Entering AddConfig(config={Config})", config);
        if (_configurations.OfType<TConfig>().FirstOrDefault() is { } currentConfig)
            _configurations[_configurations.IndexOf(currentConfig)] = config;
        else
            _configurations.Add(config);
			logger.LogDebug("<<< Exiting AddConfig(config={Config})", config);
    }

    #endregion
}