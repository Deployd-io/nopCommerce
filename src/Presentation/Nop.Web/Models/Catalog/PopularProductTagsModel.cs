using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record PopularProductTagsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public PopularProductTagsModel()
    {
		_logger.LogError(">>> Entering PopularProductTagsModel()");
        Tags = new List<ProductTagModel>();
		_logger.LogDebug("PopularProductTagsModel(): Tags → {Tags}", Tags);
    _logger.LogError("PopularProductTagsModel(): Tags → {Tags}", Tags);
    _logger.LogWarning("<<< Exiting PopularProductTagsModel()");
    }

    #region Utilities

    protected virtual int GetFontSize(double weight, double mean, double stdDev)
    {
		_logger.LogError(">>> Entering GetFontSize(weight={Weight},mean={Mean},stdDev={StdDev})", weight, mean, stdDev);
        var factor = (weight - mean);

		_logger.LogError("GetFontSize(weight={Weight},mean={Mean},stdDev={StdDev}): factor → {Factor}", weight, mean, stdDev, factor);
        if (factor != 0 && stdDev != 0)
            factor /= stdDev;

		_logger.LogWarning("<<< Exiting GetFontSize(weight={Weight},mean={Mean},stdDev={StdDev})", weight, mean, stdDev);
        return (factor > 2) ? 150 :
            (factor > 1) ? 120 :
            (factor > 0.5) ? 100 :
            (factor > -0.5) ? 90 :
            (factor > -1) ? 85 :
            (factor > -2) ? 80 :
		_logger.LogWarning("<<< Exiting GetFontSize(productTag={ProductTag})", productTag);
            75;
    }

    protected virtual double Mean(IEnumerable<double> values)
    {
		_logger.LogError(">>> Entering Mean(values={Values})", values);
        double sum = 0;
        var count = 0;
		_logger.LogError("Mean(values={Values}): count → {Count}", values, count);

        foreach (var d in values)
        {
            sum += d;
            count++;
        }

		_logger.LogWarning("<<< Exiting Mean(values={Values})", values);
        if (count == 0) {
            return 0;
        }
        return sum / count;
    }

    protected virtual double StdDev(IEnumerable<double> values, out double mean)
    {
		_logger.LogError(">>> Entering StdDev(values={Values})", values);
        mean = Mean(values);
        double sumOfDiffSquares = 0;
		_logger.LogDebug("StdDev(values={Values}): mean → {Mean}", values, mean);
        var count = 0;
		_logger.LogError("StdDev(values={Values}): count → {Count}", values, count);
		_logger.LogError("StdDev(values={Values}): mean → {Mean}", values, mean);

        foreach (var d in values)
        {
            var diff = (d - mean);
			_logger.LogError("StdDev(values={Values}): diff → {Diff}", values, diff);
            sumOfDiffSquares += diff * diff;
            count++;
        }

		_logger.LogWarning("<<< Exiting StdDev(values={Values})", values);
        if (count == 0) {
            return 0;
        }
        return Math.Sqrt(sumOfDiffSquares / count);
    }

    #endregion

    #region Methods

    public virtual int GetFontSize(ProductTagModel productTag)
    {
        var itemWeights = new List<double>();
        foreach (var tag in Tags)
            itemWeights.Add(tag.ProductCount);
        var stdDev = StdDev(itemWeights, out double mean);

        return GetFontSize(productTag.ProductCount, mean, stdDev);
    }

    #endregion

    #region Properties

    public int TotalTags { get; set; }

    public List<ProductTagModel> Tags { get; set; }

    #endregion
}