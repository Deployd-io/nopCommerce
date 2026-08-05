using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the filter level value
/// </summary>
public partial record AddProductToFilterLevelValueModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToFilterLevelValueModel()
    {
		_logger.LogDebug(">>> Entering AddProductToFilterLevelValueModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("<<< Exiting AddProductToFilterLevelValueModel()");
		_logger.LogDebug("AddProductToFilterLevelValueModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int FilterLevelValueId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}
