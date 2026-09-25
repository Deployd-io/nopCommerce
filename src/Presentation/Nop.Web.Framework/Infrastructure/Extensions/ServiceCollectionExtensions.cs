using System.Threading.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Core.Security;
using Nop.Data;
using Nop.Services.ArtificialIntelligence;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.ModelBinding.Binders;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.Validators;
using Nop.Web.Framework.WebOptimizer;
using WebMarkupMin.AspNetCoreLatest;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Configure base application settings
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="builder">A builder for web applications and services</param>
    public static void ConfigureApplicationSettings(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
		_logger.LogError(">>> Entering ConfigureApplicationSettings(services={Services},builder={Builder})", services, builder);
        //create default file provider
        CommonHelper.DefaultFileProvider = new NopFileProvider(builder.Environment);

        //register type finder
        var typeFinder = new WebAppTypeFinder();
		_logger.LogError("ConfigureApplicationSettings(services={Services},builder={Builder}): typeFinder → {TypeFinder}", services, builder, typeFinder);
        Singleton<ITypeFinder>.Instance = typeFinder;
        services.AddSingleton<ITypeFinder>(typeFinder);

        //bind general configuration
        services.BindApplicationSettings(builder);
		_logger.LogWarning("<<< Exiting ConfigureApplicationSettings(services={Services},builder={Builder})", services, builder);
    }

    /// <summary>
    /// Bind application settings
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="builder">A builder for web applications and services</param>
    public static void BindApplicationSettings(this IServiceCollection services, WebApplicationBuilder builder)
    {
		_logger.LogError(">>> Entering BindApplicationSettings(services={Services},builder={Builder})", services, builder);
        var typeFinder = Singleton<ITypeFinder>.Instance;

        //add configuration parameters
        var configurations = typeFinder
            .FindClassesOfType<IConfig>()
            .Select(configType => (IConfig)Activator.CreateInstance(configType))
            .ToList();

        foreach (var config in configurations)
            builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);

        var appSettings = Singleton<AppSettings>.Instance;
		_logger.LogError("BindApplicationSettings(services={Services},builder={Builder}): appSettings → {AppSettings}", services, builder, appSettings);

        if (appSettings == null)
        {
            appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, false);
            services.AddSingleton(appSettings);
        }
        else
        {
            var needToUpdate = configurations.Any(conf => !appSettings.Configuration.ContainsKey(conf.Name));
			_logger.LogError("BindApplicationSettings(services={Services},builder={Builder}): needToUpdate → {NeedToUpdate}", services, builder, needToUpdate);
            AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, needToUpdate);
        }
		_logger.LogWarning("<<< Exiting BindApplicationSettings(services={Services},builder={Builder})", services, builder);
    }

    /// <summary>
    /// Add services to the application and configure service provider
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="builder">A builder for web applications and services</param>
    public static void ConfigureApplicationServices(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
		_logger.LogError(">>> Entering ConfigureApplicationServices(services={Services},builder={Builder})", services, builder);
        //add accessor to HttpContext
        services.AddHttpContextAccessor();

        //initialize plugins
        var mvcCoreBuilder = services.AddMvcCore();
		_logger.LogError("ConfigureApplicationServices(services={Services},builder={Builder}): mvcCoreBuilder → {MvcCoreBuilder}", services, builder, mvcCoreBuilder);
        var pluginConfig = new PluginConfig();
		_logger.LogError("ConfigureApplicationServices(services={Services},builder={Builder}): pluginConfig → {PluginConfig}", services, builder, pluginConfig);
        builder.Configuration.GetSection(nameof(PluginConfig)).Bind(pluginConfig, options => options.BindNonPublicProperties = true);
        mvcCoreBuilder.PartManager.InitializePlugins(pluginConfig);

        //bind plugins configurations
        services.BindApplicationSettings(builder);

        //create engine and configure service provider
        var engine = EngineContext.Create();
		_logger.LogError("ConfigureApplicationServices(services={Services},builder={Builder}): engine → {Engine}", services, builder, engine);

        builder.Services.AddRateLimiter(options =>
        {
            var settings = Singleton<AppSettings>.Instance.Get<CommonConfig>();
			_logger.LogError("ConfigureApplicationServices(services={Services},builder={Builder}): settings → {Settings}", services, builder, settings);

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = settings.PermitLimit,
                        QueueLimit = settings.QueueCount,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            options.RejectionStatusCode = settings.RejectionStatusCode;
			_logger.LogDebug("ConfigureApplicationServices(services={Services},builder={Builder}): options.RejectionStatusCode → {Arg2}", services, builder, options.RejectionStatusCode);
        });

        engine.ConfigureServices(services, builder.Configuration);
		_logger.LogWarning("<<< Exiting ConfigureApplicationServices(services={Services},builder={Builder})", services, builder);
    }

    /// <summary>
    /// Register HttpContextAccessor
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddHttpContextAccessor(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddHttpContextAccessor(services={Services})", services);
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		_logger.LogWarning("<<< Exiting AddHttpContextAccessor(services={Services})", services);
    }

    /// <summary>
    /// Adds services required for anti-forgery support
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddAntiForgery(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddAntiForgery(services={Services})", services);
        //override cookie name
        services.AddAntiforgery(options =>
        {
            options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AntiforgeryCookie}";
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });
		_logger.LogWarning("<<< Exiting AddAntiForgery(services={Services})", services);
    }

    /// <summary>
    /// Adds services required for application session state
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddHttpSession(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddHttpSession(services={Services})", services);
        services.AddSession(options =>
        {
            options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.SessionCookie}";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });
		_logger.LogWarning("<<< Exiting AddHttpSession(services={Services})", services);
    }

    /// <summary>
    /// Adds services required for themes support
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddThemes(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddThemes(services={Services})", services);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //themes support
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
        });
		_logger.LogWarning("<<< Exiting AddThemes(services={Services})", services);
    }

    /// <summary>
    /// Adds services required for distributed cache
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddDistributedCache(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddDistributedCache(services={Services})", services);
        var appSettings = Singleton<AppSettings>.Instance;
        var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
		_logger.LogError("AddDistributedCache(services={Services}): distributedCacheConfig → {DistributedCacheConfig}", services, distributedCacheConfig);

        if (!distributedCacheConfig.Enabled)
            return;

        switch (distributedCacheConfig.DistributedCacheType)
        {
            case DistributedCacheType.Memory:
                services.AddDistributedMemoryCache();
                break;

            case DistributedCacheType.SqlServer:
                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = distributedCacheConfig.ConnectionString;
					_logger.LogDebug("AddDistributedCache(services={Services}): options.ConnectionString → {Arg1}", services, options.ConnectionString);
					_logger.LogDebug("AddDistributedCache(services={Services}): options.TableName → {Arg1}", services, options.TableName);
                    options.SchemaName = distributedCacheConfig.SchemaName;
					_logger.LogDebug("AddDistributedCache(services={Services}): options.SchemaName → {Arg1}", services, options.SchemaName);
                    options.TableName = distributedCacheConfig.TableName;
                });
                break;

            case DistributedCacheType.Redis:
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = distributedCacheConfig.ConnectionString;
					_logger.LogDebug("AddDistributedCache(services={Services}): options.Configuration → {Arg1}", services, options.Configuration);
                    options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
					_logger.LogDebug("AddDistributedCache(services={Services}): options.InstanceName → {Arg1}", services, options.InstanceName);
                });
                break;

            case DistributedCacheType.RedisSynchronizedMemory:
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = distributedCacheConfig.ConnectionString;
                    options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
                });
                break;
        }
		_logger.LogWarning("<<< Exiting AddDistributedCache(services={Services})", services);
    }

    /// <summary>
    /// Adds data protection services
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopDataProtection(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopDataProtection(services={Services})", services);
        var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(NopDataProtectionDefaults.DataProtectionKeysPath);
        var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);
		_logger.LogError("AddNopDataProtection(services={Services}): dataProtectionKeysFolder → {DataProtectionKeysFolder}", services, dataProtectionKeysFolder);
		_logger.LogError("AddNopDataProtection(services={Services}): dataProtectionKeysPath → {DataProtectionKeysPath}", services, dataProtectionKeysPath);

        //configure the data protection system to persist keys to the specified directory
        services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
		_logger.LogWarning("<<< Exiting AddNopDataProtection(services={Services})", services);
    }

    /// <summary>
    /// Adds authentication service
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopAuthentication(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopAuthentication(services={Services})", services);
        //set default authentication schemes
        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = NopAuthenticationDefaults.AuthenticationScheme;
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.DefaultChallengeScheme → {Arg1}", services, options.DefaultChallengeScheme);
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.DefaultSignInScheme → {Arg1}", services, options.DefaultSignInScheme);
            options.DefaultScheme = NopAuthenticationDefaults.AuthenticationScheme;
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.DefaultScheme → {Arg1}", services, options.DefaultScheme);
            options.DefaultSignInScheme = NopAuthenticationDefaults.ExternalAuthenticationScheme;
        });

        //add main cookie authentication
        authenticationBuilder.AddCookie(NopAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AuthenticationCookie}";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.LoginPath = NopAuthenticationDefaults.LoginPath;
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.LoginPath → {Arg1}", services, options.LoginPath);
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.ReturnUrlParameter → {Arg1}", services, options.ReturnUrlParameter);
            options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;
			_logger.LogDebug("AddNopAuthentication(services={Services}): options.AccessDeniedPath → {Arg1}", services, options.AccessDeniedPath);
            options.ReturnUrlParameter = NopAuthenticationDefaults.ReturnUrlParameter;
        });

        //add external authentication
        authenticationBuilder.AddCookie(NopAuthenticationDefaults.ExternalAuthenticationScheme, options =>
        {
            options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ExternalAuthenticationCookie}";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.LoginPath = NopAuthenticationDefaults.LoginPath;
            options.AccessDeniedPath = NopAuthenticationDefaults.AccessDeniedPath;
            options.ReturnUrlParameter = NopAuthenticationDefaults.ReturnUrlParameter;
        });

        //register and configure external authentication plugins now
        var typeFinder = Singleton<ITypeFinder>.Instance;
        var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
		_logger.LogError("AddNopAuthentication(services={Services}): externalAuthConfigurations → {ExternalAuthConfigurations}", services, externalAuthConfigurations);
        var externalAuthInstances = externalAuthConfigurations
            .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

        foreach (var instance in externalAuthInstances)
            instance.Configure(authenticationBuilder);
			_logger.LogWarning("<<< Exiting AddNopAuthentication(services={Services})", services);
    }

    /// <summary>
    /// Add and configure MVC for the application
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <returns>A builder for configuring MVC services</returns>
    public static IMvcBuilder AddNopMvc(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopMvc(services={Services})", services);
        //add basic MVC feature
        var mvcBuilder = services.AddControllersWithViews();
		_logger.LogError("AddNopMvc(services={Services}): mvcBuilder → {MvcBuilder}", services, mvcBuilder);

        mvcBuilder.AddRazorRuntimeCompilation();

        var appSettings = Singleton<AppSettings>.Instance;
        if (appSettings.Get<CommonConfig>().UseSessionStateTempDataProvider)
        {
            //use session-based temp data provider
            mvcBuilder.AddSessionStateTempDataProvider();
        }
        else
        {
            //use cookie-based temp data provider
            mvcBuilder.AddCookieTempDataProvider(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.TempDataCookie}";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        services.AddRazorPages();

        //MVC now serializes JSON with camel case names by default, use this code to avoid it
        mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

        //set some options
        mvcBuilder.AddMvcOptions(options =>
        {
            options.ModelBinderProviders.Insert(1, new NopModelBinderProvider());
            //add custom display metadata provider 
            options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider());

            //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
            //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => NopValidationDefaults.NotNullValidationLocaleName);
        });

        //add fluent validation
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

        //register all available validators from Nop assemblies
        var assemblies = mvcBuilder.PartManager.ApplicationParts
            .OfType<AssemblyPart>()
            .Where(part => part.Name.StartsWith("Nop", StringComparison.InvariantCultureIgnoreCase))
            .Select(part => part.Assembly);
        services.AddValidatorsFromAssemblies(assemblies);

        //register controllers as services, it'll allow to override them
        mvcBuilder.AddControllersAsServices();

		_logger.LogWarning("<<< Exiting AddNopMvc(services={Services})", services);
        return mvcBuilder;
    }

    /// <summary>
    /// Register custom RedirectResultExecutor
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopRedirectResultExecutor(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopRedirectResultExecutor(services={Services})", services);
        //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
        services.AddScoped<IActionResultExecutor<RedirectResult>, NopRedirectResultExecutor>();
        services.AddScoped<IActionResultExecutor<LocalRedirectResult>, NopLocalRedirectResultExecutor>();
		_logger.LogWarning("<<< Exiting AddNopRedirectResultExecutor(services={Services})", services);
    }

    /// <summary>
    /// Add and configure WebMarkupMin service
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopWebMarkupMin(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopWebMarkupMin(services={Services})", services);
        //check whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        services
            .AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): options.AllowMinificationInDevelopmentEnvironment → {Arg1}", services, options.AllowMinificationInDevelopmentEnvironment);
                options.AllowCompressionInDevelopmentEnvironment = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): options.AllowCompressionInDevelopmentEnvironment → {Arg1}", services, options.AllowCompressionInDevelopmentEnvironment);
                options.DisableMinification = !EngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
                options.DisableCompression = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): options.DisableCompression → {Arg1}", services, options.DisableCompression);
                options.DisablePoweredByHttpHeaders = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): options.DisablePoweredByHttpHeaders → {Arg1}", services, options.DisablePoweredByHttpHeaders);
            })
            .AddHtmlMinification(options =>
            {
                options.MinificationSettings.AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes;

                options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                options.JsMinifierFactory = new NUglifyJsMinifierFactory();
            })
            .AddXmlMinification(options =>
            {
                var settings = options.MinificationSettings;
                settings.RenderEmptyTagsWithSpace = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): settings.RenderEmptyTagsWithSpace → {Arg1}", services, settings.RenderEmptyTagsWithSpace);
                settings.CollapseTagsWithoutContent = true;
				_logger.LogDebug("AddNopWebMarkupMin(services={Services}): settings.CollapseTagsWithoutContent → {Arg1}", services, settings.CollapseTagsWithoutContent);
            });
			_logger.LogWarning("<<< Exiting AddNopWebMarkupMin(services={Services})", services);
    }

    /// <summary>
    /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopWebOptimizer(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopWebOptimizer(services={Services})", services);
        var appSettings = Singleton<AppSettings>.Instance;
        var woConfig = appSettings.Get<WebOptimizerConfig>();
		_logger.LogError("AddNopWebOptimizer(services={Services}): woConfig → {WoConfig}", services, woConfig);

        if (!woConfig.EnableCssBundling && !woConfig.EnableJavaScriptBundling)
        {
            services.AddScoped<INopAssetHelper, NopDefaultAssetHelper>();
            return;
        }

        //add minification & bundling
        var cssSettings = new CssBundlingSettings
        {
            FingerprintUrls = false,
            Minify = woConfig.EnableCssBundling
        };

        var codeSettings = new CodeBundlingSettings
        {
            Minify = woConfig.EnableJavaScriptBundling,
            AdjustRelativePaths = false //disable this feature because it breaks function names that have "Url(" at the end
        };

        services.AddWebOptimizer(null, cssSettings, codeSettings);
        services.AddScoped<INopAssetHelper, NopAssetHelper>();
		_logger.LogWarning("<<< Exiting AddNopWebOptimizer(services={Services})", services);
    }

    /// <summary>
    /// Add and configure default HTTP clients
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddNopHttpClients(this IServiceCollection services)
    {
		_logger.LogError(">>> Entering AddNopHttpClients(services={Services})", services);
        //default client
        services.AddHttpClient(NopHttpDefaults.DefaultHttpClient).WithProxy();

        //client to request current store
        services.AddHttpClient<StoreHttpClient>();

        //client to request nopCommerce official site
        services.AddHttpClient<NopHttpClient>().WithProxy();

        //client to request reCAPTCHA service
        services.AddHttpClient<CaptchaHttpClient>().WithProxy();

        //client to request artificial intelligence service
        services.AddHttpClient<ArtificialIntelligenceHttpClient>().WithProxy();
		_logger.LogWarning("<<< Exiting AddNopHttpClients(services={Services})", services);
    }
}