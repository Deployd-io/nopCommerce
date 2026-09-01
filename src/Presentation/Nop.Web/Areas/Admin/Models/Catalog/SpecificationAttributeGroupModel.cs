using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute group model
/// </summary>
public partial record SpecificationAttributeGroupModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeGroupLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SpecificationAttributeGroupModel()
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeGroupModel()");
        Locales = new List<SpecificationAttributeGroupLocalizedModel>();
		_logger.LogDebug("SpecificationAttributeGroupModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttributeGroup.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<SpecificationAttributeGroupLocalizedModel> Locales { get; set; }

    #endregion
}