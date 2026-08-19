using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents an artificial intelligence product full description generator model
/// </summary>
public partial record ArtificialIntelligenceFullDescriptionModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ArtificialIntelligenceFullDescriptionModel()
    {
		_logger.LogDebug(">>> Entering ArtificialIntelligenceFullDescriptionModel()");
        AvailableLanguages = new List<SelectListItem>();
		_logger.LogDebug("ArtificialIntelligenceFullDescriptionModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
		_logger.LogDebug("<<< Exiting ArtificialIntelligenceFullDescriptionModel()");
    }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.ProductName")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Keywords")]
    public string Keywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.ToneOfVoice")]
    public int ToneOfVoiceId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.CustomToneOfVoice")]
    public string CustomToneOfVoice { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Instructions")]
    public string Instructions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.GeneratedDescription")]
    public string GeneratedDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.AiFullDescription.Language")]
    public int TargetLanguageId { get; set; }
    public IList<SelectListItem> AvailableLanguages { get; set; }

    public int LanguageId { get; set; }

    public bool SaveButtonClicked { get; set; }
}