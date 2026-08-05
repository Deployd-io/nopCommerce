using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record LanguageSelectorModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public LanguageSelectorModel()
    {
		_logger.LogDebug(">>> Entering LanguageSelectorModel()");
        AvailableLanguages = new List<LanguageModel>();
		_logger.LogDebug("<<< Exiting LanguageSelectorModel()");
		_logger.LogDebug("LanguageSelectorModel(): AvailableLanguages → {AvailableLanguages}", AvailableLanguages);
    }

    public IList<LanguageModel> AvailableLanguages { get; set; }

    public int CurrentLanguageId { get; set; }

    public bool UseImages { get; set; }
}