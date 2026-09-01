using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Pickup.PickupInStore.Models;

public record StorePickupPointModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public StorePickupPointModel()
    {
		_logger.LogDebug(">>> Entering StorePickupPointModel()");
        Address = new AddressModel();
		_logger.LogDebug("StorePickupPointModel(): Address → {Address}", Address);
        AvailableStores = new List<SelectListItem>();
    }

    public AddressModel Address { get; set; }

    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.PickupFee")]
    public decimal PickupFee { get; set; }

    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.OpeningHours")]
    public string OpeningHours { get; set; }

    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public List<SelectListItem> AvailableStores { get; set; }
    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Store")]
    public int StoreId { get; set; }
    public string StoreName { get; set; }

    [UIHint("DecimalNullable")]
    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Latitude")]
    public decimal? Latitude { get; set; }

    [UIHint("DecimalNullable")]
    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Longitude")]
    public decimal? Longitude { get; set; }

    [UIHint("Int32Nullable")]
    [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.TransitDays")]
    public int? TransitDays { get; set; }
}

public class AddressModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AddressModel()
    {
		_logger.LogDebug(">>> Entering AddressModel()");
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("AddressModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        AvailableStates = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Admin.Address.Fields.Country")]
    public int? CountryId { get; set; }
    public IList<SelectListItem> AvailableCountries { get; set; }

    [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
    public int? StateProvinceId { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }

    [NopResourceDisplayName("Admin.Address.Fields.County")]
    public string County { get; set; }

    [NopResourceDisplayName("Admin.Address.Fields.City")]
    public string City { get; set; }

    [NopResourceDisplayName("Admin.Address.Fields.Address1")]
    public string Address1 { get; set; }

    [NopResourceDisplayName("Admin.Address.Fields.ZipPostalCode")]
    public string ZipPostalCode { get; set; }
}