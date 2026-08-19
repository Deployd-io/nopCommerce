using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Brevo.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
		_logger.LogDebug(">>> Entering RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.ImportContactsRoute, "Plugins/Brevo/ImportContacts",
            new { controller = "Brevo", action = "ImportContacts" });

        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.UnsubscribeContactRoute, "Plugins/Sendinblue/UnsubscribeWebHook",
            new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });

        endpointRouteBuilder.MapControllerRoute(BrevoDefaults.UnsubscribeContactRoute, "Plugins/Brevo/UnsubscribeWebHook",
            new { controller = "BrevoWebhook", action = "UnsubscribeWebHook" });
			_logger.LogDebug("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}