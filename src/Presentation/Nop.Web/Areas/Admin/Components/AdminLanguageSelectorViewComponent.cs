using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the admin language selector
/// </summary>
public partial class AdminLanguageSelectorViewComponent : NopViewComponent
{
    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;

    #endregion

    #region Ctor

    public AdminLanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
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
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		logger.LogDebug("InvokeAsync(): model → {Model}", model);
		logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }

    #endregion
}
