using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a return request action model
/// </summary>
public partial record ReturnRequestActionModel : BaseNopEntityModel, ILocalizedModel<ReturnRequestActionLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ReturnRequestActionModel()
    {
		_logger.LogDebug(">>> Entering ReturnRequestActionModel()");
        Locales = new List<ReturnRequestActionLocalizedModel>();
		_logger.LogDebug("<<< Exiting ReturnRequestActionModel()");
		_logger.LogDebug("ReturnRequestActionModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ReturnRequestActionLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record ReturnRequestActionLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
    public string Name { get; set; }
}