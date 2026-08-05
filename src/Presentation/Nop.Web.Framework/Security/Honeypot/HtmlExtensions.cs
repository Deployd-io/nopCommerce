using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security.Honeypot;

/// <summary>
/// HTML extensions
/// </summary>
public static class HtmlExtensions
{
    /// <summary>
    /// Generate honeypot input
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <returns>Result</returns>
    public static IHtmlContent GenerateHoneypotInput(this IHtmlHelper helper)
    {
		logger.LogDebug(">>> Entering GenerateHoneypotInput(helper={Helper})", helper);
        var sb = new StringBuilder();

		logger.LogDebug("GenerateHoneypotInput(helper={Helper}): sb → {Sb}", helper, sb);
        sb.AppendFormat("<div style=\"display:none;\">");
        sb.Append(Environment.NewLine);

        var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
        sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"text\">", securitySettings.HoneypotInputName);
		logger.LogDebug("GenerateHoneypotInput(helper={Helper}): securitySettings → {SecuritySettings}", helper, securitySettings);

        sb.Append(Environment.NewLine);
        sb.Append("</div>");

		logger.LogDebug("<<< Exiting GenerateHoneypotInput(helper={Helper})", helper);
        return new HtmlString(sb.ToString());
    }
}