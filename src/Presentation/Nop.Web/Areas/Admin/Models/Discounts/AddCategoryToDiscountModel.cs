using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a category model to add to the discount
/// </summary>
public partial record AddCategoryToDiscountModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddCategoryToDiscountModel()
    {
		_logger.LogDebug(">>> Entering AddCategoryToDiscountModel()");
        SelectedCategoryIds = new List<int>();
		_logger.LogDebug("AddCategoryToDiscountModel(): SelectedCategoryIds → {SelectedCategoryIds}", SelectedCategoryIds);
		_logger.LogDebug("<<< Exiting AddCategoryToDiscountModel()");
    }
    #endregion

    #region Properties

    public int DiscountId { get; set; }

    public IList<int> SelectedCategoryIds { get; set; }

    #endregion
}