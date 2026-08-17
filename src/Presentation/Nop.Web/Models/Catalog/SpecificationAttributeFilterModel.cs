using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a specification attribute filter model
/// </summary>
public partial record SpecificationAttributeFilterModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the specification attribute name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the values
    /// </summary>
    public IList<SpecificationAttributeValueFilterModel> Values { get; set; }

    #endregion

    #region Ctor

    public SpecificationAttributeFilterModel()
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeFilterModel()");
        Values = new List<SpecificationAttributeValueFilterModel>();
		_logger.LogDebug("<<< Exiting SpecificationAttributeFilterModel()");
		_logger.LogDebug("SpecificationAttributeFilterModel(): Values → {Values}", Values);
    }

    #endregion
}