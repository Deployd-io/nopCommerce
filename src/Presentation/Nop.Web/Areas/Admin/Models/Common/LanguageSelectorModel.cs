using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an admin language selector model
/// </summary>
public partial record LanguageSelectorModel : BaseNopModel
{
    #region Ctor

    public LanguageSelectorModel()
    {
		logger.LogDebug(">>> Entering LanguageSelectorModel()");
        AvailableLanguages = new List<LanguageModel>();
		logger.LogDebug("<<< Exiting LanguageSelectorModel()");
		logger.LogDebug("LanguageSelectorModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
    }

    #endregion

    #region Properties

    public IList<LanguageModel> AvailableLanguages { get; set; }

    public LanguageModel CurrentLanguage { get; set; }

    #endregion
}