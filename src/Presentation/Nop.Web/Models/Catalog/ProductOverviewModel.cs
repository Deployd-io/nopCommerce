using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record ProductOverviewModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ProductOverviewModel()
    {
		_logger.LogDebug(">>> Entering ProductOverviewModel()");
        ProductPrice = new ProductPriceModel();
        PictureModels = new List<PictureModel>();
		_logger.LogDebug("ProductOverviewModel(): PictureModels → {PictureModels}", PictureModels);
		_logger.LogDebug("ProductOverviewModel(): ProductPrice → {ProductPrice}", ProductPrice);
        ProductSpecificationModel = new ProductSpecificationModel();
		_logger.LogDebug("ProductOverviewModel(): ProductSpecificationModel → {ProductSpecificationModel}", ProductSpecificationModel);
        ReviewOverviewModel = new ProductReviewOverviewModel();
		_logger.LogDebug("ProductOverviewModel(): ReviewOverviewModel → {ReviewOverviewModel}", ReviewOverviewModel);
        ProductToWishlist = new ProductToWishlistModel();
		_logger.LogDebug("<<< Exiting ProductOverviewModel()");
		_logger.LogDebug("ProductOverviewModel(): ProductToWishlist → {ProductToWishlist}", ProductToWishlist);
    }

    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public string SeName { get; set; }

    public string Sku { get; set; }

    public ProductType ProductType { get; set; }

    public bool MarkAsNew { get; set; }

    //price
    public ProductPriceModel ProductPrice { get; set; }
    //pictures
    public IList<PictureModel> PictureModels { get; set; }
    //specification attributes
    public ProductSpecificationModel ProductSpecificationModel { get; set; }
    //price
    public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

    public ProductToWishlistModel ProductToWishlist { get; set; }
}