using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostModel()
    {
		_logger.LogError(">>> Entering BlogPostModel()");
        Tags = new List<string>();
        Comments = new List<BlogCommentModel>();
		_logger.LogError("BlogPostModel(): Comments → {Comments}", Comments);
        AddNewComment = new AddBlogCommentModel();
		_logger.LogError("BlogPostModel(): AddNewComment → {AddNewComment}", AddNewComment);
    }

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public string Title { get; set; }
    public string Body { get; set; }
    public string BodyOverview { get; set; }
    public bool AllowComments { get; set; }
    public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
    public int NumberOfComments { get; set; }
    public DateTime CreatedOn { get; set; }

    public IList<string> Tags { get; set; }

    public IList<BlogCommentModel> Comments { get; set; }
    public AddBlogCommentModel AddNewComment { get; set; }
}