using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart;

public partial record WishlistModel : BaseNopModel
{
    public WishlistModel()
    {
		logger.LogDebug(">>> Entering WishlistModel()");
        Items = new List<ShoppingCartItemModel>();
        CustomWishlistItems = new List<CustomWishlistModel>();
		logger.LogDebug("WishlistModel(): Items → {Items}", Items);
		logger.LogDebug("WishlistModel(): CustomWishlistItems → {CustomWishlistItems}", CustomWishlistItems);
        Warnings = new List<string>();
		logger.LogDebug("<<< Exiting WishlistModel()");
		logger.LogDebug("WishlistModel(): Warnings → {Warnings}", Warnings);
    }

    public int? ListId { get; set; }

    public Guid CustomerGuid { get; set; }
    public string CustomerFullname { get; set; }

    public bool EmailWishlistEnabled { get; set; }

    public bool ShowSku { get; set; }

    public bool ShowProductImages { get; set; }

    public bool IsEditable { get; set; }

    public bool AllowMultipleWishlist { get; set; }

    public IList<CustomWishlistModel> CustomWishlistItems { get; set; }

    public string CustomWishlistName { get; set; }

    public bool DisplayAddToCart { get; set; }

    public bool DisplayTaxShippingInfo { get; set; }

    public IList<ShoppingCartItemModel> Items { get; set; }

    public IList<string> Warnings { get; set; }

    #region Nested Classes    

    public partial record ShoppingCartItemModel : BaseNopEntityModel
    {
        public ShoppingCartItemModel()
        {
			logger.LogDebug(">>> Entering ShoppingCartItemModel()");
            Picture = new PictureModel();
            AllowedQuantities = new List<SelectListItem>();
			logger.LogDebug("ShoppingCartItemModel(): Picture → {Picture}", Picture);
			logger.LogDebug("ShoppingCartItemModel(): AllowedQuantities → {AllowedQuantities}", AllowedQuantities);
            Warnings = new List<string>();
			logger.LogDebug("<<< Exiting ShoppingCartItemModel()");
			logger.LogDebug("ShoppingCartItemModel(): Warnings → {Warnings}", Warnings);
        }

        public string Sku { get; set; }

        public PictureModel Picture { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string SubTotal { get; set; }
        public decimal SubTotalValue { get; set; }

        public string Discount { get; set; }
        public decimal DiscountValue { get; set; }
        public int? MaximumDiscountedQty { get; set; }

        public int Quantity { get; set; }
        public List<SelectListItem> AllowedQuantities { get; set; }

        public string AttributeInfo { get; set; }

        public string RecurringInfo { get; set; }

        public string RentalInfo { get; set; }

        public bool AllowItemEditing { get; set; }

        public IList<string> Warnings { get; set; }
    }

    #endregion
}