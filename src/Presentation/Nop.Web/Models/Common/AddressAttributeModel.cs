using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Models.Common;

public partial record AddressAttributeModel : BaseNopEntityModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public AddressAttributeModel()
    {
		_logger.LogError(">>> Entering AddressAttributeModel()");
        Values = new List<AddressAttributeValueModel>();
		_logger.LogDebug("AddressAttributeModel(): Values → {Values}", Values);
    _logger.LogError("AddressAttributeModel(): Values → {Values}", Values);
    _logger.LogWarning("<<< Exiting AddressAttributeModel()");
    }

    public string ControlId { get; set; }

    public string Name { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>
    /// Default value for textboxes
    /// </summary>
    public string DefaultValue { get; set; }

    public AttributeControlType AttributeControlType { get; set; }

    public IList<AddressAttributeValueModel> Values { get; set; }
}

public partial record AddressAttributeValueModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public bool IsPreSelected { get; set; }
}