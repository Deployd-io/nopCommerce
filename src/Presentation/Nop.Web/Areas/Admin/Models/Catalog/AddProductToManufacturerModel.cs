using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product model to add to the manufacturer
/// </summary>
public partial record AddProductToManufacturerModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddProductToManufacturerModel()
    {
		_logger.LogDebug(">>> Entering AddProductToManufacturerModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddProductToManufacturerModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
		_logger.LogDebug("<<< Exiting AddProductToManufacturerModel()");
    }
    #endregion

    #region Properties

    public int ManufacturerId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}