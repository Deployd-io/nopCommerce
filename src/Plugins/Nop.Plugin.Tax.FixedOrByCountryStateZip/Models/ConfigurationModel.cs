using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Models;

public record ConfigurationModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public ConfigurationModel()
    {
		_logger.LogDebug(">>> Entering ConfigurationModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("ConfigurationModel(): AvailableStores → {AvailableStores}", AvailableStores);
        AvailableCountries = new List<SelectListItem>();
		_logger.LogDebug("ConfigurationModel(): AvailableCountries → {AvailableCountries}", AvailableCountries);
        AvailableStates = new List<SelectListItem>();
		_logger.LogDebug("ConfigurationModel(): AvailableStates → {AvailableStates}", AvailableStates);
        AvailableTaxCategories = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store")]
    public int AddStoreId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country")]
    public int AddCountryId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince")]
    public int AddStateProvinceId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip")]
    public string AddZip { get; set; }
    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory")]
    public int AddTaxCategoryId { get; set; }
    [NopResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage")]
    public decimal AddPercentage { get; set; }

    public bool CountryStateZipEnabled { get; set; }

    public string TaxCategoriesCanNotLoadedError { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }
    public IList<SelectListItem> AvailableCountries { get; set; }
    public IList<SelectListItem> AvailableStates { get; set; }
    public IList<SelectListItem> AvailableTaxCategories { get; set; }
}