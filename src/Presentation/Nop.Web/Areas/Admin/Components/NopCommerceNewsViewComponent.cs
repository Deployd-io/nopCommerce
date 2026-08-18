using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the nopCommerce news
/// </summary>
public partial class NopCommerceNewsViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IHomeModelFactory _homeModelFactory;

    #endregion

    #region Ctor

    public NopCommerceNewsViewComponent(IHomeModelFactory homeModelFactory)
    {
		_logger.LogDebug(">>> Entering NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory})", homeModelFactory);
        _homeModelFactory = homeModelFactory;
		_logger.LogDebug("NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory}): _homeModelFactory → {_homeModelFactory}", homeModelFactory, _homeModelFactory);
		_logger.LogDebug("<<< Exiting NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory})", homeModelFactory);
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
        try
        {
            //prepare model
            var model = await _homeModelFactory.PrepareNopCommerceNewsModelAsync();
			_logger.LogDebug("InvokeAsync(): model → {Model}", model);

            return View(model);
        }
        catch
        {
			_logger.LogError("Exception in InvokeAsync(): {Arg0}", ex.Message);
			_logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content(string.Empty);
        }
    }

    #endregion
}