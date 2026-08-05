using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CategorySimpleModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CategorySimpleModel()
    {
		_logger.LogDebug(">>> Entering CategorySimpleModel()");
        SubCategories = new List<CategorySimpleModel>();
		_logger.LogDebug("<<< Exiting CategorySimpleModel()");
		_logger.LogDebug("CategorySimpleModel(): SubCategories → {SubCategories}", SubCategories);
    }

    public string Name { get; set; }

    public string SeName { get; set; }

    public int? NumberOfProducts { get; set; }

    public List<CategorySimpleModel> SubCategories { get; set; }

    public string Route { get; set; }
}