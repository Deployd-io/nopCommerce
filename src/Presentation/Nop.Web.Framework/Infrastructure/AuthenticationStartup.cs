using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Represents object for the configuring authentication middleware on application startup
/// </summary>
public partial class AuthenticationStartup : INopStartup
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
		_logger.LogDebug(">>> Entering ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
        //add data protection
        services.AddNopDataProtection();

        //add authentication
        services.AddNopAuthentication();
		_logger.LogDebug("<<< Exiting ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public virtual void Configure(IApplicationBuilder application)
    {
		_logger.LogDebug(">>> Entering Configure(application={Application})", application);
        //configure authentication
        application.UseNopAuthentication();
		_logger.LogDebug("<<< Exiting Configure(application={Application})", application);
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 500; //These middleware are placed between UseRouting and UseEndpoints
}