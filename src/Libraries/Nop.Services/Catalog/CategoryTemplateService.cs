using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Category template service
/// </summary>
public partial class CategoryTemplateService : ICategoryTemplateService
{
    #region Fields

    protected readonly IRepository<CategoryTemplate> _categoryTemplateRepository;

    #endregion

    #region Ctor

    public CategoryTemplateService(IRepository<CategoryTemplate> categoryTemplateRepository)
    {
		logger.LogDebug(">>> Entering CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository})", categoryTemplateRepository);
        _categoryTemplateRepository = categoryTemplateRepository;
		logger.LogDebug("<<< Exiting CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository})", categoryTemplateRepository);
		logger.LogDebug("CategoryTemplateService(categoryTemplateRepository={CategoryTemplateRepository}): _categoryTemplateRepository → {_categoryTemplateRepository}", categoryTemplateRepository, _categoryTemplateRepository);
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
		logger.LogDebug(">>> Entering DeleteCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _categoryTemplateRepository.DeleteAsync(categoryTemplate);
		logger.LogDebug("<<< Exiting DeleteCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: external service call took {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: delete query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: external service call took {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: delete query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all category templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the category templates
    /// </returns>
    public virtual async Task<IList<CategoryTemplate>> GetAllCategoryTemplatesAsync()
    {
		logger.LogDebug(">>> Entering GetAllCategoryTemplatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _categoryTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
				logger.LogInformation("GetAllCategoryTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllCategoryTemplatesAsync()");
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
		logger.LogDebug(">>> Entering GetCategoryTemplateByIdAsync(categoryTemplateId={CategoryTemplateId})", categoryTemplateId);
		logger.LogDebug("<<< Exiting GetCategoryTemplateByIdAsync(categoryTemplateId={CategoryTemplateId})", categoryTemplateId);
        return await _categoryTemplateRepository.GetByIdAsync(categoryTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		logger.LogDebug(">>> Entering InsertCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _categoryTemplateRepository.InsertAsync(categoryTemplate);
		logger.LogDebug("<<< Exiting InsertCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: save query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: save query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the category template
    /// </summary>
    /// <param name="categoryTemplate">Category template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateCategoryTemplateAsync(CategoryTemplate categoryTemplate)
    {
		logger.LogDebug(">>> Entering UpdateCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _categoryTemplateRepository.UpdateAsync(categoryTemplate);
		logger.LogDebug("<<< Exiting UpdateCategoryTemplateAsync(categoryTemplate={CategoryTemplate})", categoryTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: update query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateCategoryTemplateAsync(categoryTemplate)={CategoryTemplate}: update query executed in {Arg1} ms", categoryTemplate, __logStart.ElapsedMilliseconds);
    #endregion
}