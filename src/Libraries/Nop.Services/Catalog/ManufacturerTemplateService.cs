using Nop.Core.Domain.Catalog;
using Nop.Data;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Manufacturer template service
/// </summary>
public partial class ManufacturerTemplateService : IManufacturerTemplateService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;

    #endregion

    #region Ctor

    public ManufacturerTemplateService(IRepository<ManufacturerTemplate> manufacturerTemplateRepository)
    {
		_logger.LogError(">>> Entering ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository})", manufacturerTemplateRepository);
        _manufacturerTemplateRepository = manufacturerTemplateRepository;
		_logger.LogDebug("ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository}): _manufacturerTemplateRepository → {_manufacturerTemplateRepository}", manufacturerTemplateRepository, _manufacturerTemplateRepository);
    _logger.LogWarning("<<< Exiting ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository})", manufacturerTemplateRepository);
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
		_logger.LogError(">>> Entering DeleteManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        await _manufacturerTemplateRepository.DeleteAsync(manufacturerTemplate);
		_logger.LogError("DeleteManufacturerTemplateAsync(manufacturerTemplate)={ManufacturerTemplate}: external service call took {Arg1} ms", manufacturerTemplate, __logStart.ElapsedMilliseconds);
    _logger.LogWarning("<<< Exiting DeleteManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
    }

    /// <summary>
    /// Gets all manufacturer templates
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the manufacturer templates
    /// </returns>
    public virtual async Task<IList<ManufacturerTemplate>> GetAllManufacturerTemplatesAsync()
    {
		_logger.LogError(">>> Entering GetAllManufacturerTemplatesAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var templates = await _manufacturerTemplateRepository.GetAllAsync(query =>
        {
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
        }, cache => default);
		_logger.LogError("GetAllManufacturerTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

		_logger.LogWarning("<<< Exiting GetAllManufacturerTemplatesAsync()");
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
		_logger.LogError(">>> Entering GetManufacturerTemplateByIdAsync(manufacturerTemplateId={ManufacturerTemplateId})", manufacturerTemplateId);
		_logger.LogError("GetManufacturerTemplateByIdAsync(manufacturerTemplateId)={ManufacturerTemplateId}: find query executing", manufacturerTemplateId);
    _logger.LogWarning("<<< Exiting GetManufacturerTemplateByIdAsync(manufacturerTemplateId={ManufacturerTemplateId})", manufacturerTemplateId);
        return await _manufacturerTemplateRepository.GetByIdAsync(manufacturerTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		_logger.LogError(">>> Entering InsertManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        await _manufacturerTemplateRepository.InsertAsync(manufacturerTemplate);
		_logger.LogWarning("<<< Exiting InsertManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
    }

    /// <summary>
    /// Updates the manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		_logger.LogError(">>> Entering UpdateManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        await _manufacturerTemplateRepository.UpdateAsync(manufacturerTemplate);
		_logger.LogWarning("<<< Exiting UpdateManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
    }

    #endregion
}