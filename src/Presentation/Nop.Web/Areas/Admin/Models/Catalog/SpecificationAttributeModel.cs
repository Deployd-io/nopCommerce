using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Translation;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute model
/// </summary>
public partial record SpecificationAttributeModel : BaseNopEntityModel, ITranslationSupportedModel, ILocalizedModel<SpecificationAttributeLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SpecificationAttributeModel()
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeModel()");
        AvailableGroups = new List<SelectListItem>();
        Locales = new List<SpecificationAttributeLocalizedModel>();
		_logger.LogDebug("SpecificationAttributeModel(): Locales → {Locales}", Locales);
		_logger.LogDebug("SpecificationAttributeModel(): AvailableGroups → {AvailableGroups}", AvailableGroups);
        SpecificationAttributeOptionSearchModel = new SpecificationAttributeOptionSearchModel();
		_logger.LogDebug("SpecificationAttributeModel(): SpecificationAttributeOptionSearchModel → {SpecificationAttributeOptionSearchModel}", SpecificationAttributeOptionSearchModel);
        SpecificationAttributeProductSearchModel = new SpecificationAttributeProductSearchModel();
		_logger.LogDebug("SpecificationAttributeModel(): SpecificationAttributeProductSearchModel → {SpecificationAttributeProductSearchModel}", SpecificationAttributeProductSearchModel);
		_logger.LogDebug("<<< Exiting SpecificationAttributeModel()");
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