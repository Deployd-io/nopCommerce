using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Localization;

/// <summary>
/// Represents a language model
/// </summary>
public partial record LanguageModel : BaseNopEntityModel, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public LanguageModel()
    {
		_logger.LogDebug(">>> Entering LanguageModel()");
        AvailableCurrencies = new List<SelectListItem>();
		_logger.LogDebug("LanguageModel(): AvailableCurrencies → {AvailableCurrencies}", AvailableCurrencies);
        AvailableFlagImages = new List<SelectListItem>();
		_logger.LogDebug("LanguageModel(): AvailableFlagImages → {AvailableFlagImages}", AvailableFlagImages);
        SelectedStoreIds = new List<int>();
		_logger.LogDebug("LanguageModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("LanguageModel(): AvailableStores → {AvailableStores}", AvailableStores);
        LocaleResourceSearchModel = new LocaleResourceSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
    public string LanguageCulture { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
    public string UniqueSeoCode { get; set; }

    //flags
    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
    public string FlagImageFileName { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
    public bool Rtl { get; set; }

    //default currency
    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.DefaultCurrency")]
    public int DefaultCurrencyId { get; set; }

    public IList<SelectListItem> AvailableCurrencies { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Configuration.Languages.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableFlagImages { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    // search
    public LocaleResourceSearchModel LocaleResourceSearchModel { get; set; }

    #endregion
}