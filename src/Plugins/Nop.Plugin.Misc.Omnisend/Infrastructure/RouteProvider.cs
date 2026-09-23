using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Infrastructure;

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
		_logger.LogError(">>> Entering RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
        endpointRouteBuilder.MapControllerRoute(name: OmnisendDefaults.ConfigurationRouteName,
            pattern: "Admin/Omnisend/Configure",
            defaults: new { controller = "OmnisendAdmin", action = "Configure", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: OmnisendDefaults.AbandonedCheckoutRouteName,
            pattern: "Omnisend/AbandonedCheckout/{cartId}",
            defaults: new { controller = "Omnisend", action = "AbandonedCheckout" });
			_logger.LogWarning("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}