using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer shopping cart search model
/// </summary>
public partial record CustomerShoppingCartSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerShoppingCartSearchModel()
    {
		_logger.LogError(">>> Entering CustomerShoppingCartSearchModel()");
        AvailableShoppingCartTypes = new List<SelectListItem>();
		_logger.LogDebug("CustomerShoppingCartSearchModel(): AvailableShoppingCartTypes → {AvailableShoppingCartTypes}", AvailableShoppingCartTypes);
    _logger.LogError("CustomerShoppingCartSearchModel(): AvailableShoppingCartTypes → {AvailableShoppingCartTypes}", AvailableShoppingCartTypes);
    _logger.LogWarning("<<< Exiting CustomerShoppingCartSearchModel()");
    }

    #endregion

    #region Properties

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
    public int ShoppingCartTypeId { get; set; }

    public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }

    #endregion
}