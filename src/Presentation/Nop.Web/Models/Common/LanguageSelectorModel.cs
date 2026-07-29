using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record LanguageSelectorModel : BaseNopModel
{
    public LanguageSelectorModel()
    {
		logger.LogDebug(">>> Entering LanguageSelectorModel()");
        AvailableLanguages = new List<LanguageModel>();
		logger.LogDebug("<<< Exiting LanguageSelectorModel()");
		logger.LogDebug("LanguageSelectorModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
    }

    public IList<LanguageModel> AvailableLanguages { get; set; }

    public int CurrentLanguageId { get; set; }

    public bool UseImages { get; set; }
}