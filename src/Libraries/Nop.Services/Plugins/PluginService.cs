using System.Reflection;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Installation;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Plugins;

/// <summary>
/// Represents the plugin service implementation
/// </summary>
public partial class PluginService : IPluginService
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly Lazy<IMigrationManager> _migrationManager;
    protected readonly ILogger _logger;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IPluginsInfo _pluginsInfo;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly MediaSettings _mediaSettings;

    #endregion

    #region Ctor

    public PluginService(CatalogSettings catalogSettings,
        ICustomerService customerService,
        IHttpContextAccessor httpContextAccessor,
        Lazy<IMigrationManager> migrationManager,
        ILogger logger,
        INopFileProvider fileProvider,
        ISettingService settingService,
        IWebHelper webHelper,
        MediaSettings mediaSettings)
    {
        _catalogSettings = catalogSettings;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _catalogSettings → {_catalogSettings}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _catalogSettings);
        _customerService = customerService;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _customerService → {_customerService}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _customerService);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _httpContextAccessor → {_httpContextAccessor}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _httpContextAccessor);
        _migrationManager = migrationManager;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _migrationManager → {_migrationManager}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _migrationManager);
        _logger = logger;
		_logger.LogDebug(">>> Entering PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings})", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings);
        _fileProvider = fileProvider;
        _pluginsInfo = Singleton<IPluginsInfo>.Instance;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _fileProvider → {_fileProvider}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _fileProvider);
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _pluginsInfo → {_pluginsInfo}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _pluginsInfo);
        _settingService = settingService;
        _webHelper = webHelper;
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _settingService → {_settingService}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _settingService);
		_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _webHelper → {_webHelper}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _webHelper);
		_logger.LogDebug("<<< Exiting PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings})", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings);
        _mediaSettings = mediaSettings;
    }
	_logger.LogDebug("PluginService(catalogSettings={CatalogSettings},customerService={CustomerService},httpContextAccessor={HttpContextAccessor},migrationManager={MigrationManager},logger={Logger},fileProvider={FileProvider},settingService={SettingService},webHelper={WebHelper},mediaSettings={MediaSettings}): _mediaSettings → {_mediaSettings}", catalogSettings, customerService, httpContextAccessor, migrationManager, logger, fileProvider, settingService, webHelper, mediaSettings, _mediaSettings);

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether to load the plugin based on the load mode passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="loadMode">Load plugins mode</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
    {
		_logger.LogDebug(">>> Entering FilterByLoadMode(pluginDescriptor={PluginDescriptor},loadMode={LoadMode})", pluginDescriptor, loadMode);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

		_logger.LogDebug("FilterByLoadMode(pluginDescriptor={PluginDescriptor},loadMode={LoadMode}): LoadPluginsMode.InstalledOnly → {Arg2}", pluginDescriptor, loadMode, LoadPluginsMode.InstalledOnly);
		_logger.LogDebug("FilterByLoadMode(pluginDescriptor={PluginDescriptor},loadMode={LoadMode}): LoadPluginsMode.All → {Arg2}", pluginDescriptor, loadMode, LoadPluginsMode.All);
		_logger.LogDebug("FilterByLoadMode(pluginDescriptor={PluginDescriptor},loadMode={LoadMode}): LoadPluginsMode.NotInstalledOnly → {Arg2}", pluginDescriptor, loadMode, LoadPluginsMode.NotInstalledOnly);
		_logger.LogDebug("<<< Exiting FilterByLoadMode(pluginDescriptor={PluginDescriptor},loadMode={LoadMode})", pluginDescriptor, loadMode);
        return loadMode switch
        {
            LoadPluginsMode.All => true,
            LoadPluginsMode.InstalledOnly => pluginDescriptor.Installed,
            LoadPluginsMode.NotInstalledOnly => !pluginDescriptor.Installed,
            _ => throw new NotSupportedException(nameof(loadMode)),
        };
    }

    /// <summary>
    /// Check whether to load the plugin based on the plugin group passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="group">Group name</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByPluginGroup(PluginDescriptor pluginDescriptor, string group)
    {
		_logger.LogDebug(">>> Entering FilterByPluginGroup(pluginDescriptor={PluginDescriptor},group={Group})", pluginDescriptor, group);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        if (string.IsNullOrEmpty(group))
		_logger.LogDebug("<<< Exiting FilterByPluginGroup(pluginDescriptor={PluginDescriptor},group={Group})", pluginDescriptor, group);
            return true;

        return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Check whether to load the plugin based on the customer passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of check
    /// </returns>
    protected virtual async Task<bool> FilterByCustomerAsync(PluginDescriptor pluginDescriptor, Customer customer)
    {
		_logger.LogDebug(">>> Entering FilterByCustomerAsync(pluginDescriptor={PluginDescriptor},customer={Customer})", pluginDescriptor, customer);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
            return true;

        if (_catalogSettings.IgnoreAcl)
		_logger.LogDebug("<<< Exiting FilterByCustomerAsync(pluginDescriptor={PluginDescriptor},customer={Customer})", pluginDescriptor, customer);
            return true;

        return pluginDescriptor.LimitedToCustomerRoles.Intersect(await _customerService.GetCustomerRoleIdsAsync(customer)).Any();
    }

    /// <summary>
    /// Check whether to load the plugin based on the store identifier passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByStore(PluginDescriptor pluginDescriptor, int storeId)
    {
		_logger.LogDebug(">>> Entering FilterByStore(pluginDescriptor={PluginDescriptor},storeId={StoreId})", pluginDescriptor, storeId);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        //no validation required
        if (storeId == 0)
            return true;

        if (!pluginDescriptor.LimitedToStores.Any())
		_logger.LogDebug("<<< Exiting FilterByStore(pluginDescriptor={PluginDescriptor},storeId={StoreId})", pluginDescriptor, storeId);
            return true;

        return pluginDescriptor.LimitedToStores.Contains(storeId);
    }

    /// <summary>
    /// Check whether to load the plugin based on dependency from other plugin
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="dependsOnSystemName">Other plugin system name</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByDependsOn(PluginDescriptor pluginDescriptor, string dependsOnSystemName)
    {
		_logger.LogDebug(">>> Entering FilterByDependsOn(pluginDescriptor={PluginDescriptor},dependsOnSystemName={DependsOnSystemName})", pluginDescriptor, dependsOnSystemName);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        if (string.IsNullOrEmpty(dependsOnSystemName))
		_logger.LogDebug("<<< Exiting FilterByDependsOn(pluginDescriptor={PluginDescriptor},dependsOnSystemName={DependsOnSystemName})", pluginDescriptor, dependsOnSystemName);
            return true;

        return pluginDescriptor.DependsOn?.Contains(dependsOnSystemName) ?? false;
    }

    /// <summary>
    /// Check whether to load the plugin based on the plugin friendly name passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="friendlyName">Plugin friendly name</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByPluginFriendlyName(PluginDescriptor pluginDescriptor, string friendlyName)
    {
		_logger.LogDebug(">>> Entering FilterByPluginFriendlyName(pluginDescriptor={PluginDescriptor},friendlyName={FriendlyName})", pluginDescriptor, friendlyName);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        if (string.IsNullOrEmpty(friendlyName))
		_logger.LogDebug("<<< Exiting FilterByPluginFriendlyName(pluginDescriptor={PluginDescriptor},friendlyName={FriendlyName})", pluginDescriptor, friendlyName);
            return true;

        return pluginDescriptor.FriendlyName.Contains(friendlyName, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Check whether to load the plugin based on the plugin author passed
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor to check</param>
    /// <param name="author">Plugin author</param>
    /// <returns>Result of check</returns>
    protected virtual bool FilterByPluginAuthor(PluginDescriptor pluginDescriptor, string author)
    {
		_logger.LogDebug(">>> Entering FilterByPluginAuthor(pluginDescriptor={PluginDescriptor},author={Author})", pluginDescriptor, author);
        ArgumentNullException.ThrowIfNull(pluginDescriptor);

        if (string.IsNullOrEmpty(author))
		_logger.LogDebug("<<< Exiting FilterByPluginAuthor(pluginDescriptor={PluginDescriptor},author={Author})", pluginDescriptor, author);
            return true;

        return pluginDescriptor.Author.Contains(author, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Insert plugin data
    /// </summary>
    /// <param name="pluginType">Plugin type</param>
    /// <param name="migrationProcessType">Migration process type</param>
    protected virtual void InsertPluginData(Type pluginType, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
    {
		_logger.LogDebug(">>> Entering InsertPluginData(pluginType={PluginType},migrationProcessType={MigrationProcessType})", pluginType, migrationProcessType);
        var assembly = Assembly.GetAssembly(pluginType);
        _migrationManager.Value.ApplyUpMigrations(assembly, migrationProcessType);
		_logger.LogDebug("InsertPluginData(pluginType={PluginType},migrationProcessType={MigrationProcessType}): assembly → {Assembly}", pluginType, migrationProcessType, assembly);

        //mark update migrations as applied
        if (migrationProcessType == MigrationProcessType.Installation)
        {
            _migrationManager.Value.ApplyUpMigrations(assembly, MigrationProcessType.Update, true);
        }
		_logger.LogDebug("<<< Exiting InsertPluginData(pluginType={PluginType},migrationProcessType={MigrationProcessType})", pluginType, migrationProcessType);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get plugin descriptors
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
    /// <param name="loadMode">Filter by load plugins mode</param>
    /// <param name="customer">Filter by  customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <param name="friendlyName">Filter by plugin friendly name; pass null to load all records</param>
    /// <param name="author">Filter by plugin author; pass null to load all records</param>
    /// <param name="dependsOnSystemName">System name of the plugin to define dependencies</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin descriptors
    /// </returns>
    public virtual async Task<IList<PluginDescriptor>> GetPluginDescriptorsAsync<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string group = null, string dependsOnSystemName = "", string friendlyName = null, string author = null) where TPlugin : class, IPlugin
    {
		_logger.LogDebug(">>> Entering GetPluginDescriptorsAsync(loadMode={LoadMode},customer={Customer},storeId={StoreId},group={Group},dependsOnSystemName={DependsOnSystemName},friendlyName={FriendlyName},author={Author})", loadMode, customer, storeId, group, dependsOnSystemName, friendlyName, author);
        var pluginDescriptors = _pluginsInfo.PluginDescriptors.Select(p => p.pluginDescriptor).ToList();

		_logger.LogDebug("GetPluginDescriptorsAsync(loadMode={LoadMode},customer={Customer},storeId={StoreId},group={Group},dependsOnSystemName={DependsOnSystemName},friendlyName={FriendlyName},author={Author}): pluginDescriptors → {PluginDescriptors}", loadMode, customer, storeId, group, dependsOnSystemName, friendlyName, author, pluginDescriptors);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        //filter plugins
        pluginDescriptors = await pluginDescriptors.WhereAwait(async descriptor =>
            FilterByLoadMode(descriptor, loadMode) &&
            await FilterByCustomerAsync(descriptor, customer) &&
            FilterByStore(descriptor, storeId) &&
            FilterByPluginGroup(descriptor, group) &&
            FilterByDependsOn(descriptor, dependsOnSystemName) &&
            FilterByPluginFriendlyName(descriptor, friendlyName) &&
            FilterByPluginAuthor(descriptor, author)).ToListAsync();
			_logger.LogInformation("GetPluginDescriptorsAsync(loadMode,customer,storeId,group,dependsOnSystemName,friendlyName,author)={LoadMode},{Customer},{StoreId},{Group},{DependsOnSystemName},{FriendlyName},{Author}: database query executed in {Arg7} ms", loadMode, customer, storeId, group, dependsOnSystemName, friendlyName, author, __logStart.ElapsedMilliseconds);

        //filter by the passed type
        if (typeof(TPlugin) != typeof(IPlugin))
            pluginDescriptors = pluginDescriptors.Where(descriptor => typeof(TPlugin).IsAssignableFrom(descriptor.PluginType)).ToList();

        //order by group name
        pluginDescriptors = pluginDescriptors.OrderBy(descriptor => descriptor.Group)
            .ThenBy(descriptor => descriptor.DisplayOrder).ToList();

		_logger.LogDebug("<<< Exiting GetPluginDescriptorsAsync(loadMode={LoadMode},customer={Customer},storeId={StoreId},group={Group},dependsOnSystemName={DependsOnSystemName},friendlyName={FriendlyName},author={Author})", loadMode, customer, storeId, group, dependsOnSystemName, friendlyName, author);
        return pluginDescriptors;
    }

    /// <summary>
    /// Get a plugin descriptor by the system name
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugin to get</typeparam>
    /// <param name="systemName">Plugin system name</param>
    /// <param name="loadMode">Load plugins mode</param>
    /// <param name="customer">Filter by  customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the >Plugin descriptor
    /// </returns>
    public virtual async Task<PluginDescriptor> GetPluginDescriptorBySystemNameAsync<TPlugin>(string systemName,
        LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string @group = null) where TPlugin : class, IPlugin
    {
		_logger.LogDebug(">>> Entering GetPluginDescriptorBySystemNameAsync(systemName={SystemName},loadMode={LoadMode},customer={Customer},storeId={StoreId})", systemName, loadMode, customer, storeId);
        return (await GetPluginDescriptorsAsync<TPlugin>(loadMode, customer, storeId, group))
            .FirstOrDefault(descriptor => descriptor.SystemName.Equals(systemName));
			_logger.LogDebug("<<< Exiting GetPluginDescriptorBySystemNameAsync(systemName={SystemName},loadMode={LoadMode},customer={Customer},storeId={StoreId})", systemName, loadMode, customer, storeId);
    }

    /// <summary>
    /// Get plugins
    /// </summary>
    /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
    /// <param name="loadMode">Filter by load plugins mode</param>
    /// <param name="customer">Filter by customer; pass null to load all records</param>
    /// <param name="storeId">Filter by store; pass 0 to load all records</param>
    /// <param name="group">Filter by plugin group; pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugins
    /// </returns>
    public virtual async Task<IList<TPlugin>> GetPluginsAsync<TPlugin>(
        LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
        Customer customer = null, int storeId = 0, string @group = null) where TPlugin : class, IPlugin
    {
		_logger.LogDebug(">>> Entering GetPluginsAsync(loadMode={LoadMode},customer={Customer},storeId={StoreId})", loadMode, customer, storeId);
        return (await GetPluginDescriptorsAsync<TPlugin>(loadMode, customer, storeId, group))
            .Select(descriptor => descriptor.Instance<TPlugin>()).ToList();
			_logger.LogDebug("<<< Exiting GetPluginsAsync(loadMode={LoadMode},customer={Customer},storeId={StoreId})", loadMode, customer, storeId);
    }

    /// <summary>
    /// Find a plugin by the type which is located into the same assembly as a plugin
    /// </summary>
    /// <param name="typeInAssembly">Type</param>
    /// <returns>Plugin</returns>
    public virtual IPlugin FindPluginByTypeInAssembly(Type typeInAssembly)
    {
		_logger.LogDebug(">>> Entering FindPluginByTypeInAssembly(typeInAssembly={TypeInAssembly})", typeInAssembly);
        ArgumentNullException.ThrowIfNull(typeInAssembly);

        //try to do magic
        var pluginDescriptor = _pluginsInfo.PluginDescriptors.FirstOrDefault(descriptor =>
            descriptor.pluginDescriptor?.ReferencedAssembly?.FullName?.Equals(typeInAssembly.Assembly.FullName,
                StringComparison.InvariantCultureIgnoreCase) ?? false);

		_logger.LogDebug("<<< Exiting FindPluginByTypeInAssembly(typeInAssembly={TypeInAssembly})", typeInAssembly);
        return pluginDescriptor.pluginDescriptor?.Instance<IPlugin>();
    }

    /// <summary>
    /// Get plugin logo URL
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the logo URL
    /// </returns>
    public virtual Task<string> GetPluginLogoUrlAsync(PluginDescriptor pluginDescriptor)
    {
		_logger.LogDebug(">>> Entering GetPluginLogoUrlAsync(pluginDescriptor={PluginDescriptor})", pluginDescriptor);
        var pluginDirectory = _fileProvider.GetDirectoryName(pluginDescriptor.OriginalAssemblyFile);
        if (string.IsNullOrEmpty(pluginDirectory))
			_logger.LogDebug("GetPluginLogoUrlAsync(pluginDescriptor={PluginDescriptor}): pluginDirectory → {PluginDirectory}", pluginDescriptor, pluginDirectory);
            return Task.FromResult<string>(null);

        //check for supported extensions
        var logoExtension = NopPluginDefaults.SupportedLogoImageExtensions
            .FirstOrDefault(ext => _fileProvider.FileExists(_fileProvider.Combine(pluginDirectory, $"{NopPluginDefaults.LogoFileName}.{ext}")));
        if (string.IsNullOrWhiteSpace(logoExtension))
            return Task.FromResult<string>(null);

        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
        var logoPathUrl = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
		_logger.LogDebug("GetPluginLogoUrlAsync(pluginDescriptor={PluginDescriptor}): logoPathUrl → {LogoPathUrl}", pluginDescriptor, logoPathUrl);

        var logoUrl = $"{logoPathUrl}{NopPluginDefaults.PathName}/" +
                      $"{_fileProvider.GetDirectoryNameOnly(pluginDirectory)}/{NopPluginDefaults.LogoFileName}.{logoExtension}";

		_logger.LogDebug("<<< Exiting GetPluginLogoUrlAsync(pluginDescriptor={PluginDescriptor})", pluginDescriptor);
        return Task.FromResult(logoUrl);
    }

    /// <summary>
    /// Prepare plugin to the installation
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <param name="customer">Customer</param>
    /// <param name="checkDependencies">Specifies whether to check plugin dependencies</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginToInstallAsync(string systemName, Customer customer = null, bool checkDependencies = true)
    {
		_logger.LogDebug(">>> Entering PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies})", systemName, customer, checkDependencies);
        //add plugin name to the appropriate list (if not yet contained) and save changes
        if (_pluginsInfo.PluginNamesToInstall.Any(item => item.SystemName == systemName))
            return;

        var pluginsAfterRestart = _pluginsInfo.InstalledPlugins.Select(pd => pd.SystemName).Where(installedSystemName => !_pluginsInfo.PluginNamesToUninstall.Contains(installedSystemName)).ToList();
		_logger.LogDebug("PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies}): pluginsAfterRestart → {PluginsAfterRestart}", systemName, customer, checkDependencies, pluginsAfterRestart);
        pluginsAfterRestart.AddRange(_pluginsInfo.PluginNamesToInstall.Select(item => item.SystemName));

        if (checkDependencies)
        {
            var descriptor = await GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.NotInstalledOnly);
			_logger.LogDebug("PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies}): descriptor → {Descriptor}", systemName, customer, checkDependencies, descriptor);

            if (descriptor.DependsOn?.Any() ?? false)
            {
                var dependsOn = descriptor.DependsOn
                    .Where(dependsOnSystemName => !pluginsAfterRestart.Contains(dependsOnSystemName)).ToList();

                if (dependsOn.Any())
                {
                    var dependsOnSystemNames = dependsOn.Aggregate((all, current) => $"{all}, {current}");
					_logger.LogDebug("PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies}): dependsOnSystemNames → {DependsOnSystemNames}", systemName, customer, checkDependencies, dependsOnSystemNames);

                    //do not inject services via constructor because it'll cause circular references
                    var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
					_logger.LogDebug("PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies}): localizationService → {LocalizationService}", systemName, customer, checkDependencies, localizationService);

                    var errorMessage = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.InstallDependsOn"), string.IsNullOrEmpty(descriptor.FriendlyName) ? descriptor.SystemName : descriptor.FriendlyName, dependsOnSystemNames);
					_logger.LogDebug("PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies}): errorMessage → {ErrorMessage}", systemName, customer, checkDependencies, errorMessage);

					_logger.LogWarning("Throwing NopException from PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies})", systemName, customer, checkDependencies);
                    throw new NopException(errorMessage);
                }
            }
        }

        _pluginsInfo.PluginNamesToInstall.Add((systemName, customer?.CustomerGuid));
        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting PreparePluginToInstallAsync(systemName={SystemName},customer={Customer},checkDependencies={CheckDependencies})", systemName, customer, checkDependencies);
    }

    /// <summary>
    /// Prepare plugin to the uninstallation
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginToUninstallAsync(string systemName)
    {
		_logger.LogDebug(">>> Entering PreparePluginToUninstallAsync(systemName={SystemName})", systemName);
        //add plugin name to the appropriate list (if not yet contained) and save changes
        if (_pluginsInfo.PluginNamesToUninstall.Contains(systemName))
            return;

        var dependentPlugins = await GetPluginDescriptorsAsync<IPlugin>(dependsOnSystemName: systemName);
		_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): dependentPlugins → {DependentPlugins}", systemName, dependentPlugins);
        var descriptor = await GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName);
		_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): descriptor → {Descriptor}", systemName, descriptor);

        if (dependentPlugins.Any())
        {
            var dependsOn = new List<string>();
			_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): dependsOn → {DependsOn}", systemName, dependsOn);

            foreach (var dependentPlugin in dependentPlugins)
            {
                if (!_pluginsInfo.InstalledPlugins.Select(pd => pd.SystemName).Contains(dependentPlugin.SystemName))
                    continue;
                if (_pluginsInfo.PluginNamesToUninstall.Contains(dependentPlugin.SystemName))
                    continue;

                dependsOn.Add(string.IsNullOrEmpty(dependentPlugin.FriendlyName)
                    ? dependentPlugin.SystemName
                    : dependentPlugin.FriendlyName);
            }

            if (dependsOn.Any())
            {
                var dependsOnSystemNames = dependsOn.Aggregate((all, current) => $"{all}, {current}");
				_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): dependsOnSystemNames → {DependsOnSystemNames}", systemName, dependsOnSystemNames);

                //do not inject services via constructor because it'll cause circular references
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
				_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): localizationService → {LocalizationService}", systemName, localizationService);

                var errorMessage = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.UninstallDependsOn"),
                    string.IsNullOrEmpty(descriptor.FriendlyName) ? descriptor.SystemName : descriptor.FriendlyName,
                    dependsOnSystemNames);

				_logger.LogWarning("Throwing NopException from PreparePluginToUninstallAsync(systemName={SystemName})", systemName);
                throw new NopException(errorMessage);
            }
        }

        var plugin = descriptor?.Instance<IPlugin>();
		_logger.LogDebug("PreparePluginToUninstallAsync(systemName={SystemName}): plugin → {Plugin}", systemName, plugin);

        if (plugin != null)
            await plugin.PreparePluginToUninstallAsync();

        _pluginsInfo.PluginNamesToUninstall.Add(systemName);
        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting PreparePluginToUninstallAsync(systemName={SystemName})", systemName);
    }

    /// <summary>
    /// Prepare plugin to the removing
    /// </summary>
    /// <param name="systemName">Plugin system name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginToDeleteAsync(string systemName)
    {
		_logger.LogDebug(">>> Entering PreparePluginToDeleteAsync(systemName={SystemName})", systemName);
        //add plugin name to the appropriate list (if not yet contained) and save changes
        if (_pluginsInfo.PluginNamesToDelete.Contains(systemName))
            return;

        _pluginsInfo.PluginNamesToDelete.Add(systemName);
        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting PreparePluginToDeleteAsync(systemName={SystemName})", systemName);
    }

    /// <summary>
    /// Reset changes
    /// </summary>
    public virtual void ResetChanges()
    {
		_logger.LogDebug(">>> Entering ResetChanges()");
        //clear lists and save changes
        _pluginsInfo.PluginNamesToDelete.Clear();
        _pluginsInfo.PluginNamesToInstall.Clear();
        _pluginsInfo.PluginNamesToUninstall.Clear();
        _pluginsInfo.Save();

        //display all plugins on the plugin list page
        var pluginDescriptors = _pluginsInfo.PluginDescriptors.ToList();
		_logger.LogDebug("ResetChanges(): pluginDescriptors → {PluginDescriptors}", pluginDescriptors);
        foreach (var pluginDescriptor in pluginDescriptors)
            pluginDescriptor.pluginDescriptor.ShowInPluginsList = true;

        //clear the uploaded directory
        foreach (var directory in _fileProvider.GetDirectories(_fileProvider.MapPath(NopPluginDefaults.UploadedPath)))
            _fileProvider.DeleteDirectory(directory);
			_logger.LogDebug("<<< Exiting ResetChanges()");
    }

    /// <summary>
    /// Clear installed plugins list
    /// </summary>
    public virtual void ClearInstalledPluginsList()
    {
		_logger.LogDebug(">>> Entering ClearInstalledPluginsList()");
        _pluginsInfo.InstalledPlugins.Clear();
		_logger.LogDebug("<<< Exiting ClearInstalledPluginsList()");
    }

    /// <summary>
    /// Install plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InstallPluginsAsync()
    {
		_logger.LogDebug(">>> Entering InstallPluginsAsync()");
        //get all uninstalled plugins
        var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.pluginDescriptor.Installed).ToList();
		_logger.LogDebug("InstallPluginsAsync(): pluginDescriptors → {PluginDescriptors}", pluginDescriptors);

        //filter plugins need to install
        pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToInstall
            .Any(item => item.SystemName.Equals(descriptor.pluginDescriptor.SystemName))).ToList();
        if (!pluginDescriptors.Any())
            return;

        //do not inject services via constructor because it'll cause circular references
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogDebug("InstallPluginsAsync(): localizationService → {LocalizationService}", localizationService);
        var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
		_logger.LogDebug("InstallPluginsAsync(): customerActivityService → {CustomerActivityService}", customerActivityService);

        var installPluginSampleDataSetting =
            await _settingService.GetSettingAsync(NopInstallationDefaults.InstallPluginSampleDataSettingName);

        bool.TryParse(installPluginSampleDataSetting?.Value, out var installPluginSampleData);
        var removeInstallPluginSampleDataSetting = true;
		_logger.LogDebug("InstallPluginsAsync(): removeInstallPluginSampleDataSetting → {RemoveInstallPluginSampleDataSetting}", removeInstallPluginSampleDataSetting);

        //install plugins
        foreach (var descriptor in pluginDescriptors.OrderBy(pluginDescriptor => pluginDescriptor.pluginDescriptor.DisplayOrder))
        {
            try
            {
                InsertPluginData(descriptor.pluginDescriptor.PluginType, MigrationProcessType.Installation);

                var pluginInstance = descriptor.pluginDescriptor.Instance<IPlugin>();
				_logger.LogDebug("InstallPluginsAsync(): pluginInstance → {PluginInstance}", pluginInstance);
                //try to install an instance
                await pluginInstance.InstallAsync();

                if (installPluginSampleData)
                    //try to install a sample data of plugin
                    await pluginInstance.InstallSampleDataAsync();

                //remove and add plugin system name to appropriate lists
                var pluginToInstall = _pluginsInfo.PluginNamesToInstall
                    .FirstOrDefault(plugin => plugin.SystemName.Equals(descriptor.pluginDescriptor.SystemName));
                _pluginsInfo.InstalledPlugins.Add(descriptor.pluginDescriptor.GetBaseInfoCopy);
                _pluginsInfo.PluginNamesToInstall.Remove(pluginToInstall);

                //activity log
                var customer = await _customerService.GetCustomerByGuidAsync(pluginToInstall.CustomerGuid ?? Guid.Empty);
				_logger.LogDebug("InstallPluginsAsync(): customer → {Customer}", customer);
                await customerActivityService.InsertActivityAsync(customer, "InstallNewPlugin",
                    string.Format(await localizationService.GetResourceAsync("ActivityLog.InstallNewPlugin"), descriptor.pluginDescriptor.SystemName, descriptor.pluginDescriptor.Version));

                //mark the plugin as installed
                descriptor.pluginDescriptor.Installed = true;
                descriptor.pluginDescriptor.ShowInPluginsList = true;
            }
            catch (Exception exception)
            {
                //log error
                var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotInstalled"), descriptor.pluginDescriptor.SystemName);
				_logger.LogDebug("InstallPluginsAsync(): message → {Message}", message);
                await _logger.ErrorAsync(message, exception);

                removeInstallPluginSampleDataSetting = false;
            }
        }

        //save changes
        await _pluginsInfo.SaveAsync();

        if (removeInstallPluginSampleDataSetting && installPluginSampleDataSetting != null)
            await _settingService.DeleteSettingAsync(installPluginSampleDataSetting);
			_logger.LogDebug("<<< Exiting InstallPluginsAsync()");
    }

    /// <summary>
    /// Uninstall plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UninstallPluginsAsync()
    {
		_logger.LogDebug(">>> Entering UninstallPluginsAsync()");
        //get all installed plugins
        var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => descriptor.pluginDescriptor.Installed).ToList();
		_logger.LogDebug("UninstallPluginsAsync(): pluginDescriptors → {PluginDescriptors}", pluginDescriptors);

        //filter plugins need to uninstall
        pluginDescriptors = pluginDescriptors
            .Where(descriptor => _pluginsInfo.PluginNamesToUninstall.Contains(descriptor.pluginDescriptor.SystemName)).ToList();
        if (!pluginDescriptors.Any())
            return;

        //do not inject services via constructor because it'll cause circular references
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogDebug("UninstallPluginsAsync(): localizationService → {LocalizationService}", localizationService);
        var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
		_logger.LogDebug("UninstallPluginsAsync(): customerActivityService → {CustomerActivityService}", customerActivityService);

        //uninstall plugins
        foreach (var descriptor in pluginDescriptors.OrderByDescending(pluginDescriptor => pluginDescriptor.pluginDescriptor.DisplayOrder))
        {
            try
            {
                var plugin = descriptor.pluginDescriptor.Instance<IPlugin>();
				_logger.LogDebug("UninstallPluginsAsync(): plugin → {Plugin}", plugin);
                //try to uninstall an instance
                await plugin.UninstallAsync();

                //clear plugin data on the database
                var assembly = Assembly.GetAssembly(descriptor.pluginDescriptor.PluginType);
				_logger.LogDebug("UninstallPluginsAsync(): assembly → {Assembly}", assembly);
                _migrationManager.Value.ApplyDownMigrations(assembly);

                //remove plugin system name from appropriate lists
                _pluginsInfo.InstalledPlugins.Remove(descriptor.pluginDescriptor);
                _pluginsInfo.PluginNamesToUninstall.Remove(descriptor.pluginDescriptor.SystemName);

                //activity log
                await customerActivityService.InsertActivityAsync("UninstallPlugin",
                    string.Format(await localizationService.GetResourceAsync("ActivityLog.UninstallPlugin"), descriptor.pluginDescriptor.SystemName, descriptor.pluginDescriptor.Version));

                //mark the plugin as uninstalled
                descriptor.pluginDescriptor.Installed = false;
                descriptor.pluginDescriptor.ShowInPluginsList = true;
            }
            catch (Exception exception)
            {
                //log error
                var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotUninstalled"), descriptor.pluginDescriptor.SystemName);
				_logger.LogDebug("UninstallPluginsAsync(): message → {Message}", message);
                await _logger.ErrorAsync(message, exception);
            }
        }

        //save changes
        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting UninstallPluginsAsync()");
    }

    /// <summary>
    /// Delete plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeletePluginsAsync()
    {
		_logger.LogDebug(">>> Entering DeletePluginsAsync()");
        //get all uninstalled plugins (delete plugin only previously uninstalled)
        var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.pluginDescriptor.Installed).ToList();
		_logger.LogDebug("DeletePluginsAsync(): pluginDescriptors → {PluginDescriptors}", pluginDescriptors);

        //filter plugins need to delete
        pluginDescriptors = pluginDescriptors
            .Where(descriptor => _pluginsInfo.PluginNamesToDelete.Contains(descriptor.pluginDescriptor.SystemName)).ToList();
        if (!pluginDescriptors.Any())
            return;

        //do not inject services via constructor because it'll cause circular references
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogDebug("DeletePluginsAsync(): localizationService → {LocalizationService}", localizationService);
        var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
		_logger.LogDebug("DeletePluginsAsync(): customerActivityService → {CustomerActivityService}", customerActivityService);

        //delete plugins
        foreach (var descriptor in pluginDescriptors)
        {
            try
            {
                //try to delete a plugin directory from disk storage
                var pluginDirectory = _fileProvider.GetDirectoryName(descriptor.pluginDescriptor.OriginalAssemblyFile);
				_logger.LogDebug("DeletePluginsAsync(): pluginDirectory → {PluginDirectory}", pluginDirectory);
                if (_fileProvider.DirectoryExists(pluginDirectory))
                    _fileProvider.DeleteDirectory(pluginDirectory);

                //remove plugin system name from the appropriate list
                _pluginsInfo.PluginNamesToDelete.Remove(descriptor.pluginDescriptor.SystemName);

                //activity log
                await customerActivityService.InsertActivityAsync("DeletePlugin",
                    string.Format(await localizationService.GetResourceAsync("ActivityLog.DeletePlugin"), descriptor.pluginDescriptor.SystemName, descriptor.pluginDescriptor.Version));
            }
            catch (Exception exception)
            {
                //log error
                var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotDeleted"), descriptor.pluginDescriptor.SystemName);
				_logger.LogDebug("DeletePluginsAsync(): message → {Message}", message);
                await _logger.ErrorAsync(message, exception);
            }
        }

        //save changes
        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting DeletePluginsAsync()");
    }

    /// <summary>
    /// Check whether application restart is required to apply changes to plugins
    /// </summary>
    /// <returns>Result of check</returns>
    public virtual bool IsRestartRequired()
    {
		_logger.LogDebug(">>> Entering IsRestartRequired()");
        //return true if any of lists contains items or some plugins were uploaded
		_logger.LogDebug("<<< Exiting IsRestartRequired()");
        return _pluginsInfo.PluginNamesToInstall.Any()
               || _pluginsInfo.PluginNamesToUninstall.Any()
               || _pluginsInfo.PluginNamesToDelete.Any()
               || IsPluginsUploaded;
    }

    /// <summary>
    /// Update plugins
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdatePluginsAsync()
    {
		_logger.LogDebug(">>> Entering UpdatePluginsAsync()");
        //do not inject services via constructor because it'll cause circular references
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_logger.LogDebug("UpdatePluginsAsync(): localizationService → {LocalizationService}", localizationService);
        var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
		_logger.LogDebug("UpdatePluginsAsync(): customerActivityService → {CustomerActivityService}", customerActivityService);

        foreach (var installedPlugin in _pluginsInfo.InstalledPlugins)
        {
            var newVersion = _pluginsInfo.PluginDescriptors.FirstOrDefault(pd =>
                pd.pluginDescriptor.SystemName.Equals(installedPlugin.SystemName, StringComparison.InvariantCultureIgnoreCase));

            if (newVersion.pluginDescriptor == null)
                continue;

            if (installedPlugin.Version == newVersion.pluginDescriptor.Version)
                continue;

            //run new migrations from the plugin if there are exists
            InsertPluginData(newVersion.pluginDescriptor.PluginType, MigrationProcessType.Update);

            //run the plugin update logic
            await newVersion.pluginDescriptor.Instance<IPlugin>().UpdateAsync(installedPlugin.Version, newVersion.pluginDescriptor.Version);

            //activity log                
            await customerActivityService.InsertActivityAsync("UpdatePlugin",
                string.Format(await localizationService.GetResourceAsync("ActivityLog.UpdatePlugin"), newVersion.pluginDescriptor.SystemName, installedPlugin.Version, newVersion.pluginDescriptor.Version));

            //update installed plugin info
            installedPlugin.Version = newVersion.pluginDescriptor.Version;
			_logger.LogDebug("UpdatePluginsAsync(): installedPlugin.Version → {Arg0}", installedPlugin.Version);
        }

        await _pluginsInfo.SaveAsync();
		_logger.LogDebug("<<< Exiting UpdatePluginsAsync()");
    }

    /// <summary>
    /// Get names of incompatible plugins
    /// </summary>
    /// <returns>List of plugin names</returns>
    public virtual IDictionary<string, PluginIncompatibleType> GetIncompatiblePlugins()
    {
        return _pluginsInfo.IncompatiblePlugins;
    }

    /// <summary>
    /// Get all assembly loaded collisions
    /// </summary>
    /// <returns>List of plugin loaded assembly info</returns>
    public virtual IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions()
    {
		_logger.LogDebug(">>> Entering GetAssemblyCollisions()");
		_logger.LogDebug("<<< Exiting GetAssemblyCollisions()");
        return _pluginsInfo.AssemblyLoadedCollision;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether new or updated plugins have been uploaded.
    /// True - if the plugins were loaded, false otherwise
    /// </summary>
    protected virtual bool IsPluginsUploaded
    {
        get
        {
            var pluginsDirectories =
                _fileProvider.GetDirectories(_fileProvider.MapPath(NopPluginDefaults.UploadedPath));

            if (!pluginsDirectories.Any())
                return false;

            return pluginsDirectories.Any(d =>
                _fileProvider.GetFiles(d, "*.dll").Any() || _fileProvider.GetFiles(d, "plugin.json").Any());
        }
    }

    #endregion
}