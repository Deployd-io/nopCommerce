using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an URL record search model
/// </summary>
public partial record UrlRecordSearchModel : BaseSearchModel
{
    #region Ctor

    public UrlRecordSearchModel()
    {
		logger.LogDebug(">>> Entering UrlRecordSearchModel()");
        AvailableLanguages = new List<SelectListItem>();
        AvailableActiveOptions = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting UrlRecordSearchModel()");
		logger.LogDebug("UrlRecordSearchModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
		logger.LogDebug("UrlRecordSearchModel(): AvailableActiveOptions → {AvailableActiveOptions}", AvailableActiveOptions);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.System.SeNames.List.Name")]
    public string SeName { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.List.Language")]
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.System.SeNames.List.IsActive")]
    public int IsActiveId { get; set; }

    public IList<SelectListItem> AvailableLanguages { get; set; }

    public IList<SelectListItem> AvailableActiveOptions { get; set; }

    #endregion
}