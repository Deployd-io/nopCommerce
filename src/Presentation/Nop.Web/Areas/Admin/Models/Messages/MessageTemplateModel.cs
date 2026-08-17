using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Messages;

/// <summary>
/// Represents a message template model
/// </summary>
public partial record MessageTemplateModel : BaseNopEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public MessageTemplateModel()
    {
		_logger.LogDebug(">>> Entering MessageTemplateModel()");
        Locales = new List<MessageTemplateLocalizedModel>();
        AvailableEmailAccounts = new List<SelectListItem>();
		_logger.LogDebug("MessageTemplateModel(): AvailableEmailAccounts → {AvailableEmailAccounts}", AvailableEmailAccounts);
		_logger.LogDebug("MessageTemplateModel(): Locales → {Locales}", Locales);

        SelectedStoreIds = new List<int>();
		_logger.LogDebug("MessageTemplateModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting MessageTemplateModel()");
		_logger.LogDebug("MessageTemplateModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
    public string AllowedTokens { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
    public string BccEmailAddresses { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
    public string Subject { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
    public bool IsActive { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
    public bool SendImmediately { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
    [UIHint("Int32Nullable")]
    public int? DelayBeforeSend { get; set; }

    public int DelayPeriodId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowDirectReply")]
    public bool AllowDirectReply { get; set; }

    public bool HasAttachedDownload { get; set; }
    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
    [UIHint("Download")]
    public int AttachedDownloadId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
    public int EmailAccountId { get; set; }

    public IList<SelectListItem> AvailableEmailAccounts { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    //comma-separated list of stores used on the list page
    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
    public string ListOfStores { get; set; }

    public IList<MessageTemplateLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record MessageTemplateLocalizedModel : ILocalizedLocaleModel
{
    public MessageTemplateLocalizedModel()
    {
		_logger.LogDebug(">>> Entering MessageTemplateLocalizedModel()");
        AvailableEmailAccounts = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting MessageTemplateLocalizedModel()");
		_logger.LogDebug("MessageTemplateLocalizedModel(): AvailableEmailAccounts → {AvailableEmailAccounts}", AvailableEmailAccounts);
    }

    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
    public string BccEmailAddresses { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
    public string Subject { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
    public string Body { get; set; }

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
    public int EmailAccountId { get; set; }
    public IList<SelectListItem> AvailableEmailAccounts { get; set; }
}