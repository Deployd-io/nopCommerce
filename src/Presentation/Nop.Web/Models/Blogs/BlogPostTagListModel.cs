using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostTagListModel : BaseNopModel
{
    public BlogPostTagListModel()
    {
		logger.LogDebug(">>> Entering BlogPostTagListModel()");
        Tags = new List<BlogPostTagModel>();
		logger.LogDebug("<<< Exiting BlogPostTagListModel()");
		logger.LogDebug("BlogPostTagListModel(): Tags → {Tags}", Tags);
    }

    public int GetFontSize(BlogPostTagModel blogPostTag)
    {
		logger.LogDebug(">>> Entering GetFontSize(blogPostTag={BlogPostTag})", blogPostTag);
        ArgumentNullException.ThrowIfNull(blogPostTag);

        var itemWeights = new List<double>();
        foreach (var tag in Tags)
            itemWeights.Add(tag.BlogPostCount);
			logger.LogDebug("GetFontSize(blogPostTag={BlogPostTag}): itemWeights → {ItemWeights}", blogPostTag, itemWeights);

        var stdDev = StdDev(itemWeights, out var mean);
		logger.LogDebug("GetFontSize(blogPostTag={BlogPostTag}): stdDev → {StdDev}", blogPostTag, stdDev);
		logger.LogDebug("<<< Exiting GetFontSize(blogPostTag={BlogPostTag})", blogPostTag);
        return GetFontSize(blogPostTag.BlogPostCount, mean, stdDev);
    }

    protected int GetFontSize(double weight, double mean, double stdDev)
    {
        var factor = (weight - mean);

        if (factor != 0 && stdDev != 0)
            factor /= stdDev;

        return (factor > 2) ? 150 :
            (factor > 1) ? 120 :
            (factor > 0.5) ? 100 :
            (factor > -0.5) ? 90 :
            (factor > -1) ? 85 :
            (factor > -2) ? 80 :
            75;
    }

    protected double Mean(IEnumerable<double> values)
    {
		logger.LogDebug(">>> Entering Mean(values={Values})", values);
        ArgumentNullException.ThrowIfNull(values);

        double sum = 0;
        var count = 0;

		logger.LogDebug("Mean(values={Values}): count → {Count}", values, count);
        foreach (var d in values)
        {
            sum += d;
            count++;
        }

        if (count == 0)
		logger.LogDebug("<<< Exiting Mean(values={Values})", values);
            return 0;
        return sum / count;
    }

    protected double StdDev(IEnumerable<double> values, out double mean)
    {
		logger.LogDebug(">>> Entering StdDev(values={Values})", values);
        ArgumentNullException.ThrowIfNull(values);

        mean = Mean(values);
		logger.LogDebug("StdDev(values={Values}): mean → {Mean}", values, mean);

        double sumOfDiffSquares = 0;
        var count = 0;

		logger.LogDebug("StdDev(values={Values}): count → {Count}", values, count);
        foreach (var d in values)
        {
            var diff = (d - mean);
            sumOfDiffSquares += diff * diff;
			logger.LogDebug("StdDev(values={Values}): diff → {Diff}", values, diff);
            count++;
        }

        if (count == 0)
		logger.LogDebug("<<< Exiting StdDev(values={Values})", values);
            return 0;
        return Math.Sqrt(sumOfDiffSquares / count);
    }


    public List<BlogPostTagModel> Tags { get; set; }
}