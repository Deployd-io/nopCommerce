using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record VendorNavigationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public VendorNavigationModel()
    {
		_logger.LogDebug(">>> Entering VendorNavigationModel()");
        Vendors = new List<VendorBriefInfoModel>();
		_logger.LogDebug("VendorNavigationModel(): Vendors → {Vendors}", Vendors);
		_logger.LogDebug("<<< Exiting VendorNavigationModel()");
    }

    public IList<VendorBriefInfoModel> Vendors { get; set; }

    public int TotalVendors { get; set; }
}

public partial record VendorBriefInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }
}