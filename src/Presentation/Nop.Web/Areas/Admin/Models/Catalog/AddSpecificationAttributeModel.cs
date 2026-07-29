using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a model for adding or editing a specification attribute
/// </summary>
public partial record AddSpecificationAttributeModel : BaseNopEntityModel, ILocalizedModel<AddSpecificationAttributeLocalizedModel>
{
    #region Ctor

    public AddSpecificationAttributeModel()
    {
		logger.LogDebug(">>> Entering AddSpecificationAttributeModel()");
        AvailableOptions = new List<SelectListItem>();
        AvailableAttributes = new List<SelectListItem>();
		logger.LogDebug("AddSpecificationAttributeModel(): AvailableOptions → {AvailableOptions}", AvailableOptions);
		logger.LogDebug("AddSpecificationAttributeModel(): AvailableAttributes → {AvailableAttributes}", AvailableAttributes);
        ShowOnProductPage = true;
        AttributeName = string.Empty;
        AttributeTypeName = string.Empty;
		logger.LogDebug("AddSpecificationAttributeModel(): ShowOnProductPage → {ShowOnProductPage}", ShowOnProductPage);
		logger.LogDebug("AddSpecificationAttributeModel(): AttributeName → {AttributeName}", AttributeName);
		logger.LogDebug("AddSpecificationAttributeModel(): AttributeTypeName → {AttributeTypeName}", AttributeTypeName);
        Value = string.Empty;
        ValueRaw = string.Empty;
        Locales = new List<AddSpecificationAttributeLocalizedModel>();
		logger.LogDebug("<<< Exiting AddSpecificationAttributeModel()");
		logger.LogDebug("AddSpecificationAttributeModel(): Locales → {Locales}", Locales);
		logger.LogDebug("AddSpecificationAttributeModel(): Value → {Value}", Value);
		logger.LogDebug("AddSpecificationAttributeModel(): ValueRaw → {ValueRaw}", ValueRaw);
    }

    #endregion

    #region Properties

    public int SpecificationId { get; set; }

    public int AttributeTypeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AttributeType")]
    public string AttributeTypeName { get; set; }

    public int AttributeId { get; set; }

    public int ProductId { get; set; }

    public IList<SelectListItem> AvailableAttributes { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttribute")]
    public string AttributeName { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue")]
    public string ValueRaw { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.CustomValue")]
    public string Value { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.AllowFiltering")]
    public bool AllowFiltering { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.ShowOnProductPage")]
    public bool ShowOnProductPage { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.SpecificationAttributes.Fields.SpecificationAttributeOption")]
    public int SpecificationAttributeOptionId { get; set; }

    public IList<SelectListItem> AvailableOptions { get; set; }

    public IList<AddSpecificationAttributeLocalizedModel> Locales { get; set; }

    #endregion
}