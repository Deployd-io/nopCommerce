using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record CustomerBackInStockSubscriptionsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerBackInStockSubscriptionsModel()
    {
		_logger.LogError(">>> Entering CustomerBackInStockSubscriptionsModel()");
        Subscriptions = new List<BackInStockSubscriptionModel>();
		_logger.LogDebug("CustomerBackInStockSubscriptionsModel(): Subscriptions → {Subscriptions}", Subscriptions);
    _logger.LogError("CustomerBackInStockSubscriptionsModel(): Subscriptions → {Subscriptions}", Subscriptions);
    _logger.LogWarning("<<< Exiting CustomerBackInStockSubscriptionsModel()");
    }

    public IList<BackInStockSubscriptionModel> Subscriptions { get; set; }
    public PagerModel PagerModel { get; set; }

    #region Nested classes

    public partial record BackInStockSubscriptionModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SeName { get; set; }
    }

    #endregion
}