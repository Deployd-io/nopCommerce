using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents a page rendering event
/// </summary>
public partial class PageRenderingEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="overriddenRouteName">Overridden route name</param>
    public PageRenderingEvent(INopHtmlHelper helper, string overriddenRouteName = null)
    {
		logger.LogDebug(">>> Entering PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName})", helper, overriddenRouteName);
        Helper = helper;
        OverriddenRouteName = overriddenRouteName;
		logger.LogDebug("<<< Exiting PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName})", helper, overriddenRouteName);
		logger.LogDebug("PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName}): OverriddenRouteName → {OverriddenRouteName1}", helper, overriddenRouteName, OverriddenRouteName);
		logger.LogDebug("PageRenderingEvent(helper={Helper},overriddenRouteName={OverriddenRouteName}): Helper → {Helper1}", helper, overriddenRouteName, Helper);
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
		logger.LogDebug(">>> Entering GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
        //if an overridden route name is specified, then use it
        //we use it to specify a custom route name when some custom page uses a custom route. But we still need this event to be invoked
        if (!string.IsNullOrEmpty(OverriddenRouteName))
		logger.LogDebug("<<< Exiting GetRouteName(handleDefaultRoutes={HandleDefaultRoutes})", handleDefaultRoutes);
            return OverriddenRouteName;

        //or try to get a registered endpoint route name
        return Helper.GetRouteName(handleDefaultRoutes);
    }

    #endregion
}