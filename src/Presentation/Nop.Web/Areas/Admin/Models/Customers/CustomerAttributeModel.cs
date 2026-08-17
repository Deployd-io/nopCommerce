using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer attribute model
/// </summary>
public partial record CustomerAttributeModel : BaseNopEntityModel, ILocalizedModel<CustomerAttributeLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public CustomerAttributeModel()
    {
		_logger.LogDebug(">>> Entering CustomerAttributeModel()");
        Locales = new List<CustomerAttributeLocalizedModel>();
        CustomerAttributeValueSearchModel = new CustomerAttributeValueSearchModel();
		_logger.LogDebug("<<< Exiting CustomerAttributeModel()");
		_logger.LogDebug("CustomerAttributeModel(): CustomerAttributeValueSearchModel → {CustomerAttributeValueSearchModel}", CustomerAttributeValueSearchModel);
		_logger.LogDebug("CustomerAttributeModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
    public int AttributeControlTypeId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
    public string AttributeControlTypeName { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<CustomerAttributeLocalizedModel> Locales { get; set; }

    public CustomerAttributeValueSearchModel CustomerAttributeValueSearchModel { get; set; }

    #endregion
}

public partial record CustomerAttributeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
    public string Name { get; set; }
}