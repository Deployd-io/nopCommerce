using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a specification filter model
/// </summary>
public partial record SpecificationFilterModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether filtering is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the filtrable specification attributes
    /// </summary>
    public IList<SpecificationAttributeFilterModel> Attributes { get; set; }

    #endregion

    #region Ctor

    public SpecificationFilterModel()
    {
		_logger.LogError(">>> Entering SpecificationFilterModel()");
        Attributes = new List<SpecificationAttributeFilterModel>();
		_logger.LogDebug("SpecificationFilterModel(): Attributes → {Attributes}", Attributes);
    _logger.LogError("SpecificationFilterModel(): Attributes → {Attributes}", Attributes);
    _logger.LogWarning("<<< Exiting SpecificationFilterModel()");
    }

    #endregion
}