using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents an associated product model to add to the product
/// </summary>
public partial record AddAssociatedProductModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddAssociatedProductModel()
    {
		_logger.LogDebug(">>> Entering AddAssociatedProductModel()");
        SelectedProductIds = new List<int>();
		_logger.LogDebug("AddAssociatedProductModel(): SelectedProductIds → {SelectedProductIds}", SelectedProductIds);
    }
    #endregion

    #region Properties

    public int ProductId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}