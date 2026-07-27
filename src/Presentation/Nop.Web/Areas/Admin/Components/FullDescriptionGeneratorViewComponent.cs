using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate product's full description
/// </summary>
public partial class FullDescriptionGeneratorViewComponent : NopViewComponent
{
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public FullDescriptionGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
		logger.LogDebug(">>> Entering FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
		logger.LogDebug("<<< Exiting FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
		logger.LogDebug("FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings}): _artificialIntelligenceSettings → {_artificialIntelligenceSettings}", artificialIntelligenceSettings, _artificialIntelligenceSettings);
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(FullDescriptionGeneratorModel model)
    {
		logger.LogDebug(">>> Entering Invoke(model={Model})", model);
        if (!_artificialIntelligenceSettings.Enabled)
		logger.LogDebug("<<< Exiting Invoke(model={Model})", model);
            return Content(string.Empty);

        return View(model);
    }

    #endregion
}
