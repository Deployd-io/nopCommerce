using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Admin tabstrip created event
/// </summary>
public partial class AdminTabStripCreated
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="tabStripName">Tabstrip name</param>
    public AdminTabStripCreated(IHtmlHelper helper, string tabStripName)
    {
		_logger.LogDebug(">>> Entering AdminTabStripCreated(helper={Helper},tabStripName={TabStripName})", helper, tabStripName);
        Helper = helper;
        TabStripName = tabStripName;
		_logger.LogDebug("AdminTabStripCreated(helper={Helper},tabStripName={TabStripName}): TabStripName → {TabStripName1}", helper, tabStripName, TabStripName);
        BlocksToRender = new List<IHtmlContent>();
		_logger.LogDebug("AdminTabStripCreated(helper={Helper},tabStripName={TabStripName}): BlocksToRender → {BlocksToRender}", helper, tabStripName, BlocksToRender);
		_logger.LogDebug("<<< Exiting AdminTabStripCreated(helper={Helper},tabStripName={TabStripName})", helper, tabStripName);
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