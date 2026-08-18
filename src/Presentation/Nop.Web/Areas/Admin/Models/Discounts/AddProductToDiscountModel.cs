using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a product model to add to the discount
/// </summary>
public partial record AddProductToDiscountModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToDiscountModel()
    {
		_logger.LogDebug(">>> Entering AddProductToDiscountModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddProductToDiscountModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
		_logger.LogDebug("<<< Exiting AddProductToDiscountModel()");
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}