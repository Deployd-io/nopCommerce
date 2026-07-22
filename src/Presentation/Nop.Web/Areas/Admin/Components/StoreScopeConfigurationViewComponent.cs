using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the store scope configuration
/// </summary>
public partial class StoreScopeConfigurationViewComponent : NopViewComponent
{
    #region Fields

    protected readonly ISettingModelFactory _settingModelFactory;

    #endregion

    #region Ctor

    public StoreScopeConfigurationViewComponent(ISettingModelFactory settingModelFactory)
    {
		logger.LogDebug(">>> Entering StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
        _settingModelFactory = settingModelFactory;
		logger.LogDebug("<<< Exiting StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory})", settingModelFactory);
		logger.LogDebug("StoreScopeConfigurationViewComponent(settingModelFactory={SettingModelFactory}): _settingModelFactory → {_settingModelFactory}", settingModelFactory, _settingModelFactory);
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
		logger.LogDebug(">>> Entering InvokeAsync()");
        //prepare model
        var model = await _settingModelFactory.PrepareStoreScopeConfigurationModelAsync();

		logger.LogDebug("InvokeAsync(): model → {Model}", model);
        if (model.Stores.Count < 2)
		logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content(string.Empty);

        return View(model);
    }

    #endregion
}
