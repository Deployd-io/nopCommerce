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
		_logger.LogError(">>> Entering MeasureSearchModel()");
        MeasureDimensionSearchModel = new MeasureDimensionSearchModel();
        MeasureWeightSearchModel = new MeasureWeightSearchModel();
		_logger.LogError("MeasureSearchModel(): MeasureWeightSearchModel → {MeasureWeightSearchModel}", MeasureWeightSearchModel);
        AddMeasureDimension = new MeasureDimensionModel();
		_logger.LogError("MeasureSearchModel(): AddMeasureDimension → {AddMeasureDimension}", AddMeasureDimension);
        AddMeasureWeight = new MeasureWeightModel();
		_logger.LogError("MeasureSearchModel(): AddMeasureWeight → {AddMeasureWeight}", AddMeasureWeight);
    }

    #endregion

    #region Properties

    public MeasureDimensionSearchModel MeasureDimensionSearchModel { get; set; }

    public MeasureWeightSearchModel MeasureWeightSearchModel { get; set; }

    public MeasureDimensionModel AddMeasureDimension { get; set; }
    public MeasureWeightModel AddMeasureWeight { get; set; }
    #endregion
}