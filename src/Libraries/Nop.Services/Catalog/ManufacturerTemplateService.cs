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
		_logger.LogDebug(">>> Entering ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository})", manufacturerTemplateRepository);
        _manufacturerTemplateRepository = manufacturerTemplateRepository;
		_logger.LogDebug("ManufacturerTemplateService(manufacturerTemplateRepository={ManufacturerTemplateRepository}): _manufacturerTemplateRepository → {_manufacturerTemplateRepository}", manufacturerTemplateRepository, _manufacturerTemplateRepository);
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
		_logger.LogDebug(">>> Entering DeleteManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        await _manufacturerTemplateRepository.DeleteAsync(manufacturerTemplate);
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
        var templates = await _manufacturerTemplateRepository.GetAllAsync(query =>
        {
			_logger.LogDebug(">>> Entering GetAllManufacturerTemplatesAsync()");
            return from pt in query
                orderby pt.DisplayOrder, pt.Id
                select pt;
			_logger.LogDebug("<<< Exiting GetAllManufacturerTemplatesAsync()");
				_logger.LogInformation("GetAllManufacturerTemplatesAsync(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
        }, cache => default);

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
		_logger.LogDebug(">>> Entering GetManufacturerTemplateByIdAsync(manufacturerTemplateId={ManufacturerTemplateId})", manufacturerTemplateId);
		_logger.LogInformation("GetManufacturerTemplateByIdAsync(manufacturerTemplateId)={ManufacturerTemplateId}: find query executing", manufacturerTemplateId);
        return await _manufacturerTemplateRepository.GetByIdAsync(manufacturerTemplateId, cache => default);
    }

    /// <summary>
    /// Inserts manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		_logger.LogDebug(">>> Entering InsertManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        await _manufacturerTemplateRepository.InsertAsync(manufacturerTemplate);
    }

    /// <summary>
    /// Updates the manufacturer template
    /// </summary>
    /// <param name="manufacturerTemplate">Manufacturer template</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateManufacturerTemplateAsync(ManufacturerTemplate manufacturerTemplate)
    {
		_logger.LogDebug(">>> Entering UpdateManufacturerTemplateAsync(manufacturerTemplate={ManufacturerTemplate})", manufacturerTemplate);
        await _manufacturerTemplateRepository.UpdateAsync(manufacturerTemplate);
    }

    #endregion
}