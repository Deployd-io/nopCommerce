using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : BaseRouteProvider, IRouteProvider
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
		_logger.LogError(">>> Entering RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
        var lang = GetLanguageRoutePattern();

		_logger.LogError("RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder}): lang → {Lang}", endpointRouteBuilder, lang);
        endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.ConfigurationRouteName,
            pattern: "Admin/Avalara/Configure",
            defaults: new { controller = "Avalara", action = "Configure" });

        //override some of default routes in Admin area
        endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.TaxCategoriesRouteName,
            pattern: "Admin/Tax/Categories",
            defaults: new { controller = "AvalaraTax", action = "Categories", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.ExemptionCertificatesRouteName,
            pattern: $"{lang}/customer/exemption-certificates",
            defaults: new { controller = "AvalaraPublic", action = "ExemptionCertificates" });

        endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.DownloadCertificateRouteName,
            pattern: "download-tax-exemption-certificate/{id:min(0)}",
            defaults: new { controller = "AvalaraPublic", action = "DownloadCertificate" });

        endpointRouteBuilder.MapControllerRoute(name: AvalaraTaxDefaults.ItemClassificationWebhookRouteName,
            pattern: "avalara/item-classification-webhook",
            defaults: new { controller = "AvalaraWebhook", action = "ItemClassificationWebhook" });
			_logger.LogWarning("<<< Exiting RegisterRoutes(endpointRouteBuilder={EndpointRouteBuilder})", endpointRouteBuilder);
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 1; //set a value that is greater than the default one in Nop.Web to override routes
}