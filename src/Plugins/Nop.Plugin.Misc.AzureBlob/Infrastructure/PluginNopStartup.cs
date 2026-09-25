using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AzureBlob.Services;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.AzureBlob.Infrastructure;

/// <summary>
/// Represents the object for the configuring services on application startup
/// </summary>
public class PluginNopStartup : INopStartup
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Add and configure any of the middleware
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
		_logger.LogError(">>> Entering ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
        services.AddTransient<AzureThumbService>();
        services.AddTransient<ThumbService>();
        services.AddTransient<IThumbService>(provider =>
        {
            var settings = provider.GetRequiredService<AzureBlobSettings>();
			_logger.LogError("ConfigureServices(services={Services},configuration={Configuration}): settings → {Settings}", services, configuration, settings);

            if (settings.Enabled && !string.IsNullOrEmpty(settings.ConnectionString) && !string.IsNullOrEmpty(settings.EndPoint))
                return provider.GetRequiredService<AzureThumbService>();

		_logger.LogWarning("<<< Exiting ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
            return provider.GetRequiredService<ThumbService>();
        });
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public void Configure(IApplicationBuilder application)
    {
	_logger.LogError(">>> Entering Configure(application={Application})", application);
    _logger.LogWarning("<<< Exiting Configure(application={Application})", application);
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 3000;
}