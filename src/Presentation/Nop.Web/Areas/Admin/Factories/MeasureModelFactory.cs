using Nop.Core.Domain.Directory;
using Nop.Services.Directory;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the measures model factory implementation
/// </summary>
public partial class MeasureModelFactory : IMeasureModelFactory
{
    #region Fields

    protected readonly IMeasureService _measureService;
    protected readonly MeasureSettings _measureSettings;

    #endregion

    #region Ctor

    public MeasureModelFactory(IMeasureService measureService,
        MeasureSettings measureSettings)
    {
        _measureService = measureService;
        _measureSettings = measureSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare measure dimension search model
    /// </summary>
    /// <param name="searchModel">Measure dimension search model</param>
    /// <returns>Measure dimension search model</returns>
    protected virtual MeasureDimensionSearchModel PrepareMeasureDimensionSearchModel(MeasureDimensionSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareMeasureDimensionSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareMeasureDimensionSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare measure weight search model
    /// </summary>
    /// <param name="searchModel">Measure weight search model</param>
    /// <returns>Measure weight search model</returns>
    protected virtual MeasureWeightSearchModel PrepareMeasureWeightSearchModel(MeasureWeightSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareMeasureWeightSearchModel(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

		logger.LogDebug("<<< Exiting PrepareMeasureWeightSearchModel(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare measure search model
    /// </summary>
    /// <param name="searchModel">Measure search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure search model
    /// </returns>
    public virtual Task<MeasureSearchModel> PrepareMeasureSearchModelAsync(MeasureSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareMeasureSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare nested search models
        PrepareMeasureDimensionSearchModel(searchModel.MeasureDimensionSearchModel);
        PrepareMeasureWeightSearchModel(searchModel.MeasureWeightSearchModel);

		logger.LogDebug("<<< Exiting PrepareMeasureSearchModelAsync(searchModel={SearchModel})", searchModel);
        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged measure dimension list model
    /// </summary>
    /// <param name="searchModel">Measure dimension search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure dimension list model
    /// </returns>
    public virtual async Task<MeasureDimensionListModel> PrepareMeasureDimensionListModelAsync(MeasureDimensionSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareMeasureDimensionListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get dimensions
        var dimensions = (await _measureService.GetAllMeasureDimensionsAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareMeasureDimensionListModelAsync(searchModel={SearchModel}): dimensions → {Dimensions}", searchModel, dimensions);
        //prepare list model
        var model = new MeasureDimensionListModel().PrepareToGrid(searchModel, dimensions, () =>
        {
            return dimensions.Select(dimension =>
            {
                //fill in model values from the entity
                var dimensionModel = dimension.ToModel<MeasureDimensionModel>();

				logger.LogDebug("PrepareMeasureDimensionListModelAsync(searchModel={SearchModel}): dimensionModel → {DimensionModel}", searchModel, dimensionModel);
                //fill in additional values (not existing in the entity)
                dimensionModel.IsPrimaryDimension = dimension.Id == _measureSettings.BaseDimensionId;

                return dimensionModel;
            });
        });

		logger.LogDebug("<<< Exiting PrepareMeasureDimensionListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare paged measure weight list model
    /// </summary>
    /// <param name="searchModel">Measure weight search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure weight list model
    /// </returns>
    public virtual async Task<MeasureWeightListModel> PrepareMeasureWeightListModelAsync(MeasureWeightSearchModel searchModel)
    {
		logger.LogDebug(">>> Entering PrepareMeasureWeightListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get weights
        var weights = (await _measureService.GetAllMeasureWeightsAsync()).ToPagedList(searchModel);

		logger.LogDebug("PrepareMeasureWeightListModelAsync(searchModel={SearchModel}): weights → {Weights}", searchModel, weights);
        //prepare list model
        var model = new MeasureWeightListModel().PrepareToGrid(searchModel, weights, () =>
        {
            return weights.Select(weight =>
            {
                //fill in model values from the entity
                var weightModel = weight.ToModel<MeasureWeightModel>();

				logger.LogDebug("PrepareMeasureWeightListModelAsync(searchModel={SearchModel}): weightModel → {WeightModel}", searchModel, weightModel);
                //fill in additional values (not existing in the entity)
                weightModel.IsPrimaryWeight = weight.Id == _measureSettings.BaseWeightId;

                return weightModel;
            });
        });

		logger.LogDebug("<<< Exiting PrepareMeasureWeightListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    #endregion
}
