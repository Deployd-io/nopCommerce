using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record VendorModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public VendorModel()
    {
		logger.LogDebug(">>> Entering VendorModel()");
        PictureModel = new PictureModel();
        CatalogProductsModel = new CatalogProductsModel();
		logger.LogDebug("VendorModel(): PictureModel → {PictureModel}", PictureModel);
		logger.LogDebug("VendorModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
        ProductReviews = new VendorProductReviewsListModel();
		logger.LogDebug("<<< Exiting VendorModel()");
		logger.LogDebug("VendorModel(): ProductReviews → {ProductReviews}", ProductReviews);
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }
    public bool AllowCustomersToContactVendors { get; set; }
    public int? PmCustomerId { get; set; }

    public PictureModel PictureModel { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }

    public VendorProductReviewsListModel ProductReviews { get; set; }
}