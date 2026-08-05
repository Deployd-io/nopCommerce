using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;

/// <summary>
/// Tax rate service
/// </summary>
public class CountryStateZipService : ICountryStateZipService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<TaxRate> _taxRateRepository;
    protected readonly IShortTermCacheManager _shortTermCacheManager;

    #endregion

    #region Ctor

    public CountryStateZipService(IRepository<TaxRate> taxRateRepository,
        IShortTermCacheManager shortTermCacheManager)
    {
        _taxRateRepository = taxRateRepository;
        _shortTermCacheManager = shortTermCacheManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTaxRateAsync(TaxRate taxRate)
    {
		_logger.LogDebug(">>> Entering DeleteTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxRateRepository.DeleteAsync(taxRate);
		_logger.LogDebug("<<< Exiting DeleteTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteTaxRateAsync(taxRate)={TaxRate}: external service call took {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("DeleteTaxRateAsync(taxRate)={TaxRate}: delete query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("DeleteTaxRateAsync(taxRate)={TaxRate}: external service call took {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	_logger.LogInformation("DeleteTaxRateAsync(taxRate)={TaxRate}: delete query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all tax rates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax rates
    /// </returns>
    public virtual async Task<IPagedList<TaxRate>> GetAllTaxRatesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogDebug(">>> Entering GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await _shortTermCacheManager.GetAsync(async () => await _taxRateRepository.GetAllAsync(query => from tr in query
            orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
			var __logStart = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogInformation("GetAllTaxRatesAsync(pageIndex,pageSize)={PageIndex},{PageSize}: find query executed in {Arg2} ms", pageIndex, pageSize, __logStart.ElapsedMilliseconds);
            select tr), ModelCacheEventConsumer.TAXRATE_ALL_KEY);

        var records = new PagedList<TaxRate>(rez, pageIndex, pageSize);

		_logger.LogDebug("GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize}): records → {Records}", pageIndex, pageSize, records);
		_logger.LogInformation("GetAllTaxRatesAsync(pageIndex,pageSize)={PageIndex},{PageSize}: external service call took {Arg2} ms", pageIndex, pageSize, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        return records;
    }

    /// <summary>
    /// Gets a tax rate
    /// </summary>
    /// <param name="taxRateId">Tax rate identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax rate
    /// </returns>
    public virtual async Task<TaxRate> GetTaxRateByIdAsync(int taxRateId)
    {
		_logger.LogDebug(">>> Entering GetTaxRateByIdAsync(taxRateId={TaxRateId})", taxRateId);
		_logger.LogDebug("<<< Exiting GetTaxRateByIdAsync(taxRateId={TaxRateId})", taxRateId);
        return await _taxRateRepository.GetByIdAsync(taxRateId);
    }

    /// <summary>
    /// Inserts a tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTaxRateAsync(TaxRate taxRate)
    {
		_logger.LogDebug(">>> Entering InsertTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxRateRepository.InsertAsync(taxRate);
		_logger.LogDebug("<<< Exiting InsertTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("InsertTaxRateAsync(taxRate)={TaxRate}: save query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("InsertTaxRateAsync(taxRate)={TaxRate}: save query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTaxRateAsync(TaxRate taxRate)
    {
		_logger.LogDebug(">>> Entering UpdateTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxRateRepository.UpdateAsync(taxRate);
		_logger.LogDebug("<<< Exiting UpdateTaxRateAsync(taxRate={TaxRate})", taxRate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogInformation("UpdateTaxRateAsync(taxRate)={TaxRate}: update query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    }

	_logger.LogInformation("UpdateTaxRateAsync(taxRate)={TaxRate}: update query executed in {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    #endregion
}