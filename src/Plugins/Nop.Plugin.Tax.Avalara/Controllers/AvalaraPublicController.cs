using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.Tax.Avalara.Models.Customer;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraPublicController : BasePublicController
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly AvalaraTaxSettings _avalaraTaxSettings;
    protected readonly ICustomerService _customerService;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AvalaraPublicController(AvalaraTaxManager avalaraTaxManager,
        AvalaraTaxSettings avalaraTaxSettings,
        ICustomerService customerService,
        ITaxPluginManager taxPluginManager,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext);
        _avalaraTaxManager = avalaraTaxManager;
        _avalaraTaxSettings = avalaraTaxSettings;
		_logger.LogDebug("AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _avalaraTaxSettings → {_avalaraTaxSettings}", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext, _avalaraTaxSettings);
		_logger.LogDebug("AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _avalaraTaxManager → {_avalaraTaxManager}", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext, _avalaraTaxManager);
    _logger.LogWarning("<<< Exiting AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext})", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext);
        _customerService = customerService;
		_logger.LogDebug("AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _customerService → {_customerService}", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext, _customerService);
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("AvalaraPublicController(avalaraTaxManager={AvalaraTaxManager},avalaraTaxSettings={AvalaraTaxSettings},customerService={CustomerService},taxPluginManager={TaxPluginManager},workContext={WorkContext}): _taxPluginManager → {_taxPluginManager}", avalaraTaxManager, avalaraTaxSettings, customerService, taxPluginManager, workContext, _taxPluginManager);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> ExemptionCertificates()
    {
		_logger.LogError(">>> Entering ExemptionCertificates()");
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!_avalaraTaxSettings.EnableCertificates)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //ACL
        if (_avalaraTaxSettings.CustomerRoleIds.Any())
        {
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
			_logger.LogError("ExemptionCertificates(): customerRoleIds → {CustomerRoleIds}", customerRoleIds);
            if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        var token = await _avalaraTaxManager.CreateTokenAsync(customer);
		_logger.LogError("ExemptionCertificates(): token → {Token}", token);
        var link = await _avalaraTaxManager.GetInvitationAsync(customer) ?? AvalaraTaxDefaults.CertExpressUrl;
		_logger.LogError("ExemptionCertificates(): link → {Link}", link);
        var certificates = await _avalaraTaxManager.GetCustomerCertificatesAsync(customer);
		_logger.LogError("ExemptionCertificates(): certificates → {Certificates}", certificates);
        var model = new TaxExemptionModel
        {
            Token = token,
            Link = link,
            CustomerId = customer.Id,
            Certificates = certificates?.Select(certificate => new ExemptionCertificateModel
            {
                Id = certificate.id ?? 0,
                Status = certificate.status,
                SignedDate = certificate.signedDate.ToShortDateString(),
                ExpirationDate = certificate.expirationDate.ToShortDateString(),
                ExposureZone = certificate.exposureZone?.name
            }).ToList() ?? new List<ExemptionCertificateModel>(),
            AvailableExposureZones = (await _avalaraTaxManager.GetExposureZonesAsync())
                .Select(zone => new SelectListItem(zone.name, zone.name))
                .ToList()
        };
				_logger.LogDebug("ExemptionCertificates(): AvailableExposureZones → {AvailableExposureZones}", AvailableExposureZones);

		_logger.LogWarning("<<< Exiting ExemptionCertificates()");
        return View("~/Plugins/Tax.Avalara/Views/Customer/ExemptionCertificates.cshtml", model);
    }

    [CheckLanguageSeoCode(ignore: true)]
    public async Task<IActionResult> DownloadCertificate(int id)
    {
		_logger.LogError(">>> Entering DownloadCertificate(id={Id})", id);
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, customer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        if (!_avalaraTaxSettings.EnableCertificates)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        //ACL
        if (_avalaraTaxSettings.CustomerRoleIds.Any())
        {
            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
			_logger.LogError("DownloadCertificate(id={Id}): customerRoleIds → {CustomerRoleIds}", id, customerRoleIds);
            if (!customerRoleIds.Intersect(_avalaraTaxSettings.CustomerRoleIds).Any())
                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
        }

        //try to get a file by the identifier
        var file = await _avalaraTaxManager.DownloadCertificateAsync(id);
		_logger.LogError("DownloadCertificate(id={Id}): file → {File}", id, file);
        if (file is null)
            return InvokeHttp404();

        return File(file.Data, file.ContentType, file.Filename?.Split(';')?.FirstOrDefault() ?? "certificate");
    }

    #endregion
}