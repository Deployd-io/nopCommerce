using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record CompareProductsModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CompareProductsModel()
    {
		_logger.LogDebug(">>> Entering CompareProductsModel()");
        Products = new List<ProductOverviewModel>();
		_logger.LogDebug("CompareProductsModel(): Products → {Products}", Products);
    }
    public IList<ProductOverviewModel> Products { get; set; }

    public bool IncludeShortDescriptionInCompareProducts { get; set; }
    public bool IncludeFullDescriptionInCompareProducts { get; set; }
}