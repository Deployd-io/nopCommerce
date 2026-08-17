using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.ShoppingCart;

public partial record WishlistModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public WishlistModel()
    {
		_logger.LogDebug(">>> Entering WishlistModel()");
        Items = new List<ShoppingCartItemModel>();
        CustomWishlistItems = new List<CustomWishlistModel>();
		_logger.LogDebug("WishlistModel(): CustomWishlistItems → {CustomWishlistItems}", CustomWishlistItems);
		_logger.LogDebug("WishlistModel(): Items → {Items}", Items);
        Warnings = new List<string>();
		_logger.LogDebug("<<< Exiting WishlistModel()");
		_logger.LogDebug("WishlistModel(): Warnings → {Warnings}", Warnings);
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
			_logger.LogDebug(">>> Entering ShoppingCartItemModel()");
            Picture = new PictureModel();
            AllowedQuantities = new List<SelectListItem>();
			_logger.LogDebug("ShoppingCartItemModel(): AllowedQuantities → {AllowedQuantities}", AllowedQuantities);
			_logger.LogDebug("ShoppingCartItemModel(): Picture → {Picture}", Picture);
            Warnings = new List<string>();
			_logger.LogDebug("<<< Exiting ShoppingCartItemModel()");
			_logger.LogDebug("ShoppingCartItemModel(): Warnings → {Warnings}", Warnings);
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