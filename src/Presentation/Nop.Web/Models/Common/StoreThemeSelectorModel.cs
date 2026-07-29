using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record StoreThemeSelectorModel : BaseNopModel
{
    public StoreThemeSelectorModel()
    {
		logger.LogDebug(">>> Entering StoreThemeSelectorModel()");
        AvailableStoreThemes = new List<StoreThemeModel>();
		logger.LogDebug("<<< Exiting StoreThemeSelectorModel()");
		logger.LogDebug("StoreThemeSelectorModel(): AvailableStoreThemes → {AvailableStoreThemes}", AvailableStoreThemes);
    }

    public IList<StoreThemeModel> AvailableStoreThemes { get; set; }

    public StoreThemeModel CurrentStoreTheme { get; set; }
}