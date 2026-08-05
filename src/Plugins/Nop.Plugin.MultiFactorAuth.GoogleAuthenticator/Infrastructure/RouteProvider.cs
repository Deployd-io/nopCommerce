using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Infrastructure;

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
        endpointRouteBuilder.MapControllerRoute(GoogleAuthenticatorDefaults.ConfigurationRouteName, "Plugins/GoogleAuthenticator/Configure",
            new { controller = "GoogleAuthenticator", action = "Configure", area = AreaNames.ADMIN });
			_logger.LogDebug("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}