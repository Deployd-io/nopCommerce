using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Directory;

/// <summary>
/// Measure dimension service
/// </summary>
public partial class MeasureService : IMeasureService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
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
		_logger.LogDebug(">>> Entering MeasureService(measureDimensionRepository={MeasureDimensionRepository},measureWeightRepository={MeasureWeightRepository},measureSettings={MeasureSettings})", measureDimensionRepository, measureWeightRepository, measureSettings);
        _measureDimensionRepository = measureDimensionRepository;
        _measureWeightRepository = measureWeightRepository;
		_logger.LogDebug("MeasureService(measureDimensionRepository={MeasureDimensionRepository},measureWeightRepository={MeasureWeightRepository},measureSettings={MeasureSettings}): _measureWeightRepository → {_measureWeightRepository}", measureDimensionRepository, measureWeightRepository, measureSettings, _measureWeightRepository);
		_logger.LogDebug("MeasureService(measureDimensionRepository={MeasureDimensionRepository},measureWeightRepository={MeasureWeightRepository},measureSettings={MeasureSettings}): _measureDimensionRepository → {_measureDimensionRepository}", measureDimensionRepository, measureWeightRepository, measureSettings, _measureDimensionRepository);
        _measureSettings = measureSettings;
		_logger.LogDebug("<<< Exiting MeasureService(measureDimensionRepository={MeasureDimensionRepository},measureWeightRepository={MeasureWeightRepository},measureSettings={MeasureSettings})", measureDimensionRepository, measureWeightRepository, measureSettings);
		_logger.LogDebug("MeasureService(measureDimensionRepository={MeasureDimensionRepository},measureWeightRepository={MeasureWeightRepository},measureSettings={MeasureSettings}): _measureSettings → {_measureSettings}", measureDimensionRepository, measureWeightRepository, measureSettings, _measureSettings);
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
		_logger.LogDebug(">>> Entering DeleteMeasureDimensionAsync(measureDimension={MeasureDimension})", measureDimension);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureDimensionRepository.DeleteAsync(measureDimension);
		_logger.LogDebug("<<< Exiting DeleteMeasureDimensionAsync(measureDimension={MeasureDimension})", measureDimension);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: external service call took {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteMeasureDimensionAsync(measureDimension)={MeasureDimension}: external service call took {Arg1} ms", measureDimension, __logStart.ElapsedMilliseconds);
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
		_logger.LogDebug(">>> Entering GetMeasureDimensionByIdAsync(measureDimensionId={MeasureDimensionId})", measureDimensionId);
		_logger.LogDebug("<<< Exiting GetMeasureDimensionByIdAsync(measureDimensionId={MeasureDimensionId})", measureDimensionId);
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
		_logger.LogDebug(">>> Entering GetMeasureDimensionBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
        if (string.IsNullOrEmpty(systemKeyword))
            return null;

        var measureDimensions = await GetAllMeasureDimensionsAsync();
		_logger.LogDebug("GetMeasureDimensionBySystemKeywordAsync(systemKeyword={SystemKeyword}): measureDimensions → {MeasureDimensions}", systemKeyword, measureDimensions);
        foreach (var measureDimension in measureDimensions)
            if (measureDimension.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
		_logger.LogDebug("<<< Exiting GetMeasureDimensionBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
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
		_logger.LogDebug(">>> Entering GetAllMeasureDimensionsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var measureDimensions = await _measureDimensionRepository.GetAllAsync(query =>
        {
            return from md in query
                orderby md.DisplayOrder, md.Id
                select md;
				_logger.LogInformation("GetAllMeasureDimensionsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllMeasureDimensionsAsync()");
        return measureDimensions;
    }

    /// <summary>
    /// Inserts a measure dimension
    /// </summary>
    /// <param name="measure">Measure dimension</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertMeasureDimensionAsync(MeasureDimension measure)
    {
		_logger.LogDebug(">>> Entering InsertMeasureDimensionAsync(measure={Measure})", measure);
        await _measureDimensionRepository.InsertAsync(measure);
		_logger.LogDebug("<<< Exiting InsertMeasureDimensionAsync(measure={Measure})", measure);
    }

    /// <summary>
    /// Updates the measure dimension
    /// </summary>
    /// <param name="measure">Measure dimension</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMeasureDimensionAsync(MeasureDimension measure)
    {
		_logger.LogDebug(">>> Entering UpdateMeasureDimensionAsync(measure={Measure})", measure);
        await _measureDimensionRepository.UpdateAsync(measure);
		_logger.LogDebug("<<< Exiting UpdateMeasureDimensionAsync(measure={Measure})", measure);
    }

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
		_logger.LogDebug(">>> Entering ConvertDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension},targetMeasureDimension={TargetMeasureDimension},round={Round})", value, sourceMeasureDimension, targetMeasureDimension, round);
        ArgumentNullException.ThrowIfNull(sourceMeasureDimension);

        ArgumentNullException.ThrowIfNull(targetMeasureDimension);

        var result = value;
		_logger.LogDebug("ConvertDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension},targetMeasureDimension={TargetMeasureDimension},round={Round}): result → {Result}", value, sourceMeasureDimension, targetMeasureDimension, round, result);
        if (result != decimal.Zero && sourceMeasureDimension.Id != targetMeasureDimension.Id)
        {
            result = await ConvertToPrimaryMeasureDimensionAsync(result, sourceMeasureDimension);
            result = await ConvertFromPrimaryMeasureDimensionAsync(result, targetMeasureDimension);
        }

        if (round)
            result = Math.Round(result, 2);

		_logger.LogDebug("<<< Exiting ConvertDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension},targetMeasureDimension={TargetMeasureDimension},round={Round})", value, sourceMeasureDimension, targetMeasureDimension, round);
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
		_logger.LogDebug(">>> Entering ConvertFromPrimaryMeasureDimensionAsync(value={Value},targetMeasureDimension={TargetMeasureDimension})", value, targetMeasureDimension);
        ArgumentNullException.ThrowIfNull(targetMeasureDimension);

        var result = value;
		_logger.LogDebug("ConvertFromPrimaryMeasureDimensionAsync(value={Value},targetMeasureDimension={TargetMeasureDimension}): result → {Result}", value, targetMeasureDimension, result);
        var baseDimensionIn = await GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
		_logger.LogDebug("ConvertFromPrimaryMeasureDimensionAsync(value={Value},targetMeasureDimension={TargetMeasureDimension}): baseDimensionIn → {BaseDimensionIn}", value, targetMeasureDimension, baseDimensionIn);
        if (result == decimal.Zero || targetMeasureDimension.Id == baseDimensionIn.Id)
            return result;

        var exchangeRatio = targetMeasureDimension.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for dimension [{targetMeasureDimension.Name}]");
        result *= exchangeRatio;

		_logger.LogDebug("<<< Exiting ConvertFromPrimaryMeasureDimensionAsync(value={Value},targetMeasureDimension={TargetMeasureDimension})", value, targetMeasureDimension);
		_logger.LogDebug(">>> Entering ConvertToPrimaryMeasureDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension})", value, sourceMeasureDimension);
		_logger.LogDebug("<<< Exiting ConvertToPrimaryMeasureDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension})", value, sourceMeasureDimension);
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
		_logger.LogDebug("ConvertToPrimaryMeasureDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension}): result → {Result}", value, sourceMeasureDimension, result);
        var baseDimensionIn = await GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId);
		_logger.LogDebug("ConvertToPrimaryMeasureDimensionAsync(value={Value},sourceMeasureDimension={SourceMeasureDimension}): baseDimensionIn → {BaseDimensionIn}", value, sourceMeasureDimension, baseDimensionIn);
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
		_logger.LogDebug(">>> Entering DeleteMeasureWeightAsync(measureWeight={MeasureWeight})", measureWeight);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _measureWeightRepository.DeleteAsync(measureWeight);
		_logger.LogDebug("<<< Exiting DeleteMeasureWeightAsync(measureWeight={MeasureWeight})", measureWeight);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: external service call took {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteMeasureWeightAsync(measureWeight)={MeasureWeight}: external service call took {Arg1} ms", measureWeight, __logStart.ElapsedMilliseconds);
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
		_logger.LogDebug(">>> Entering GetMeasureWeightByIdAsync(measureWeightId={MeasureWeightId})", measureWeightId);
		_logger.LogDebug("<<< Exiting GetMeasureWeightByIdAsync(measureWeightId={MeasureWeightId})", measureWeightId);
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
		_logger.LogDebug(">>> Entering GetMeasureWeightBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
        if (string.IsNullOrEmpty(systemKeyword))
            return null;

        var measureWeights = await GetAllMeasureWeightsAsync();
		_logger.LogDebug("GetMeasureWeightBySystemKeywordAsync(systemKeyword={SystemKeyword}): measureWeights → {MeasureWeights}", systemKeyword, measureWeights);
        foreach (var measureWeight in measureWeights)
            if (measureWeight.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
		_logger.LogDebug("<<< Exiting GetMeasureWeightBySystemKeywordAsync(systemKeyword={SystemKeyword})", systemKeyword);
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
		_logger.LogDebug(">>> Entering GetAllMeasureWeightsAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var measureWeights = await _measureWeightRepository.GetAllAsync(query =>
        {
            return from mw in query
                orderby mw.DisplayOrder, mw.Id
                select mw;
				_logger.LogInformation("GetAllMeasureWeightsAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllMeasureWeightsAsync()");
        return measureWeights;
    }

    /// <summary>
    /// Inserts a measure weight
    /// </summary>
    /// <param name="measure">Measure weight</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertMeasureWeightAsync(MeasureWeight measure)
    {
		_logger.LogDebug(">>> Entering InsertMeasureWeightAsync(measure={Measure})", measure);
        await _measureWeightRepository.InsertAsync(measure);
		_logger.LogDebug("<<< Exiting InsertMeasureWeightAsync(measure={Measure})", measure);
    }

    /// <summary>
    /// Updates the measure weight
    /// </summary>
    /// <param name="measure">Measure weight</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateMeasureWeightAsync(MeasureWeight measure)
    {
		_logger.LogDebug(">>> Entering UpdateMeasureWeightAsync(measure={Measure})", measure);
        await _measureWeightRepository.UpdateAsync(measure);
		_logger.LogDebug("<<< Exiting UpdateMeasureWeightAsync(measure={Measure})", measure);
    }

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
		_logger.LogDebug(">>> Entering ConvertWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight},targetMeasureWeight={TargetMeasureWeight},round={Round})", value, sourceMeasureWeight, targetMeasureWeight, round);
        ArgumentNullException.ThrowIfNull(sourceMeasureWeight);

        ArgumentNullException.ThrowIfNull(targetMeasureWeight);

        var result = value;
		_logger.LogDebug("ConvertWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight},targetMeasureWeight={TargetMeasureWeight},round={Round}): result → {Result}", value, sourceMeasureWeight, targetMeasureWeight, round, result);
        if (result != decimal.Zero && sourceMeasureWeight.Id != targetMeasureWeight.Id)
        {
            result = await ConvertToPrimaryMeasureWeightAsync(result, sourceMeasureWeight);
            result = await ConvertFromPrimaryMeasureWeightAsync(result, targetMeasureWeight);
        }

        if (round)
            result = Math.Round(result, 2);

		_logger.LogDebug("<<< Exiting ConvertWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight},targetMeasureWeight={TargetMeasureWeight},round={Round})", value, sourceMeasureWeight, targetMeasureWeight, round);
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
		_logger.LogDebug(">>> Entering ConvertFromPrimaryMeasureWeightAsync(value={Value},targetMeasureWeight={TargetMeasureWeight})", value, targetMeasureWeight);
        ArgumentNullException.ThrowIfNull(targetMeasureWeight);

        var result = value;
		_logger.LogDebug("ConvertFromPrimaryMeasureWeightAsync(value={Value},targetMeasureWeight={TargetMeasureWeight}): result → {Result}", value, targetMeasureWeight, result);
        var baseWeightIn = await GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
		_logger.LogDebug("ConvertFromPrimaryMeasureWeightAsync(value={Value},targetMeasureWeight={TargetMeasureWeight}): baseWeightIn → {BaseWeightIn}", value, targetMeasureWeight, baseWeightIn);
        if (result == decimal.Zero || targetMeasureWeight.Id == baseWeightIn.Id)
            return result;

        var exchangeRatio = targetMeasureWeight.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for weight [{targetMeasureWeight.Name}]");
        result *= exchangeRatio;

		_logger.LogDebug("<<< Exiting ConvertFromPrimaryMeasureWeightAsync(value={Value},targetMeasureWeight={TargetMeasureWeight})", value, targetMeasureWeight);
		_logger.LogDebug(">>> Entering ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight})", value, sourceMeasureWeight);
		_logger.LogDebug("<<< Exiting ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight})", value, sourceMeasureWeight);
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
        ArgumentNullException.ThrowIfNull(sourceMeasureWeight);

        var result = value;
		_logger.LogDebug("ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight}): result → {Result}", value, sourceMeasureWeight, result);
        var baseWeightIn = await GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId);
		_logger.LogDebug("ConvertToPrimaryMeasureWeightAsync(value={Value},sourceMeasureWeight={SourceMeasureWeight}): baseWeightIn → {BaseWeightIn}", value, sourceMeasureWeight, baseWeightIn);
        if (result == decimal.Zero || sourceMeasureWeight.Id == baseWeightIn.Id)
            return result;

        var exchangeRatio = sourceMeasureWeight.Ratio;
        if (exchangeRatio == decimal.Zero)
            throw new NopException($"Exchange ratio not set for weight [{sourceMeasureWeight.Name}]");
        result /= exchangeRatio;

        return result;
    }

    #endregion

    #endregion
}