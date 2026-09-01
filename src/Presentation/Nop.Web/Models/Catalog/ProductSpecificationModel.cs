using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a product specification model
/// </summary>
public partial record ProductSpecificationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the grouped specification attribute models
    /// </summary>
    public IList<ProductSpecificationAttributeGroupModel> Groups { get; set; }

    #endregion

    #region Ctor

    public ProductSpecificationModel()
    {
		_logger.LogDebug(">>> Entering ProductSpecificationModel()");
        Groups = new List<ProductSpecificationAttributeGroupModel>();
		_logger.LogDebug("ProductSpecificationModel(): Groups → {Groups}", Groups);
    }

    #endregion
}