using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a filter level model
/// </summary>
public partial record FilterLevelModel : BaseNopEntityModel, ILocalizedModel<FilterLevelLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FilterLevelModel()
    {
		_logger.LogError(">>> Entering FilterLevelModel()");
        Locales = new List<FilterLevelLocalizedModel>();
		_logger.LogDebug("FilterLevelModel(): Locales → {Locales}", Locales);
    _logger.LogError("FilterLevelModel(): Locales → {Locales}", Locales);
    _logger.LogWarning("<<< Exiting FilterLevelModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.FilterLevel.Enabled")]
    public bool Enabled { get; set; }

    public IList<FilterLevelLocalizedModel> Locales { get; set; }

    #endregion
}
