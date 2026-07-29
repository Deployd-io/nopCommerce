using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record CategoryModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
    public CategoryModel()
    {
		logger.LogDebug(">>> Entering CategoryModel()");
        PictureModel = new PictureModel();
        FeaturedProducts = new List<ProductOverviewModel>();
		logger.LogDebug("CategoryModel(): PictureModel → {PictureModel}", PictureModel);
		logger.LogDebug("CategoryModel(): FeaturedProducts → {FeaturedProducts}", FeaturedProducts);
        SubCategories = new List<SubCategoryModel>();
        CategoryBreadcrumb = new List<CategoryModel>();
		logger.LogDebug("CategoryModel(): SubCategories → {SubCategories}", SubCategories);
		logger.LogDebug("CategoryModel(): CategoryBreadcrumb → {CategoryBreadcrumb}", CategoryBreadcrumb);
        CatalogProductsModel = new CatalogProductsModel();
		logger.LogDebug("<<< Exiting CategoryModel()");
		logger.LogDebug("CategoryModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }
    public string SeName { get; set; }

    public PictureModel PictureModel { get; set; }

    public bool DisplayCategoryBreadcrumb { get; set; }
    public IList<CategoryModel> CategoryBreadcrumb { get; set; }

    public IList<SubCategoryModel> SubCategories { get; set; }

    public IList<ProductOverviewModel> FeaturedProducts { get; set; }

    public CatalogProductsModel CatalogProductsModel { get; set; }

    public string JsonLd { get; set; }

    #region Nested Classes

    public partial record SubCategoryModel : BaseNopEntityModel
    {
        public SubCategoryModel()
        {
			logger.LogDebug(">>> Entering SubCategoryModel()");
            PictureModel = new PictureModel();
			logger.LogDebug("<<< Exiting SubCategoryModel()");
			logger.LogDebug("SubCategoryModel(): PictureModel → {PictureModel}", PictureModel);
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public string Description { get; set; }

        public PictureModel PictureModel { get; set; }
    }

    #endregion
}