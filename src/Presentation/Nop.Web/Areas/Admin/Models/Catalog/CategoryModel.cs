using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.ArtificialIntelligence;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a category model
/// </summary>
public partial record CategoryModel : BaseNopEntityModel, IAclSupportedModel, IDiscountSupportedModel,
    ITranslationSupportedModel, ILocalizedModel<CategoryLocalizedModel>, IStoreMappingSupportedModel,
    IMetaTagsSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CategoryModel()
    {
		_logger.LogError(">>> Entering CategoryModel()");
        if (PageSize < 1)
        {
            PageSize = 5;
			_logger.LogDebug("CategoryModel(): PageSize → {PageSize}", PageSize);
        }
		_logger.LogWarning("<<< Exiting CategoryModel()");

        Locales = new List<CategoryLocalizedModel>();
		_logger.LogError("CategoryModel(): Locales → {Locales}", Locales);
		_logger.LogDebug("CategoryModel(): Locales → {Locales}", Locales);
        AvailableCategoryTemplates = new List<SelectListItem>();
		_logger.LogError("CategoryModel(): AvailableCategoryTemplates → {AvailableCategoryTemplates}", AvailableCategoryTemplates);
        AvailableCategories = new List<SelectListItem>();
		_logger.LogError("CategoryModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableDiscounts = new List<SelectListItem>();
		_logger.LogError("CategoryModel(): AvailableDiscounts → {AvailableDiscounts}", AvailableDiscounts);
        SelectedDiscountIds = new List<int>();
		_logger.LogError("CategoryModel(): SelectedDiscountIds → {SelectedDiscountIds}", SelectedDiscountIds);

        SelectedCustomerRoleIds = new List<int>();
		_logger.LogError("CategoryModel(): SelectedCustomerRoleIds → {SelectedCustomerRoleIds}", SelectedCustomerRoleIds);
		_logger.LogDebug("CategoryModel(): AvailableCategoryTemplates → {AvailableCategoryTemplates}", AvailableCategoryTemplates);
		_logger.LogDebug("CategoryModel(): AvailableCategories → {AvailableCategories}", AvailableCategories);
        AvailableCustomerRoles = new List<SelectListItem>();
		_logger.LogError("CategoryModel(): AvailableCustomerRoles → {AvailableCustomerRoles}", AvailableCustomerRoles);

        SelectedStoreIds = new List<int>();
		_logger.LogError("CategoryModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
		_logger.LogError("CategoryModel(): AvailableStores → {AvailableStores}", AvailableStores);

        CategoryProductSearchModel = new CategoryProductSearchModel();
		_logger.LogError("CategoryModel(): CategoryProductSearchModel → {CategoryProductSearchModel}", CategoryProductSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.CategoryTemplate")]
    public int CategoryTemplateId { get; set; }

    public IList<SelectListItem> AvailableCategoryTemplates { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
    public string SeName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
    public int ParentCategoryId { get; set; }

    [UIHint("Picture")]
    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
    public int PictureId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
    public int PageSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize")]
    public bool AllowCustomersToSelectPageSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
    public string PageSizeOptions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceRangeFiltering")]
    public bool PriceRangeFiltering { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceFrom")]
    public decimal PriceFrom { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PriceTo")]
    public decimal PriceTo { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ManuallyPriceRange")]
    public bool ManuallyPriceRange { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomepage")]
    public bool ShowOnHomepage { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
    public bool Published { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
    public bool Deleted { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.RestrictFromVendors")]
    public bool RestrictFromVendors { get; set; }

    public IList<CategoryLocalizedModel> Locales { get; set; }

    public string Breadcrumb { get; set; }

    public IList<int> SelectedCustomerRoleIds { get; set; }
    public IList<SelectListItem> AvailableCustomerRoles { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public IList<SelectListItem> AvailableCategories { get; set; }

    //discounts
    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Discounts")]
    public IList<int> SelectedDiscountIds { get; set; }

    public IList<SelectListItem> AvailableDiscounts { get; set; }

    public CategoryProductSearchModel CategoryProductSearchModel { get; set; }

    public string PrimaryStoreCurrencyCode { get; set; }

    public bool PreTranslationAvailable { get; set; }

    #endregion
}

public partial record CategoryLocalizedModel : ILocalizedLocaleModel, IMetaTagsSupportedModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
    public string MetaKeywords { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
    public string MetaDescription { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
    public string MetaTitle { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
    public string SeName { get; set; }
}