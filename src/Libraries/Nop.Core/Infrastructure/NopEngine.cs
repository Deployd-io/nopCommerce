using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure.Mapper;

namespace Nop.Core.Infrastructure;

/// <summary>
/// Represents Nop engine
/// </summary>
public partial class NopEngine : IEngine
{
    #region Utilities

    /// <summary>
    /// Get IServiceProvider
    /// </summary>
    /// <returns>IServiceProvider</returns>
    protected virtual IServiceProvider GetServiceProvider(IServiceScope scope = null)
    {
		logger.LogDebug(">>> Entering GetServiceProvider(scope={Scope})", scope);
        if (scope == null)
        {
            var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
            var context = accessor?.HttpContext;
			logger.LogDebug("GetServiceProvider(scope={Scope}): accessor → {Accessor}", scope, accessor);
            return context?.RequestServices ?? ServiceProvider;
        }
		logger.LogDebug("<<< Exiting GetServiceProvider(scope={Scope})", scope);
        return scope.ServiceProvider;
    }

    /// <summary>
    /// Run startup tasks
    /// </summary>
    protected virtual void RunStartupTasks()
    {
		logger.LogDebug(">>> Entering RunStartupTasks()");
        //find startup tasks provided by other assemblies
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

		logger.LogDebug("RunStartupTasks(): startupTasks → {StartupTasks}", startupTasks);
        //create and sort instances of startup tasks
        //we startup this interface even for not installed plugins. 
        //otherwise, DbContext initializers won't run and a plugin installation won't work
        var instances = startupTasks
            .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
            .Where(startupTask => startupTask != null)
            .OrderBy(startupTask => startupTask.Order);

        //execute tasks
        foreach (var task in instances)
            task.Execute();
			logger.LogDebug("<<< Exiting RunStartupTasks()");
    }

    /// <summary>
    /// Register and configure AutoMapper
    /// </summary>
    protected virtual void AddAutoMapper()
    {
		logger.LogDebug(">>> Entering AddAutoMapper()");
        //find mapper configurations provided by other assemblies
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

		logger.LogDebug("AddAutoMapper(): mapperConfigurations → {MapperConfigurations}", mapperConfigurations);
        //create and sort instances of mapper configurations
        var instances = mapperConfigurations
            .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
            .Where(mapperConfiguration => mapperConfiguration != null)
            .OrderBy(mapperConfiguration => mapperConfiguration.Order);

        //create AutoMapper configuration
        var config = new MapperConfiguration(cfg =>
        {
            foreach (var instance in instances)
                cfg.AddProfile(instance.GetType());
        });

        //register
        AutoMapperConfiguration.Init(config);
		logger.LogDebug("<<< Exiting AddAutoMapper()");
    }

    protected virtual Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
		logger.LogDebug(">>> Entering CurrentDomain_AssemblyResolve(sender={Sender},args={Args})", sender, args);
        var assemblyFullName = args.Name;

        //check for assembly already loaded
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyFullName);
        if (assembly != null)
			logger.LogDebug("CurrentDomain_AssemblyResolve(sender={Sender},args={Args}): assembly → {Assembly}", sender, args, assembly);
            return assembly;

        //get assembly from TypeFinder
        var typeFinder = Singleton<ITypeFinder>.Instance;

		logger.LogDebug(">>> Entering Resolve(scope={Scope})", scope);
		logger.LogDebug("<<< Exiting Resolve(scope={Scope})", scope);
        return typeFinder?.GetAssemblyByName(assemblyFullName);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add and configure services
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
		logger.LogDebug(">>> Entering ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
        //register engine
        services.AddSingleton<IEngine>(this);

        //find startup configurations provided by other assemblies
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var startupConfigurations = typeFinder.FindClassesOfType<INopStartup>();

		logger.LogDebug("ConfigureServices(services={Services},configuration={Configuration}): startupConfigurations → {StartupConfigurations}", services, configuration, startupConfigurations);
        //create and sort instances of startup configurations
        var instances = startupConfigurations
            .Select(startup => (INopStartup)Activator.CreateInstance(startup))
            .Where(startup => startup != null)
            .OrderBy(startup => startup.Order);

        //configure services
        foreach (var instance in instances)
            instance.ConfigureServices(services, configuration);

        services.AddSingleton(services);

        //register mapper configurations
        AddAutoMapper();

        //run startup tasks
        RunStartupTasks();

        //resolve assemblies here. otherwise, plugins can throw an exception when rendering views
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		logger.LogDebug("<<< Exiting ConfigureServices(services={Services},configuration={Configuration})", services, configuration);
    }

    /// <summary>
    /// Configure HTTP request pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public virtual void ConfigureRequestPipeline(IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering ConfigureRequestPipeline(application={Application})", application);
        ServiceProvider = application.ApplicationServices;

		logger.LogDebug("ConfigureRequestPipeline(application={Application}): ServiceProvider → {ServiceProvider}", application, ServiceProvider);
        //find startup configurations provided by other assemblies
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var startupConfigurations = typeFinder.FindClassesOfType<INopStartup>();

		logger.LogDebug("ConfigureRequestPipeline(application={Application}): startupConfigurations → {StartupConfigurations}", application, startupConfigurations);
        //create and sort instances of startup configurations
        var instances = startupConfigurations
            .Select(startup => (INopStartup)Activator.CreateInstance(startup))
            .Where(startup => startup != null)
            .OrderBy(startup => startup.Order);

        //configure request pipeline
        foreach (var instance in instances)
            instance.Configure(application);
			logger.LogDebug("<<< Exiting ConfigureRequestPipeline(application={Application})", application);
    }

    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="scope">Scope</param>
    /// <typeparam name="T">Type of resolved service</typeparam>
    /// <returns>Resolved service</returns>
    public virtual T Resolve<T>(IServiceScope scope = null) where T : class
    {
        return (T)Resolve(typeof(T), scope);
    }

    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="type">Type of resolved service</param>
    /// <param name="scope">Scope</param>
    /// <returns>Resolved service</returns>
    public virtual object Resolve(Type type, IServiceScope scope = null)
    {
        return GetServiceProvider(scope)?.GetService(type);
    }

    /// <summary>
    /// Resolve dependencies
    /// </summary>
    /// <typeparam name="T">Type of resolved services</typeparam>
    /// <returns>Collection of resolved services</returns>
    public virtual IEnumerable<T> ResolveAll<T>()
    {
		logger.LogDebug(">>> Entering ResolveAll()");
        return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
    }

    /// <summary>
    /// Resolve unregistered service
    /// </summary>
    /// <param name="type">Type of service</param>
    /// <returns>Resolved service</returns>
    public virtual object ResolveUnregistered(Type type)
    {
		logger.LogDebug(">>> Entering ResolveUnregistered(type={Type})", type);
        Exception innerException = null;
        foreach (var constructor in type.GetConstructors())
            try
			logger.LogDebug("ResolveUnregistered(type={Type}): innerException → {InnerException}", type, innerException);
            {
                //try to resolve constructor parameters
                var parameters = constructor.GetParameters().Select(parameter =>
                {
                    var service = Resolve(parameter.ParameterType) ?? throw new NopException("Unknown dependency");
                    return service;
                });

                //all is ok, so create instance
				logger.LogDebug("<<< Exiting ResolveUnregistered(type={Type})", type);
                return Activator.CreateInstance(type, parameters.ToArray());
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

        throw new NopException("No constructor was found that had all the dependencies satisfied.", innerException);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Service provider
    /// </summary>
    public virtual IServiceProvider ServiceProvider { get; protected set; }

    #endregion
}
