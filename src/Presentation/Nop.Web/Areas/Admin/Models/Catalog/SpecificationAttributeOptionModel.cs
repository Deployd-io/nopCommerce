using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a specification attribute option model
/// </summary>
public partial record SpecificationAttributeOptionModel : BaseNopEntityModel, ILocalizedModel<SpecificationAttributeOptionLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public SpecificationAttributeOptionModel()
    {
		_logger.LogDebug(">>> Entering SpecificationAttributeOptionModel()");
        Locales = new List<SpecificationAttributeOptionLocalizedModel>();
		_logger.LogDebug("<<< Exiting SpecificationAttributeOptionModel()");
		_logger.LogDebug("SpecificationAttributeOptionModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    public int SpecificationAttributeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.ColorSquaresRgb")]
    public string ColorSquaresRgb { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.EnableColorSquaresRgb")]
    public bool EnableColorSquaresRgb { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.NumberOfAssociatedProducts")]
    public int NumberOfAssociatedProducts { get; set; }

    public IList<SpecificationAttributeOptionLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record SpecificationAttributeOptionLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.SpecificationAttributes.SpecificationAttribute.Options.Fields.Name")]
    public string Name { get; set; }
}