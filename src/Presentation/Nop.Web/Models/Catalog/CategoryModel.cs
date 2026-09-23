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
		_logger.LogError(">>> Entering CategoryModel()");
        PictureModel = new PictureModel();
        FeaturedProducts = new List<ProductOverviewModel>();
		_logger.LogError("CategoryModel(): FeaturedProducts → {FeaturedProducts}", FeaturedProducts);
        SubCategories = new List<SubCategoryModel>();
		_logger.LogError("CategoryModel(): SubCategories → {SubCategories}", SubCategories);
        CategoryBreadcrumb = new List<CategoryModel>();
		_logger.LogError("CategoryModel(): CategoryBreadcrumb → {CategoryBreadcrumb}", CategoryBreadcrumb);
        CatalogProductsModel = new CatalogProductsModel();
		_logger.LogError("CategoryModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
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
    	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        public SubCategoryModel()
        {
			_logger.LogError(">>> Entering SubCategoryModel()");
            PictureModel = new PictureModel();
			_logger.LogDebug("SubCategoryModel(): PictureModel → {PictureModel}", PictureModel);
        _logger.LogError("SubCategoryModel(): PictureModel → {PictureModel}", PictureModel);
        _logger.LogWarning("<<< Exiting SubCategoryModel()");
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public string Description { get; set; }

        public PictureModel PictureModel { get; set; }
    }

    #endregion
}