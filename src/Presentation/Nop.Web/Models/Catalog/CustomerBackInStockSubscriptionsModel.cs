using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Catalog;

public partial record CustomerBackInStockSubscriptionsModel : BaseNopModel
{
    public CustomerBackInStockSubscriptionsModel()
    {
		logger.LogDebug(">>> Entering CustomerBackInStockSubscriptionsModel()");
        Subscriptions = new List<BackInStockSubscriptionModel>();
		logger.LogDebug("<<< Exiting CustomerBackInStockSubscriptionsModel()");
		logger.LogDebug("CustomerBackInStockSubscriptionsModel(): Subscriptions → {Subscriptions}", Subscriptions);
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