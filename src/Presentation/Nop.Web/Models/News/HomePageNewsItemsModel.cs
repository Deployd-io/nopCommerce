using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.News;

public partial record HomepageNewsItemsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public HomepageNewsItemsModel()
    {
		_logger.LogDebug(">>> Entering HomepageNewsItemsModel()");
        NewsItems = new List<NewsItemModel>();
		_logger.LogDebug("<<< Exiting HomepageNewsItemsModel()");
		_logger.LogDebug("HomepageNewsItemsModel(): NewsItems → {NewsItems}", NewsItems);
    }

    public int WorkingLanguageId { get; set; }
    public IList<NewsItemModel> NewsItems { get; set; }
}