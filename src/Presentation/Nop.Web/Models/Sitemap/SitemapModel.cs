using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Sitemap;

public partial record SitemapModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SitemapModel()
    {
		_logger.LogError(">>> Entering SitemapModel()");
        Items = new List<SitemapItemModel>();
        PageModel = new SitemapPageModel();
		_logger.LogError("SitemapModel(): PageModel → {PageModel}", PageModel);
    }

    #endregion

    #region Properties

    public List<SitemapItemModel> Items { get; set; }

    public SitemapPageModel PageModel { get; set; }

    #endregion

    #region Nested classes

    public partial record SitemapItemModel
    {
        public string GroupTitle { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    #endregion
}