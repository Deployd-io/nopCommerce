using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Directory;

/// <summary>
/// Represents a country model
/// </summary>
public partial record CountryModel : BaseNopEntityModel, ILocalizedModel<CountryLocalizedModel>, IStoreMappingSupportedModel
{
    #region Ctor

    public CountryModel()
    {
		logger.LogDebug(">>> Entering CountryModel()");
        Locales = new List<CountryLocalizedModel>();
        SelectedStoreIds = new List<int>();
		logger.LogDebug("CountryModel(): Locales → {Locales}", Locales);
		logger.LogDebug("CountryModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
        StateProvinceSearchModel = new StateProvinceSearchModel();
		logger.LogDebug("<<< Exiting CountryModel()");
		logger.LogDebug("CountryModel(): AvailableStores → {AvailableStores}", AvailableStores);
		logger.LogDebug("CountryModel(): StateProvinceSearchModel → {StateProvinceSearchModel}", StateProvinceSearchModel);
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