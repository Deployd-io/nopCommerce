using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.ShoppingCart;

/// <summary>
/// Represents a shopping cart search model
/// </summary>
public partial record ShoppingCartSearchModel : BaseSearchModel
{
    #region Ctor

    public ShoppingCartSearchModel()
    {
		logger.LogDebug(">>> Entering ShoppingCartSearchModel()");
        AvailableShoppingCartTypes = new List<SelectListItem>();
        ShoppingCartItemSearchModel = new ShoppingCartItemSearchModel();
		logger.LogDebug("ShoppingCartSearchModel(): AvailableShoppingCartTypes → {AvailableShoppingCartTypes}", AvailableShoppingCartTypes);
		logger.LogDebug("ShoppingCartSearchModel(): ShoppingCartItemSearchModel → {ShoppingCartItemSearchModel}", ShoppingCartItemSearchModel);
        AvailableStores = new List<SelectListItem>();
        AvailableCountries = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting ShoppingCartSearchModel()");
		logger.LogDebug("ShoppingCartSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("ShoppingCartSearchModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
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