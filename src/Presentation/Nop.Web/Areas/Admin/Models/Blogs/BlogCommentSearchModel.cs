using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Blogs;

/// <summary>
/// Represents a blog comment search model
/// </summary>
public partial record BlogCommentSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public BlogCommentSearchModel()
    {
		_logger.LogDebug(">>> Entering BlogCommentSearchModel()");
        AvailableApprovedOptions = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting BlogCommentSearchModel()");
		_logger.LogDebug("BlogCommentSearchModel(): AvailableApprovedOptions → {AvailableApprovedOptions}", AvailableApprovedOptions);
    }

    #endregion

    #region Properties

    public int? BlogPostId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnFrom")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnFrom { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.CreatedOnTo")]
    [UIHint("DateNullable")]
    public DateTime? CreatedOnTo { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchText")]
    public string SearchText { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.Blog.Comments.List.SearchApproved")]
    public int SearchApprovedId { get; set; }

    public IList<SelectListItem> AvailableApprovedOptions { get; set; }

    #endregion
}