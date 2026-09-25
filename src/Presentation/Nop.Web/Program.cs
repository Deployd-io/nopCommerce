using Autofac.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web;

public partial class Program
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public static async Task Main(string[] args)
    {
		_logger.LogError(">>> Entering Main(args={Args})", args);
        var builder = WebApplication.CreateBuilder(args);

		_logger.LogError("Main(args={Args}): builder → {Builder}", args, builder);
        builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
        if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
        {
            var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
			_logger.LogError("Main(args={Args}): path → {Path}", args, path);
            builder.Configuration.AddJsonFile(path, true, true);
        }
        builder.Configuration.AddEnvironmentVariables();

        //load application settings
        builder.Services.ConfigureApplicationSettings(builder);

        var appSettings = Singleton<AppSettings>.Instance;
        var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
		_logger.LogError("Main(args={Args}): useAutofac → {UseAutofac}", args, useAutofac);

        if (useAutofac)
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        else
            builder.Host.UseDefaultServiceProvider(options =>
            {
                //we don't validate the scopes, since at the app start and the initial configuration we need 
                //to resolve some services (registered as "scoped") through the root container
                options.ValidateScopes = false;
				_logger.LogDebug("Main(args={Args}): options.ValidateScopes → {Arg1}", args, options.ValidateScopes);
                options.ValidateOnBuild = true;
				_logger.LogDebug("Main(args={Args}): options.ValidateOnBuild → {Arg1}", args, options.ValidateOnBuild);
            });

        //add services to the application and configure service provider
        builder.Services.ConfigureApplicationServices(builder);

        var app = builder.Build();
		_logger.LogError("Main(args={Args}): app → {App}", args, app);

        //configure the application HTTP request pipeline
        app.ConfigureRequestPipeline();
        await app.PublishAppStartedEventAsync();

        await app.RunAsync();
		_logger.LogWarning("<<< Exiting Main(args={Args})", args);
    }
}