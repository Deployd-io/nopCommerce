using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record ManufacturerNavigationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ManufacturerNavigationModel()
    {
		_logger.LogDebug(">>> Entering ManufacturerNavigationModel()");
        Manufacturers = new List<ManufacturerBriefInfoModel>();
		_logger.LogDebug("<<< Exiting ManufacturerNavigationModel()");
		_logger.LogDebug("ManufacturerNavigationModel(): Manufacturers → {Manufacturers}", Manufacturers);
    }

    public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

    public int TotalManufacturers { get; set; }
}

public partial record ManufacturerBriefInfoModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string SeName { get; set; }

    public bool IsActive { get; set; }
}