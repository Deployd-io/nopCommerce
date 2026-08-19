using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an admin language selector model
/// </summary>
public partial record LanguageSelectorModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public LanguageSelectorModel()
    {
		_logger.LogDebug(">>> Entering LanguageSelectorModel()");
        AvailableLanguages = new List<LanguageModel>();
		_logger.LogDebug("LanguageSelectorModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
		_logger.LogDebug("<<< Exiting LanguageSelectorModel()");
    }

    #endregion

    #region Properties

    public IList<LanguageModel> AvailableLanguages { get; set; }

    public LanguageModel CurrentLanguage { get; set; }

    #endregion
}