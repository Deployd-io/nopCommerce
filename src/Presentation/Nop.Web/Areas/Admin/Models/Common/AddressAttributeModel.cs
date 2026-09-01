using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute model
/// </summary>
public partial record AddressAttributeModel : BaseNopEntityModel, ILocalizedModel<AddressAttributeLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddressAttributeModel()
    {
		_logger.LogDebug(">>> Entering AddressAttributeModel()");
        Locales = new List<AddressAttributeLocalizedModel>();
		_logger.LogDebug("AddressAttributeModel(): Locales → {Locales}", Locales);
        AddressAttributeValueSearchModel = new AddressAttributeValueSearchModel();
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.IsRequired")]
    public bool IsRequired { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.AttributeControlType")]
    public int AttributeControlTypeId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.AttributeControlType")]
    public string AttributeControlTypeName { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<AddressAttributeLocalizedModel> Locales { get; set; }

    public AddressAttributeValueSearchModel AddressAttributeValueSearchModel { get; set; }

    #endregion
}

public partial record AddressAttributeLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Fields.Name")]
    public string Name { get; set; }
}