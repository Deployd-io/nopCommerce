using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a product attribute model
/// </summary>
public partial record ProductAttributeModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the attribute id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the value IDs of the attribute
    /// </summary>
    public IList<int> ValueIds { get; set; }

    #endregion

    #region Ctor

    public ProductAttributeModel()
    {
		_logger.LogError(">>> Entering ProductAttributeModel()");
        ValueIds = new List<int>();
		_logger.LogDebug("ProductAttributeModel(): ValueIds → {ValueIds}", ValueIds);
    _logger.LogError("ProductAttributeModel(): ValueIds → {ValueIds}", ValueIds);
    _logger.LogWarning("<<< Exiting ProductAttributeModel()");
    }

    #endregion
}