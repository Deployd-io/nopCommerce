using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Models.Catalog;

public partial record ProductOverviewModel : BaseNopEntityModel
{
    public ProductOverviewModel()
    {
		logger.LogDebug(">>> Entering ProductOverviewModel()");
        ProductPrice = new ProductPriceModel();
        PictureModels = new List<PictureModel>();
		logger.LogDebug("ProductOverviewModel(): ProductPrice → {ProductPrice}", ProductPrice);
		logger.LogDebug("ProductOverviewModel(): PictureModels → {PictureModels}", PictureModels);
        ProductSpecificationModel = new ProductSpecificationModel();
        ReviewOverviewModel = new ProductReviewOverviewModel();
		logger.LogDebug("ProductOverviewModel(): ProductSpecificationModel → {ProductSpecificationModel}", ProductSpecificationModel);
		logger.LogDebug("ProductOverviewModel(): ReviewOverviewModel → {ReviewOverviewModel}", ReviewOverviewModel);
        ProductToWishlist = new ProductToWishlistModel();
		logger.LogDebug("<<< Exiting ProductOverviewModel()");
		logger.LogDebug("ProductOverviewModel(): ProductToWishlist → {ProductToWishlist}", ProductToWishlist);
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