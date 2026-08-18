using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CategoryNavigationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CategoryNavigationModel()
    {
		_logger.LogDebug(">>> Entering CategoryNavigationModel()");
        Categories = new List<CategorySimpleModel>();
		_logger.LogDebug("CategoryNavigationModel(): Categories → {Categories}", Categories);
		_logger.LogDebug("<<< Exiting CategoryNavigationModel()");
    }

    public int CurrentCategoryId { get; set; }
    public List<CategorySimpleModel> Categories { get; set; }

    #region Nested classes

    public partial record CategoryLineModel : BaseNopModel
    {
        public int CurrentCategoryId { get; set; }
        public CategorySimpleModel Category { get; set; }
    }

    #endregion
}