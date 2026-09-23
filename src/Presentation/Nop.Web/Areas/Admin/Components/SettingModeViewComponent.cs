using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the setting mode
/// </summary>
public partial class SettingModeViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingModelFactory _settingModelFactory;

    #endregion

    #region Ctor

    public SettingModeViewComponent(ISettingModelFactory settingModelFactory)
    {
		_logger.LogError(">>> Entering SettingModeViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
        _settingModelFactory = settingModelFactory;
		_logger.LogDebug("SettingModeViewComponent(settingModelFactory={SettingModelFactory}): _settingModelFactory → {_settingModelFactory}", settingModelFactory, _settingModelFactory);
    _logger.LogWarning("<<< Exiting SettingModeViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="modeName">Setting mode name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(string modeName = "settings-advanced-mode")
    {
		_logger.LogError(">>> Entering InvokeAsync(modeName={ModeName})", modeName);
        //prepare model
        var model = await _settingModelFactory.PrepareSettingModeModelAsync(modeName);
		_logger.LogError("InvokeAsync(modeName={ModeName}): model → {Model}", modeName, model);

		_logger.LogWarning("<<< Exiting InvokeAsync(modeName={ModeName})", modeName);
        return View(model);
    }

    #endregion
}