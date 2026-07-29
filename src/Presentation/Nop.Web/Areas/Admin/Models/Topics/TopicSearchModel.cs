using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Topics;

/// <summary>
/// Represents a topic search model
/// </summary>
public partial record TopicSearchModel : BaseSearchModel
{
    #region Ctor

    public TopicSearchModel()
    {
		logger.LogDebug(">>> Entering TopicSearchModel()");
        AvailableStores = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting TopicSearchModel()");
		logger.LogDebug("TopicSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Topics.List.SearchKeywords")]
    public string SearchKeywords { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}