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
		_logger.LogDebug(">>> Entering CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository})", categoryTemplateRepository);
        _categoryTemplateRepository = categoryTemplateRepository;
		_logger.LogDebug("CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository}): _categoryTemplateRepository → {_categoryTemplateRepository}", categoryTemplateRepository, _categoryTemplateRepository);
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
		_logger.LogDebug(">>> Entering DeleteCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        await _categoryTemplateRepository.DeleteAsync(categoryTemplate);
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
        var templates = await _categoryTemplateRepository.GetAllAsync(query =>
        {
			_logger.LogDebug(">>> Entering GetAllCategoryTemplatesAsync()");
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
			_logger.LogDebug("<<< Exiting GetAllCategoryTemplatesAsync()");
				_logger.LogInformation("GetAllCategoryTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

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
		_logger.LogDebug(">>> Entering GetCategoryTemplateByIdAsync(categoryTemplateId={CategoryTemplateId})", categoryTemplateId);
		_logger.LogInformation("GetCategoryTemplateByIdAsync(categoryTemplateId)={CategoryTemplateId}: find query executing", categoryTemplateId);
        return await _categoryTemplateRepository.GetByIdAsync(categoryTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		_logger.LogDebug(">>> Entering InsertCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        await _categoryTemplateRepository.InsertAsync(categoryTemplate);
    }

    /// <summary>
    /// Updates the category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		_logger.LogDebug(">>> Entering UpdateCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
        await _categoryTemplateRepository.UpdateAsync(categoryTemplate);
    }

    #endregion
}