using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a specification attribute model
/// </summary>
public partial record ProductSpecificationAttributeModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the values
    /// </summary>
    public IList<ProductSpecificationAttributeValueModel> Values { get; set; }

    #endregion

    #region Ctor

    public ProductSpecificationAttributeModel()
    {
		_logger.LogDebug(">>> Entering ProductSpecificationAttributeModel()");
        Values = new List<ProductSpecificationAttributeValueModel>();
		_logger.LogDebug("ProductSpecificationAttributeModel(): Values → {Values}", Values);
    }

    #endregion
}