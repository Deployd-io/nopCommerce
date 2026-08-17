using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Boards;

public partial record SearchModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SearchModel()
    {
		_logger.LogDebug(">>> Entering SearchModel()");
        LimitList = new List<SelectListItem>();
        ForumList = new List<SelectListItem>();
		_logger.LogDebug("SearchModel(): ForumList → {ForumList}", ForumList);
		_logger.LogDebug("SearchModel(): LimitList → {LimitList}", LimitList);
        WithinList = new List<SelectListItem>();
		_logger.LogDebug("SearchModel(): WithinList → {WithinList}", WithinList);
        ForumTopics = new List<ForumTopicRowModel>();
		_logger.LogDebug("<<< Exiting SearchModel()");
		_logger.LogDebug("SearchModel(): ForumTopics → {ForumTopics}", ForumTopics);
    }

    public bool ShowAdvancedSearch { get; set; }

    public string SearchTerms { get; set; }

    public int? ForumId { get; set; }

    public int? Within { get; set; }

    public int? LimitDays { get; set; }

    public IList<ForumTopicRowModel> ForumTopics { get; set; }
    public int TopicPageSize { get; set; }
    public int TopicTotalRecords { get; set; }
    public int TopicPageIndex { get; set; }

    public List<SelectListItem> LimitList { get; set; }

    public List<SelectListItem> ForumList { get; set; }

    public List<SelectListItem> WithinList { get; set; }

    public int ForumIdSelected { get; set; }

    public int WithinSelected { get; set; }

    public int LimitDaysSelected { get; set; }

    public bool SearchResultsVisible { get; set; }

    public bool NoResultsVisisble { get; set; }

    public string Error { get; set; }

    public int PostsPageSize { get; set; }

    public bool AllowPostVoting { get; set; }
}