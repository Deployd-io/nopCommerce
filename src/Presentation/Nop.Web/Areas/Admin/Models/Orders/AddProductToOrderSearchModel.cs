using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a product search model to add to the order
/// </summary>
public partial record AddProductToOrderSearchModel : BaseSearchModel
{
    #region Ctor

    public AddProductToOrderSearchModel()
    {
		logger.LogDebug(">>> Entering AddProductToOrderSearchModel()");
        AvailableCategories = new List<SelectListItem>();
        AvailableManufacturers = new List<SelectListItem>();
		logger.LogDebug("AddProductToOrderSearchModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
		logger.LogDebug("AddProductToOrderSearchModel(): AvailableManufacturers → {AvailableManufacturers}", AvailableManufacturers);
        AvailableProductTypes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting AddProductToOrderSearchModel()");
		logger.LogDebug("AddProductToOrderSearchModel(): AvailableProductTypes → {AvailableProductTypes}", AvailableProductTypes);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
    public string SearchProductName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
    public int SearchCategoryId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
    public int SearchManufacturerId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
    public int SearchProductTypeId { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    public IList<SelectListItem> AvailableManufacturers { get; set; }

    public IList<SelectListItem> AvailableProductTypes { get; set; }

    public int OrderId { get; set; }

    #endregion
}