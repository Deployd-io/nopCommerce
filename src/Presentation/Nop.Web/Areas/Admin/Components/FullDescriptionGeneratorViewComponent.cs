using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate product's full description
/// </summary>
public partial class FullDescriptionGeneratorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public FullDescriptionGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
		_logger.LogDebug(">>> Entering FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
		_logger.LogDebug("FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings}): _artificialIntelligenceSettings → {_artificialIntelligenceSettings}", artificialIntelligenceSettings, _artificialIntelligenceSettings);
		_logger.LogDebug("<<< Exiting FullDescriptionGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(FullDescriptionGeneratorModel model)
    {
		_logger.LogDebug(">>> Entering Invoke(model={Model})", model);
        if (!_artificialIntelligenceSettings.Enabled)
            return Content(string.Empty);

        return View(model);
    }

    #endregion
}
