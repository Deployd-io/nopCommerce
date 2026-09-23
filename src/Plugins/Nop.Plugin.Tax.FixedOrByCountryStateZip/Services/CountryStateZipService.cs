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
		_logger.LogError(">>> Entering CountryStateZipService(taxRateRepository={TaxRateRepository},shortTermCacheManager={ShortTermCacheManager})", taxRateRepository, shortTermCacheManager);
        _taxRateRepository = taxRateRepository;
        _shortTermCacheManager = shortTermCacheManager;
		_logger.LogDebug("CountryStateZipService(taxRateRepository={TaxRateRepository},shortTermCacheManager={ShortTermCacheManager}): _shortTermCacheManager → {_shortTermCacheManager}", taxRateRepository, shortTermCacheManager, _shortTermCacheManager);
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
		_logger.LogError(">>> Entering DeleteTaxRateAsync(taxRate={TaxRate})", taxRate);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxRateRepository.DeleteAsync(taxRate);
		_logger.LogError("DeleteTaxRateAsync(taxRate)={TaxRate}: external service call took {Arg1} ms", taxRate, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteTaxRateAsync(taxRate={TaxRate})", taxRate);
    }

    /// <summary>
    /// Gets all tax rates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax rates
    /// </returns>
    public virtual async Task<IPagedList<TaxRate>> GetAllTaxRatesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
		_logger.LogError(">>> Entering GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var rez = await _shortTermCacheManager.GetAsync(async () => await _taxRateRepository.GetAllAsync(query => from tr in query
            orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
            select tr), ModelCacheEventConsumer.TAXRATE_ALL_KEY);
			var __logStartDb = System.Diagnostics.Stopwatch.StartNew();
			_logger.LogError("GetAllTaxRatesAsync(pageIndex,pageSize)={PageIndex},{PageSize}: find query executed in {Arg2} ms", pageIndex, pageSize, __logStartDb.ElapsedMilliseconds);

        var records = new PagedList<TaxRate>(rez, pageIndex, pageSize);
		_logger.LogError("GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize}): records → {Records}", pageIndex, pageSize, records);

		_logger.LogError("GetAllTaxRatesAsync(pageIndex,pageSize)={PageIndex},{PageSize}: external service call took {Arg2} ms", pageIndex, pageSize, __logStart.ElapsedMilliseconds);
		_logger.LogWarning("<<< Exiting GetAllTaxRatesAsync(pageIndex={PageIndex},pageSize={PageSize})", pageIndex, pageSize);
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
		_logger.LogError(">>> Entering GetTaxRateByIdAsync(taxRateId={TaxRateId})", taxRateId);
        _logger.LogWarning("<<< Exiting GetTaxRateByIdAsync(taxRateId={TaxRateId})", taxRateId);
        return await _taxRateRepository.GetByIdAsync(taxRateId);
    }

    /// <summary>
    /// Inserts a tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTaxRateAsync(TaxRate taxRate)
    {
		_logger.LogError(">>> Entering InsertTaxRateAsync(taxRate={TaxRate})", taxRate);
        await _taxRateRepository.InsertAsync(taxRate);
		_logger.LogWarning("<<< Exiting InsertTaxRateAsync(taxRate={TaxRate})", taxRate);
    }

    /// <summary>
    /// Updates the tax rate
    /// </summary>
    /// <param name="taxRate">Tax rate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTaxRateAsync(TaxRate taxRate)
    {
		_logger.LogError(">>> Entering UpdateTaxRateAsync(taxRate={TaxRate})", taxRate);
        await _taxRateRepository.UpdateAsync(taxRate);
		_logger.LogWarning("<<< Exiting UpdateTaxRateAsync(taxRate={TaxRate})", taxRate);
    }

    #endregion
}