using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a nopCommerce news model
/// </summary>
public partial record NopCommerceNewsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public NopCommerceNewsModel()
    {
		_logger.LogDebug(">>> Entering NopCommerceNewsModel()");
        Items = new List<NopCommerceNewsDetailsModel>();
		_logger.LogDebug("NopCommerceNewsModel(): Items → {Items}", Items);
		_logger.LogDebug("<<< Exiting NopCommerceNewsModel()");
    }

    #endregion

    #region Properties

    public List<NopCommerceNewsDetailsModel> Items { get; set; }

    public bool HasNewItems { get; set; }

    public bool HideAdvertisements { get; set; }

    #endregion
}