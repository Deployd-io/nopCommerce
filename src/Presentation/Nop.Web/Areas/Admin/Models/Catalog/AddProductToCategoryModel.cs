using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the category
/// </summary>
public partial record AddProductToCategoryModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToCategoryModel()
    {
		_logger.LogDebug(">>> Entering AddProductToCategoryModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("<<< Exiting AddProductToCategoryModel()");
		_logger.LogDebug("AddProductToCategoryModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int CategoryId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}