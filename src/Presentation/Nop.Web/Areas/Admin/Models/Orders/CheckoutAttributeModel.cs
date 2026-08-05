using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a checkout attribute model
/// </summary>
public partial record CheckoutAttributeModel : BaseNopEntityModel,
    ILocalizedModel<CheckoutAttributeLocalizedModel>, IStoreMappingSupportedModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CheckoutAttributeModel()
    {
		_logger.LogDebug(">>> Entering CheckoutAttributeModel()");
        Locales = new List<CheckoutAttributeLocalizedModel>();
        AvailableTaxCategories = new List<SelectListItem>();
		_logger.LogDebug("CheckoutAttributeModel(): Locales → {Locales}", Locales);
		_logger.LogDebug("CheckoutAttributeModel(): AvailableTaxCategories → {AvailableTaxCategories}", AvailableTaxCategories);
        ConditionModel = new ConditionModel();
        SelectedStoreIds = new List<int>();
		_logger.LogDebug("CheckoutAttributeModel(): ConditionModel → {ConditionModel}", ConditionModel);
		_logger.LogDebug("CheckoutAttributeModel(): SelectedStoreIds → {SelectedStoreIds}", SelectedStoreIds);
        AvailableStores = new List<SelectListItem>();
        CheckoutAttributeValueSearchModel = new CheckoutAttributeValueSearchModel();
		_logger.LogDebug("<<< Exiting CheckoutAttributeModel()");
		_logger.LogDebug("CheckoutAttributeModel(): AvailableStores → {AvailableStores}", AvailableStores);
		_logger.LogDebug("CheckoutAttributeModel(): CheckoutAttributeValueSearchModel → {CheckoutAttributeValueSearchModel}", CheckoutAttributeValueSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
    public string TextPrompt { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.ShippableProductRequired")]
    public bool ShippableProductRequired { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.IsTaxExempt")]
    public bool IsTaxExempt { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TaxCategory")]
    public int TaxCategoryId { get; set; }
    public IList<SelectListItem> AvailableTaxCategories { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.AttributeControlType")]
    public int AttributeControlTypeId { get; set; }
    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.AttributeControlType")]
    public string AttributeControlTypeName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.MinLength")]
    [UIHint("Int32Nullable")]
    public int? ValidationMinLength { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.MaxLength")]
    [UIHint("Int32Nullable")]
    public int? ValidationMaxLength { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileAllowedExtensions")]
    public string ValidationFileAllowedExtensions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.FileMaximumSize")]
    [UIHint("Int32Nullable")]
    public int? ValidationFileMaximumSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.DefaultValue")]
    public string DefaultValue { get; set; }

    public IList<CheckoutAttributeLocalizedModel> Locales { get; set; }

    //condition
    public bool ConditionAllowed { get; set; }
    public ConditionModel ConditionModel { get; set; }

    //store mapping
    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.LimitedToStores")]
    public IList<int> SelectedStoreIds { get; set; }
    public IList<SelectListItem> AvailableStores { get; set; }

    public CheckoutAttributeValueSearchModel CheckoutAttributeValueSearchModel { get; set; }

    #endregion
}

public partial record ConditionModel : BaseNopEntityModel
{
    public ConditionModel()
    {
		_logger.LogDebug(">>> Entering ConditionModel()");
        ConditionAttributes = new List<AttributeConditionModel>();
		_logger.LogDebug("<<< Exiting ConditionModel()");
		_logger.LogDebug("ConditionModel(): ConditionAttributes → {ConditionAttributes}", ConditionAttributes);
    }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Condition.EnableCondition")]
    public bool EnableCondition { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Condition.Attributes")]
    public int SelectedAttributeId { get; set; }

    public IList<AttributeConditionModel> ConditionAttributes { get; set; }
}

public partial record AttributeConditionModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public AttributeControlType AttributeControlType { get; set; }

    public IList<SelectListItem> Values { get; set; }

    public string SelectedValueId { get; set; }
}

public partial record CheckoutAttributeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.TextPrompt")]
    public string TextPrompt { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Fields.DefaultValue")]
    public string DefaultValue { get; set; }
}