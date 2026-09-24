using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record StoreThemeSelectorModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public StoreThemeSelectorModel()
    {
		_logger.LogError(">>> Entering StoreThemeSelectorModel()");
        AvailableStoreThemes = new List<StoreThemeModel>();
		_logger.LogDebug("StoreThemeSelectorModel(): AvailableStoreThemes → {AvailableStoreThemes}", AvailableStoreThemes);
    _logger.LogError("StoreThemeSelectorModel(): AvailableStoreThemes → {AvailableStoreThemes}", AvailableStoreThemes);
    _logger.LogWarning("<<< Exiting StoreThemeSelectorModel()");
    }

    public IList<StoreThemeModel> AvailableStoreThemes { get; set; }

    public StoreThemeModel CurrentStoreTheme { get; set; }
}