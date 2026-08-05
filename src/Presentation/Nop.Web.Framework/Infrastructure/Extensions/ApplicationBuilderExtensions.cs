using System.Globalization;
using System.Net;
using System.Runtime.ExceptionServices;
using iTextSharp.text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Media.RoxyFileman;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework.Globalization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.WebOptimizer;
using WebMarkupMin.AspNetCoreLatest;
using WebOptimizer;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Nop.Web.Framework.Infrastructure.Extensions;

/// <summary>
/// Represents extensions of IApplicationBuilder
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure the application HTTP request pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void ConfigureRequestPipeline(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering ConfigureRequestPipeline(application={Application})", application);
        EngineContext.Current.ConfigureRequestPipeline(application);
		logger.LogDebug("<<< Exiting ConfigureRequestPipeline(application={Application})", application);
    }

    /// <summary>
    /// Publish AppStarted event
    /// </summary>
    /// <param name="_">Builder for configuring an application's request pipeline</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task PublishAppStartedEventAsync(this IApplicationBuilder _)
    {
		logger.LogDebug(">>> Entering PublishAppStartedEventAsync(_={_})", _);
        //publish AppStartedEvent
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        await eventPublisher.PublishAsync(new AppStartedEvent());
		logger.LogDebug("<<< Exiting PublishAppStartedEventAsync(_={_})", _);
		logger.LogDebug("PublishAppStartedEventAsync(_={_}): eventPublisher → {EventPublisher}", _, eventPublisher);
    }

    /// <summary>
    /// Add exception handling
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopExceptionHandler(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopExceptionHandler(application={Application})", application);
        var appSettings = EngineContext.Current.Resolve<AppSettings>();
        var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
		logger.LogDebug("UseNopExceptionHandler(application={Application}): appSettings → {AppSettings}", application, appSettings);
		logger.LogDebug("UseNopExceptionHandler(application={Application}): webHostEnvironment → {WebHostEnvironment}", application, webHostEnvironment);
        var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
        if (useDetailedExceptionPage)
		logger.LogDebug("UseNopExceptionHandler(application={Application}): useDetailedExceptionPage → {UseDetailedExceptionPage}", application, useDetailedExceptionPage);
        {
            //get detailed exceptions for developing and testing purposes
            application.UseDeveloperExceptionPage();
        }
        else
        {
            //or use special exception handler
            application.UseExceptionHandler("/Error/Error");
        }

        //log errors
        application.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception == null)
					logger.LogDebug("UseNopExceptionHandler(application={Application}): exception → {Exception}", application, exception);
                    return;

                try
                {
                    //check whether database is installed
                    if (DataSettingsManager.IsDatabaseInstalled())
                    {
                        //get current customer
                        var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

						logger.LogDebug("UseNopExceptionHandler(application={Application}): currentCustomer → {CurrentCustomer}", application, currentCustomer);
                        //log error
                        await EngineContext.Current.Resolve<ILogger>().ErrorAsync(exception.Message, exception, currentCustomer);
                    }
                }
                finally
                {
                    //rethrow the exception to show the error page
                    ExceptionDispatchInfo.Throw(exception);
                }
            });
        });
		logger.LogDebug("<<< Exiting UseNopExceptionHandler(application={Application})", application);
    }

    /// <summary>
    /// Adds a special handler that checks for responses with the 404 status code that do not have a body
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UsePageNotFound(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UsePageNotFound(application={Application})", application);
        application.UseStatusCodePages(async context =>
        {
            //handle 404 Not Found
            if (context.HttpContext.Response.StatusCode != StatusCodes.Status404NotFound)
                return;

            //return a browser-dependent error message indicating the static file can't be found
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource())
				logger.LogDebug("UsePageNotFound(application={Application}): webHelper → {WebHelper}", application, webHelper);
                return;

            //get original path and query
            var originalPath = context.HttpContext.Request.Path;
            var originalQueryString = context.HttpContext.Request.QueryString;

            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

				logger.LogDebug("UsePageNotFound(application={Application}): commonSettings → {CommonSettings}", application, commonSettings);
                if (commonSettings.Log404Errors)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
					logger.LogDebug("UsePageNotFound(application={Application}): logger → {Logger}", application, logger);
					logger.LogDebug("UsePageNotFound(application={Application}): workContext → {WorkContext}", application, workContext);

                    await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
                        customer: await workContext.GetCurrentCustomerAsync());
                }
            }

            var routeValuesFeature = context.HttpContext.Features.Get<IRouteValuesFeature>();

			logger.LogDebug("UsePageNotFound(application={Application}): routeValuesFeature → {RouteValuesFeature}", application, routeValuesFeature);
            //store the original paths so we can check it later
            context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature
            {
                OriginalPathBase = context.HttpContext.Request.PathBase.Value!,
                OriginalPath = originalPath.Value!,
                OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                Endpoint = context.HttpContext.GetEndpoint(),
                RouteValues = routeValuesFeature?.RouteValues
            });

            string getLanguageRouteValue()
            {
                if (!DataSettingsManager.IsDatabaseInstalled())
                    return string.Empty;

                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
					logger.LogDebug("UsePageNotFound(application={Application}): localizationSettings → {LocalizationSettings}", application, localizationSettings);
                    return string.Empty;

                return routeValuesFeature?.RouteValues.GetValueOrDefault(NopRoutingDefaults.RouteValue.Language) is string lang ? $"/{lang}" : "/en";
            }

            //set new path
            context.HttpContext.Request.Path = $"{getLanguageRouteValue()}/page-not-found";
            context.HttpContext.Request.QueryString = QueryString.Empty;

            //since we're going to re-invoke the middleware pipeline we need to reset the endpoint and route values
            context.HttpContext.SetEndpoint(null);
            if (routeValuesFeature is not null)
                routeValuesFeature.RouteValues = null!;
				logger.LogDebug("UsePageNotFound(application={Application}): routeValuesFeature.RouteValues → {Arg1}", application, routeValuesFeature.RouteValues);

            try
            {
                //re-execute request with new path
                await context.Next(context.HttpContext);
            }
            finally
            {
                //return original path to request
                context.HttpContext.Request.QueryString = originalQueryString;
                context.HttpContext.Request.Path = originalPath;
                context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
            }
        });
		logger.LogDebug("<<< Exiting UsePageNotFound(application={Application})", application);
    }

    /// <summary>
    /// Adds a special handler that checks for responses with the 400 status code (bad request)
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseBadRequestResult(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseBadRequestResult(application={Application})", application);
        application.UseStatusCodePages(async context =>
        {
            //handle 404 (Bad request)
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
				logger.LogDebug("UseBadRequestResult(application={Application}): logger → {Logger}", application, logger);
				logger.LogDebug("UseBadRequestResult(application={Application}): workContext → {WorkContext}", application, workContext);
                await logger.ErrorAsync("Error 400. Bad request", null, customer: await workContext.GetCurrentCustomerAsync());
            }
        });
		logger.LogDebug("<<< Exiting UseBadRequestResult(application={Application})", application);
    }

    /// <summary>
    /// Configure middleware for dynamically compressing HTTP responses
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopResponseCompression(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopResponseCompression(application={Application})", application);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //whether to use compression (gzip by default)
        if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
            application.UseResponseCompression();
			logger.LogDebug("<<< Exiting UseNopResponseCompression(application={Application})", application);
    }

    /// <summary>
    /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopWebOptimizer(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopWebOptimizer(application={Application})", application);
        var appSettings = Singleton<AppSettings>.Instance;
        var woConfig = appSettings.Get<WebOptimizerConfig>();

		logger.LogDebug("UseNopWebOptimizer(application={Application}): woConfig → {WoConfig}", application, woConfig);
        if (!woConfig.EnableCssBundling && !woConfig.EnableJavaScriptBundling)
            return;

        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
		logger.LogDebug("UseNopWebOptimizer(application={Application}): fileProvider → {FileProvider}", application, fileProvider);
		logger.LogDebug("UseNopWebOptimizer(application={Application}): webHostEnvironment → {WebHostEnvironment}", application, webHostEnvironment);

        application.UseWebOptimizer(webHostEnvironment,
        [
            new FileProviderOptions
            {
                RequestPath = new PathString("/Plugins"),
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins"))
            },
            new FileProviderOptions
            {
                RequestPath = new PathString("/Themes"),
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes"))
            }
        ]);
		logger.LogDebug("<<< Exiting UseNopWebOptimizer(application={Application})", application);
    }

    /// <summary>
    /// Configure static file serving
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopStaticFiles(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopStaticFiles(application={Application})", application);
        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
        var appSettings = EngineContext.Current.Resolve<AppSettings>();
		logger.LogDebug("UseNopStaticFiles(application={Application}): fileProvider → {FileProvider}", application, fileProvider);
		logger.LogDebug("UseNopStaticFiles(application={Application}): appSettings → {AppSettings}", application, appSettings);

        void staticFileResponse(StaticFileResponseContext context)
        {
            if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
                context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
        }

        //add handling if sitemaps 
        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopSeoDefaults.SitemapXmlDirectory)),
            RequestPath = new PathString($"/{NopSeoDefaults.SitemapXmlDirectory}"),
            OnPrepareResponse = context =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled() ||
                    !EngineContext.Current.Resolve<SitemapXmlSettings>().SitemapXmlEnabled)
                {
                    context.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Context.Response.ContentLength = 0;
                    context.Context.Response.Body = Stream.Null;
                }
            }
        });

        //common static files
        application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

        //images
        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetLocalImagesPath(EngineContext.Current.Resolve<MediaSettings>())),
            RequestPath = new PathString("/images"),
            OnPrepareResponse = staticFileResponse
        });

        //themes static files
        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.MapPath("Themes")),
            RequestPath = new PathString("/Themes"),
            OnPrepareResponse = staticFileResponse
        });

        //plugins static files
        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.MapPath("Plugins")),
            RequestPath = new PathString("/Plugins"),
            OnPrepareResponse = staticFileResponse
        });

        //add support for backups
        var provider = new FileExtensionContentTypeProvider
        {
            Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
        };

        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(NopCommonDefaults.DbBackupsPath)),
            RequestPath = new PathString("/db_backups"),
            ContentTypeProvider = provider,
            OnPrepareResponse = context =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled() ||
                    !EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermission.System.MANAGE_MAINTENANCE).Result)
                {
                    context.Context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Context.Response.ContentLength = 0;
                    context.Context.Response.Body = Stream.Null;
                }
            }
        });

        //add support for webmanifest files
        provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

        application.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
            RequestPath = "/icons",
            ContentTypeProvider = provider
        });

        if (DataSettingsManager.IsDatabaseInstalled())
        {
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
                RequestPath = new PathString(NopRoxyFilemanDefaults.DefaultRootDirectory),
                OnPrepareResponse = staticFileResponse
            });
        }

        if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
        {
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
                RequestPath = new PathString("/.well-known"),
                ServeUnknownFileTypes = true,
            });
        }
		logger.LogDebug("<<< Exiting UseNopStaticFiles(application={Application})", application);
    }

    /// <summary>
    /// Configure middleware checking whether requested page is keep alive page
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseKeepAlive(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseKeepAlive(application={Application})", application);
        application.UseMiddleware<KeepAliveMiddleware>();
		logger.LogDebug("<<< Exiting UseKeepAlive(application={Application})", application);
    }

    /// <summary>
    /// Configure middleware checking whether database is installed
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseInstallUrl(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseInstallUrl(application={Application})", application);
        application.UseMiddleware<InstallUrlMiddleware>();
		logger.LogDebug("<<< Exiting UseInstallUrl(application={Application})", application);
    }

    /// <summary>
    /// Adds the authentication middleware, which enables authentication capabilities.
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopAuthentication(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopAuthentication(application={Application})", application);
        //check whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        application.UseMiddleware<AuthenticationMiddleware>();
		logger.LogDebug("<<< Exiting UseNopAuthentication(application={Application})", application);
    }

    /// <summary>
    /// Configure PDF
    /// </summary>
    public static void UseNopPdf(this IApplicationBuilder _)
    {
		logger.LogDebug(">>> Entering UseNopPdf(_={_})", _);
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

		logger.LogDebug("UseNopPdf(_={_}): fileProvider → {FileProvider}", _, fileProvider);
        var fontPaths = fileProvider.EnumerateFiles(fileProvider.MapPath("~/App_Data/Pdf/"), "*.ttf") ?? Enumerable.Empty<string>();
        foreach (var fp in fontPaths)
        {
		logger.LogDebug("UseNopPdf(_={_}): fontPaths → {FontPaths}", _, fontPaths);
            FontFactory.Register(fp, fileProvider.GetFileNameWithoutExtension(fp));
        }
		logger.LogDebug("<<< Exiting UseNopPdf(_={_})", _);
    }

    /// <summary>
    /// Configure the request localization feature
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopRequestLocalization(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopRequestLocalization(application={Application})", application);
        application.UseRequestLocalization(options =>
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
			logger.LogDebug("UseNopRequestLocalization(application={Application}): languageService → {LanguageService}", application, languageService);
			logger.LogDebug("UseNopRequestLocalization(application={Application}): localizationSettings → {LocalizationSettings}", application, localizationSettings);

            //prepare supported cultures
            var cultures = languageService
                .GetAllLanguages()
                .OrderBy(language => language.DisplayOrder)
                .Select(language => new CultureInfo(language.LanguageCulture))
                .ToList();
            options.SupportedCultures = cultures;
			logger.LogDebug("UseNopRequestLocalization(application={Application}): options.SupportedCultures → {Arg1}", application, options.SupportedCultures);
			logger.LogDebug("UseNopRequestLocalization(application={Application}): options.SupportedUICultures → {Arg1}", application, options.SupportedUICultures);
            options.SupportedUICultures = cultures;
            options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault() ?? new CultureInfo(NopCommonDefaults.DefaultLanguageCulture));
            options.ApplyCurrentCultureToResponseHeaders = true;
			logger.LogDebug("UseNopRequestLocalization(application={Application}): options.ApplyCurrentCultureToResponseHeaders → {Arg1}", application, options.ApplyCurrentCultureToResponseHeaders);

            //configure culture providers
            var headerRequestCultureProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().FirstOrDefault();
            if (headerRequestCultureProvider is not null)
				logger.LogDebug("UseNopRequestLocalization(application={Application}): headerRequestCultureProvider → {HeaderRequestCultureProvider}", application, headerRequestCultureProvider);
                options.RequestCultureProviders.Remove(headerRequestCultureProvider);

            options.AddInitialRequestCultureProvider(new NopSeoUrlCultureProvider());
            var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
            if (cookieRequestCultureProvider is not null)
				logger.LogDebug("UseNopRequestLocalization(application={Application}): cookieRequestCultureProvider → {CookieRequestCultureProvider}", application, cookieRequestCultureProvider);
				logger.LogDebug("UseNopRequestLocalization(application={Application}): cookieRequestCultureProvider.CookieName → {Arg1}", application, cookieRequestCultureProvider.CookieName);
                cookieRequestCultureProvider.CookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CultureCookie}";

            if (localizationSettings.AutomaticallyDetectLanguage)
                options.RequestCultureProviders.Add(new NopAcceptLanguageHeaderRequestCultureProvider());
        });
		logger.LogDebug("<<< Exiting UseNopRequestLocalization(application={Application})", application);
    }

    /// <summary>
    /// Configure Endpoints routing
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopEndpoints(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopEndpoints(application={Application})", application);
        //Execute the endpoint selected by the routing middleware
        application.UseEndpoints(endpoints =>
        {
            //register all routes
            EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
        });
		logger.LogDebug("<<< Exiting UseNopEndpoints(application={Application})", application);
    }

    /// <summary>
    /// Configure applying forwarded headers to their matching fields on the current request.
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopProxy(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopProxy(application={Application})", application);
        var appSettings = EngineContext.Current.Resolve<AppSettings>();
        var hostingConfig = appSettings.Get<HostingConfig>();
		logger.LogDebug("UseNopProxy(application={Application}): appSettings → {AppSettings}", application, appSettings);
		logger.LogDebug("UseNopProxy(application={Application}): hostingConfig → {HostingConfig}", application, hostingConfig);

        if (hostingConfig.UseProxy)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                ForwardLimit = 2
            };

            if (!string.IsNullOrEmpty(hostingConfig.ForwardedForHeaderName))
                options.ForwardedForHeaderName = hostingConfig.ForwardedForHeaderName;
				logger.LogDebug("UseNopProxy(application={Application}): options.ForwardedForHeaderName → {Arg1}", application, options.ForwardedForHeaderName);

            if (!string.IsNullOrEmpty(hostingConfig.ForwardedProtoHeaderName))
                options.ForwardedProtoHeaderName = hostingConfig.ForwardedProtoHeaderName;
				logger.LogDebug("UseNopProxy(application={Application}): options.ForwardedProtoHeaderName → {Arg1}", application, options.ForwardedProtoHeaderName);

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            if (!string.IsNullOrEmpty(hostingConfig.KnownProxies))
            {
                foreach (var strIp in hostingConfig.KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    if (IPAddress.TryParse(strIp, out var ip))
                        options.KnownProxies.Add(ip);
                }
            }

            if (!string.IsNullOrEmpty(hostingConfig.KnownNetworks))
            {
                foreach (var strIpNet in hostingConfig.KnownNetworks.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var ipNetParts = strIpNet.Split("/");
                    if (ipNetParts.Length == 2)
					logger.LogDebug("UseNopProxy(application={Application}): ipNetParts → {IpNetParts}", application, ipNetParts);
                    {
                        if (IPAddress.TryParse(ipNetParts[0], out var ip) && int.TryParse(ipNetParts[1], out var length))
                            options.KnownNetworks.Add(new IPNetwork(ip, length));
                    }
                }
            }

            if (options.KnownProxies.Count > 1 || options.KnownNetworks.Count > 1)
                options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
				logger.LogDebug("UseNopProxy(application={Application}): options.ForwardLimit → {Arg1}", application, options.ForwardLimit);

            //configure forwarding
            application.UseForwardedHeaders(options);
        }
		logger.LogDebug("<<< Exiting UseNopProxy(application={Application})", application);
    }

    /// <summary>
    /// Configure WebMarkupMin
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseNopWebMarkupMin(this IApplicationBuilder application)
    {
		logger.LogDebug(">>> Entering UseNopWebMarkupMin(application={Application})", application);
        //check whether database is installed
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        application.UseWebMarkupMin();
		logger.LogDebug("<<< Exiting UseNopWebMarkupMin(application={Application})", application);
    }
}