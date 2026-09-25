using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate model
/// </summary>
public partial record AffiliateModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AffiliateModel()
    {
		_logger.LogError(">>> Entering AffiliateModel()");
        Address = new AddressModel();
        AffiliatedOrderSearchModel = new AffiliatedOrderSearchModel();
		_logger.LogError("AffiliateModel(): AffiliatedOrderSearchModel → {AffiliatedOrderSearchModel}", AffiliatedOrderSearchModel);
        AffiliatedCustomerSearchModel = new AffiliatedCustomerSearchModel();
		_logger.LogError("AffiliateModel(): AffiliatedCustomerSearchModel → {AffiliatedCustomerSearchModel}", AffiliatedCustomerSearchModel);
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