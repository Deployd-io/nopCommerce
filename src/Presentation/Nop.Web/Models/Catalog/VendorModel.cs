using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record VendorModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public VendorModel()
    {
		_logger.LogDebug(">>> Entering VendorModel()");
        PictureModel = new PictureModel();
        CatalogProductsModel = new CatalogProductsModel();
		_logger.LogDebug("VendorModel(): PictureModel → {PictureModel}", PictureModel);
		_logger.LogDebug("VendorModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
        ProductReviews = new VendorProductReviewsListModel();
		_logger.LogDebug("<<< Exiting VendorModel()");
		_logger.LogDebug("VendorModel(): ProductReviews → {ProductReviews}", ProductReviews);
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