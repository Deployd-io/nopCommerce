using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a campaign search model
/// </summary>
public partial record CampaignSearchModel : BaseSearchModel
{
    #region Ctor

    public CampaignSearchModel()
    {
		logger.LogDebug(">>> Entering CampaignSearchModel()");
        AvailableStores = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting CampaignSearchModel()");
		logger.LogDebug("CampaignSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.Campaigns.List.Stores")]
    public int StoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}