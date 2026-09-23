using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Stores;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Framework;

/// <summary>
/// Store context for web application
/// </summary>
public partial class WebStoreContext : IStoreContext
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IRepository<Store> _storeRepository;
    protected readonly IStoreService _storeService;

    protected Store _cachedStore;
    protected int? _cachedActiveStoreScopeConfiguration;

    #endregion

    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="genericAttributeService">Generic attribute service</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="storeRepository">Store repository</param>
    /// <param name="storeService">Store service</param>
    public WebStoreContext(IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        IRepository<Store> storeRepository,
        IStoreService storeService)
    {
		_logger.LogError(">>> Entering WebStoreContext(genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},storeRepository={StoreRepository},storeService={StoreService})", genericAttributeService, httpContextAccessor, storeRepository, storeService);
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("WebStoreContext(genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},storeRepository={StoreRepository},storeService={StoreService}): _httpContextAccessor → {_httpContextAccessor}", genericAttributeService, httpContextAccessor, storeRepository, storeService, _httpContextAccessor);
        _storeRepository = storeRepository;
		_logger.LogDebug("WebStoreContext(genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},storeRepository={StoreRepository},storeService={StoreService}): _storeRepository → {_storeRepository}", genericAttributeService, httpContextAccessor, storeRepository, storeService, _storeRepository);
        _storeService = storeService;
		_logger.LogDebug("WebStoreContext(genericAttributeService={GenericAttributeService},httpContextAccessor={HttpContextAccessor},storeRepository={StoreRepository},storeService={StoreService}): _storeService → {_storeService}", genericAttributeService, httpContextAccessor, storeRepository, storeService, _storeService);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<Store> GetCurrentStoreAsync()
    {
		_logger.LogError(">>> Entering GetCurrentStoreAsync()");
        if (_cachedStore != null)
            return _cachedStore;

        //try to determine the current store by HOST header
        string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

        var allStores = await _storeService.GetAllStoresAsync();
		_logger.LogError("GetCurrentStoreAsync(): allStores → {AllStores}", allStores);
        var store = allStores.FirstOrDefault(s => _storeService.ContainsHostValue(s, host)) ?? allStores.FirstOrDefault();
		_logger.LogError("GetCurrentStoreAsync(): store → {Store}", store);

        _cachedStore = store ?? throw new Exception("No store could be loaded");
		_logger.LogError("GetCurrentStoreAsync(): _cachedStore → {_cachedStore}", _cachedStore);
		_logger.LogDebug("GetCurrentStoreAsync(): _cachedStore → {_cachedStore}", _cachedStore);

		_logger.LogWarning("<<< Exiting GetCurrentStoreAsync()");
        return _cachedStore;
    }

    /// <summary>
    /// Gets the current store
    /// </summary>
    public virtual Store GetCurrentStore()
    {
		_logger.LogError(">>> Entering GetCurrentStore()");
        if (_cachedStore != null)
            return _cachedStore;

        //try to determine the current store by HOST header
        string host = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Host];

        //we cannot call async methods here. otherwise, an application can hang. so it's a workaround to avoid that
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        var allStores = _storeRepository.GetAll(query =>
        {
            return from s in query orderby s.DisplayOrder, s.Id select s;
        }, _ => default, includeDeleted: false);
		_logger.LogError("GetCurrentStore(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        var store = allStores.FirstOrDefault(s => _storeService.ContainsHostValue(s, host)) ?? allStores.FirstOrDefault();
		_logger.LogError("GetCurrentStore(): store → {Store}", store);

        _cachedStore = store ?? throw new Exception("No store could be loaded");
		_logger.LogError("GetCurrentStore(): _cachedStore → {_cachedStore}", _cachedStore);
		_logger.LogDebug("GetCurrentStore(): _cachedStore → {_cachedStore}", _cachedStore);

		_logger.LogWarning("<<< Exiting GetCurrentStore()");
        return _cachedStore;
    }

    /// <summary>
    /// Gets active store scope configuration
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetActiveStoreScopeConfigurationAsync()
    {
		_logger.LogError(">>> Entering GetActiveStoreScopeConfigurationAsync()");
        if (_cachedActiveStoreScopeConfiguration.HasValue)
            return _cachedActiveStoreScopeConfiguration.Value;

        //ensure that we have 2 (or more) stores
        if ((await _storeService.GetAllStoresAsync()).Count > 1)
        {
            //do not inject IWorkContext via constructor because it'll cause circular references
            var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();
			_logger.LogError("GetActiveStoreScopeConfigurationAsync(): currentCustomer → {CurrentCustomer}", currentCustomer);

            //try to get store identifier from attributes
            var storeId = await _genericAttributeService
                .GetAttributeAsync<int>(currentCustomer, NopCustomerDefaults.AdminAreaStoreScopeConfigurationAttribute);

            _cachedActiveStoreScopeConfiguration = (await _storeService.GetStoreByIdAsync(storeId))?.Id ?? 0;
			_logger.LogError("GetActiveStoreScopeConfigurationAsync(): _cachedActiveStoreScopeConfiguration → {_cachedActiveStoreScopeConfiguration}", _cachedActiveStoreScopeConfiguration);
        }
        else
            _cachedActiveStoreScopeConfiguration = 0;
			_logger.LogDebug("GetActiveStoreScopeConfigurationAsync(): _cachedActiveStoreScopeConfiguration → {_cachedActiveStoreScopeConfiguration}", _cachedActiveStoreScopeConfiguration);

		_logger.LogWarning("<<< Exiting GetActiveStoreScopeConfigurationAsync()");
        return _cachedActiveStoreScopeConfiguration ?? 0;
    }

    #endregion
}