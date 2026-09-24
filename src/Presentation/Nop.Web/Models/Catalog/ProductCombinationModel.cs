using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a product combination model
/// </summary>
public partial record ProductCombinationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the attributes
    /// </summary>
    public IList<ProductAttributeModel> Attributes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to the combination have stock
    /// </summary>
    public bool InStock { get; set; }

    #endregion

    #region Ctor

    public ProductCombinationModel()
    {
		_logger.LogError(">>> Entering ProductCombinationModel()");
        Attributes = new List<ProductAttributeModel>();
		_logger.LogDebug("ProductCombinationModel(): Attributes → {Attributes}", Attributes);
    _logger.LogError("ProductCombinationModel(): Attributes → {Attributes}", Attributes);
    _logger.LogWarning("<<< Exiting ProductCombinationModel()");
    }

    #endregion
}