using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a tier price model
/// </summary>
public partial record TierPriceModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public TierPriceModel()
    {
		_logger.LogError(">>> Entering TierPriceModel()");
        AvailableStores = new List<SelectListItem>();
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogError("TierPriceModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);
    }

    #endregion

    #region Properties

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.CustomerRole")]
    public int CustomerRoleId { get; set; }

    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    public string CustomerRole { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.Store")]
    public int StoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public string Store { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.Quantity")]
    public int Quantity { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.Price")]
    public decimal Price { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.Price")]
    public string FormattedPrice { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.StartDateTimeUtc")]
    [UIHint("DateTimeNullable")]
    public DateTime? StartDateTimeUtc { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.TierPrices.Fields.EndDateTimeUtc")]
    [UIHint("DateTimeNullable")]
    public DateTime? EndDateTimeUtc { get; set; }

    public string PrimaryStoreCurrencyCode { get; set; }

    #endregion
}