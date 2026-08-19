using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a manufacturer filter model
/// </summary>
public partial record ManufacturerFilterModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the filtrable manufacturers
    /// </summary>
    public IList<SelectListItem> Manufacturers { get; set; }

    #endregion

    #region Ctor

    public ManufacturerFilterModel()
    {
		_logger.LogDebug(">>> Entering ManufacturerFilterModel()");
        Manufacturers = new List<SelectListItem>();
		_logger.LogDebug("ManufacturerFilterModel(): Manufacturers → {Manufacturers}", Manufacturers);
		_logger.LogDebug("<<< Exiting ManufacturerFilterModel()");
    }

    #endregion
}