using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.News;

public partial record NewsItemModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NewsItemModel()
    {
		_logger.LogDebug(">>> Entering NewsItemModel()");
        Comments = new List<NewsCommentModel>();
        AddNewComment = new AddNewsCommentModel();
		_logger.LogDebug("<<< Exiting NewsItemModel()");
		_logger.LogDebug("NewsItemModel(): AddNewComment → {AddNewComment}", AddNewComment);
		_logger.LogDebug("NewsItemModel(): Comments → {Comments}", Comments);
    }

    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public string Title { get; set; }
    public string Short { get; set; }
    public string Full { get; set; }
    public bool AllowComments { get; set; }
    public bool PreventNotRegisteredUsersToLeaveComments { get; set; }
    public int NumberOfComments { get; set; }
    public DateTime CreatedOn { get; set; }

    public IList<NewsCommentModel> Comments { get; set; }
    public AddNewsCommentModel AddNewComment { get; set; }
}