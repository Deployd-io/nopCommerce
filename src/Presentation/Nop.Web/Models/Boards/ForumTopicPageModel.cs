using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Boards;

public partial record ForumTopicPageModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ForumTopicPageModel()
    {
		_logger.LogDebug(">>> Entering ForumTopicPageModel()");
        ForumPostModels = new List<ForumPostModel>();
		_logger.LogDebug("ForumTopicPageModel(): ForumPostModels → {ForumPostModels}", ForumPostModels);
    }

    public int Id { get; set; }
    public string Subject { get; set; }
    public string SeName { get; set; }

    public string WatchTopicText { get; set; }

    public bool IsCustomerAllowedToEditTopic { get; set; }
    public bool IsCustomerAllowedToDeleteTopic { get; set; }
    public bool IsCustomerAllowedToMoveTopic { get; set; }
    public bool IsCustomerAllowedToSubscribe { get; set; }

    public IList<ForumPostModel> ForumPostModels { get; set; }
    public EditorType ForumEditor { get; set; }
    public int PostsPageIndex { get; set; }
    public int PostsPageSize { get; set; }
    public int PostsTotalRecords { get; set; }

    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }

    public string JsonLd { get; set; }
}