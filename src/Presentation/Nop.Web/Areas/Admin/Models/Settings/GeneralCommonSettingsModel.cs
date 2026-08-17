using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a general and common settings model
/// </summary>
public partial record GeneralCommonSettingsModel : BaseNopModel, ISettingsModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public GeneralCommonSettingsModel()
    {
		_logger.LogDebug(">>> Entering GeneralCommonSettingsModel()");
        StoreInformationSettings = new StoreInformationSettingsModel();
        SitemapSettings = new SitemapSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): SitemapSettings → {SitemapSettings}", SitemapSettings);
		_logger.LogDebug("GeneralCommonSettingsModel(): StoreInformationSettings → {StoreInformationSettings}", StoreInformationSettings);
        SeoSettings = new SeoSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): SeoSettings → {SeoSettings}", SeoSettings);
        SecuritySettings = new SecuritySettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): SecuritySettings → {SecuritySettings}", SecuritySettings);
        CaptchaSettings = new CaptchaSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): CaptchaSettings → {CaptchaSettings}", CaptchaSettings);
        PdfSettings = new PdfSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): PdfSettings → {PdfSettings}", PdfSettings);
        LocalizationSettings = new LocalizationSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): LocalizationSettings → {LocalizationSettings}", LocalizationSettings);
        TranslationSettings = new TranslationSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): TranslationSettings → {TranslationSettings}", TranslationSettings);
        AdminAreaSettings = new AdminAreaSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): AdminAreaSettings → {AdminAreaSettings}", AdminAreaSettings);
        MinificationSettings = new MinificationSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): MinificationSettings → {MinificationSettings}", MinificationSettings);
        CustomHtmlSettings = new CustomHtmlSettingsModel();
		_logger.LogDebug("GeneralCommonSettingsModel(): CustomHtmlSettings → {CustomHtmlSettings}", CustomHtmlSettings);
        RobotsTxtSettings = new RobotsTxtSettingsModel();
		_logger.LogDebug("<<< Exiting GeneralCommonSettingsModel()");
		_logger.LogDebug("GeneralCommonSettingsModel(): RobotsTxtSettings → {RobotsTxtSettings}", RobotsTxtSettings);
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