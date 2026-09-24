using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog post search model
/// </summary>
public partial record BlogPostSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public BlogPostSearchModel()
    {
		_logger.LogError(">>> Entering BlogPostSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("BlogPostSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    _logger.LogError("BlogPostSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    _logger.LogWarning("<<< Exiting BlogPostSearchModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.List.SearchStore")]
    public int SearchStoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public string SearchTitle { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}