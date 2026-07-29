using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents a view component that displays the nopCommerce news
/// </summary>
public partial class NopCommerceNewsViewComponent : NopViewComponent
{
    #region Fields

    protected readonly IHomeModelFactory _homeModelFactory;

    #endregion

    #region Ctor

    public NopCommerceNewsViewComponent(IHomeModelFactory homeModelFactory)
    {
		logger.LogDebug(">>> Entering NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory})", homeModelFactory);
        _homeModelFactory = homeModelFactory;
		logger.LogDebug("<<< Exiting NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory})", homeModelFactory);
		logger.LogDebug("NopCommerceNewsViewComponent(homeModelFactory={HomeModelFactory}): _homeModelFactory → {_homeModelFactory}", homeModelFactory, _homeModelFactory);
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
        try
        {
            //prepare model
            var model = await _homeModelFactory.PrepareNopCommerceNewsModelAsync();

			logger.LogDebug("InvokeAsync(): model → {Model}", model);
            return View(model);
        }
        catch
        {
			logger.LogError("Exception in InvokeAsync(): {Arg0}", ex.Message);
			logger.LogDebug("<<< Exiting InvokeAsync()");
            return Content(string.Empty);
        }
    }

    #endregion
}