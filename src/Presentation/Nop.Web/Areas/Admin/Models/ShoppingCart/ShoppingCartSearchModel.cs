using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart;

/// <summary>
/// Represents a shopping cart search model
/// </summary>
public partial record ShoppingCartSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ShoppingCartSearchModel()
    {
		_logger.LogError(">>> Entering ShoppingCartSearchModel()");
        AvailableShoppingCartTypes = new List<SelectListItem>();
        ShoppingCartItemSearchModel = new ShoppingCartItemSearchModel();
		_logger.LogError("ShoppingCartSearchModel(): ShoppingCartItemSearchModel → {ShoppingCartItemSearchModel}", ShoppingCartItemSearchModel);
        AvailableStores = new List<SelectListItem>();
		_logger.LogError("ShoppingCartSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogError("ShoppingCartSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.ShoppingCartType.ShoppingCartType")]
    public ShoppingCartType ShoppingCartType { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.StartDate")]
    [UIHint("DateNullable")]
    public DateTime? StartDate { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.EndDate")]
    [UIHint("DateNullable")]
    public DateTime? EndDate { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.Product")]
    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.BillingCountry")]
    public int BillingCountryId { get; set; }

    [NopResourceDisplayName("Admin.ShoppingCartType.Store")]
    public int StoreId { get; set; }

    public IList<SelectListItem> AvailableShoppingCartTypes { get; set; }

    public ShoppingCartItemSearchModel ShoppingCartItemSearchModel { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    public bool HideStoresList { get; set; }

    #endregion
}