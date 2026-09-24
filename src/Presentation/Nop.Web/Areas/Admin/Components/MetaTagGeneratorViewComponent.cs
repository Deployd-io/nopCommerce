using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Components;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Components;

/// <summary>
/// Represents view component to use artificial intelligence service to generate meta tags
/// </summary>
public partial class MetaTagsGeneratorViewComponent : NopViewComponent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Filds

    protected readonly ArtificialIntelligenceSettings _artificialIntelligenceSettings;

    #endregion

    #region Ctor

    public MetaTagsGeneratorViewComponent(ArtificialIntelligenceSettings artificialIntelligenceSettings)
    {
		_logger.LogError(">>> Entering MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
        _artificialIntelligenceSettings = artificialIntelligenceSettings;
		_logger.LogDebug("MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings}): _artificialIntelligenceSettings → {_artificialIntelligenceSettings}", artificialIntelligenceSettings, _artificialIntelligenceSettings);
    _logger.LogWarning("<<< Exiting MetaTagsGeneratorViewComponent(artificialIntelligenceSettings={ArtificialIntelligenceSettings})", artificialIntelligenceSettings);
    }

    #endregion

    #region Methods

    public IViewComponentResult Invoke(MetaTagsGeneratorModel model)
    {
		_logger.LogError(">>> Entering Invoke(model={Model})", model);
        if (!_artificialIntelligenceSettings.Enabled)
            return Content(string.Empty);

        if (!_artificialIntelligenceSettings.AllowMetaTitleGeneration &&
            !_artificialIntelligenceSettings.AllowMetaKeywordsGeneration &&
            !_artificialIntelligenceSettings.AllowMetaDescriptionGeneration)
        {
		_logger.LogWarning("<<< Exiting Invoke(model={Model})", model);
            return Content(string.Empty);
        }

        return View(model);
    }

    #endregion
}