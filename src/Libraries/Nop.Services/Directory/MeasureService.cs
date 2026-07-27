using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;

namespace Nop.Services.Directory;

/// <summary>
/// Measure dimension service
/// </summary>
public partial class MeasureService : IMeasureService
{
    #region Fields

    protected readonly IRepository<MeasureDimension> _measureDimensionRepository;
    protected readonly IRepository<MeasureWeight> _measureWeightRepository;
    protected readonly MeasureSettings _measureSettings;

    #endregion

    #region Ctor

    public MeasureService(IRepository<MeasureDimension> measureDimensionRepository,
        IRepository<MeasureWeight> measureWeightRepository,
        MeasureSettings measureSettings)
    {
        _measureDimensionRepository = measureDimensionRepository;
        _measureWeightRepository = measureWeightRepository;
        _measureSettings = measureSettings;
    }

    #endregion

    #region Methods

    #region Dimensions

    /// <summary>
    /// Deletes measure dimension
    /// </summary>
    /// <param name="measureDimension">Measure dimension</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteMeasureDimensionAsync(MeasureDimension measureDimension)
    {
		logger.LogDebug(">>> Entering DeleteMeasureDimensionAsync(measureDimension={MeasureDimension})", measureDimension);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureDimensionRepository.DeleteAsync(measureDimension);
		logger.LogDebug("<<< Exiting DeleteMeasureDimensionAsync(measureDimension={MeasureDimension})", measureDimension);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: external service call took {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: delete query executed in {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: external service call took {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: delete query executed in {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a measure dimension by identifier
    /// </summary>
    /// <param name="measureDimensionId">Measure dimension identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure dimension
    /// </returns>
    public virtual async Task<MeasureDimension> GetMeasureDimensionByIdAsync(int measureDimensionId)
    {
		logger.LogDebug(">>> Entering GetMeasureDimensionByIdAsync(measureDimensionId={MeasureDimensionId})", measureDimensionId);
		logger.LogDebug("<<< Exiting GetMeasureDimensionByIdAsync(measureDimensionId={MeasureDimensionId})", measureDimensionId);
        return await _measureDimensionRepository.GetByIdAsync(measureDimensionId, cache => default);
    }

    /// <summary>
    /// Gets a measure dimension by system keyword
    /// </summary>
    /// <param name="systemKeyword">The system keyword</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure dimension
    /// </returns>
    public virtual async Task<MeasureDimension> GetMeasureDimensionBySystemKeywordAsync(string systemKeyword)
    {
		logger.LogDebug(">>> Entering GetMeasureDimensionBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
        if (string.IsNullOrEmpty(systemKeyword))
            return null;

        var measureDimensions = await GetAllMeasureDimensionsAsync();
        foreach (var measureDimension in measureDimensions)
            if (measureDimension.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
			logger.LogDebug("GetMeasureDimensionBySystemKeywordAsync(systemKeyword={SystemKeyword}): measureDimensions → {MeasureDimensions}", systemKeyword, measureDimensions);
                return measureDimension;
        return null;
    }

    /// <summary>
    /// Gets all measure dimensions
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure dimensions
    /// </returns>
    public virtual async Task<IList<MeasureDimension>> GetAllMeasureDimensionsAsync()
    {
		logger.LogDebug(">>> Entering GetAllMeasureDimensionsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var measureDimensions = await _measureDimensionRepository.GetAllAsync(query =>
        {
            return from md in query
                orderby md.DisplayOrder, md.Id
                select md;
				logger.LogInformation("GetAllMeasureDimensionsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllMeasureDimensionsAsync()");
        return measureDimensions;
    }

    /// <summary>
    /// Inserts a measure dimension
    /// </summary>
    /// <param name="measure">Measure dimension</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertMeasureDimensionAsync(MeasureDimension measure)
    {
		logger.LogDebug(">>> Entering InsertMeasureDimensionAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureDimensionRepository.InsertAsync(measure);
		logger.LogDebug("<<< Exiting InsertMeasureDimensionAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertMeasureDimensionAsync(measure)={Measure}: save query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertMeasureDimensionAsync(measure)={Measure}: save query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the measure dimension
    /// </summary>
    /// <param name="measure">Measure dimension</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMeasureDimensionAsync(MeasureDimension measure)
    {
		logger.LogDebug(">>> Entering UpdateMeasureDimensionAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureDimensionRepository.UpdateAsync(measure);
		logger.LogDebug("<<< Exiting UpdateMeasureDimensionAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateMeasureDimensionAsync(measure)={Measure}: update query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateMeasureDimensionAsync(measure)={Measure}: update query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Converts dimension
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="sourceMeasureDimension">Source dimension</param>
    /// <param name="targetMeasureDimension">Target dimension</param>
    /// <param name="round">A value indicating whether a result should be rounded</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertDimensionAsync(decimal value,
        MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension, bool round = true)
    {
        ArgumentNullException.ThrowIfNull(sourceMeasureDimension);

        ArgumentNullException.ThrowIfNull(targetMeasureDimension);

        var result = value;
        if (result != decimal.Zero && sourceMeasureDimension.Id != targetMeasureDimension.Id)
        {
            result = await ConvertToPrimaryMeasureDimensionAsync(result, sourceMeasureDimension);
            result = await ConvertFromPrimaryMeasureDimensionAsync(result, targetMeasureDimension);
        }

        if (round)
            result = Math.Round(result, 2);

        return result;
    }

    /// <summary>
    /// Converts from primary dimension
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetMeasureDimension">Target dimension</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertFromPrimaryMeasureDimensionAsync(decimal value,
        MeasureDimension targetMeasureDimension)
    {
        ArgumentNullException.ThrowIfNull(targetMeasureDimension);

        var result = value;
        var baseDimensionIn = await GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
        if (result == decimal.Zero || targetMeasureDimension.Id == baseDimensionIn.Id)
            return result;

        var exchangeRatio = targetMeasureDimension.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for dimension [{targetMeasureDimension.Name}]");
        result *= exchangeRatio;

        return result;
    }

    /// <summary>
    /// Converts to primary measure dimension
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="sourceMeasureDimension">Source dimension</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertToPrimaryMeasureDimensionAsync(decimal value,
        MeasureDimension sourceMeasureDimension)
    {
        ArgumentNullException.ThrowIfNull(sourceMeasureDimension);

        var result = value;
        var baseDimensionIn = await GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
        if (result == decimal.Zero || sourceMeasureDimension.Id == baseDimensionIn.Id)
            return result;

        var exchangeRatio = sourceMeasureDimension.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for dimension [{sourceMeasureDimension.Name}]");
        result /= exchangeRatio;

        return result;
    }


    #endregion

    #region Weights

    /// <summary>
    /// Deletes measure weight
    /// </summary>
    /// <param name="measureWeight">Measure weight</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteMeasureWeightAsync(MeasureWeight measureWeight)
    {
		logger.LogDebug(">>> Entering DeleteMeasureWeightAsync(measureWeight={MeasureWeight})", measureWeight);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureWeightRepository.DeleteAsync(measureWeight);
		logger.LogDebug("<<< Exiting DeleteMeasureWeightAsync(measureWeight={MeasureWeight})", measureWeight);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: external service call took {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: delete query executed in {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: external service call took {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: delete query executed in {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets a measure weight by identifier
    /// </summary>
    /// <param name="measureWeightId">Measure weight identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure weight
    /// </returns>
    public virtual async Task<MeasureWeight> GetMeasureWeightByIdAsync(int measureWeightId)
    {
		logger.LogDebug(">>> Entering GetMeasureWeightByIdAsync(measureWeightId={MeasureWeightId})", measureWeightId);
		logger.LogDebug("<<< Exiting GetMeasureWeightByIdAsync(measureWeightId={MeasureWeightId})", measureWeightId);
        return await _measureWeightRepository.GetByIdAsync(measureWeightId, cache => default);
    }

    /// <summary>
    /// Gets a measure weight by system keyword
    /// </summary>
    /// <param name="systemKeyword">The system keyword</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure weight
    /// </returns>
    public virtual async Task<MeasureWeight> GetMeasureWeightBySystemKeywordAsync(string systemKeyword)
    {
		logger.LogDebug(">>> Entering GetMeasureWeightBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
        if (string.IsNullOrEmpty(systemKeyword))
            return null;

        var measureWeights = await GetAllMeasureWeightsAsync();
        foreach (var measureWeight in measureWeights)
            if (measureWeight.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
			logger.LogDebug("GetMeasureWeightBySystemKeywordAsync(systemKeyword={SystemKeyword}): measureWeights → {MeasureWeights}", systemKeyword, measureWeights);
                return measureWeight;
        return null;
    }

    /// <summary>
    /// Gets all measure weights
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the measure weights
    /// </returns>
    public virtual async Task<IList<MeasureWeight>> GetAllMeasureWeightsAsync()
    {
		logger.LogDebug(">>> Entering GetAllMeasureWeightsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var measureWeights = await _measureWeightRepository.GetAllAsync(query =>
        {
            return from mw in query
                orderby mw.DisplayOrder, mw.Id
                select mw;
				logger.LogInformation("GetAllMeasureWeightsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllMeasureWeightsAsync()");
        return measureWeights;
    }

    /// <summary>
    /// Inserts a measure weight
    /// </summary>
    /// <param name="measure">Measure weight</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertMeasureWeightAsync(MeasureWeight measure)
    {
		logger.LogDebug(">>> Entering InsertMeasureWeightAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureWeightRepository.InsertAsync(measure);
		logger.LogDebug("<<< Exiting InsertMeasureWeightAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertMeasureWeightAsync(measure)={Measure}: save query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertMeasureWeightAsync(measure)={Measure}: save query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the measure weight
    /// </summary>
    /// <param name="measure">Measure weight</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMeasureWeightAsync(MeasureWeight measure)
    {
		logger.LogDebug(">>> Entering UpdateMeasureWeightAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureWeightRepository.UpdateAsync(measure);
		logger.LogDebug("<<< Exiting UpdateMeasureWeightAsync(measure={Measure})", measure);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateMeasureWeightAsync(measure)={Measure}: update query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateMeasureWeightAsync(measure)={Measure}: update query executed in {Arg1} ms", measure, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Converts weight
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="sourceMeasureWeight">Source weight</param>
    /// <param name="targetMeasureWeight">Target weight</param>
    /// <param name="round">A value indicating whether a result should be rounded</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertWeightAsync(decimal value,
        MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight, bool round = true)
    {
        ArgumentNullException.ThrowIfNull(sourceMeasureWeight);

        ArgumentNullException.ThrowIfNull(targetMeasureWeight);

        var result = value;
        if (result != decimal.Zero && sourceMeasureWeight.Id != targetMeasureWeight.Id)
        {
            result = await ConvertToPrimaryMeasureWeightAsync(result, sourceMeasureWeight);
            result = await ConvertFromPrimaryMeasureWeightAsync(result, targetMeasureWeight);
        }

        if (round)
            result = Math.Round(result, 2);

        return result;
    }

    /// <summary>
    /// Converts from primary weight
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="targetMeasureWeight">Target weight</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertFromPrimaryMeasureWeightAsync(decimal value,
        MeasureWeight targetMeasureWeight)
    {
        ArgumentNullException.ThrowIfNull(targetMeasureWeight);

        var result = value;
        var baseWeightIn = await GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
        if (result == decimal.Zero || targetMeasureWeight.Id == baseWeightIn.Id)
            return result;

        var exchangeRatio = targetMeasureWeight.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for weight [{targetMeasureWeight.Name}]");
        result *= exchangeRatio;

        return result;
    }

    /// <summary>
    /// Converts to primary measure weight
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="sourceMeasureWeight">Source weight</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the converted value
    /// </returns>
    public virtual async Task<decimal> ConvertToPrimaryMeasureWeightAsync(decimal value, MeasureWeight sourceMeasureWeight)
    {
		logger.LogDebug(">>> Entering ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight})", value, sourceMeasureWeight);
        ArgumentNullException.ThrowIfNull(sourceMeasureWeight);

        var result = value;
        var baseWeightIn = await GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
		logger.LogDebug("ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight}): result → {Result}", value, sourceMeasureWeight, result);
		logger.LogDebug("ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight}): baseWeightIn → {BaseWeightIn}", value, sourceMeasureWeight, baseWeightIn);
        if (result == decimal.Zero || sourceMeasureWeight.Id == baseWeightIn.Id)
            return result;

        var exchangeRatio = sourceMeasureWeight.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for weight [{sourceMeasureWeight.Name}]");
        result /= exchangeRatio;

		logger.LogDebug("<<< Exiting ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight})", value, sourceMeasureWeight);
        return result;
    }

    #endregion

    #endregion
}