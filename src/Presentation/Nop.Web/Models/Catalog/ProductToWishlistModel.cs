using Nop.Web.Framework.Models;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Models.Catalog;

public partial record ProductToWishlistModel : BaseNopModel
{
    public int ProductId { get; set; }
    public IList<CustomWishlistModel> CustomWishlistItems { get; set; }
    
    public ProductToWishlistModel()
    {
		logger.LogDebug(">>> Entering ProductToWishlistModel()");
        CustomWishlistItems = new List<CustomWishlistModel>();
		logger.LogDebug("<<< Exiting ProductToWishlistModel()");
		logger.LogDebug("ProductToWishlistModel(): CustomWishlistItems → {CustomWishlistItems}", CustomWishlistItems);
    }
}
