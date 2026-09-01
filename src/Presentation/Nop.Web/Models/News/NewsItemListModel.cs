using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News;

public partial record NewsItemListModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NewsItemListModel()
    {
		_logger.LogDebug(">>> Entering NewsItemListModel()");
        PagingFilteringContext = new NewsPagingFilteringModel();
		_logger.LogDebug("NewsItemListModel(): PagingFilteringContext → {PagingFilteringContext}", PagingFilteringContext);
        NewsItems = new List<NewsItemModel>();
    }

    public int WorkingLanguageId { get; set; }
    public NewsPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<NewsItemModel> NewsItems { get; set; }
}