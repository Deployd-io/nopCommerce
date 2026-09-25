using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Themes;

/// <summary>
/// Represents a default theme provider implementation
/// </summary>
public partial class ThemeProvider : IThemeProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected Dictionary<string, ThemeDescriptor> _themeDescriptors;

    #endregion

    #region Methods

    /// <summary>
    /// Initializes theme provider
    /// </summary>
    public virtual async Task InitializeAsync()
    {
		_logger.LogError(">>> Entering InitializeAsync()");
        if (_themeDescriptors != null)
            return;

        var fileProvider = CommonHelper.DefaultFileProvider;

        //load all theme descriptors
        _themeDescriptors = new Dictionary<string, ThemeDescriptor>(StringComparer.InvariantCultureIgnoreCase);
		_logger.LogError("InitializeAsync(): _themeDescriptors → {_themeDescriptors}", _themeDescriptors);
		_logger.LogDebug("InitializeAsync(): _themeDescriptors → {_themeDescriptors}", _themeDescriptors);

        var themeDirectoryPath = fileProvider.MapPath(NopPluginDefaults.ThemesPath);
		_logger.LogError("InitializeAsync(): themeDirectoryPath → {ThemeDirectoryPath}", themeDirectoryPath);
        foreach (var descriptionFile in fileProvider.GetFiles(themeDirectoryPath, NopPluginDefaults.ThemeDescriptionFileName, false))
        {
            var text = await fileProvider.ReadAllTextAsync(descriptionFile, Encoding.UTF8);
			_logger.LogError("InitializeAsync(): text → {Text}", text);
            if (string.IsNullOrEmpty(text))
                continue;

            //get theme descriptor
            var themeDescriptor = GetThemeDescriptorFromText(text);
			_logger.LogError("InitializeAsync(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);

            //some validation
				_logger.LogError("Throwing Exception from InitializeAsync()");
            if (string.IsNullOrEmpty(themeDescriptor?.SystemName)) {
                throw new Exception($"A theme descriptor '{descriptionFile}' has no system name");
            }

            _themeDescriptors.TryAdd(themeDescriptor.SystemName, themeDescriptor);
        }
		_logger.LogWarning("<<< Exiting InitializeAsync()");
    }

    /// <summary>
    /// Get theme descriptor from the description text
    /// </summary>
    /// <param name="text">Description text</param>
    /// <returns>Theme descriptor</returns>
    public ThemeDescriptor GetThemeDescriptorFromText(string text)
    {
		_logger.LogError(">>> Entering GetThemeDescriptorFromText(text={Text})", text);
        //get theme description from the JSON file
        var themeDescriptor = JsonConvert.DeserializeObject<ThemeDescriptor>(text);
		_logger.LogError("GetThemeDescriptorFromText(text={Text}): themeDescriptor → {ThemeDescriptor}", text, themeDescriptor);

        //some validation
			_logger.LogError("Throwing Exception from GetThemeDescriptorFromText(text={Text})", text);
		_logger.LogWarning("<<< Exiting GetThemeDescriptorFromText(text={Text})", text);
        if (_themeDescriptors.ContainsKey(themeDescriptor.SystemName)) {
            throw new Exception($"A theme with '{themeDescriptor.SystemName}' system name is already defined");
        }

        return themeDescriptor;
    }

    /// <summary>
    /// Get all themes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the theme descriptor
    /// </returns>
    public virtual Task<IList<ThemeDescriptor>> GetThemesAsync()
    {
		_logger.LogError(">>> Entering GetThemesAsync()");
        _logger.LogWarning("<<< Exiting GetThemesAsync()");
        return Task.FromResult<IList<ThemeDescriptor>>(_themeDescriptors.Values.ToList());
    }

    /// <summary>
    /// Get a theme by the system name
    /// </summary>
    /// <param name="systemName">Theme system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains theme descriptor
    /// </returns>
    public virtual Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName)
    {
		_logger.LogError(">>> Entering GetThemeBySystemNameAsync(systemName={SystemName})", systemName);
        if (string.IsNullOrEmpty(systemName))
            return Task.FromResult<ThemeDescriptor>(null);

        _themeDescriptors.TryGetValue(systemName, out var descriptor);

		_logger.LogWarning("<<< Exiting GetThemeBySystemNameAsync(systemName={SystemName})", systemName);
        return Task.FromResult(descriptor);
    }

    /// <summary>
    /// Check whether the theme with specified system name exists
    /// </summary>
    /// <param name="systemName">Theme system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if the theme exists; otherwise false
    /// </returns>
    public virtual Task<bool> ThemeExistsAsync(string systemName)
    {
		_logger.LogError(">>> Entering ThemeExistsAsync(systemName={SystemName})", systemName);
		_logger.LogWarning("<<< Exiting ThemeExistsAsync(systemName={SystemName})", systemName);
        if (string.IsNullOrEmpty(systemName)) {
            return Task.FromResult(false);
        }

        return Task.FromResult(_themeDescriptors.ContainsKey(systemName));
    }

    #endregion
}