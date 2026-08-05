using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.UPS.Models;

public record UPSShippingModel : BaseNopModel
{
    #region Ctor

    public UPSShippingModel()
    {
		logger.LogDebug(">>> Entering UPSShippingModel()");
        CarrierServices = new List<string>();
        AvailableCarrierServices = new List<SelectListItem>();
		logger.LogDebug("UPSShippingModel(): CarrierServices → {CarrierServices}", CarrierServices);
		logger.LogDebug("UPSShippingModel(): AvailableCarrierServices → {AvailableCarrierServices}", AvailableCarrierServices);
        AvailableCustomerClassifications = new List<SelectListItem>();
        AvailablePickupTypes = new List<SelectListItem>();
		logger.LogDebug("UPSShippingModel(): AvailableCustomerClassifications → {AvailableCustomerClassifications}", AvailableCustomerClassifications);
		logger.LogDebug("UPSShippingModel(): AvailablePickupTypes → {AvailablePickupTypes}", AvailablePickupTypes);
        AvailablePackagingTypes = new List<SelectListItem>();
        AvaliablePackingTypes = new List<SelectListItem>();
		logger.LogDebug("UPSShippingModel(): AvailablePackagingTypes → {AvailablePackagingTypes}", AvailablePackagingTypes);
		logger.LogDebug("UPSShippingModel(): AvaliablePackingTypes → {AvaliablePackingTypes}", AvaliablePackingTypes);
        AvaliableWeightTypes = new List<SelectListItem>();
        AvaliableDimensionsTypes = new List<SelectListItem>();
		logger.LogDebug("<<< Exiting UPSShippingModel()");
		logger.LogDebug("UPSShippingModel(): AvaliableWeightTypes → {AvaliableWeightTypes}", AvaliableWeightTypes);
		logger.LogDebug("UPSShippingModel(): AvaliableDimensionsTypes → {AvaliableDimensionsTypes}", AvaliableDimensionsTypes);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AccountNumber")]
    public string AccountNumber { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.ClientId")]
    public string ClientId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.ClientSecret")]
    public string ClientSecret { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge")]
    public decimal AdditionalHandlingCharge { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.InsurePackage")]
    public bool InsurePackage { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.CustomerClassification")]
    public int CustomerClassification { get; set; }
    public IList<SelectListItem> AvailableCustomerClassifications { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PickupType")]
    public int PickupType { get; set; }
    public IList<SelectListItem> AvailablePickupTypes { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackagingType")]
    public int PackagingType { get; set; }
    public IList<SelectListItem> AvailablePackagingTypes { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AvailableCarrierServices")]
    public IList<SelectListItem> AvailableCarrierServices { get; set; }
    public IList<string> CarrierServices { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled")]
    public bool SaturdayDeliveryEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PassDimensions")]
    public bool PassDimensions { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingPackageVolume")]
    public int PackingPackageVolume { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingType")]
    public int PackingType { get; set; }
    public IList<SelectListItem> AvaliablePackingTypes { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Tracing")]
    public bool Tracing { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.WeightType")]
    public string WeightType { get; set; }
    public IList<SelectListItem> AvaliableWeightTypes { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.DimensionsType")]
    public string DimensionsType { get; set; }
    public IList<SelectListItem> AvaliableDimensionsTypes { get; set; }

    #endregion
}