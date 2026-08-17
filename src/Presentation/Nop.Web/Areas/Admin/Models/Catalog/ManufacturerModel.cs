using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a manufacturer model
/// </summary>
public partial record ManufacturerModel : BaseNopEntityModel, IAclSupportedModel, IDiscountSupportedModel,
    ITranslationSupportedModel, ILocalizedModel<ManufacturerLocalizedModel>, IStoreMappingSupportedModel,
    IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ManufacturerModel()
    {
		_logger.LogDebug(">>> Entering ManufacturerModel()");
        if (PageSize < 1)
        {
            PageSize = 5;
			_logger.LogDebug("ManufacturerModel(): PageSize → {PageSize}", PageSize);
        }

        Locales = new List<ManufacturerLocalizedModel>();
		_logger.LogDebug("ManufacturerModel(): Locales → {Locales}", Locales);
        AvailableManufacturerTemplates = new List<SelectListItem>();
		_logger.LogDebug("ManufacturerModel(): AvailableManufacturerTemplates → {AvailableManufacturerTemplates}", AvailableManufacturerTemplates);

        AvailableDiscounts = new List<SelectListItem>();
		_logger.LogDebug("ManufacturerModel(): AvailableDiscounts → {AvailableDiscounts}", AvailableDiscounts);
        SelectedDiscountIds = new List<int>();
		_logger.LogDebug("ManufacturerModel(): SelectedDiscountIds → {SelectedDiscountIds}", SelectedDiscountIds);

        SelectedCustomerRoleIds = new List<int>();
		_logger.LogDebug("ManufacturerModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogDebug("ManufacturerModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);

        SelectedStoreIds = new List<int>();
		_logger.LogDebug("ManufacturerModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("ManufacturerModel(): AvailableStores → {AvailableStores}", AvailableStores);

        ManufacturerProductSearchModel = new ManufacturerProductSearchModel();
		_logger.LogDebug("<<< Exiting ManufacturerModel()");
		_logger.LogDebug("ManufacturerModel(): ManufacturerProductSearchModel → {ManufacturerProductSearchModel}", ManufacturerProductSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate")]
    public int ManufacturerTemplateId { get; set; }

    public IList<SelectListItem> AvailableManufacturerTemplates { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
    public string SeName { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Picture")]
    public int PictureId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSize")]
    public int PageSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AllowCustomersToSelectPageSize")]
    public bool AllowCustomersToSelectPageSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSizeOptions")]
    public string PageSizeOptions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceRangeFiltering")]
    public bool PriceRangeFiltering { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceFrom")]
    public decimal PriceFrom { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceTo")]
    public decimal PriceTo { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ManuallyPriceRange")]
    public bool ManuallyPriceRange { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Deleted")]
    public bool Deleted { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ManufacturerLocalizedModel> Locales { get; set; }

    public IList<int> SelectedCustomerRoleIds { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    //discounts
    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Discounts")]
    public IList<int> SelectedDiscountIds { get; set; }

    public IList<SelectListItem> AvailableDiscounts { get; set; }

    public ManufacturerProductSearchModel ManufacturerProductSearchModel { get; set; }

    public string PrimaryStoreCurrencyCode { get; set; }

    public bool PreTranslationAvailable { get; set; }

    #endregion
}

public partial record ManufacturerLocalizedModel : ILocalizedLocaleModel, IMetaTagsSupportedModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
    public string SeName { get; set; }
}