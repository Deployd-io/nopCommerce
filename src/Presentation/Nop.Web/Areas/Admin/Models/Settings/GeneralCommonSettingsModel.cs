using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a general and common settings model
/// </summary>
public partial record GeneralCommonSettingsModel : BaseNopModel, ISettingsModel
{
    #region Ctor

    public GeneralCommonSettingsModel()
    {
		logger.LogDebug(">>> Entering GeneralCommonSettingsModel()");
        StoreInformationSettings = new StoreInformationSettingsModel();
        SitemapSettings = new SitemapSettingsModel();
		logger.LogDebug("GeneralCommonSettingsModel(): StoreInformationSettings → {StoreInformationSettings}", StoreInformationSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): SitemapSettings → {SitemapSettings}", SitemapSettings);
        SeoSettings = new SeoSettingsModel();
        SecuritySettings = new SecuritySettingsModel();
		logger.LogDebug("GeneralCommonSettingsModel(): SeoSettings → {SeoSettings}", SeoSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): SecuritySettings → {SecuritySettings}", SecuritySettings);
        CaptchaSettings = new CaptchaSettingsModel();
        PdfSettings = new PdfSettingsModel();
		logger.LogDebug("GeneralCommonSettingsModel(): CaptchaSettings → {CaptchaSettings}", CaptchaSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): PdfSettings → {PdfSettings}", PdfSettings);
        LocalizationSettings = new LocalizationSettingsModel();
        TranslationSettings = new TranslationSettingsModel();
		logger.LogDebug("GeneralCommonSettingsModel(): LocalizationSettings → {LocalizationSettings}", LocalizationSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): TranslationSettings → {TranslationSettings}", TranslationSettings);
        AdminAreaSettings = new AdminAreaSettingsModel();
        MinificationSettings = new MinificationSettingsModel();
		logger.LogDebug("GeneralCommonSettingsModel(): AdminAreaSettings → {AdminAreaSettings}", AdminAreaSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): MinificationSettings → {MinificationSettings}", MinificationSettings);
        CustomHtmlSettings = new CustomHtmlSettingsModel();
        RobotsTxtSettings = new RobotsTxtSettingsModel();
		logger.LogDebug("<<< Exiting GeneralCommonSettingsModel()");
		logger.LogDebug("GeneralCommonSettingsModel(): CustomHtmlSettings → {CustomHtmlSettings}", CustomHtmlSettings);
		logger.LogDebug("GeneralCommonSettingsModel(): RobotsTxtSettings → {RobotsTxtSettings}", RobotsTxtSettings);
    }

    #endregion

    #region Properties

    public int ActiveStoreScopeConfiguration { get; set; }

    public StoreInformationSettingsModel StoreInformationSettings { get; set; }

    public SitemapSettingsModel SitemapSettings { get; set; }

    public SeoSettingsModel SeoSettings { get; set; }

    public SecuritySettingsModel SecuritySettings { get; set; }

    public CaptchaSettingsModel CaptchaSettings { get; set; }

    public PdfSettingsModel PdfSettings { get; set; }

    public LocalizationSettingsModel LocalizationSettings { get; set; }

    public TranslationSettingsModel TranslationSettings { get; set; }

    public AdminAreaSettingsModel AdminAreaSettings { get; set; }

    public MinificationSettingsModel MinificationSettings { get; set; }

    public CustomHtmlSettingsModel CustomHtmlSettings { get; set; }

    public RobotsTxtSettingsModel RobotsTxtSettings { get; set; }

    #endregion
}