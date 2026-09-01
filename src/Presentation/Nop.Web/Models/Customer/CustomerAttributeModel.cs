using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Customer;

public partial record CustomerAttributeModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public CustomerAttributeModel()
    {
		_logger.LogDebug(">>> Entering CustomerAttributeModel()");
        Values = new List<CustomerAttributeValueModel>();
		_logger.LogDebug("CustomerAttributeModel(): Values → {Values}", Values);
    }

    public string Name { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>
    /// Default value for textboxes
    /// </summary>
    public string DefaultValue { get; set; }

    public AttributeControlType AttributeControlType { get; set; }

    public IList<CustomerAttributeValueModel> Values { get; set; }

}

public partial record CustomerAttributeValueModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public bool IsPreSelected { get; set; }
}