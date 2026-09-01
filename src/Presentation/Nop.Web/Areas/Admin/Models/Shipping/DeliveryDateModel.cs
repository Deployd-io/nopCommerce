using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a delivery date model
/// </summary>
public partial record DeliveryDateModel : BaseNopEntityModel, ILocalizedModel<DeliveryDateLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public DeliveryDateModel()
    {
		_logger.LogDebug(">>> Entering DeliveryDateModel()");
        Locales = new List<DeliveryDateLocalizedModel>();
		_logger.LogDebug("DeliveryDateModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<DeliveryDateLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record DeliveryDateLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Shipping.DeliveryDates.Fields.Name")]
    public string Name { get; set; }
}