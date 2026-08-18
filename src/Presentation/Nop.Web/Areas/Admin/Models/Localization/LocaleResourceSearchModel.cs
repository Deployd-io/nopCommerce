using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Localization;

/// <summary>
/// Represents a locale resource search model
/// </summary>
public partial record LocaleResourceSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public LocaleResourceSearchModel()
    {
		_logger.LogDebug(">>> Entering LocaleResourceSearchModel()");
        AddResourceString = new LocaleResourceModel();
		_logger.LogDebug("LocaleResourceSearchModel(): AddResourceString → {AddResourceString}", AddResourceString);
		_logger.LogDebug("<<< Exiting LocaleResourceSearchModel()");
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