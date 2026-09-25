using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Models;

/// <summary>
/// Represents base search model
/// </summary>
public abstract partial record BaseSearchModel : BaseNopModel, IPagingRequestModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    protected BaseSearchModel()
    {
		_logger.LogError(">>> Entering BaseSearchModel()");
        //set the default values
		_logger.LogWarning("<<< Exiting BaseSearchModel()");
        Length = 10;
		_logger.LogDebug("BaseSearchModel(): Length → {Length}", Length);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a page number
    /// </summary>
    public int Page => (Start / Length) + 1;

    /// <summary>
    /// Gets a page size
    /// </summary>
    public int PageSize => Length;

    /// <summary>
    /// Gets or sets a comma-separated list of available page sizes
    /// </summary>
    public string AvailablePageSizes { get; set; }

    /// <summary>
    /// Gets or sets draw. Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
    /// </summary>
    public string Draw { get; set; }

    /// <summary>
    /// Gets or sets skipping number of rows count. Paging first record indicator.
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Gets or sets paging length. Number of records that the table can display in the current draw. 
    /// </summary>
    public int Length { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Set grid page parameters
    /// </summary>
    public virtual void SetGridPageSize()
    {
		_logger.LogError(">>> Entering SetGridPageSize()");
        var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();
		_logger.LogError("SetGridPageSize(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        SetGridPageSize(adminAreaSettings?.DefaultGridPageSize ?? 0, adminAreaSettings?.GridPageSizes);
		_logger.LogWarning("<<< Exiting SetGridPageSize(pageSize={PageSize},availablePageSizes={AvailablePageSizes})", pageSize, availablePageSizes);
    }

    /// <summary>
    /// Set popup grid page parameters
    /// </summary>
    public virtual void SetPopupGridPageSize()
    {
		_logger.LogError(">>> Entering SetPopupGridPageSize()");
        var adminAreaSettings = EngineContext.Current.Resolve<AdminAreaSettings>();
		_logger.LogWarning("<<< Exiting SetPopupGridPageSize()");
		_logger.LogError("SetPopupGridPageSize(): adminAreaSettings → {AdminAreaSettings}", adminAreaSettings);
        SetGridPageSize(adminAreaSettings.PopupGridPageSize, adminAreaSettings.GridPageSizes);
    }

    /// <summary>
    /// Set grid page parameters
    /// </summary>
    /// <param name="pageSize">Page size; pass null to use default value</param>
    /// <param name="availablePageSizes">Available page sizes; pass null to use default value</param>
    public virtual void SetGridPageSize(int pageSize, string availablePageSizes = null)
    {
		_logger.LogError(">>> Entering SetGridPageSize(pageSize={PageSize},availablePageSizes={AvailablePageSizes})", pageSize, availablePageSizes);
		_logger.LogDebug("SetGridPageSize(pageSize={PageSize},availablePageSizes={AvailablePageSizes}): Length → {Length}", pageSize, availablePageSizes, Length);
        Start = 0;
        Length = pageSize;
        AvailablePageSizes = availablePageSizes;
		_logger.LogDebug("SetGridPageSize(pageSize={PageSize},availablePageSizes={AvailablePageSizes}): AvailablePageSizes → {AvailablePageSizes1}", pageSize, availablePageSizes, AvailablePageSizes);
    }

    #endregion
}