using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CompareProductsModel : BaseNopEntityModel
{
    public CompareProductsModel()
    {
		logger.LogDebug(">>> Entering CompareProductsModel()");
        Products = new List<ProductOverviewModel>();
		logger.LogDebug("<<< Exiting CompareProductsModel()");
		logger.LogDebug("CompareProductsModel(): Products → {Products}", Products);
    }
    public IList<ProductOverviewModel> Products { get; set; }

    public bool IncludeShortDescriptionInCompareProducts { get; set; }
    public bool IncludeFullDescriptionInCompareProducts { get; set; }
}