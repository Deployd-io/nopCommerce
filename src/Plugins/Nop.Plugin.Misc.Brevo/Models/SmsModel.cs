using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Models;

/// <summary>
/// Represents SMS model
/// </summary>
public record SmsModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SmsModel()
    {
		_logger.LogDebug(">>> Entering SmsModel()");
        AvailableMessages = new List<SelectListItem>();
		_logger.LogDebug("SmsModel(): AvailableMessages → {AvailableMessages}", AvailableMessages);
        AvailablePhoneTypes = new List<SelectListItem>();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
    public int MessageId { get; set; }

    public IList<SelectListItem> AvailableMessages { get; set; }

    public string DefaultSelectedMessageId { get; set; }

    public string Name { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.PhoneType")]
    public int PhoneTypeId { get; set; }

    public IList<SelectListItem> AvailablePhoneTypes { get; set; }

    public string DefaultSelectedPhoneTypeId { get; set; }

    public string PhoneType { get; set; }

    [NopResourceDisplayName("Plugins.Misc.Brevo.SMSText")]
    public string Text { get; set; }

    #endregion
}