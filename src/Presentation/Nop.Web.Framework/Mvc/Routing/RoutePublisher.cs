using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents implementation of route publisher
/// </summary>
public partial class RoutePublisher : IRoutePublisher
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    /// <summary>
    /// Type finder
    /// </summary>
    protected readonly ITypeFinder _typeFinder;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="typeFinder">Type finder</param>
    public RoutePublisher(ITypeFinder typeFinder)
    {
		_logger.LogDebug(">>> Entering RoutePublisher(typeFinder={TypeFinder})", typeFinder);
        _typeFinder = typeFinder;
		_logger.LogDebug("<<< Exiting RoutePublisher(typeFinder={TypeFinder})", typeFinder);
		_logger.LogDebug("RoutePublisher(typeFinder={TypeFinder}): _typeFinder → {_typeFinder}", typeFinder, _typeFinder);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public virtual void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
		_logger.LogDebug(">>> Entering RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
        //find route providers provided by other assemblies
        var routeProviders = _typeFinder.FindClassesOfType<IRouteProvider>();
		_logger.LogDebug("RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder}): routeProviders → {RouteProviders}", endpointRouteBuilder, routeProviders);

        //create and sort instances of route providers
        var instances = routeProviders
            .Select(routeProvider => (IRouteProvider)Activator.CreateInstance(routeProvider))
            .OrderByDescending(routeProvider => routeProvider.Priority);

        //register all provided routes
        foreach (var routeProvider in instances)
            routeProvider.RegisterRoutes(endpointRouteBuilder);
			_logger.LogDebug("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    #endregion
}