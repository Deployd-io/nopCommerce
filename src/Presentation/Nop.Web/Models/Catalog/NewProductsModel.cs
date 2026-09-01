using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

/// <summary>
/// Represents a new products model
/// </summary>
public partial record NewProductsModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Properties

    /// <summary>
    /// Gets or sets the catalog products model
    /// </summary>
    public CatalogProductsModel CatalogProductsModel { get; set; }

    #endregion

    #region Ctor

    public NewProductsModel()
    {
		_logger.LogDebug(">>> Entering NewProductsModel()");
        CatalogProductsModel = new CatalogProductsModel();
		_logger.LogDebug("NewProductsModel(): CatalogProductsModel → {CatalogProductsModel}", CatalogProductsModel);
    }

    #endregion
}