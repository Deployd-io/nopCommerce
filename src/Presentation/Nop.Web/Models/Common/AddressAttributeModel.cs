using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record AddressAttributeModel : BaseNopEntityModel
{
    public AddressAttributeModel()
    {
		logger.LogDebug(">>> Entering AddressAttributeModel()");
        Values = new List<AddressAttributeValueModel>();
		logger.LogDebug("<<< Exiting AddressAttributeModel()");
		logger.LogDebug("AddressAttributeModel(): Values → {Values}", Values);
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