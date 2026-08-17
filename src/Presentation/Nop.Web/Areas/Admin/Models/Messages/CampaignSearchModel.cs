using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a campaign search model
/// </summary>
public partial record CampaignSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CampaignSearchModel()
    {
		_logger.LogDebug(">>> Entering CampaignSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting CampaignSearchModel()");
		_logger.LogDebug("CampaignSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.Campaigns.List.Stores")]
    public int StoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}