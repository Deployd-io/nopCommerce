using Nop.Core.Domain.Catalog;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product template service
/// </summary>
public partial class ProductTemplateService : IProductTemplateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ProductTemplate> _productTemplateRepository;

    #endregion

    #region Ctor

    public ProductTemplateService(IRepository<ProductTemplate> productTemplateRepository)
    {
		_logger.LogDebug(">>> Entering ProductTemplateService(productTemplateRepository={ProductTemplateRepository})", productTemplateRepository);
        _productTemplateRepository = productTemplateRepository;
		_logger.LogDebug("ProductTemplateService(productTemplateRepository={ProductTemplateRepository}): _productTemplateRepository → {_productTemplateRepository}", productTemplateRepository, _productTemplateRepository);
		_logger.LogDebug("<<< Exiting ProductTemplateService(productTemplateRepository={ProductTemplateRepository})", productTemplateRepository);
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
		_logger.LogDebug(">>> Entering DeleteProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
		_logger.LogInformation("DeleteProductTemplateAsync(productTemplate)={ProductTemplate}: external service call took {Arg1} ms", productTemplate, __logStart.ElapsedMilliseconds);
        await _productTemplateRepository.DeleteAsync(productTemplate);
		_logger.LogDebug("<<< Exiting DeleteProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
    }

    /// <summary>
    /// Gets all product templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product templates
    /// </returns>
    public virtual async Task<IList<ProductTemplate>> GetAllProductTemplatesAsync()
    {
		_logger.LogDebug(">>> Entering GetAllProductTemplatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _productTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
				_logger.LogInformation("GetAllProductTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		_logger.LogDebug("<<< Exiting GetAllProductTemplatesAsync()");
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
		_logger.LogDebug(">>> Entering GetProductTemplateByIdAsync(productTemplateId={ProductTemplateId})", productTemplateId);
		_logger.LogDebug("<<< Exiting GetProductTemplateByIdAsync(productTemplateId={ProductTemplateId})", productTemplateId);
        return await _productTemplateRepository.GetByIdAsync(productTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertProductTemplateAsync(ProductTemplate productTemplate)
    {
		_logger.LogDebug(">>> Entering InsertProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
        await _productTemplateRepository.InsertAsync(productTemplate);
		_logger.LogDebug("<<< Exiting InsertProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
    }

    /// <summary>
    /// Updates the product template
    /// </summary>
    /// <param name="productTemplate">Product template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateProductTemplateAsync(ProductTemplate productTemplate)
    {
		_logger.LogDebug(">>> Entering UpdateProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
        await _productTemplateRepository.UpdateAsync(productTemplate);
		_logger.LogDebug("<<< Exiting UpdateProductTemplateAsync(productTemplate={ProductTemplate})", productTemplate);
    }

    #endregion
}