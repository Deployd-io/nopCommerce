using Nop.Web.Framework.Models;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record ProductToWishlistModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public int ProductId { get; set; }
    public IList<CustomWishlistModel> CustomWishlistItems { get; set; }
    
    public ProductToWishlistModel()
    {
		_logger.LogDebug(">>> Entering ProductToWishlistModel()");
        CustomWishlistItems = new List<CustomWishlistModel>();
		_logger.LogDebug("ProductToWishlistModel(): CustomWishlistItems → {CustomWishlistItems}", CustomWishlistItems);
		_logger.LogDebug("<<< Exiting ProductToWishlistModel()");
    }
}
