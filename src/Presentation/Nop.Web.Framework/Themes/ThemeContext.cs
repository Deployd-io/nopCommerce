using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Themes;

namespace Nop.Web.Framework.Themes;

/// <summary>
/// Represents the theme context implementation
/// </summary>
public partial class ThemeContext : IThemeContext
{
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IWorkContext _workContext;
    protected readonly StoreInformationSettings _storeInformationSettings;

    protected string _cachedThemeName;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="genericAttributeService">Generic attribute service</param>
    /// <param name="storeContext">Store context</param>
    /// <param name="themeProvider">Theme provider</param>
    /// <param name="workContext">Work context</param>
    /// <param name="storeInformationSettings">Store information settings</param>
    public ThemeContext(IGenericAttributeService genericAttributeService,
        IStoreContext storeContext,
        IThemeProvider themeProvider,
        IWorkContext workContext,
        StoreInformationSettings storeInformationSettings)
    {
        _genericAttributeService = genericAttributeService;
        _storeContext = storeContext;
        _themeProvider = themeProvider;
        _workContext = workContext;
        _storeInformationSettings = storeInformationSettings;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get or set current theme system name
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetWorkingThemeNameAsync()
    {
		logger.LogDebug(">>> Entering GetWorkingThemeNameAsync()");
        if (!string.IsNullOrEmpty(_cachedThemeName))
            return _cachedThemeName;

        var themeName = string.Empty;

		logger.LogDebug("GetWorkingThemeNameAsync(): themeName → {ThemeName}", themeName);
        //whether customers are allowed to select a theme
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (_storeInformationSettings.AllowCustomerToSelectTheme &&
			logger.LogDebug("GetWorkingThemeNameAsync(): customer → {Customer}", customer);
            customer != null)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            themeName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.WorkingThemeNameAttribute, store.Id);
				logger.LogDebug("GetWorkingThemeNameAsync(): store → {Store}", store);
        }

        //if not, try to get default store theme
        if (string.IsNullOrEmpty(themeName))
            themeName = _storeInformationSettings.DefaultStoreTheme;

        //ensure that this theme exists
        if (!await _themeProvider.ThemeExistsAsync(themeName))
        {
            //if it does not exist, try to get the first one
            themeName = (await _themeProvider.GetThemesAsync()).FirstOrDefault()?.SystemName
                        ?? throw new Exception("No theme could be loaded");
        }

        //cache theme system name
        _cachedThemeName = themeName;
		logger.LogDebug("GetWorkingThemeNameAsync(): _cachedThemeName → {_cachedThemeName}", _cachedThemeName);

		logger.LogDebug("<<< Exiting GetWorkingThemeNameAsync()");
        return themeName;
    }

    /// <summary>
    /// Set current theme system name
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingThemeNameAsync(string workingThemeName)
    {
		logger.LogDebug(">>> Entering SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName})", workingThemeName);
        //whether customers are allowed to select a theme
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!_storeInformationSettings.AllowCustomerToSelectTheme ||
			logger.LogDebug("SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName}): customer → {Customer}", workingThemeName, customer);
            customer == null)
            return;

        //save selected by customer theme system name
        var store = await _storeContext.GetCurrentStoreAsync();
        await _genericAttributeService.SaveAttributeAsync(customer,
            NopCustomerDefaults.WorkingThemeNameAttribute, workingThemeName,
            store.Id);

        //clear cache
        _cachedThemeName = null;
		logger.LogDebug("SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName}): _cachedThemeName → {_cachedThemeName}", workingThemeName, _cachedThemeName);
		logger.LogDebug("<<< Exiting SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName})", workingThemeName);
    }

    #endregion
}
