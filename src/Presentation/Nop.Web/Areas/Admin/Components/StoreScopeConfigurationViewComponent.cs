using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the store scope configuration
/// </summary>
public partial class StoreScopeConfigurationViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ISettingModelFactory _settingModelFactory;

    #endregion

    #region Ctor

    public StoreScopeConfigurationViewComponent(ISettingModelFactory settingModelFactory)
    {
		_logger.LogDebug(">>> Entering StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
        _settingModelFactory = settingModelFactory;
		_logger.LogDebug("<<< Exiting StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
		_logger.LogDebug("StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory}): _settingModelFactory → {_settingModelFactory}", settingModelFactory, _settingModelFactory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync()
    {
		_logger.LogDebug(">>> Entering InvokeAsync()");
        //prepare model
        var model = await _settingModelFactory.PrepareStoreScopeConfigurationModelAsync();

		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
        if (model.Stores.Count < 2)
		_logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content(string.Empty);

        return View(model);
    }

    #endregion
}