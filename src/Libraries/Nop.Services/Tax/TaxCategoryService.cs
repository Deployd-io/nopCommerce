using Nop.Core.Domain.Tax;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Tax;

/// <summary>
/// Tax category service
/// </summary>
public partial class TaxCategoryService : ITaxCategoryService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<TaxCategory> _taxCategoryRepository;

    #endregion

    #region Ctor

    public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
    {
		_logger.LogDebug(">>> Entering TaxCategoryService(taxCategoryRepository={TaxCategoryRepository})", taxCategoryRepository);
        _taxCategoryRepository = taxCategoryRepository;
		_logger.LogDebug("TaxCategoryService(taxCategoryRepository={TaxCategoryRepository}): _taxCategoryRepository → {_taxCategoryRepository}", taxCategoryRepository, _taxCategoryRepository);
		_logger.LogDebug("<<< Exiting TaxCategoryService(taxCategoryRepository={TaxCategoryRepository})", taxCategoryRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteTaxCategoryAsync(TaxCategory taxCategory)
    {
		_logger.LogDebug(">>> Entering DeleteTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxCategoryRepository.DeleteAsync(taxCategory);
		_logger.LogInformation("DeleteTaxCategoryAsync(taxCategory)={TaxCategory}: external service call took {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting DeleteTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
    }

    /// <summary>
    /// Gets all tax categories
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax categories
    /// </returns>
    public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
    {
		_logger.LogDebug(">>> Entering GetAllTaxCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var taxCategories = await _taxCategoryRepository.GetAllAsync(query =>
        {
            return from tc in query
                orderby tc.DisplayOrder, tc.Id
                select tc;
				_logger.LogInformation("GetAllTaxCategoriesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllTaxCategoriesAsync()");
        return taxCategories;
    }

    /// <summary>
    /// Gets a tax category
    /// </summary>
    /// <param name="taxCategoryId">Tax category identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax category
    /// </returns>
    public virtual async Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId)
    {
		_logger.LogDebug(">>> Entering GetTaxCategoryByIdAsync(taxCategoryId={TaxCategoryId})", taxCategoryId);
		_logger.LogDebug("<<< Exiting GetTaxCategoryByIdAsync(taxCategoryId={TaxCategoryId})", taxCategoryId);
        return await _taxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
    }

    /// <summary>
    /// Inserts a tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
    {
		_logger.LogDebug(">>> Entering InsertTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
        await _taxCategoryRepository.InsertAsync(taxCategory);
		_logger.LogDebug("<<< Exiting InsertTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
    }

    /// <summary>
    /// Updates the tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
    {
		_logger.LogDebug(">>> Entering UpdateTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
        await _taxCategoryRepository.UpdateAsync(taxCategory);
		_logger.LogDebug("<<< Exiting UpdateTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
    }

    #endregion
}