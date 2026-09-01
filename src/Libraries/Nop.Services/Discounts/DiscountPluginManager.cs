using Nop.Services.Customers;
using Nop.Services.Plugins;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Discounts;

/// <summary>
/// Represents a discount requirement plugin manager implementation
/// </summary>
public partial class DiscountPluginManager : PluginManager<IDiscountRequirementRule>, IDiscountPluginManager
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DiscountPluginManager(ICustomerService customerService,
        IPluginService pluginService) : base(customerService, pluginService)
    {
	_logger.LogDebug(">>> Entering DiscountPluginManager(customerService={CustomerService},pluginService={PluginService})", customerService, pluginService);
    }

    #endregion
}