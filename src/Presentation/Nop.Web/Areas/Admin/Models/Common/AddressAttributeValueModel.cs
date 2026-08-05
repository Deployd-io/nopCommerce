using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Common;

/// <summary>
/// Represents an address attribute value model
/// </summary>
public partial record AddressAttributeValueModel : BaseNopEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public AddressAttributeValueModel()
    {
		_logger.LogDebug(">>> Entering AddressAttributeValueModel()");
        Locales = new List<AddressAttributeValueLocalizedModel>();
		_logger.LogDebug("<<< Exiting AddressAttributeValueModel()");
		_logger.LogDebug("AddressAttributeValueModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    public int AttributeId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
    public bool IsPreSelected { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record AddressAttributeValueLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
    public string Name { get; set; }
}