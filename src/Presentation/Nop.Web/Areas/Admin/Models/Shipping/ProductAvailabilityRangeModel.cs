using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a product availability range model
/// </summary>
public partial record ProductAvailabilityRangeModel : BaseNopEntityModel, ILocalizedModel<ProductAvailabilityRangeLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ProductAvailabilityRangeModel()
    {
		_logger.LogDebug(">>> Entering ProductAvailabilityRangeModel()");
        Locales = new List<ProductAvailabilityRangeLocalizedModel>();
		_logger.LogDebug("ProductAvailabilityRangeModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ProductAvailabilityRangeLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record ProductAvailabilityRangeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name")]
    public string Name { get; set; }
}