using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute mapping model
/// </summary>
public partial record ProductAttributeMappingModel : BaseNopEntityModel, ITranslationSupportedModel, ILocalizedModel<ProductAttributeMappingLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ProductAttributeMappingModel()
    {
		_logger.LogError(">>> Entering ProductAttributeMappingModel()");
        AvailableProductAttributes = new List<SelectListItem>();
        Locales = new List<ProductAttributeMappingLocalizedModel>();
		_logger.LogError("ProductAttributeMappingModel(): Locales → {Locales}", Locales);
        ConditionModel = new ProductAttributeConditionModel();
		_logger.LogError("ProductAttributeMappingModel(): ConditionModel → {ConditionModel}", ConditionModel);
        ProductAttributeValueSearchModel = new ProductAttributeValueSearchModel();
		_logger.LogError("ProductAttributeMappingModel(): ProductAttributeValueSearchModel → {ProductAttributeValueSearchModel}", ProductAttributeValueSearchModel);
    }

    #endregion

    #region Properties

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.Attribute")]
    public int ProductAttributeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.Attribute")]
    public string ProductAttribute { get; set; }

    public IList<SelectListItem> AvailableProductAttributes { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.TextPrompt")]
    public string TextPrompt { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.AttributeControlType")]
    public int AttributeControlTypeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.AttributeControlType")]
    public string AttributeControlType { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    //validation fields
    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength")]
    [UIHint("Int32Nullable")]
    public int? ValidationMinLength { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength")]
    [UIHint("Int32Nullable")]
    public int? ValidationMaxLength { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions")]
    public string ValidationFileAllowedExtensions { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize")]
    [UIHint("Int32Nullable")]
    public int? ValidationFileMaximumSize { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue")]
    public string DefaultValue { get; set; }

    public string ValidationRulesString { get; set; }

    //condition
    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Condition")]
    public bool ConditionAllowed { get; set; }

    public string ConditionString { get; set; }

    public ProductAttributeConditionModel ConditionModel { get; set; }

    public IList<ProductAttributeMappingLocalizedModel> Locales { get; set; }

    public ProductAttributeValueSearchModel ProductAttributeValueSearchModel { get; set; }

    public bool PreTranslationAvailable { get; set; }

    #endregion
}

public partial record ProductAttributeMappingLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.Fields.TextPrompt")]
    public string TextPrompt { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue")]
    public string DefaultValue { get; set; }
}