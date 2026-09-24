using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Represents object for the configuring routing on application startup
/// </summary>
public partial class NopRoutingStartup : INopStartup
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
	_logger.LogError(">>> Entering ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
    _logger.LogWarning("<<< Exiting ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public virtual void Configure(IApplicationBuilder application)
    {
		_logger.LogError(">>> Entering Configure(application={Application})", application);
        //add the RoutingMiddleware
        application.UseRouting();

        var commonConfig = Singleton<AppSettings>.Instance.Get<CommonConfig>();
		_logger.LogError("Configure(application={Application}): commonConfig → {CommonConfig}", application, commonConfig);
        if (commonConfig.PermitLimit > 0)
            application.UseRateLimiter();
			_logger.LogWarning("<<< Exiting Configure(application={Application})", application);
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 400; // Routing should be loaded before authentication
}