using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Widgets.FacebookPixel.Models;

/// <summary>
/// Represents a Facebook Pixel search model
/// </summary>
public record FacebookPixelSearchModel : BaseSearchModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    public FacebookPixelSearchModel()
    {
		_logger.LogDebug(">>> Entering FacebookPixelSearchModel()");
        AvailableStores = new List<SelectListItem>();
		_logger.LogDebug("<<< Exiting FacebookPixelSearchModel()");
		_logger.LogDebug("FacebookPixelSearchModel(): AvailableStores → {AvailableStores}", AvailableStores);
    }

    #endregion

    #region Properties

    [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Search.Store")]
    public int StoreId { get; set; }

    public IList<SelectListItem> AvailableStores { get; set; }

    public bool HideStoresList { get; set; }

    public bool HideSearchBlock { get; set; }

    #endregion
}