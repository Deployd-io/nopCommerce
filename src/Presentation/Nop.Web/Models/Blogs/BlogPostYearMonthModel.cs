using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostYearModel : BaseNopModel
{
    public BlogPostYearModel()
    {
		logger.LogDebug(">>> Entering BlogPostYearModel()");
        Months = new List<BlogPostMonthModel>();
		logger.LogDebug("<<< Exiting BlogPostYearModel()");
		logger.LogDebug("BlogPostYearModel(): Months → {Months}", Months);
    }
    public int Year { get; set; }
    public IList<BlogPostMonthModel> Months { get; set; }
}

public partial record BlogPostMonthModel : BaseNopModel
{
    public int Month { get; set; }

    public int BlogPostCount { get; set; }
}