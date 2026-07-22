using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Components;

public partial class MultistorePreviewViewComponent : NopViewComponent
{
    #region Fields

    protected readonly ICommonModelFactory _commonModelFactory;

    #endregion

    #region Ctor

    public MultistorePreviewViewComponent(ICommonModelFactory commonModelFactory)
    {
		logger.LogDebug(">>> Entering MultistorePreviewViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
        _commonModelFactory = commonModelFactory;
		logger.LogDebug("<<< Exiting MultistorePreviewViewComponent(commonModelFactory={CommonModelFactory})", commonModelFactory);
		logger.LogDebug("MultistorePreviewViewComponent(commonModelFactory={CommonModelFactory}): _commonModelFactory → {_commonModelFactory}", commonModelFactory, _commonModelFactory);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Invoke view component
    /// </summary>
    /// <param name="model">Entity model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the view component result
    /// </returns>
    public async Task<IViewComponentResult> InvokeAsync(object model)
    {
		logger.LogDebug(">>> Entering InvokeAsync(model={Model})", model);
        if (model is not BaseNopEntityModel entityModel)
            return Content(string.Empty);

        var multistorePreviewModels = await _commonModelFactory.PrepareMultistorePreviewModelsAsync(entityModel);

		logger.LogDebug("InvokeAsync(model={Model}): multistorePreviewModels → {MultistorePreviewModels}", model, multistorePreviewModels);
		logger.LogDebug("<<< Exiting InvokeAsync(model={Model})", model);
        return View(multistorePreviewModels);
    }

    #endregion
}
