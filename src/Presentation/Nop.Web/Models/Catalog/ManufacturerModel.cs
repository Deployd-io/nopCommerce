using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record ManufacturerModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public ManufacturerModel()
    {
		logger.LogDebug(">>> Entering ManufacturerModel()");
        PictureModel = new PictureModel();
        FeaturedProducts = new List<ProductOverviewModel>();
		logger.LogDebug("ManufacturerModel(): PictureModel → {PictureModel}", PictureModel);
		logger.LogDebug("ManufacturerModel(): FeaturedProducts → {FeaturedProducts}", FeaturedProducts);
        CatalogProductsModel = new CatalogProductsModel();
		logger.LogDebug("<<< Exiting ManufacturerModel()");
		logger.LogDebug("ManufacturerModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
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