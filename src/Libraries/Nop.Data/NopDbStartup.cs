using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.MySql;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Data.DataProviders.Fluentmigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Microsoft.Extensions.Logging;

namespace Nop.Data;

/// <summary>
/// Represents object for the configuring DB context on application startup
/// </summary>
public partial class NopDbStartup : INopStartup
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
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var mAssemblies = typeFinder.FindClassesOfType<MigrationBase>()
            .Select(t => t.Assembly)
            .Where(assembly => !assembly.FullName.Contains("FluentMigrator.Runner"))
            .Distinct()
            .ToArray();

        services
            // add common FluentMigrator services
            .AddFluentMigratorCore()
            .AddScoped<IGeneratorAccessor, NopGeneratorAccessor>()
            .AddScoped<IProcessorAccessor, NopProcessorAccessor>()
            // set accessor for the connection string
            .AddScoped<IConnectionStringAccessor>(x => DataSettingsManager.LoadSettings())
            .AddScoped<IMigrationManager, MigrationManager>()
            .AddScoped<IConventionSet, NopConventionSet>()
            .ConfigureRunner(rb =>
                rb.WithVersionTable(new MigrationVersionInfo())
                    .AddNopDbEngines()
                    // define the assembly containing the migrations
                    .ScanIn(mAssemblies).For.Migrations()
                    .SetCommandTimeout());

        services.AddScoped<IMySqlTypeMap>(_ => new NopMySql5TypeMap());
        services.AddTransient(p => new Lazy<IVersionLoader>(p.GetRequiredService<IVersionLoader>()));

        //data layer
        services.AddTransient<IDataProviderManager, DataProviderManager>();
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);

        //repositories	
        services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        using var scope = services.BuildServiceProvider().CreateScope();
		_logger.LogDebug("ConfigureServices(services={Services},configuration={Configuration}): scope → {Scope}", services, configuration, scope);
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
		_logger.LogDebug("ConfigureServices(services={Services},configuration={Configuration}): runner → {Runner}", services, configuration, runner);
        foreach (var assembly in mAssemblies)
            runner.ApplyUpSchemaMigrations(assembly);
			_logger.LogDebug("<<< Exiting ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
    }

    /// <summary>
    /// Configure the using of added middleware
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public virtual void Configure(IApplicationBuilder application)
    {
		_logger.LogDebug(">>> Entering Configure(application={Application})", application);
        var config = Singleton<AppSettings>.Instance.Get<CacheConfig>();

		_logger.LogDebug("Configure(application={Application}): config → {Config}", application, config);
        LinqToDB.Common.Configuration.Linq.DisableQueryCache = config.LinqDisableQueryCache;
		_logger.LogDebug("<<< Exiting Configure(application={Application})", application);
    }

    /// <summary>
    /// Gets order of this startup configuration implementation
    /// </summary>
    public int Order => 10;
}