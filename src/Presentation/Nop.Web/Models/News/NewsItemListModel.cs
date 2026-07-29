using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News;

public partial record NewsItemListModel : BaseNopModel
{
    public NewsItemListModel()
    {
		logger.LogDebug(">>> Entering NewsItemListModel()");
        PagingFilteringContext = new NewsPagingFilteringModel();
        NewsItems = new List<NewsItemModel>();
		logger.LogDebug("<<< Exiting NewsItemListModel()");
		logger.LogDebug("NewsItemListModel(): PagingFilteringContext → {PagingFilteringContext}", PagingFilteringContext);
		logger.LogDebug("NewsItemListModel(): NewsItems → {NewsItems}", NewsItems);
    }

    public int WorkingLanguageId { get; set; }
    public NewsPagingFilteringModel PagingFilteringContext { get; set; }
    public IList<NewsItemModel> NewsItems { get; set; }
}