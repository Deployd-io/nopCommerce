using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a cross-sell product model to add to the product
/// </summary>
public partial record AddCrossSellProductModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddCrossSellProductModel()
    {
		_logger.LogDebug(">>> Entering AddCrossSellProductModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("<<< Exiting AddCrossSellProductModel()");
		_logger.LogDebug("AddCrossSellProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}