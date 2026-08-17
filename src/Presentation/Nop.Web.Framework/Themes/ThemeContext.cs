using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Themes;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Themes;

/// <summary>
/// Represents the theme context implementation
/// </summary>
public partial class ThemeContext : IThemeContext
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings);
        _genericAttributeService = genericAttributeService;
		_logger.LogDebug("ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _genericAttributeService → {_genericAttributeService}", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings, _genericAttributeService);
        _storeContext = storeContext;
		_logger.LogDebug("ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeContext → {_storeContext}", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings, _storeContext);
        _themeProvider = themeProvider;
		_logger.LogDebug("ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _themeProvider → {_themeProvider}", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings, _themeProvider);
        _workContext = workContext;
		_logger.LogDebug("ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _workContext → {_workContext}", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings, _workContext);
        _storeInformationSettings = storeInformationSettings;
		_logger.LogDebug("<<< Exiting ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings})", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings);
		_logger.LogDebug("ThemeContext(genericAttributeService={GenericAttributeService},storeContext={StoreContext},themeProvider={ThemeProvider},workContext={WorkContext},storeInformationSettings={StoreInformationSettings}): _storeInformationSettings → {_storeInformationSettings}", genericAttributeService, storeContext, themeProvider, workContext, storeInformationSettings, _storeInformationSettings);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get or set current theme system name
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<string> GetWorkingThemeNameAsync()
    {
		_logger.LogDebug(">>> Entering GetWorkingThemeNameAsync()");
        if (!string.IsNullOrEmpty(_cachedThemeName))
            return _cachedThemeName;

        var themeName = string.Empty;
		_logger.LogDebug("GetWorkingThemeNameAsync(): themeName → {ThemeName}", themeName);

        //whether customers are allowed to select a theme
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("GetWorkingThemeNameAsync(): customer → {Customer}", customer);
        if (_storeInformationSettings.AllowCustomerToSelectTheme &&
            customer != null)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
			_logger.LogDebug("GetWorkingThemeNameAsync(): store → {Store}", store);
            themeName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.WorkingThemeNameAttribute, store.Id);
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
		_logger.LogDebug("GetWorkingThemeNameAsync(): _cachedThemeName → {_cachedThemeName}", _cachedThemeName);

		_logger.LogDebug("<<< Exiting GetWorkingThemeNameAsync()");
        return themeName;
    }

    /// <summary>
    /// Set current theme system name
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SetWorkingThemeNameAsync(string workingThemeName)
    {
		_logger.LogDebug(">>> Entering SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName})", workingThemeName);
        //whether customers are allowed to select a theme
        var customer = await _workContext.GetCurrentCustomerAsync();
		_logger.LogDebug("SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName}): customer → {Customer}", workingThemeName, customer);
        if (!_storeInformationSettings.AllowCustomerToSelectTheme ||
            customer == null)
			_logger.LogDebug("<<< Exiting SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName})", workingThemeName);
            return;

        //save selected by customer theme system name
        var store = await _storeContext.GetCurrentStoreAsync();
        await _genericAttributeService.SaveAttributeAsync(customer,
            NopCustomerDefaults.WorkingThemeNameAttribute, workingThemeName,
            store.Id);

        //clear cache
        _cachedThemeName = null;
		_logger.LogDebug("SetWorkingThemeNameAsync(workingThemeName={WorkingThemeName}): _cachedThemeName → {_cachedThemeName}", workingThemeName, _cachedThemeName);
    }

    #endregion
}