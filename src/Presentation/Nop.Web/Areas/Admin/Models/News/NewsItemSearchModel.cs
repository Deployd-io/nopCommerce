using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.News;

/// <summary>
/// Represents a news item search model
/// </summary>
public partial record NewsItemSearchModel : BaseSearchModel
{
    #region Ctor

    public NewsItemSearchModel()
    {
		logger.LogDebug(">>> Entering NewsItemSearchModel()");
        AvailableStores = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting NewsItemSearchModel()");
		logger.LogDebug("NewsItemSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchStore")]
    public int SearchStoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public string SearchTitle { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}