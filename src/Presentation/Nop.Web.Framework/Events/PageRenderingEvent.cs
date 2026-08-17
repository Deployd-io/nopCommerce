using Microsoft.Extensions.Logging;
﻿using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents a page rendering event
/// </summary>
public partial class PageRenderingEvent
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="overriddenRouteName">Overridden route name</param>
    public PageRenderingEvent(INopHtmlHelper helper, string overriddenRouteName = null)
    {
		_logger.LogDebug(">>> Entering PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName})", helper, overriddenRouteName);
        Helper = helper;
        OverriddenRouteName = overriddenRouteName;
		_logger.LogDebug("<<< Exiting PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName})", helper, overriddenRouteName);
		_logger.LogDebug("PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName}): OverriddenRouteName → {OverriddenRouteName1}", helper, overriddenRouteName, OverriddenRouteName);
		_logger.LogDebug("PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName}): Helper → {Helper1}", helper, overriddenRouteName, Helper);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets HTML helper
    /// </summary>
    public INopHtmlHelper Helper { get; protected set; }

    /// <summary>
    /// Gets overridden route name
    /// </summary>
    public string OverriddenRouteName { get; protected set; }

    #endregion

    #region Methods

    /// <summary>
    /// Get the route name associated with the request rendering this page
    /// </summary>
    /// <param name="handleDefaultRoutes">A value indicating whether to build the name using engine information unless otherwise specified</param>
    /// <returns>Route name</returns>
    public string GetRouteName(bool handleDefaultRoutes = false)
    {
		_logger.LogDebug(">>> Entering GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
        //if an overridden route name is specified, then use it
        //we use it to specify a custom route name when some custom page uses a custom route. But we still need this event to be invoked
        if (!string.IsNullOrEmpty(OverriddenRouteName))
		_logger.LogDebug("<<< Exiting GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
            return OverriddenRouteName;

        //or try to get a registered endpoint route name
        return Helper.GetRouteName(handleDefaultRoutes);
    }

    #endregion
}