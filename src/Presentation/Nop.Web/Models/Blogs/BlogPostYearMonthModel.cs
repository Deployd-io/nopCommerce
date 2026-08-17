using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostYearModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostYearModel()
    {
		_logger.LogDebug(">>> Entering BlogPostYearModel()");
        Months = new List<BlogPostMonthModel>();
		_logger.LogDebug("<<< Exiting BlogPostYearModel()");
		_logger.LogDebug("BlogPostYearModel(): Months → {Months}", Months);
    }
    public int Year { get; set; }
    public IList<BlogPostMonthModel> Months { get; set; }
}

public partial record BlogPostMonthModel : BaseNopModel
{
    public int Month { get; set; }

    public int BlogPostCount { get; set; }
}