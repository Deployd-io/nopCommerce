using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart;

public partial record MiniShoppingCartModel : BaseNopModel
{
    public MiniShoppingCartModel()
    {
		logger.LogDebug(">>> Entering MiniShoppingCartModel()");
        Items = new List<ShoppingCartItemModel>();
		logger.LogDebug("<<< Exiting MiniShoppingCartModel()");
		logger.LogDebug("MiniShoppingCartModel(): Items → {Items}", Items);
    }

    public IList<ShoppingCartItemModel> Items { get; set; }
    public int TotalProducts { get; set; }
    public string SubTotal { get; set; }
    public decimal SubTotalValue { get; set; }
    public bool DisplayShoppingCartButton { get; set; }
    public bool DisplayCheckoutButton { get; set; }
    public bool CurrentCustomerIsGuest { get; set; }
    public bool AnonymousCheckoutAllowed { get; set; }
    public bool ShowProductImages { get; set; }

    #region Nested Classes

    public partial record ShoppingCartItemModel : BaseNopEntityModel
    {
        public ShoppingCartItemModel()
        {
			logger.LogDebug(">>> Entering ShoppingCartItemModel()");
            Picture = new PictureModel();
			logger.LogDebug("<<< Exiting ShoppingCartItemModel()");
			logger.LogDebug("ShoppingCartItemModel(): Picture → {Picture}", Picture);
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string AttributeInfo { get; set; }

        public PictureModel Picture { get; set; }
    }

    #endregion
}