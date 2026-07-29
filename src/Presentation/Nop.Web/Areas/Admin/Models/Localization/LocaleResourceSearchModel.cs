using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Localization;

/// <summary>
/// Represents a locale resource search model
/// </summary>
public partial record LocaleResourceSearchModel : BaseSearchModel
{
    #region Ctor

    public LocaleResourceSearchModel()
    {
		logger.LogDebug(">>> Entering LocaleResourceSearchModel()");
        AddResourceString = new LocaleResourceModel();
		logger.LogDebug("<<< Exiting LocaleResourceSearchModel()");
		logger.LogDebug("LocaleResourceSearchModel(): AddResourceString → {AddResourceString}", AddResourceString);
    }

    #endregion

    #region Properties

    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceName")]
    public string SearchResourceName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceValue")]
    public string SearchResourceValue { get; set; }

    public LocaleResourceModel AddResourceString { get; set; }

    #endregion
}