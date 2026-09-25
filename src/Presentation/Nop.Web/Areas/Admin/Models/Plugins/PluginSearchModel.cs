using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Plugins;

/// <summary>
/// Represents a plugin search model
/// </summary>
public partial record PluginSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public PluginSearchModel()
    {
		_logger.LogError(">>> Entering PluginSearchModel()");
        AvailableLoadModes = new List<SelectListItem>();
        AvailableGroups = new List<SelectListItem>();
		_logger.LogError("PluginSearchModel(): AvailableGroups → {AvailableGroups}", AvailableGroups);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
    public int SearchLoadModeId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Group")]
    public string SearchGroup { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.FriendlyName")]
    public string SearchFriendlyName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Author")]
    public string SearchAuthor { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
    public IList<SelectListItem> AvailableLoadModes { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Plugins.Group")]
    public IList<SelectListItem> AvailableGroups { get; set; }

    public bool NeedToRestart { get; set; }

    #endregion
}