using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a checkout attribute value model
/// </summary>
public partial record CheckoutAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CheckoutAttributeValueLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CheckoutAttributeValueModel()
    {
		_logger.LogDebug(">>> Entering CheckoutAttributeValueModel()");
        Locales = new List<CheckoutAttributeValueLocalizedModel>();
		_logger.LogDebug("<<< Exiting CheckoutAttributeValueModel()");
		_logger.LogDebug("CheckoutAttributeValueModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    public int AttributeId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.ColorSquaresRgb")]
    public string ColorSquaresRgb { get; set; }
    public bool DisplayColorSquaresRgb { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.PriceAdjustment")]
    public decimal PriceAdjustment { get; set; }
    public string PrimaryStoreCurrencyCode { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.WeightAdjustment")]
    public decimal WeightAdjustment { get; set; }
    public string BaseWeightIn { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<CheckoutAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record CheckoutAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.Name")]
    public string Name { get; set; }
}