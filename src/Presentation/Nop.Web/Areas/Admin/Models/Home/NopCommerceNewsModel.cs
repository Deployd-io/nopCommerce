using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Home;

/// <summary>
/// Represents a nopCommerce news model
/// </summary>
public partial record NopCommerceNewsModel : BaseNopModel
{
    #region Ctor

    public NopCommerceNewsModel()
    {
		logger.LogDebug(">>> Entering NopCommerceNewsModel()");
        Items = new List<NopCommerceNewsDetailsModel>();
		logger.LogDebug("<<< Exiting NopCommerceNewsModel()");
		logger.LogDebug("NopCommerceNewsModel(): Items → {Items}", Items);
    }

    #endregion

    #region Properties

    public List<NopCommerceNewsDetailsModel> Items { get; set; }

    public bool HasNewItems { get; set; }

    public bool HideAdvertisements { get; set; }

    #endregion
}