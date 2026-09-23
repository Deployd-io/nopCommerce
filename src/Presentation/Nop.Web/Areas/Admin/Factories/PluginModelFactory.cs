using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Plugins.Marketplace;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Web.Areas.Admin.Models.Plugins.Marketplace;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the plugin model factory implementation
/// </summary>
public partial class PluginModelFactory : IPluginModelFactory
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IPluginService _pluginService;
    protected readonly ISearchPluginManager _searchPluginManager;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly IWidgetPluginManager _widgetPluginManager;
    protected readonly IWorkContext _workContext;
    protected readonly OfficialFeedManager _officialFeedManager;

    #endregion

    #region Ctor

    public PluginModelFactory(IAuthenticationPluginManager authenticationPluginManager,
        IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        ILocalizedModelFactory localizedModelFactory,
        IPaymentPluginManager paymentPluginManager,
        IPickupPluginManager pickupPluginManager,
        IPluginService pluginService,
        ISearchPluginManager searchPluginManager,
        IShippingPluginManager shippingPluginManager,
        IStaticCacheManager staticCacheManager,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        ITaxPluginManager taxPluginManager,
        IWidgetPluginManager widgetPluginManager,
        IWorkContext workContext,
        OfficialFeedManager officialFeedManager)
    {
		_logger.LogError(">>> Entering PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager})", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager);
        _authenticationPluginManager = authenticationPluginManager;
        _baseAdminModelFactory = baseAdminModelFactory;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _baseAdminModelFactory → {_baseAdminModelFactory}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _baseAdminModelFactory);
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _authenticationPluginManager → {_authenticationPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _authenticationPluginManager);
    _logger.LogWarning("<<< Exiting PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager})", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager);
        _localizationService = localizationService;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _localizationService → {_localizationService}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _localizationService);
        _localizedModelFactory = localizedModelFactory;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _localizedModelFactory → {_localizedModelFactory}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _localizedModelFactory);
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _multiFactorAuthenticationPluginManager → {_multiFactorAuthenticationPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _multiFactorAuthenticationPluginManager);
        _paymentPluginManager = paymentPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _paymentPluginManager → {_paymentPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _paymentPluginManager);
        _pickupPluginManager = pickupPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _pickupPluginManager → {_pickupPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _pickupPluginManager);
        _pluginService = pluginService;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _pluginService → {_pluginService}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _pluginService);
        _searchPluginManager = searchPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _searchPluginManager → {_searchPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _searchPluginManager);
        _shippingPluginManager = shippingPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _shippingPluginManager → {_shippingPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _shippingPluginManager);
        _staticCacheManager = staticCacheManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _staticCacheManager → {_staticCacheManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _staticCacheManager);
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _storeMappingSupportedModelFactory → {_storeMappingSupportedModelFactory}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _storeMappingSupportedModelFactory);
        _taxPluginManager = taxPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _taxPluginManager → {_taxPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _taxPluginManager);
        _widgetPluginManager = widgetPluginManager;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _widgetPluginManager → {_widgetPluginManager}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _widgetPluginManager);
        _workContext = workContext;
		_logger.LogDebug("PluginModelFactory(authenticationPluginManager={AuthenticationPluginManager},baseAdminModelFactory={BaseAdminModelFactory},localizationService={LocalizationService},multiFactorAuthenticationPluginManager={MultiFactorAuthenticationPluginManager},localizedModelFactory={LocalizedModelFactory},paymentPluginManager={PaymentPluginManager},pickupPluginManager={PickupPluginManager},pluginService={PluginService},searchPluginManager={SearchPluginManager},shippingPluginManager=<redacted>,staticCacheManager={StaticCacheManager},storeMappingSupportedModelFactory=<redacted>,taxPluginManager={TaxPluginManager},widgetPluginManager={WidgetPluginManager},workContext={WorkContext},officialFeedManager={OfficialFeedManager}): _workContext → {_workContext}", authenticationPluginManager, baseAdminModelFactory, localizationService, multiFactorAuthenticationPluginManager, localizedModelFactory, paymentPluginManager, pickupPluginManager, pluginService, searchPluginManager, staticCacheManager, taxPluginManager, widgetPluginManager, workContext, officialFeedManager, _workContext);
        _officialFeedManager = officialFeedManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare plugin model properties of the installed plugin
    /// </summary>
    /// <param name="model">Plugin model</param>
    /// <param name="plugin">Plugin</param>
    protected virtual void PrepareInstalledPluginModel(PluginModel model, IPlugin plugin)
    {
		_logger.LogError(">>> Entering PrepareInstalledPluginModel(model={Model},plugin={Plugin})", model, plugin);
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(plugin);

        //prepare configuration URL
        model.ConfigurationUrl = plugin.GetConfigurationPageUrl();

        //prepare enabled/disabled (only for some plugin types)
        model.CanChangeEnabled = true;
		_logger.LogDebug("PrepareInstalledPluginModel(model={Model},plugin={Plugin}): model.CanChangeEnabled → {Arg2}", model, plugin, model.CanChangeEnabled);
        switch (plugin)
        {
            case IMiscPlugin:
                model.CanChangeEnabled = false;
                break;

            case IPaymentMethod paymentMethod:
                model.IsEnabled = _paymentPluginManager.IsPluginActive(paymentMethod);
                break;

            case IShippingRateComputationMethod shippingRateComputationMethod:
                model.IsEnabled = _shippingPluginManager.IsPluginActive(shippingRateComputationMethod);
                break;

            case IPickupPointProvider pickupPointProvider:
                model.IsEnabled = _pickupPluginManager.IsPluginActive(pickupPointProvider);
                break;

            case ITaxProvider taxProvider:
                model.IsEnabled = _taxPluginManager.IsPluginActive(taxProvider);
                break;

            case IExternalAuthenticationMethod externalAuthenticationMethod:
                model.IsEnabled = _authenticationPluginManager.IsPluginActive(externalAuthenticationMethod);
                break;

            case IMultiFactorAuthenticationMethod multiFactorAuthenticationMethod:
                model.IsEnabled = _multiFactorAuthenticationPluginManager.IsPluginActive(multiFactorAuthenticationMethod);
                break;

            case ISearchProvider searchProvider:
                model.IsEnabled = _searchPluginManager.IsPluginActive(searchProvider);
                break;

            case IWidgetPlugin widgetPlugin:
                model.IsEnabled = _widgetPluginManager.IsPluginActive(widgetPlugin);
                break;

            default:
                model.CanChangeEnabled = false;
                break;
        }
		_logger.LogWarning("<<< Exiting PrepareInstalledPluginModel(model={Model},plugin={Plugin})", model, plugin);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare plugin search model
    /// </summary>
    /// <param name="searchModel">Plugin search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin search model
    /// </returns>
    public virtual async Task<PluginSearchModel> PreparePluginSearchModelAsync(PluginSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePluginSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available load plugin modes
        await _baseAdminModelFactory.PrepareLoadPluginModesAsync(searchModel.AvailableLoadModes, false);

        //prepare available groups
        await _baseAdminModelFactory.PreparePluginGroupsAsync(searchModel.AvailableGroups);

        //prepare page parameters
        searchModel.SetGridPageSize();

        searchModel.NeedToRestart = _pluginService.IsRestartRequired();

		_logger.LogWarning("<<< Exiting PreparePluginSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged plugin list model
    /// </summary>
    /// <param name="searchModel">Plugin search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin list model
    /// </returns>
    public virtual async Task<PluginListModel> PreparePluginListModelAsync(PluginSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PreparePluginListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter plugins
        var group = string.IsNullOrEmpty(searchModel.SearchGroup) || searchModel.SearchGroup.Equals("0") ? null : searchModel.SearchGroup;
		_logger.LogError("PreparePluginListModelAsync(searchModel={SearchModel}): group → {Group}", searchModel, group);
        var loadMode = (LoadPluginsMode)searchModel.SearchLoadModeId;
		_logger.LogError("PreparePluginListModelAsync(searchModel={SearchModel}): loadMode → {LoadMode}", searchModel, loadMode);
        var friendlyName = string.IsNullOrEmpty(searchModel.SearchFriendlyName) ? null : searchModel.SearchFriendlyName;
		_logger.LogError("PreparePluginListModelAsync(searchModel={SearchModel}): friendlyName → {FriendlyName}", searchModel, friendlyName);
        var author = string.IsNullOrEmpty(searchModel.SearchAuthor) ? null : searchModel.SearchAuthor;
		_logger.LogError("PreparePluginListModelAsync(searchModel={SearchModel}): author → {Author}", searchModel, author);

        //filter visible plugins
        var plugins = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(group: group, loadMode: loadMode, friendlyName: friendlyName, author: author))
            .Where(p => p.ShowInPluginsList)
            .OrderBy(plugin => plugin.Group).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new PluginListModel().PrepareToGridAsync(searchModel, plugins, () =>
        {
            return plugins.SelectAwait(async pluginDescriptor =>
            {
                //fill in model values from the entity
                var pluginModel = pluginDescriptor.ToPluginModel<PluginModel>();
				_logger.LogError("PreparePluginListModelAsync(searchModel={SearchModel}): pluginModel → {PluginModel}", searchModel, pluginModel);

                //fill in additional values (not existing in the entity)
                pluginModel.LogoUrl = await _pluginService.GetPluginLogoUrlAsync(pluginDescriptor);

                if (pluginDescriptor.Installed)
                    PrepareInstalledPluginModel(pluginModel, pluginDescriptor.Instance<IPlugin>());

                return pluginModel;
            });
        });

		_logger.LogWarning("<<< Exiting PreparePluginListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare plugin model
    /// </summary>
    /// <param name="model">Plugin model</param>
    /// <param name="pluginDescriptor">Plugin descriptor</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the plugin model
    /// </returns>
    public virtual async Task<PluginModel> PreparePluginModelAsync(PluginModel model, PluginDescriptor pluginDescriptor, bool excludeProperties = false)
    {
		_logger.LogError(">>> Entering PreparePluginModelAsync(model={Model},pluginDescriptor={PluginDescriptor},excludeProperties={ExcludeProperties})", model, pluginDescriptor, excludeProperties);
        Func<PluginLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (pluginDescriptor != null)
        {
            //fill in model values from the entity
            model ??= pluginDescriptor.ToPluginModel(model);

            model.LogoUrl = await _pluginService.GetPluginLogoUrlAsync(pluginDescriptor);
            model.SelectedStoreIds = pluginDescriptor.LimitedToStores;
			_logger.LogDebug("PreparePluginModelAsync(model={Model},pluginDescriptor={PluginDescriptor},excludeProperties={ExcludeProperties}): model.SelectedStoreIds → {Arg3}", model, pluginDescriptor, excludeProperties, model.SelectedStoreIds);
			_logger.LogDebug("PreparePluginModelAsync(model={Model},pluginDescriptor={PluginDescriptor},excludeProperties={ExcludeProperties}): model.SelectedCustomerRoleIds → {Arg3}", model, pluginDescriptor, excludeProperties, model.SelectedCustomerRoleIds);
            model.SelectedCustomerRoleIds = pluginDescriptor.LimitedToCustomerRoles;
            var plugin = pluginDescriptor.Instance<IPlugin>();
			_logger.LogError("PreparePluginModelAsync(model={Model},pluginDescriptor={PluginDescriptor},excludeProperties={ExcludeProperties}): plugin → {Plugin}", model, pluginDescriptor, excludeProperties, plugin);
            if (pluginDescriptor.Installed)
                PrepareInstalledPluginModel(model, plugin);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.FriendlyName = await _localizationService.GetLocalizedFriendlyNameAsync(plugin, languageId, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);
        
        //prepare available stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model);

		_logger.LogWarning("<<< Exiting PreparePluginModelAsync(model={Model},pluginDescriptor={PluginDescriptor},excludeProperties={ExcludeProperties})", model, pluginDescriptor, excludeProperties);
        return model;
    }

    /// <summary>
    /// Prepare search model of plugins of the official feed
    /// </summary>
    /// <param name="searchModel">Search model of plugins of the official feed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search model of plugins of the official feed
    /// </returns>
    public virtual async Task<OfficialFeedPluginSearchModel> PrepareOfficialFeedPluginSearchModelAsync(OfficialFeedPluginSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available versions
        var pluginVersions = await _officialFeedManager.GetVersionsAsync();
		_logger.LogError("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): pluginVersions → {PluginVersions}", searchModel, pluginVersions);
        searchModel.AvailableVersions.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
        foreach (var version in pluginVersions)
            searchModel.AvailableVersions.Add(new SelectListItem { Text = version.Name, Value = version.Id.ToString() });

        //pre-select current version
        //current version name and named on official site do not match. that's why we use "Contains"
        var currentVersionItem = searchModel.AvailableVersions.FirstOrDefault(x => x.Text.Contains(NopVersion.CURRENT_VERSION));
		_logger.LogError("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): currentVersionItem → {CurrentVersionItem}", searchModel, currentVersionItem);
        if (currentVersionItem != null)
        {
            searchModel.SearchVersionId = int.Parse(currentVersionItem.Value);
            currentVersionItem.Selected = true;
			_logger.LogDebug("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): currentVersionItem.Selected → {Arg1}", searchModel, currentVersionItem.Selected);
        }

        //prepare available plugin categories
        var pluginCategories = await _officialFeedManager.GetCategoriesAsync();
		_logger.LogError("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): pluginCategories → {PluginCategories}", searchModel, pluginCategories);
        searchModel.AvailableCategories.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
        foreach (var pluginCategory in pluginCategories)
        {
            var pluginCategoryNames = new List<string>();
			_logger.LogError("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): pluginCategoryNames → {PluginCategoryNames}", searchModel, pluginCategoryNames);
            var tmpCategory = pluginCategory;
			_logger.LogError("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): tmpCategory → {TmpCategory}", searchModel, tmpCategory);
            while (tmpCategory != null)
            {
                pluginCategoryNames.Add(tmpCategory.Name);
                tmpCategory = pluginCategories.FirstOrDefault(category => category.Id == tmpCategory.ParentCategoryId);
            }

            pluginCategoryNames.Reverse();

            searchModel.AvailableCategories.Add(new SelectListItem
            {
                Value = pluginCategory.Id.ToString(),
                Text = string.Join(" >> ", pluginCategoryNames)
            });
				_logger.LogDebug("PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel}): Text → {Text}", searchModel, Text);
        }

        //prepare available prices
        searchModel.AvailablePrices.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Common.All")
        });
        searchModel.AvailablePrices.Add(new SelectListItem
        {
            Value = "10",
            Text = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.OfficialFeed.Price.Free")
        });
        searchModel.AvailablePrices.Add(new SelectListItem
        {
            Value = "20",
            Text = await _localizationService.GetResourceAsync("Admin.Configuration.Plugins.OfficialFeed.Price.Commercial")
        });

        //prepare page parameters
        searchModel.SetGridPageSize(15, "15");

		_logger.LogWarning("<<< Exiting PrepareOfficialFeedPluginSearchModelAsync(searchModel={SearchModel})", searchModel);
        return searchModel;
    }

    /// <summary>
    /// Prepare paged list model of plugins of the official feed
    /// </summary>
    /// <param name="searchModel">Search model of plugins of the official feed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list model of plugins of the official feed
    /// </returns>
    public virtual async Task<OfficialFeedPluginListModel> PrepareOfficialFeedPluginListModelAsync(OfficialFeedPluginSearchModel searchModel)
    {
		_logger.LogError(">>> Entering PrepareOfficialFeedPluginListModelAsync(searchModel={SearchModel})", searchModel);
        ArgumentNullException.ThrowIfNull(searchModel);

        //get plugins
        var plugins = await _officialFeedManager.GetAllPluginsAsync(categoryId: searchModel.SearchCategoryId,
            versionId: searchModel.SearchVersionId,
            price: searchModel.SearchPriceId,
            searchTerm: searchModel.SearchName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = new OfficialFeedPluginListModel().PrepareToGrid(searchModel, plugins, () =>
        {
            //fill in model values from the entity
            return plugins?.Select(plugin => new OfficialFeedPluginModel
            {
                Url = plugin.Url,
                Name = plugin.Name,
                CategoryName = plugin.Category,
                SupportedVersions = plugin.SupportedVersions,
                PictureUrl = plugin.PictureUrl,
                Price = plugin.Price
            }) ?? new List<OfficialFeedPluginModel>();
        });

		_logger.LogWarning("<<< Exiting PrepareOfficialFeedPluginListModelAsync(searchModel={SearchModel})", searchModel);
        return model;
    }

    /// <summary>
    /// Prepare plugin models for admin navigation
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of models
    /// </returns>
    public virtual async Task<IList<AdminNavigationPluginModel>> PrepareAdminNavigationPluginModelsAsync()
    {
		_logger.LogError(">>> Entering PrepareAdminNavigationPluginModelsAsync()");
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopPluginDefaults.AdminNavigationPluginsCacheKey, customer);
		_logger.LogError("PrepareAdminNavigationPluginModelsAsync(): customer → {Customer}", customer);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            //get installed plugins
			_logger.LogError("PrepareAdminNavigationPluginModelsAsync(): cacheKey → {CacheKey}", cacheKey);
			_logger.LogError("PrepareAdminNavigationPluginModelsAsync(): external service call took {Arg0} ms", __logStart.ElapsedMilliseconds);
			_logger.LogWarning("<<< Exiting PrepareAdminNavigationPluginModelsAsync()");
            return (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.InstalledOnly, customer))
                .Where(plugin => plugin.ShowInPluginsList)
                .Select(plugin => new AdminNavigationPluginModel
                {
                    FriendlyName = plugin.FriendlyName,
                    ConfigurationUrl = plugin.Instance<IPlugin>().GetConfigurationPageUrl()
                }).Where(model => !string.IsNullOrEmpty(model.ConfigurationUrl)).ToList();
        });
    }

    #endregion
}