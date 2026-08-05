using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a campaign model
/// </summary>
public partial record CampaignModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CampaignModel()
    {
		_logger.LogDebug(">>> Entering CampaignModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("CampaignModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("CampaignModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
        AvailableNewsLetterSubscriptionTypes = new List<SelectListItem>();
        AvailableEmailAccounts = new List<SelectListItem>();
		_logger.LogDebug("CampaignModel(): AvailableNewsLetterSubscriptionTypes → {AvailableNewsLetterSubscriptionTypes}", AvailableNewsLetterSubscriptionTypes);
		_logger.LogDebug("CampaignModel(): AvailableEmailAccounts → {AvailableEmailAccounts}", AvailableEmailAccounts);
        CopyCampaignModel = new CopyCampaignModel();
		_logger.LogDebug("<<< Exiting CampaignModel()");
		_logger.LogDebug("CampaignModel(): CopyCampaignModel → {CopyCampaignModel}", CopyCampaignModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Subject")]
    public string Subject { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.Store")]
    public int StoreId { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.CustomerRole")]
    public int CustomerRoleId { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.NewsLetterSubscriptionType")]
    public int NewsLetterSubscriptionTypeId { get; set; }
    public IList<SelectListItem> AvailableNewsLetterSubscriptionTypes { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.CreatedOn")]
    public DateTime CreatedOn { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.DontSendBeforeDate")]
    [UIHint("DateTimeNullable")]
    public DateTime? DontSendBeforeDate { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.AllowedTokens")]
    public string AllowedTokens { get; set; }

    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.EmailAccount")]
    public int EmailAccountId { get; set; }
    public IList<SelectListItem> AvailableEmailAccounts { get; set; }

    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("Admin.Promotions.Campaigns.Fields.TestEmail")]
    public string TestEmail { get; set; }

    public CopyCampaignModel CopyCampaignModel { get; set; }

    #endregion
}