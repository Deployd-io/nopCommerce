using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;

namespace Nop.Web.Models.News;

public partial record NewsItemModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public NewsItemModel()
    {
		logger.LogDebug(">>> Entering NewsItemModel()");
        Comments = new List<NewsCommentModel>();
        AddNewComment = new AddNewsCommentModel();
		logger.LogDebug("<<< Exiting NewsItemModel()");
		logger.LogDebug("NewsItemModel(): Comments → {Comments}", Comments);
		logger.LogDebug("NewsItemModel(): AddNewComment → {AddNewComment}", AddNewComment);
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