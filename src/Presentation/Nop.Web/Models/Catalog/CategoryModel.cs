using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Models.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Catalog;

public partial record CategoryModel : BaseNopEntityModel, IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CategoryModel()
    {
		_logger.LogDebug(">>> Entering CategoryModel()");
        PictureModel = new PictureModel();
        FeaturedProducts = new List<ProductOverviewModel>();
		_logger.LogDebug("CategoryModel(): FeaturedProducts → {FeaturedProducts}", FeaturedProducts);
		_logger.LogDebug("CategoryModel(): PictureModel → {PictureModel}", PictureModel);
        SubCategories = new List<SubCategoryModel>();
		_logger.LogDebug("CategoryModel(): SubCategories → {SubCategories}", SubCategories);
        CategoryBreadcrumb = new List<CategoryModel>();
		_logger.LogDebug("CategoryModel(): CategoryBreadcrumb → {CategoryBreadcrumb}", CategoryBreadcrumb);
        CatalogProductsModel = new CatalogProductsModel();
		_logger.LogDebug("<<< Exiting CategoryModel()");
		_logger.LogDebug("CategoryModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
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
			_logger.LogDebug(">>> Entering SubCategoryModel()");
            PictureModel = new PictureModel();
			_logger.LogDebug("<<< Exiting SubCategoryModel()");
			_logger.LogDebug("SubCategoryModel(): PictureModel → {PictureModel}", PictureModel);
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public string Description { get; set; }

        public PictureModel PictureModel { get; set; }
    }

    #endregion
}