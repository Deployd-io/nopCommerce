using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Admin tabstrip created event
/// </summary>
public partial class AdminTabStripCreated
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="tabStripName">Tabstrip name</param>
    public AdminTabStripCreated(IHtmlHelper helper, string tabStripName)
    {
		logger.LogDebug(">>> Entering AdminTabStripCreated(helper={Helper},tabStripName={TabStripName})", helper, tabStripName);
        Helper = helper;
        TabStripName = tabStripName;
		logger.LogDebug("AdminTabStripCreated(helper={Helper},tabStripName={TabStripName}): TabStripName → {TabStripName1}", helper, tabStripName, TabStripName);
		logger.LogDebug("AdminTabStripCreated(helper={Helper},tabStripName={TabStripName}): Helper → {Helper1}", helper, tabStripName, Helper);
        BlocksToRender = new List<IHtmlContent>();
		logger.LogDebug("<<< Exiting AdminTabStripCreated(helper={Helper},tabStripName={TabStripName})", helper, tabStripName);
		logger.LogDebug("AdminTabStripCreated(helper={Helper},tabStripName={TabStripName}): BlocksToRender → {BlocksToRender}", helper, tabStripName, BlocksToRender);
    }

    /// <summary>
    /// HTML Helper
    /// </summary>
    public IHtmlHelper Helper { get; protected set; }
    /// <summary>
    /// TabStripName
    /// </summary>
    public string TabStripName { get; protected set; }
    /// <summary>
    /// Blocks to render
    /// </summary>
    public IList<IHtmlContent> BlocksToRender { get; set; }
}