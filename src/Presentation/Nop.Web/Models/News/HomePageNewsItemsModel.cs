using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News;

public partial record HomepageNewsItemsModel : BaseNopModel
{
    public HomepageNewsItemsModel()
    {
		logger.LogDebug(">>> Entering HomepageNewsItemsModel()");
        NewsItems = new List<NewsItemModel>();
		logger.LogDebug("<<< Exiting HomepageNewsItemsModel()");
		logger.LogDebug("HomepageNewsItemsModel(): NewsItems → {NewsItems}", NewsItems);
    }

    public int WorkingLanguageId { get; set; }
    public IList<NewsItemModel> NewsItems { get; set; }
}