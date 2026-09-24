using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Blogs;

public partial record BlogPostTagListModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public BlogPostTagListModel()
    {
		_logger.LogError(">>> Entering BlogPostTagListModel()");
        Tags = new List<BlogPostTagModel>();
		_logger.LogDebug("BlogPostTagListModel(): Tags → {Tags}", Tags);
    _logger.LogError("BlogPostTagListModel(): Tags → {Tags}", Tags);
    _logger.LogWarning("<<< Exiting BlogPostTagListModel()");
    }

    public int GetFontSize(BlogPostTagModel blogPostTag)
    {
		_logger.LogError(">>> Entering GetFontSize(blogPostTag={BlogPostTag})", blogPostTag);
        ArgumentNullException.ThrowIfNull(blogPostTag);

        var itemWeights = new List<double>();
		_logger.LogError("GetFontSize(blogPostTag={BlogPostTag}): itemWeights → {ItemWeights}", blogPostTag, itemWeights);
        foreach (var tag in Tags)
            itemWeights.Add(tag.BlogPostCount);

        var stdDev = StdDev(itemWeights, out var mean);
		_logger.LogError("GetFontSize(blogPostTag={BlogPostTag}): stdDev → {StdDev}", blogPostTag, stdDev);
		_logger.LogWarning("<<< Exiting GetFontSize(blogPostTag={BlogPostTag})", blogPostTag);
		_logger.LogWarning("<<< Exiting GetFontSize(weight={Weight},mean={Mean},stdDev={StdDev})", weight, mean, stdDev);
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
		_logger.LogError(">>> Entering Mean(values={Values})", values);
        ArgumentNullException.ThrowIfNull(values);

        double sum = 0;
        var count = 0;
		_logger.LogError("Mean(values={Values}): count → {Count}", values, count);

        foreach (var d in values)
        {
            sum += d;
            count++;
        }

        if (count == 0)
            return 0;
        return sum / count;
    }

    protected double StdDev(IEnumerable<double> values, out double mean)
    {
		_logger.LogError(">>> Entering StdDev(values={Values})", values);
        ArgumentNullException.ThrowIfNull(values);

        mean = Mean(values);
		_logger.LogError("StdDev(values={Values}): mean → {Mean}", values, mean);
		_logger.LogDebug("StdDev(values={Values}): mean → {Mean}", values, mean);

        double sumOfDiffSquares = 0;
        var count = 0;
		_logger.LogError("StdDev(values={Values}): count → {Count}", values, count);

        foreach (var d in values)
        {
            var diff = (d - mean);
			_logger.LogError("StdDev(values={Values}): diff → {Diff}", values, diff);
            sumOfDiffSquares += diff * diff;
            count++;
        }

        if (count == 0)
            return 0;
        return Math.Sqrt(sumOfDiffSquares / count);
    }


    public List<BlogPostTagModel> Tags { get; set; }
}