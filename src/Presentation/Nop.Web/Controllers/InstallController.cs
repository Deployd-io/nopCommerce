using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services.Common;
using Nop.Services.Installation;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Security;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class InstallController : Controller
{
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly Lazy<IInstallationLocalizationService> _locService;
    protected readonly Lazy<IInstallationService> _installationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly Lazy<IPermissionService> _permissionService;
    protected readonly Lazy<IPluginService> _pluginService;
    protected readonly Lazy<IStaticCacheManager> _staticCacheManager;
    protected readonly Lazy<IUploadService> _uploadService;
    protected readonly Lazy<IWebHelper> _webHelper;
    protected readonly Lazy<NopHttpClient> _nopHttpClient;

    #endregion

    #region Ctor

    public InstallController(AppSettings appSettings,
        Lazy<IInstallationLocalizationService> locService,
        Lazy<IInstallationService> installationService,
        INopFileProvider fileProvider,
        Lazy<IPermissionService> permissionService,
        Lazy<IPluginService> pluginService,
        Lazy<IStaticCacheManager> staticCacheManager,
        Lazy<IUploadService> uploadService,
        Lazy<IWebHelper> webHelper,
        Lazy<NopHttpClient> nopHttpClient)
    {
		_logger.LogDebug(">>> Entering InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient})", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient);
        _appSettings = appSettings;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _appSettings → {_appSettings}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _appSettings);
        _locService = locService;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _locService → {_locService}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _locService);
        _installationService = installationService;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _installationService → {_installationService}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _installationService);
        _fileProvider = fileProvider;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _fileProvider → {_fileProvider}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _fileProvider);
        _permissionService = permissionService;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _permissionService → {_permissionService}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _permissionService);
        _pluginService = pluginService;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _pluginService → {_pluginService}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _pluginService);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _staticCacheManager → {_staticCacheManager}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _staticCacheManager);
        _uploadService = uploadService;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _uploadService → {_uploadService}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _uploadService);
        _webHelper = webHelper;
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _webHelper → {_webHelper}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _webHelper);
        _nopHttpClient = nopHttpClient;
		_logger.LogDebug("<<< Exiting InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient})", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient);
		_logger.LogDebug("InstallController(appSettings={AppSettings},locService={LocService},installationService={InstallationService},fileProvider={FileProvider},permissionService={PermissionService},pluginService={PluginService},staticCacheManager={StaticCacheManager},uploadService={UploadService},webHelper={WebHelper},nopHttpClient={NopHttpClient}): _nopHttpClient → {_nopHttpClient}", appSettings, locService, installationService, fileProvider, permissionService, pluginService, staticCacheManager, uploadService, webHelper, nopHttpClient, _nopHttpClient);
    }

    #endregion

    #region Utilities

    protected virtual InstallModel PrepareCountryList(InstallModel model)
    {
		_logger.LogDebug(">>> Entering PrepareCountryList(model={Model})", model);
        if (!model.InstallRegionalResources)
            return model;

        var browserCulture = _locService.Value.GetBrowserCulture();
		_logger.LogDebug("PrepareCountryList(model={Model}): browserCulture → {BrowserCulture}", model, browserCulture);
        var countries = new List<SelectListItem>
        {
            //This item was added in case it was not possible to automatically determine the country by culture
            new() { Value = string.Empty, Text = _locService.Value.GetResource("CountrySelect") }
        };
        countries.AddRange(from country in ISO3166.GetCollection()
            from localization in ISO3166.GetLocalizationInfo(country.Alpha2)
			using Microsoft.Extensions.Logging;
            let lang = ISO3166.GetLocalizationInfo(country.Alpha2).Count() > 1 ? $" [{localization.Language} language]" : string.Empty
            let item = new SelectListItem
            {
				private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
                Value = $"{country.Alpha2}-{localization.Culture}",
                Text = $"{country.Name}{lang}",
                Selected = (localization.Culture == browserCulture) && browserCulture[^2..] == country.Alpha2
            }
            select item);
        model.AvailableCountries.AddRange(countries);

		_logger.LogDebug("<<< Exiting PrepareCountryList(model={Model})", model);
        return model;
    }

    protected virtual InstallModel PrepareLanguageList(InstallModel model)
    {
		_logger.LogDebug(">>> Entering PrepareLanguageList(model={Model})", model);
        foreach (var lang in _locService.Value.GetAvailableLanguages())
        {
            model.AvailableLanguages.Add(new SelectListItem
            {
                Value = Url.RouteUrl(NopRouteNames.Standard.INSTALLATION_CHANGE_LANGUAGE, new { language = lang.Code }),
                Text = lang.Name,
                Selected = _locService.Value.GetCurrentLanguage().Code == lang.Code
            });
        }

		_logger.LogDebug("<<< Exiting PrepareLanguageList(model={Model})", model);
        return model;
    }

    protected virtual InstallModel PrepareAvailableDataProviders(InstallModel model)
    {
		_logger.LogDebug(">>> Entering PrepareAvailableDataProviders(model={Model})", model);
        model.AvailableDataProviders.AddRange(
            _locService.Value.GetAvailableProviderTypes()
                .OrderBy(v => v.Value)
                .Select(pt => new SelectListItem
                {
                    Value = pt.Key.ToString(),
                    Text = pt.Value
                }));

		_logger.LogDebug("<<< Exiting PrepareAvailableDataProviders(model={Model})", model);
        return model;
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
		_logger.LogDebug(">>> Entering Index()");
        if (DataSettingsManager.IsDatabaseInstalled())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new InstallModel
        {
            AdminEmail = "admin@yourStore.com",
            InstallSampleData = false,
            SubscribeNewsletters = true,
            InstallRegionalResources = _appSettings.Get<InstallationConfig>().InstallRegionalResources,
            DisableSampleDataOption = _appSettings.Get<InstallationConfig>().DisableSampleData,
            CreateDatabaseIfNotExists = false,
            ConnectionStringRaw = false,
            DataProvider = DataProviderType.SqlServer
        };

        PrepareAvailableDataProviders(model);
        PrepareLanguageList(model);
        PrepareCountryList(model);

		var __logStart = System.Diagnostics.Stopwatch.StartNew();
		_logger.LogDebug("<<< Exiting Index()");
		_logger.LogInformation("Index(model)={Model}: external service call {PluginsIgnoredDuringInstallation} took {Arg2} ms", model, pluginsIgnoredDuringInstallation, __logStart.ElapsedMilliseconds);
		_logger.LogDebug("<<< Exiting Index(model={Model})", model);
        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Index(InstallModel model)
    {
        if (DataSettingsManager.IsDatabaseInstalled())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        model.DisableSampleDataOption = _appSettings.Get<InstallationConfig>().DisableSampleData;
        model.InstallRegionalResources = _appSettings.Get<InstallationConfig>().InstallRegionalResources;

        PrepareAvailableDataProviders(model);
        PrepareLanguageList(model);
        PrepareCountryList(model);

        //Consider granting access rights to the resource to the ASP.NET request identity. 
        //ASP.NET has a base process identity 
        //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
        //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
        //If the application is impersonating via <identity impersonate="true"/>, 
        //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

        //validate permissions
        var dirsToCheck = _fileProvider.GetDirectoriesWrite();
        foreach (var dir in dirsToCheck)
        {
            if (!_fileProvider.CheckPermissions(dir, false, true, true, false))
                ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureDirectoryPermissions"), CurrentOSUser.FullName, dir));
        }

        var filesToCheck = _fileProvider.GetFilesWrite();
        foreach (var file in filesToCheck)
        {
            if (!_fileProvider.FileExists(file))
                continue;

            if (!_fileProvider.CheckPermissions(file, false, true, true, true))
                ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureFilePermissions"), CurrentOSUser.FullName, file));
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var dataProvider = DataProviderManager.GetDataProvider(model.DataProvider);

            var connectionString = model.ConnectionStringRaw ? model.ConnectionString : dataProvider.BuildConnectionString(model);

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception(_locService.Value.GetResource("ConnectionStringWrongFormat"));

            DataSettingsManager.SaveSettings(new DataConfig
            {
                DataProvider = model.DataProvider,
                ConnectionString = connectionString,
                Collation = model.Collation,
                CharacterSet = model.CharacterSet
            }, _fileProvider);

            if (model.CreateDatabaseIfNotExists && !await dataProvider.DatabaseExistsAsync())
            {
                try
                {
                    dataProvider.CreateDatabase();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(_locService.Value.GetResource("DatabaseCreationError"), ex.Message));
                }
            }
            else
            {
                //check whether database exists
                if (!await dataProvider.DatabaseExistsAsync())
                    throw new Exception(_locService.Value.GetResource("DatabaseNotExists"));
            }

            dataProvider.InitializeDatabase();

            var cultureInfo = new CultureInfo(NopCommonDefaults.DefaultLanguageCulture);
            var regionInfo = new RegionInfo(NopCommonDefaults.DefaultLanguageCulture);

            var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);
            if (model.InstallRegionalResources)
            {
                //try to get CultureInfo and RegionInfo
                if (model.Country != null)
                {
                    try
                    {
                        cultureInfo = new CultureInfo(model.Country[3..]);
                        regionInfo = new RegionInfo(model.Country[3..]);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                //get URL to download language pack
                if (cultureInfo.Name != NopCommonDefaults.DefaultLanguageCulture)
                {
                    try
                    {
                        var languageCode = _locService.Value.GetCurrentLanguage().Code[0..2];
                        var resultString = await _nopHttpClient.Value.InstallationCompletedAsync(model.AdminEmail, languageCode, cultureInfo.Name);
                        var result = JsonConvert.DeserializeAnonymousType(resultString,
                            new { Message = string.Empty, LanguagePack = new { Culture = string.Empty, Progress = 0, DownloadLink = string.Empty } });

                        if (result != null && result.LanguagePack.Progress > NopCommonDefaults.LanguagePackMinTranslationProgressToInstall)
                        {
                            languagePackInfo.DownloadUrl = result.LanguagePack.DownloadLink;
                            languagePackInfo.Progress = result.LanguagePack.Progress;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                //upload CLDR
                await _uploadService.Value.UploadLocalePatternAsync(cultureInfo);
            }

            if (model.SubscribeNewsletters)
            {
                try
                {
                    var resultRequest = await _nopHttpClient.Value.SubscribeNewsLettersAsync(model.AdminEmail);
                }
                catch
                {
                    // ignored
                }
            }

            //now resolve installation service and install nopCommerce
            await _installationService.Value.InstallAsync(new InstallationSettings
            {
               AdminEmail = model.AdminEmail,
               AdminPassword = model.AdminPassword,
               LanguagePackDownloadLink = languagePackInfo.DownloadUrl,
               LanguagePackProgress = languagePackInfo.Progress,
               RegionInfo = regionInfo,
               CultureInfo = cultureInfo,
               InstallSampleData = model.InstallSampleData
            });
            
            //prepare plugins to install
            _pluginService.Value.ClearInstalledPluginsList();

            var pluginsIgnoredDuringInstallation = new List<string>();
            if (!string.IsNullOrEmpty(_appSettings.Get<InstallationConfig>().DisabledPlugins))
            {
                pluginsIgnoredDuringInstallation = _appSettings.Get<InstallationConfig>().DisabledPlugins
                    .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pluginName => pluginName.Trim()).ToList();
            }

            var plugins = (await _pluginService.Value.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
                .Where(pluginDescriptor => !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                .OrderBy(pluginDescriptor => pluginDescriptor.Group).ThenBy(pluginDescriptor => pluginDescriptor.DisplayOrder)
                .ToList();

            foreach (var plugin in plugins)
            {
                await _pluginService.Value.PreparePluginToInstallAsync(plugin.SystemName, checkDependencies: false);
            }

            return View(new InstallModel { RestartUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) });

        }
        catch (Exception exception)
        {
            await _staticCacheManager.Value.ClearAsync();

            //clear provider settings if something got wrong
            DataSettingsManager.SaveSettings(new DataConfig(), _fileProvider);

            ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("SetupFailed"), exception.Message));
        }

        return View(model);
    }

    public virtual IActionResult ChangeLanguage(string language)
    {
		_logger.LogDebug(">>> Entering ChangeLanguage(language={Language})", language);
        if (DataSettingsManager.IsDatabaseInstalled())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        _locService.Value.SaveCurrentLanguage(language);

        //Reload the page
		_logger.LogDebug("<<< Exiting ChangeLanguage(language={Language})", language);
        return RedirectToAction("Index", "Install");
    }

    [HttpPost]
    public virtual IActionResult RestartInstall()
    {
		_logger.LogDebug(">>> Entering RestartInstall()");
        if (DataSettingsManager.IsDatabaseInstalled())
		_logger.LogDebug("<<< Exiting RestartInstall()");
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        return View("Index", new InstallModel { RestartUrl = Url.RouteUrl(NopRouteNames.Standard.INSTALLATION) });
    }

    public virtual IActionResult RestartApplication()
    {
		_logger.LogDebug(">>> Entering RestartApplication()");
        if (DataSettingsManager.IsDatabaseInstalled())
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //restart application
        _webHelper.Value.RestartAppDomain();

		_logger.LogDebug("<<< Exiting RestartApplication()");
        return new EmptyResult();
    }

    #endregion
}