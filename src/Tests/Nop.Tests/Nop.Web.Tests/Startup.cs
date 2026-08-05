using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests;

/// <summary>
/// Represents startup class of application
/// </summary>
public class Startup
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Add services to the application and configure service provider
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public void ConfigureServices(IServiceCollection services)
    {
	_logger.LogDebug(">>> Entering ConfigureServices(services={Services})", services);

	_logger.LogDebug("<<< Exiting ConfigureServices(services={Services})", services);
    }

    /// <summary>
    /// Configure the DI container 
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public void ConfigureContainer(IServiceCollection services)
    {
	_logger.LogDebug(">>> Entering ConfigureContainer(services={Services})", services);

	_logger.LogDebug("<<< Exiting ConfigureContainer(services={Services})", services);
    }

    /// <summary>
    /// Configure the application HTTP request pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
	_logger.LogDebug(">>> Entering Configure(application={Application})", application);

	_logger.LogDebug("<<< Exiting Configure(application={Application})", application);
    }
}