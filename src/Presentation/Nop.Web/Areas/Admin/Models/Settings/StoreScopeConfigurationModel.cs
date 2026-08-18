using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Settings;

/// <summary>
/// Represents a store scope configuration model
/// </summary>
public partial record StoreScopeConfigurationModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public StoreScopeConfigurationModel()
    {
		_logger.LogDebug(">>> Entering StoreScopeConfigurationModel()");
        Stores = new List<StoreModel>();
		_logger.LogDebug("StoreScopeConfigurationModel(): Stores → {Stores}", Stores);
		_logger.LogDebug("<<< Exiting StoreScopeConfigurationModel()");
    }

    #endregion

    #region Properties

    public int StoreId { get; set; }

    public IList<StoreModel> Stores { get; set; }

    #endregion
}