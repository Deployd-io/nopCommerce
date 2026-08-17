using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a return request reason model
/// </summary>
public partial record ReturnRequestReasonModel : BaseNopEntityModel, ILocalizedModel<ReturnRequestReasonLocalizedModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public ReturnRequestReasonModel()
    {
		_logger.LogDebug(">>> Entering ReturnRequestReasonModel()");
        Locales = new List<ReturnRequestReasonLocalizedModel>();
		_logger.LogDebug("<<< Exiting ReturnRequestReasonModel()");
		_logger.LogDebug("ReturnRequestReasonModel(): Locales → {Locales}", Locales);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
    public string Name { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.DisplayOrder")]
    public int DisplayOrder { get; set; }

    public IList<ReturnRequestReasonLocalizedModel> Locales { get; set; }

    #endregion
}

public partial record ReturnRequestReasonLocalizedModel : ILocalizedLocaleModel
{
    public int LanguageId { get; set; }

    [NopResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
    public string Name { get; set; }
}