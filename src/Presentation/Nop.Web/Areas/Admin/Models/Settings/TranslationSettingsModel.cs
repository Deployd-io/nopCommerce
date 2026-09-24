using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a translation settings model
/// </summary>
public partial record TranslationSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TranslationSettingsModel()
    {
		_logger.LogError(">>> Entering TranslationSettingsModel()");
        AvailableLanguages = new List<SelectListItem>();
        NotTranslateLanguages = new List<int>();
		_logger.LogError("TranslationSettingsModel(): NotTranslateLanguages → {NotTranslateLanguages}", NotTranslateLanguages);
        AvailableTranslationService = new List<SelectListItem>();
		_logger.LogError("TranslationSettingsModel(): AvailableTranslationService → {AvailableTranslationService}", AvailableTranslationService);
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.AllowPreTranslate")]
    public bool AllowPreTranslate { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TranslateFromLanguage")]
    public int TranslateFromLanguageId { get; set; }
    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.NotTranslateLanguages")]
    public IList<int> NotTranslateLanguages { get; set; }
    public IList<SelectListItem> AvailableLanguages { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.GoogleTranslateApiKey")]
    public string GoogleApiKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DeepLAuthKey")]
    public string DeepLAuthKey { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.TranslationService")]
    public int TranslationServiceId { get; set; }
    public IList<SelectListItem> AvailableTranslationService { get; set; }

    #endregion
}