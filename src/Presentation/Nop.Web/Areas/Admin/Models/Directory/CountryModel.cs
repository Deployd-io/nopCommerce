using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a country model
/// </summary>
public partial record CountryModel : BaseNopEntityModel, ILocalizedModel<CountryLocalizedModel>, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CountryModel()
    {
		_logger.LogDebug(">>> Entering CountryModel()");
        Locales = new List<CountryLocalizedModel>();
        SelectedStoreIds = new List<int>();
		_logger.LogDebug("CountryModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
		_logger.LogDebug("CountryModel(): Locales → {Locales}", Locales);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("CountryModel(): AvailableStores → {AvailableStores}", AvailableStores);
        StateProvinceSearchModel = new StateProvinceSearchModel();
		_logger.LogDebug("CountryModel(): StateProvinceSearchModel → {StateProvinceSearchModel}", StateProvinceSearchModel);
		_logger.LogDebug("<<< Exiting CountryModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsBilling")]
    public bool AllowsBilling { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsShipping")]
    public bool AllowsShipping { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.TwoLetterIsoCode")]
    public string TwoLetterIsoCode { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode")]
    public string ThreeLetterIsoCode { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.NumericIsoCode")]
    public int NumericIsoCode { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.SubjectToVat")]
    public bool SubjectToVat { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.NumberOfStates")]
    public int NumberOfStates { get; set; }

    public IList<CountryLocalizedModel> Locales { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public StateProvinceSearchModel StateProvinceSearchModel { get; set; }

    #endregion
}

public partial record CountryLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
    public string Name { get; set; }
}