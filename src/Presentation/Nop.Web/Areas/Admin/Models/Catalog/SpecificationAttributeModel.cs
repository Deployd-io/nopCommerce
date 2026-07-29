using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute model
/// </summary>
public partial record SpecificationAttributeModel : BaseNopEntityModel, ITranslationSupportedModel, ILocalizedModel<SpecificationAttributeLocalizedModel>
{
    #region Ctor

    public SpecificationAttributeModel()
    {
		logger.LogDebug(">>> Entering SpecificationAttributeModel()");
        AvailableGroups = new List<SelectListItem>();
        Locales = new List<SpecificationAttributeLocalizedModel>();
		logger.LogDebug("SpecificationAttributeModel(): AvailableGroups → {AvailableGroups}", AvailableGroups);
		logger.LogDebug("SpecificationAttributeModel(): Locales → {Locales}", Locales);
        SpecificationAttributeOptionSearchModel = new SpecificationAttributeOptionSearchModel();
        SpecificationAttributeProductSearchModel = new SpecificationAttributeProductSearchModel();
		logger.LogDebug("<<< Exiting SpecificationAttributeModel()");
		logger.LogDebug("SpecificationAttributeModel(): SpecificationAttributeOptionSearchModel → {SpecificationAttributeOptionSearchModel}", SpecificationAttributeOptionSearchModel);
		logger.LogDebug("SpecificationAttributeModel(): SpecificationAttributeProductSearchModel → {SpecificationAttributeProductSearchModel}", SpecificationAttributeProductSearchModel);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Fields.SpecificationAttributeGroup")]
    public int? SpecificationAttributeGroupId { get; set; }

    public IList<SelectListItem> AvailableGroups { get; set; }

    public IList<SpecificationAttributeLocalizedModel> Locales { get; set; }

    public SpecificationAttributeOptionSearchModel SpecificationAttributeOptionSearchModel { get; set; }

    public SpecificationAttributeProductSearchModel SpecificationAttributeProductSearchModel { get; set; }

    public bool PreTranslationAvailable { get; set; }

    #endregion
}