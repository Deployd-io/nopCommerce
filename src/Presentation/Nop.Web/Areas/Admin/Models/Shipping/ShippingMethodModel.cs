using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a shipping method model
/// </summary>
public partial record ShippingMethodModel : BaseNopEntityModel, ILocalizedModel<ShippingMethodLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ShippingMethodModel()
    {
		_logger.LogError(">>> Entering ShippingMethodModel()");
        Locales = new List<ShippingMethodLocalizedModel>();
		_logger.LogDebug("ShippingMethodModel(): Locales → {Locales}", Locales);
    _logger.LogError("ShippingMethodModel(): Locales → {Locales}", Locales);
    _logger.LogWarning("<<< Exiting ShippingMethodModel()");
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
    public string Description { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ShippingMethodLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record ShippingMethodLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.Methods.Fields.Description")]
    public string Description { get; set; }
}