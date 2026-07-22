using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Product template service
/// </summary>
public partial class ProductTemplateService : IProductTemplateService
{
    #region Fields

    protected readonly IRepository<ProductTemplate> _productTemplateRepository;

    #endregion

    #region Ctor

    public ProductTemplateService(IRepository<ProductTemplate> productTemplateRepository)
    {
		logger.LogDebug(">>> Entering ProductTemplateService(productTemplateRepository={ProductTemplateRepository})", productTemplateRepository);
        _productTemplateRepository = productTemplateRepository;
		logger.LogDebug("<<< Exiting ProductTemplateService(productTemplateRepository={ProductTemplateRepository})", productTemplateRepository);
		logger.LogDebug("ProductTemplateService(productTemplateRepository={ProductTemplateRepository}): _productTemplateRepository → {_productTemplateRepository}", productTemplateRepository, _productTemplateRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteProductTemplateAsync(ProductTemplate productTemplate)
    {
		logger.LogDebug(">>> Entering DeleteProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productTemplateRepository.DeleteAsync(productTemplate);
		logger.LogDebug("<<< Exiting DeleteProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductTemplateAsync(productTemplate)={ProductTemplate}: external service call took {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteProductTemplateAsync(productTemplate)={ProductTemplate}: delete query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteProductTemplateAsync(productTemplate)={ProductTemplate}: external service call took {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteProductTemplateAsync(productTemplate)={ProductTemplate}: delete query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all product templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product templates
    /// </returns>
    public virtual async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync()
    {
		logger.LogDebug(">>> Entering GetAllProductTemplatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _productTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
				logger.LogInformation("GetAllProductTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllProductTemplatesAsync()");
        return templates;
    }

    /// <summary>
    /// Gets a product template
    /// </summary>
    /// <param name="productTemplateId">Product template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product template
    /// </returns>
    public virtual async Task<ProductTemplate> GetProductTemplateByIdAsync(int productTemplateId)
    {
		logger.LogDebug(">>> Entering GetProductTemplateByIdAsync(productTemplateId={ProductTemplateId})", productTemplateId);
		logger.LogDebug("<<< Exiting GetProductTemplateByIdAsync(productTemplateId={ProductTemplateId})", productTemplateId);
        return await _productTemplateRepository.GetByIdAsync(productTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductTemplateAsync(ProductTemplate productTemplate)
    {
		logger.LogDebug(">>> Entering InsertProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productTemplateRepository.InsertAsync(productTemplate);
		logger.LogDebug("<<< Exiting InsertProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertProductTemplateAsync(productTemplate)={ProductTemplate}: save query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertProductTemplateAsync(productTemplate)={ProductTemplate}: save query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductTemplateAsync(ProductTemplate productTemplate)
    {
		logger.LogDebug(">>> Entering UpdateProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _productTemplateRepository.UpdateAsync(productTemplate);
		logger.LogDebug("<<< Exiting UpdateProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateProductTemplateAsync(productTemplate)={ProductTemplate}: update query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateProductTemplateAsync(productTemplate)={ProductTemplate}: update query executed in {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
    #endregion
}
