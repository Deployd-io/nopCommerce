using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate meta tags
/// </summary>
public partial class MetaTagsGeneratorViewComponent : NopViewComponent
{
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public MetaTagsGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
		logger.LogDebug(">>> Entering MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
		logger.LogDebug("<<< Exiting MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
		logger.LogDebug("MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings}): _artificialIntelligenceSettings → {_artificialIntelligenceSettings}", artificialIntelligenceSettings, _artificialIntelligenceSettings);
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(MetaTagsGeneratorModel model)
    {
		logger.LogDebug(">>> Entering Invoke(model={Model})", model);
        if (!_artificialIntelligenceSettings.Enabled)
            return Content(string.Empty);

        if (!_artificialIntelligenceSettings.AllowMetaTitleGeneration &&
            !_artificialIntelligenceSettings.AllowMetaKeywordsGeneration &&
            !_artificialIntelligenceSettings.AllowMetaDescriptionGeneration)
        {
            return Content(string.Empty);
        }

		logger.LogDebug("<<< Exiting Invoke(model={Model})", model);
        return View(model);
    }

    #endregion
}