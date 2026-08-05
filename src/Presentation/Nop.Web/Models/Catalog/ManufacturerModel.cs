using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record ManufacturerModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ManufacturerModel()
    {
		_logger.LogDebug(">>> Entering ManufacturerModel()");
        PictureModel = new PictureModel();
        FeaturedProducts = new List<ProductOverviewModel>();
		_logger.LogDebug("ManufacturerModel(): PictureModel → {PictureModel}", PictureModel);
		_logger.LogDebug("ManufacturerModel(): FeaturedProducts → {FeaturedProducts}", FeaturedProducts);
        CatalogProductsModel = new CatalogProductsModel();
		_logger.LogDebug("<<< Exiting ManufacturerModel()");
		_logger.LogDebug("ManufacturerModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public PictureModel PictureModel { get; set; }

    public IList<ProductOverviewModel> FeaturedProducts { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }
}