using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Plugins;

namespace Nop.Services.Themes;

/// <summary>
/// Represents a default theme provider implementation
/// </summary>
public partial class ThemeProvider : IThemeProvider
{
    #region Fields

    protected Dictionary<string, ThemeDescriptor> _themeDescriptors;

    #endregion

    #region Methods

    /// <summary>
    /// Initializes theme provider
    /// </summary>
    public virtual async Task InitializeAsync()
    {
		logger.LogDebug(">>> Entering InitializeAsync()");
        if (_themeDescriptors != null)
            return;

        var fileProvider = CommonHelper.DefaultFileProvider;

        //load all theme descriptors
        _themeDescriptors = new Dictionary<string, ThemeDescriptor>(StringComparer.InvariantCultureIgnoreCase);
		logger.LogDebug("InitializeAsync(): _themeDescriptors → {_themeDescriptors}", _themeDescriptors);

        var themeDirectoryPath = fileProvider.MapPath(NopPluginDefaults.ThemesPath);
        foreach (var descriptionFile in fileProvider.GetFiles(themeDirectoryPath, NopPluginDefaults.ThemeDescriptionFileName, false))
        {
		logger.LogDebug("InitializeAsync(): themeDirectoryPath → {ThemeDirectoryPath}", themeDirectoryPath);
            var text = await fileProvider.ReadAllTextAsync(descriptionFile, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
				logger.LogDebug("InitializeAsync(): text → {Text}", text);
                continue;

            //get theme descriptor
            var themeDescriptor = GetThemeDescriptorFromText(text);

			logger.LogDebug("InitializeAsync(): themeDescriptor → {ThemeDescriptor}", themeDescriptor);
            //some validation
            if (string.IsNullOrEmpty(themeDescriptor?.SystemName))
                throw new Exception($"A theme descriptor '{descriptionFile}' has no system name");

            _themeDescriptors.TryAdd(themeDescriptor.SystemName, themeDescriptor);
        }
		logger.LogDebug("<<< Exiting InitializeAsync()");
    }

    /// <summary>
    /// Get theme descriptor from the description text
    /// </summary>
    /// <param name="text">Description text</param>
    /// <returns>Theme descriptor</returns>
    public ThemeDescriptor GetThemeDescriptorFromText(string text)
    {
        //get theme description from the JSON file
        var themeDescriptor = JsonConvert.DeserializeObject<ThemeDescriptor>(text);

        //some validation
        if (_themeDescriptors.ContainsKey(themeDescriptor.SystemName))
            throw new Exception($"A theme with '{themeDescriptor.SystemName}' system name is already defined");

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
		logger.LogDebug(">>> Entering GetThemesAsync()");
		logger.LogDebug("<<< Exiting GetThemesAsync()");
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
		logger.LogDebug(">>> Entering GetThemeBySystemNameAsync(systemName={SystemName})", systemName);
        if (string.IsNullOrEmpty(systemName))
            return Task.FromResult<ThemeDescriptor>(null);

        _themeDescriptors.TryGetValue(systemName, out var descriptor);

		logger.LogDebug("<<< Exiting GetThemeBySystemNameAsync(systemName={SystemName})", systemName);
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
		logger.LogDebug(">>> Entering ThemeExistsAsync(systemName={SystemName})", systemName);
        if (string.IsNullOrEmpty(systemName))
		logger.LogDebug("<<< Exiting ThemeExistsAsync(systemName={SystemName})", systemName);
            return Task.FromResult(false);

        return Task.FromResult(_themeDescriptors.ContainsKey(systemName));
    }

    #endregion
}