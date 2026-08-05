using Nop.Core.Domain.Tax;
using Nop.Data;

namespace Nop.Services.Tax;

/// <summary>
/// Tax category service
/// </summary>
public partial class TaxCategoryService : ITaxCategoryService
{
    #region Fields

    protected readonly IRepository<TaxCategory> _taxCategoryRepository;

    #endregion

    #region Ctor

    public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
    {
		logger.LogDebug(">>> Entering TaxCategoryService(taxCategoryRepository={TaxCategoryRepository})", taxCategoryRepository);
        _taxCategoryRepository = taxCategoryRepository;
		logger.LogDebug("<<< Exiting TaxCategoryService(taxCategoryRepository={TaxCategoryRepository})", taxCategoryRepository);
		logger.LogDebug("TaxCategoryService(taxCategoryRepository={TaxCategoryRepository}): _taxCategoryRepository → {_taxCategoryRepository}", taxCategoryRepository, _taxCategoryRepository);
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
		logger.LogDebug(">>> Entering DeleteTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxCategoryRepository.DeleteAsync(taxCategory);
		logger.LogDebug("<<< Exiting DeleteTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxCategoryAsync(taxCategory)={TaxCategory}: external service call took {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteTaxCategoryAsync(taxCategory)={TaxCategory}: delete query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteTaxCategoryAsync(taxCategory)={TaxCategory}: external service call took {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteTaxCategoryAsync(taxCategory)={TaxCategory}: delete query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all tax categories
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax categories
    /// </returns>
    public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
    {
		logger.LogDebug(">>> Entering GetAllTaxCategoriesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var taxCategories = await _taxCategoryRepository.GetAllAsync(query =>
        {
            return from tc in query
                orderby tc.DisplayOrder, tc.Id
                select tc;
				logger.LogInformation("GetAllTaxCategoriesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllTaxCategoriesAsync()");
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
		logger.LogDebug(">>> Entering GetTaxCategoryByIdAsync(taxCategoryId={TaxCategoryId})", taxCategoryId);
		logger.LogDebug("<<< Exiting GetTaxCategoryByIdAsync(taxCategoryId={TaxCategoryId})", taxCategoryId);
        return await _taxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
    }

    /// <summary>
    /// Inserts a tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
    {
		logger.LogDebug(">>> Entering InsertTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxCategoryRepository.InsertAsync(taxCategory);
		logger.LogDebug("<<< Exiting InsertTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertTaxCategoryAsync(taxCategory)={TaxCategory}: save query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertTaxCategoryAsync(taxCategory)={TaxCategory}: save query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the tax category
    /// </summary>
    /// <param name="taxCategory">Tax category</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
    {
		logger.LogDebug(">>> Entering UpdateTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _taxCategoryRepository.UpdateAsync(taxCategory);
		logger.LogDebug("<<< Exiting UpdateTaxCategoryAsync(taxCategory={TaxCategory})", taxCategory);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateTaxCategoryAsync(taxCategory)={TaxCategory}: update query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateTaxCategoryAsync(taxCategory)={TaxCategory}: update query executed in {Arg1} ms", taxCategory, __logStart.ElapsedMilliseconds);
    #endregion
}