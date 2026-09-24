using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Security.Honeypot;

/// <summary>
/// HTML extensions
/// </summary>
public static class HtmlExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Generate honeypot input
    /// </summary>
    /// <param name="helper">HTML helper</param>
    /// <returns>Result</returns>
    public static IHtmlContent GenerateHoneypotInput(this IHtmlHelper helper)
    {
		_logger.LogError(">>> Entering GenerateHoneypotInput(helper={Helper})", helper);
        var sb = new StringBuilder();

		_logger.LogError("GenerateHoneypotInput(helper={Helper}): sb → {Sb}", helper, sb);
        sb.AppendFormat("<div style=\"display:none;\">");
        sb.Append(Environment.NewLine);

        var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
		_logger.LogError("GenerateHoneypotInput(helper={Helper}): securitySettings → {SecuritySettings}", helper, securitySettings);
        sb.AppendFormat("<input id=\"{0}\" name=\"{0}\" type=\"text\">", securitySettings.HoneypotInputName);

        sb.Append(Environment.NewLine);
        sb.Append("</div>");

		_logger.LogWarning("<<< Exiting GenerateHoneypotInput(helper={Helper})", helper);
        return new HtmlString(sb.ToString());
    }
}