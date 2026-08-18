using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Models;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ConfigurationModel()
    {
		_logger.LogDebug(">>> Entering ConfigurationModel()");
        AvailableLists = new List<SelectListItem>();
        NewsLetterSubscriptionTypes = new List<NewsLetterSubscriptionMapModel>();
		_logger.LogDebug("ConfigurationModel(): NewsLetterSubscriptionTypes → {NewsLetterSubscriptionTypes}", NewsLetterSubscriptionTypes);
		_logger.LogDebug("ConfigurationModel(): AvailableLists → {AvailableLists}", AvailableLists);
        AvailableSenders = new List<SelectListItem>();
		_logger.LogDebug("ConfigurationModel(): AvailableSenders → {AvailableSenders}", AvailableSenders);
        AvailableMessageTemplates = new List<SelectListItem>();
		_logger.LogDebug("ConfigurationModel(): AvailableMessageTemplates → {AvailableMessageTemplates}", AvailableMessageTemplates);
        MessageTemplateSearchModel = new BrevoMessageTemplateSearchModel();
		_logger.LogDebug("ConfigurationModel(): MessageTemplateSearchModel → {MessageTemplateSearchModel}", MessageTemplateSearchModel);
        SmsSearchModel = new SmsSearchModel();
		_logger.LogDebug("ConfigurationModel(): SmsSearchModel → {SmsSearchModel}", SmsSearchModel);
        AddSms = new SmsModel();
		_logger.LogDebug("ConfigurationModel(): AddSms → {AddSms}", AddSms);
		_logger.LogDebug("<<< Exiting ConfigurationModel()");
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.ApiKey")]
    public string ApiKey { get; set; }

    public IList<SelectListItem> AvailableLists { get; set; }

    public IList<NewsLetterSubscriptionMapModel> NewsLetterSubscriptionTypes { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.SmtpKey")]
    public string SmtpKey { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.UseSmtp")]
    public bool UseSmtp { get; set; }
    public bool UseSmtp_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.Sender")]
    public string SenderId { get; set; }
    public bool SenderId_OverrideForStore { get; set; }
    public IList<SelectListItem> AvailableSenders { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.UseSmsNotifications")]
    public bool UseSmsNotifications { get; set; }
    public bool UseSmsNotifications_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.SmsSenderName")]
    public string SmsSenderName { get; set; }
    public bool SmsSenderName_OverrideForStore { get; set; }

    [DataType(DataType.PhoneNumber)]
    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.StoreOwnerPhoneNumber")]
    public string StoreOwnerPhoneNumber { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.CampaignList")]
    public int CampaignListId { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.CampaignSenderName")]
    public string CampaignSenderName { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.CampaignText")]
    public string CampaignText { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.MaKey")]
    public string MarketingAutomationKey { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.UseMarketingAutomation")]
    public bool UseMarketingAutomation { get; set; }
    public bool UseMarketingAutomation_OverrideForStore { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.AccountInfo")]
    public string AccountInfo { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.AllowedTokens")]
    public string AllowedTokens { get; set; }

    public IList<SelectListItem> AvailableMessageTemplates { get; set; }

    public bool MarketingAutomationDisabled { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.Fields.TrackingScript")]
    public string TrackingScript { get; set; }

    public bool HideGeneralBlock { get; set; }

    public bool HideSynchronizationBlock { get; set; }

    public bool HideTransactionalBlock { get; set; }

    public bool HideSmsBlock { get; set; }

    public bool HideMarketingAutomationBlock { get; set; }

    public BrevoMessageTemplateSearchModel MessageTemplateSearchModel { get; set; }

    public SmsSearchModel SmsSearchModel { get; set; }

    public SmsModel AddSms { get; set; }

    #endregion
}