using Nop.Core.Domain.Catalog;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Category template service
/// </summary>
public partial class CategoryTemplateService : ICategoryTemplateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<CategoryTemplate> _categoryTemplateRepository;

    #endregion

    #region Ctor

    public CategoryTemplateService(IRepository<CategoryTemplate> categoryTemplateRepository)
    {
		_logger.LogError(">>> Entering CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository})", categoryTemplateRepository);
        _categoryTemplateRepository = categoryTemplateRepository;
		_logger.LogDebug("CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository}): _categoryTemplateRepository → {_categoryTemplateRepository}", categoryTemplateRepository, _categoryTemplateRepository);
    _logger.LogWarning("<<< Exiting CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository})", categoryTemplateRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		_logger.LogError(">>> Entering DeleteCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _categoryTemplateRepository.DeleteAsync(categoryTemplate);
		_logger.LogError("DeleteCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: external service call took {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
    }

    /// <summary>
    /// Gets all category templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category templates
    /// </returns>
    public virtual async Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync()
    {
		_logger.LogError(">>> Entering GetAllCategoryTemplatesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _categoryTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
        }, cache => default);
		_logger.LogError("GetAllCategoryTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllCategoryTemplatesAsync()");
        return templates;
    }

    /// <summary>
    /// Gets a category template
    /// </summary>
    /// <param name="categoryTemplateId">Category template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category template
    /// </returns>
    public virtual async Task<CategoryTemplate> GetCategoryTemplateByIdAsync(int categoryTemplateId)
    {
		_logger.LogError(">>> Entering GetCategoryTemplateByIdAsync(categoryTemplateId={CategoryTemplateId})", categoryTemplateId);
		_logger.LogError("GetCategoryTemplateByIdAsync(categoryTemplateId)={CategoryTemplateId}: find query executing", categoryTemplateId);
    _logger.LogWarning("<<< Exiting GetCategoryTemplateByIdAsync(categoryTemplateId={CategoryTemplateId})", categoryTemplateId);
        return await _categoryTemplateRepository.GetByIdAsync(categoryTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		_logger.LogError(">>> Entering InsertCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        await _categoryTemplateRepository.InsertAsync(categoryTemplate);
		_logger.LogWarning("<<< Exiting InsertCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
    }

    /// <summary>
    /// Updates the category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		_logger.LogError(">>> Entering UpdateCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        await _categoryTemplateRepository.UpdateAsync(categoryTemplate);
		_logger.LogWarning("<<< Exiting UpdateCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
    }

    #endregion
}