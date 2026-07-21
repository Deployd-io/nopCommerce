using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents implementation of route publisher
/// </summary>
public partial class RoutePublisher : IRoutePublisher
{
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
		logger.LogDebug(">>> Entering RoutePublisher(typeFinder={TypeFinder})", typeFinder);
        _typeFinder = typeFinder;
		logger.LogDebug("<<< Exiting RoutePublisher(typeFinder={TypeFinder})", typeFinder);
		logger.LogDebug("RoutePublisher(typeFinder={TypeFinder}): _typeFinder → {_typeFinder}", typeFinder, _typeFinder);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public virtual void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
		logger.LogDebug(">>> Entering RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
        //find route providers provided by other assemblies
        var routeProviders = _typeFinder.FindClassesOfType<IRouteProvider>();

		logger.LogDebug("RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder}): routeProviders → {RouteProviders}", endpointRouteBuilder, routeProviders);
        //create and sort instances of route providers
        var instances = routeProviders
            .Select(routeProvider => (IRouteProvider)Activator.CreateInstance(routeProvider))
            .OrderByDescending(routeProvider => routeProvider.Priority);

        //register all provided routes
        foreach (var routeProvider in instances)
            routeProvider.RegisterRoutes(endpointRouteBuilder);
			logger.LogDebug("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    #endregion
}
