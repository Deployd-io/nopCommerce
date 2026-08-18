using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a grouped product specification attribute model
/// </summary>
public partial record ProductSpecificationAttributeGroupModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the specification attribute group name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the specification attribute group attributes
    /// </summary>
    public IList<ProductSpecificationAttributeModel> Attributes { get; set; }

    #endregion

    #region Ctor

    public ProductSpecificationAttributeGroupModel()
    {
		_logger.LogDebug(">>> Entering ProductSpecificationAttributeGroupModel()");
        Attributes = new List<ProductSpecificationAttributeModel>();
		_logger.LogDebug("ProductSpecificationAttributeGroupModel(): Attributes → {Attributes}", Attributes);
		_logger.LogDebug("<<< Exiting ProductSpecificationAttributeGroupModel()");
    }

    #endregion
}