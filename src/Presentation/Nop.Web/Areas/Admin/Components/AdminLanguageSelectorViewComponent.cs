using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the admin language selector
/// </summary>
public partial class AdminLanguageSelectorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;

    #endregion

    #region Ctor

    public AdminLanguageSelectorViewComponent(ICommonModelFactory commonModelFactory)
    {
		_logger.LogDebug(">>> Entering AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		_logger.LogDebug("<<< Exiting AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		_logger.LogDebug("AdminLanguageSelectorViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
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
        var model = await _commonModelFactory.PrepareLanguageSelectorModelAsync();

		_logger.LogDebug("InvokeAsync(): model → {Model}", model);
		_logger.LogDebug("<<< Exiting InvokeAsync()");
        return View(model);
    }

    #endregion
}