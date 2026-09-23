using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a related product model to add to the product
/// </summary>
public partial record AddRelatedProductModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddRelatedProductModel()
    {
		_logger.LogError(">>> Entering AddRelatedProductModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddRelatedProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogError("AddRelatedProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    _logger.LogWarning("<<< Exiting AddRelatedProductModel()");
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}