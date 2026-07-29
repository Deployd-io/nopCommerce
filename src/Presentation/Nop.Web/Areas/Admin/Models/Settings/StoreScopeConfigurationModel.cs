using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a store scope configuration model
/// </summary>
public partial record StoreScopeConfigurationModel : BaseNopModel
{
    #region Ctor

    public StoreScopeConfigurationModel()
    {
		logger.LogDebug(">>> Entering StoreScopeConfigurationModel()");
        Stores = new List<StoreModel>();
		logger.LogDebug("<<< Exiting StoreScopeConfigurationModel()");
		logger.LogDebug("StoreScopeConfigurationModel(): Stores → {Stores}", Stores);
    }

    #endregion

    #region Properties

    public int StoreId { get; set; }

    public IList<StoreModel> Stores { get; set; }

    #endregion
}