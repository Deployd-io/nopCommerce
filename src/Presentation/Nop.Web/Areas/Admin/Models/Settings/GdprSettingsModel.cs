using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a GDPR settings model
/// </summary>
public partial record GdprSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GdprSettingsModel()
    {
		_logger.LogDebug(">>> Entering GdprSettingsModel()");
        GdprConsentSearchModel = new GdprConsentSearchModel();
		_logger.LogDebug("GdprSettingsModel(): GdprConsentSearchModel → {GdprConsentSearchModel}", GdprConsentSearchModel);
		_logger.LogDebug("<<< Exiting GdprSettingsModel()");
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.GdprEnabled")]
    public bool GdprEnabled { get; set; }
    public bool GdprEnabled_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogPrivacyPolicyConsent")]
    public bool LogPrivacyPolicyConsent { get; set; }
    public bool LogPrivacyPolicyConsent_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogNewsletterConsent")]
    public bool LogNewsLetterConsent { get; set; }
    public bool LogNewsLetterConsent_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.LogUserProfileChanges")]
    public bool LogUserProfileChanges { get; set; }
    public bool LogUserProfileChanges_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Gdpr.DeleteInactiveCustomersAfterMonths")]
    public int DeleteInactiveCustomersAfterMonths { get; set; }
    public bool DeleteInactiveCustomersAfterMonths_OverrideForStore { get; set; }

    public GdprConsentSearchModel GdprConsentSearchModel { get; set; }

    #endregion
}