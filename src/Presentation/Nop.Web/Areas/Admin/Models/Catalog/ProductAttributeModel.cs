using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute model
/// </summary>
public partial record ProductAttributeModel : BaseNopEntityModel, ITranslationSupportedModel, ILocalizedModel<ProductAttributeLocalizedModel>
{
    #region Ctor

    public ProductAttributeModel()
    {
		logger.LogDebug(">>> Entering ProductAttributeModel()");
        Locales = new List<ProductAttributeLocalizedModel>();
        PredefinedProductAttributeValueSearchModel = new PredefinedProductAttributeValueSearchModel();
		logger.LogDebug("ProductAttributeModel(): Locales → {Locales}", Locales);
		logger.LogDebug("ProductAttributeModel(): PredefinedProductAttributeValueSearchModel → {PredefinedProductAttributeValueSearchModel}", PredefinedProductAttributeValueSearchModel);
        ProductAttributeProductSearchModel = new ProductAttributeProductSearchModel();
		logger.LogDebug("<<< Exiting ProductAttributeModel()");
		logger.LogDebug("ProductAttributeModel(): ProductAttributeProductSearchModel → {ProductAttributeProductSearchModel}", ProductAttributeProductSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.Fields.Description")]
    public string Description { get; set; }

    public IList<ProductAttributeLocalizedModel> Locales { get; set; }

    public PredefinedProductAttributeValueSearchModel PredefinedProductAttributeValueSearchModel { get; set; }

    public ProductAttributeProductSearchModel ProductAttributeProductSearchModel { get; set; }

    public bool PreTranslationAvailable { get; set; }

    #endregion
}

public partial record ProductAttributeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.Fields.Description")]
    public string Description { get; set; }
}