using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR consent model
/// </summary>
public partial record GdprConsentModel : BaseNopEntityModel, ILocalizedModel<GdprConsentLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GdprConsentModel()
    {
		_logger.LogDebug(">>> Entering GdprConsentModel()");
        Locales = new List<GdprConsentLocalizedModel>();
		_logger.LogDebug("<<< Exiting GdprConsentModel()");
		_logger.LogDebug("GdprConsentModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.Message")]
    public string Message { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.RequiredMessage")]
    public string RequiredMessage { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayDuringRegistration")]
    public bool DisplayDuringRegistration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOnCustomerInfoPage")]
    public bool DisplayOnCustomerInfoPage { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.Consent.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<GdprConsentLocalizedModel> Locales { get; set; }

    #endregion
}