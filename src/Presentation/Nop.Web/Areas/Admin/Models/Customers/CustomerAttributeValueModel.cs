using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer attribute value model
/// </summary>
public partial record CustomerAttributeValueModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeValueLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerAttributeValueModel()
    {
		_logger.LogError(">>> Entering CustomerAttributeValueModel()");
        Locales = new List<CustomerAttributeValueLocalizedModel>();
		_logger.LogDebug("CustomerAttributeValueModel(): Locales → {Locales}", Locales);
    _logger.LogError("CustomerAttributeValueModel(): Locales → {Locales}", Locales);
    _logger.LogWarning("<<< Exiting CustomerAttributeValueModel()");
    }

    #endregion

    #region Properties

    public int AttributeId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<CustomerAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record CustomerAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
    public string Name { get; set; }
}