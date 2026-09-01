using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a measure search model
/// </summary>
public partial record MeasureSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MeasureSearchModel()
    {
		_logger.LogDebug(">>> Entering MeasureSearchModel()");
        MeasureDimensionSearchModel = new MeasureDimensionSearchModel();
		_logger.LogDebug("MeasureSearchModel(): MeasureDimensionSearchModel → {MeasureDimensionSearchModel}", MeasureDimensionSearchModel);
        MeasureWeightSearchModel = new MeasureWeightSearchModel();
		_logger.LogDebug("MeasureSearchModel(): MeasureWeightSearchModel → {MeasureWeightSearchModel}", MeasureWeightSearchModel);
        AddMeasureDimension = new MeasureDimensionModel();
		_logger.LogDebug("MeasureSearchModel(): AddMeasureDimension → {AddMeasureDimension}", AddMeasureDimension);
        AddMeasureWeight = new MeasureWeightModel();
    }

    #endregion

    #region Properties

    public MeasureDimensionSearchModel MeasureDimensionSearchModel { get; set; }

    public MeasureWeightSearchModel MeasureWeightSearchModel { get; set; }

    public MeasureDimensionModel AddMeasureDimension { get; set; }
    public MeasureWeightModel AddMeasureWeight { get; set; }
    #endregion
}