using Newtonsoft.Json;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Events;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Infrastructure;

/// <summary>
/// Represents license terms event consumer
/// </summary>
public partial class LicenseTermsConsumer : IConsumer<PageRenderingEvent>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ISettingService _settingService;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public LicenseTermsConsumer(IHttpContextAccessor httpContextAccessor,
        ISettingService settingService,
        NopHttpClient nopHttpClient)
    {
		_logger.LogDebug(">>> Entering LicenseTermsConsumer(httpContextAccessor={HttpContextAccessor},settingService={SettingService},nopHttpClient={NopHttpClient})", httpContextAccessor, settingService, nopHttpClient);
        _httpContextAccessor = httpContextAccessor;
        _settingService = settingService;
		_logger.LogDebug("LicenseTermsConsumer(httpContextAccessor={HttpContextAccessor},settingService={SettingService},nopHttpClient={NopHttpClient}): _settingService → {_settingService}", httpContextAccessor, settingService, nopHttpClient, _settingService);
		_logger.LogDebug("LicenseTermsConsumer(httpContextAccessor={HttpContextAccessor},settingService={SettingService},nopHttpClient={NopHttpClient}): _httpContextAccessor → {_httpContextAccessor}", httpContextAccessor, settingService, nopHttpClient, _httpContextAccessor);
        _nopHttpClient = nopHttpClient;
		_logger.LogDebug("<<< Exiting LicenseTermsConsumer(httpContextAccessor={HttpContextAccessor},settingService={SettingService},nopHttpClient={NopHttpClient})", httpContextAccessor, settingService, nopHttpClient);
		_logger.LogDebug("LicenseTermsConsumer(httpContextAccessor={HttpContextAccessor},settingService={SettingService},nopHttpClient={NopHttpClient}): _nopHttpClient → {_nopHttpClient}", httpContextAccessor, settingService, nopHttpClient, _nopHttpClient);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle page rendering event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(PageRenderingEvent eventMessage)
    {
		_logger.LogDebug(">>> Entering HandleEventAsync(eventMessage={EventMessage})", eventMessage);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        if (_httpContextAccessor.HttpContext.GetRouteValue("area") is not string area || area != AreaNames.ADMIN)
            return;

        var (exists, currentVersion) = await _settingService.TryGetLicenseAsync();
        if (!exists)
            return;

        if (currentVersion.AcceptedLicenseTerms)
            return;

        if ((currentVersion.InstallationDate ?? DateTime.UtcNow).AddMonths(1) > DateTime.UtcNow)
            return;

        if (currentVersion.LastCheckDate.HasValue && currentVersion.LastCheckDate.Value.AddHours(3) > DateTime.UtcNow)
            return;

        var licenseCheckModel = new LicenseCheckModel();
		_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): licenseCheckModel → {LicenseCheckModel}", eventMessage, licenseCheckModel);
        try
        {
            currentVersion.LastCheckDate = DateTime.UtcNow;
			_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): currentVersion.LastCheckDate → {Arg1}", eventMessage, currentVersion.LastCheckDate);
            var result = await _nopHttpClient.CheckLicenseTermsAsync();
			_logger.LogDebug("HandleEventAsync(eventMessage={EventMessage}): result → {Result}", eventMessage, result);
            if (!string.IsNullOrEmpty(result))
                licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
        }
        catch { }
		_logger.LogError("Exception in HandleEventAsync(eventMessage={EventMessage}): {Arg1}", eventMessage, ex.Message);

        await _settingService.TryGetLicenseAsync(licenseCheckModel.AcceptedLicenseTerms == true, currentVersion.LastCheckDate);

        if (licenseCheckModel.DisplayWarning == true && !string.IsNullOrEmpty(licenseCheckModel.WarningText))
            _httpContextAccessor.HttpContext.Items.TryAdd("licenseTerms", licenseCheckModel);
			_logger.LogInformation("HandleEventAsync(eventMessage)={EventMessage}: external service call took {Arg1} ms", eventMessage, __logStart.ElapsedMilliseconds);
			_logger.LogDebug("<<< Exiting HandleEventAsync(eventMessage={EventMessage})", eventMessage);
    }

    #endregion
}