using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Models.EntityUseCode;

/// <summary>
/// Represents an entity use code model
/// </summary>
public record EntityUseCodeModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public EntityUseCodeModel()
    {
		_logger.LogDebug(">>> Entering EntityUseCodeModel()");
        EntityUseCodes = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting EntityUseCodeModel()");
		_logger.LogDebug("EntityUseCodeModel(): EntityUseCodes → {EntityUseCodes}", EntityUseCodes);
    }

    #endregion

    #region Properties

    public string PrecedingElementId { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.EntityUseCode")]
    public string AvalaraEntityUseCode { get; set; }
    public List<SelectListItem> EntityUseCodes { get; set; }

    #endregion
}