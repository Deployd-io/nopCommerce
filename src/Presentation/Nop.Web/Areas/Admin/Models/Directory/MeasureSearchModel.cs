using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a measure search model
/// </summary>
public partial record MeasureSearchModel : BaseSearchModel
{
    #region Ctor

    public MeasureSearchModel()
    {
		logger.LogDebug(">>> Entering MeasureSearchModel()");
        MeasureDimensionSearchModel = new MeasureDimensionSearchModel();
        MeasureWeightSearchModel = new MeasureWeightSearchModel();
		logger.LogDebug("MeasureSearchModel(): MeasureDimensionSearchModel → {MeasureDimensionSearchModel}", MeasureDimensionSearchModel);
		logger.LogDebug("MeasureSearchModel(): MeasureWeightSearchModel → {MeasureWeightSearchModel}", MeasureWeightSearchModel);
        AddMeasureDimension = new MeasureDimensionModel();
        AddMeasureWeight = new MeasureWeightModel();
		logger.LogDebug("<<< Exiting MeasureSearchModel()");
		logger.LogDebug("MeasureSearchModel(): AddMeasureDimension → {AddMeasureDimension}", AddMeasureDimension);
		logger.LogDebug("MeasureSearchModel(): AddMeasureWeight → {AddMeasureWeight}", AddMeasureWeight);
    }

    #endregion

    #region Properties

    public MeasureDimensionSearchModel MeasureDimensionSearchModel { get; set; }

    public MeasureWeightSearchModel MeasureWeightSearchModel { get; set; }

    public MeasureDimensionModel AddMeasureDimension { get; set; }
    public MeasureWeightModel AddMeasureWeight { get; set; }
    #endregion
}