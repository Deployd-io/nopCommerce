using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Models.EntityUseCode;

/// <summary>
/// Represents an entity use code model
/// </summary>
public record EntityUseCodeModel : BaseNopEntityModel
{
    #region Ctor

    public EntityUseCodeModel()
    {
		logger.LogDebug(">>> Entering EntityUseCodeModel()");
        EntityUseCodes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting EntityUseCodeModel()");
		logger.LogDebug("EntityUseCodeModel(): EntityUseCodes → {EntityUseCodes}", EntityUseCodes);
    }

    #endregion

    #region Properties

    public string PrecedingElementId { get; set; }

    [NopResourceDisplayName("Plugins.Tax.Avalara.Fields.EntityUseCode")]
    public string AvalaraEntityUseCode { get; set; }
    public List<SelectListItem> EntityUseCodes { get; set; }

    #endregion
}