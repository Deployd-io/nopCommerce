using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog;

/// <summary>
/// Manufacturer template service
/// </summary>
public partial class ManufacturerTemplateService : IManufacturerTemplateService
{
    #region Fields

    protected readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;

    #endregion

    #region Ctor

    public ManufacturerTemplateService(IRepository<ManufacturerTemplate> manufacturerTemplateRepository)
    {
		logger.LogDebug(">>> Entering ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository})", manufacturerTemplateRepository);
        _manufacturerTemplateRepository = manufacturerTemplateRepository;
		logger.LogDebug("<<< Exiting ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository})", manufacturerTemplateRepository);
		logger.LogDebug("ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository}): _manufacturerTemplateRepository → {_manufacturerTemplateRepository}", manufacturerTemplateRepository, _manufacturerTemplateRepository);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Delete manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		logger.LogDebug(">>> Entering DeleteManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerTemplateRepository.DeleteAsync(manufacturerTemplate);
		logger.LogDebug("<<< Exiting DeleteManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: external service call took {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("DeleteManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: delete query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("DeleteManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: external service call took {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
	var __logStart = System.Diagnostics.Stopwatch.StartNew();
	logger.LogInformation("DeleteManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: delete query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Gets all manufacturer templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer templates
    /// </returns>
    public virtual async Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync()
    {
		logger.LogDebug(">>> Entering GetAllManufacturerTemplatesAsync()");
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _manufacturerTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
				logger.LogInformation("GetAllManufacturerTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

		logger.LogDebug("<<< Exiting GetAllManufacturerTemplatesAsync()");
        return templates;
    }

    /// <summary>
    /// Gets a manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplateId">Manufacturer template identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer template
    /// </returns>
    public virtual async Task<ManufacturerTemplate> GetManufacturerTemplateByIdAsync(int manufacturerTemplateId)
    {
		logger.LogDebug(">>> Entering GetManufacturerTemplateByIdAsync(manufacturerTemplateId={ManufacturerTemplateId})", manufacturerTemplateId);
		logger.LogDebug("<<< Exiting GetManufacturerTemplateByIdAsync(manufacturerTemplateId={ManufacturerTemplateId})", manufacturerTemplateId);
        return await _manufacturerTemplateRepository.GetByIdAsync(manufacturerTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		logger.LogDebug(">>> Entering InsertManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerTemplateRepository.InsertAsync(manufacturerTemplate);
		logger.LogDebug("<<< Exiting InsertManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("InsertManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: save query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("InsertManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: save query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    /// <summary>
    /// Updates the manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		logger.LogDebug(">>> Entering UpdateManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerTemplateRepository.UpdateAsync(manufacturerTemplate);
		logger.LogDebug("<<< Exiting UpdateManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		logger.LogInformation("UpdateManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: update query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    }

	logger.LogInformation("UpdateManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: update query executed in {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    #endregion
}
