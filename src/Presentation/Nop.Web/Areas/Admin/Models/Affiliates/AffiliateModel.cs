using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate model
/// </summary>
public partial record AffiliateModel : BaseNopEntityModel
{
    #region Ctor

    public AffiliateModel()
    {
		logger.LogDebug(">>> Entering AffiliateModel()");
        Address = new AddressModel();
        AffiliatedOrderSearchModel = new AffiliatedOrderSearchModel();
		logger.LogDebug("AffiliateModel(): Address → {Address}", Address);
		logger.LogDebug("AffiliateModel(): AffiliatedOrderSearchModel → {AffiliatedOrderSearchModel}", AffiliatedOrderSearchModel);
        AffiliatedCustomerSearchModel = new AffiliatedCustomerSearchModel();
		logger.LogDebug("<<< Exiting AffiliateModel()");
		logger.LogDebug("AffiliateModel(): AffiliatedCustomerSearchModel → {AffiliatedCustomerSearchModel}", AffiliatedCustomerSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
    public string Url { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
    public string AdminComment { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
    public string FriendlyUrlName { get; set; }

    [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
    public bool Active { get; set; }

    public AddressModel Address { get; set; }

    public AffiliatedOrderSearchModel AffiliatedOrderSearchModel { get; set; }

    public AffiliatedCustomerSearchModel AffiliatedCustomerSearchModel { get; set; }

    #endregion
}