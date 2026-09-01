using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Core.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Recently viewed products service
/// </summary>
public partial class RecentlyViewedProductsService : IRecentlyViewedProductsService
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly CookieSettings _cookieSettings;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IProductService _productService;
    protected readonly IWebHelper _webHelper;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public RecentlyViewedProductsService(CatalogSettings catalogSettings,
        CookieSettings cookieSettings,
        IHttpContextAccessor httpContextAccessor,
        IProductService productService,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper})", catalogSettings, httpContextAccessor, productService, webHelper);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _catalogSettings → {_catalogSettings}", catalogSettings, httpContextAccessor, productService, webHelper, _catalogSettings);
        _cookieSettings = cookieSettings;
		_logger.LogDebug("RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _cookieSettings → {_cookieSettings}", catalogSettings, httpContextAccessor, productService, webHelper, _cookieSettings);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _httpContextAccessor → {_httpContextAccessor}", catalogSettings, httpContextAccessor, productService, webHelper, _httpContextAccessor);
        _productService = productService;
		_logger.LogDebug("RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _productService → {_productService}", catalogSettings, httpContextAccessor, productService, webHelper, _productService);
        _webHelper = webHelper;
		_logger.LogDebug("RecentlyViewedProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _webHelper → {_webHelper}", catalogSettings, httpContextAccessor, productService, webHelper, _webHelper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a list of identifier of recently viewed products
    /// </summary>
    /// <returns>List of identifier</returns>
    protected List<int> GetRecentlyViewedProductsIds()
    {
		_logger.LogDebug(">>> Entering GetRecentlyViewedProductsIds()");
		_logger.LogDebug("<<< Exiting GetRecentlyViewedProductsIds()");
		_logger.LogDebug("<<< Exiting GetRecentlyViewedProductsIds(number={Number})", number);
        return GetRecentlyViewedProductsIds(int.MaxValue);
    }

    /// <summary>
    /// Gets a list of identifier of recently viewed products
    /// </summary>
    /// <param name="number">Number of products to load</param>
    /// <returns>List of identifier</returns>
    protected List<int> GetRecentlyViewedProductsIds(int number)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request == null)
            return new List<int>();

        //try to get cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";
        if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
            return new List<int>();

        //get array of string product identifiers from cookie
        var productIds = productIdsCookie.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

        //return list of int product identifiers
        return productIds.Select(int.Parse).Distinct().Take(number).ToList();
    }

    /// <summary>
    /// Add cookie value for the recently viewed products
    /// </summary>
    /// <param name="recentlyViewedProductIds">Collection of the recently viewed products identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task AddRecentlyViewedProductsCookieAsync(IEnumerable<int> recentlyViewedProductIds)
    {
		_logger.LogDebug(">>> Entering AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds})", recentlyViewedProductIds);
        //delete current cookie if exists
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";
		_logger.LogDebug("AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds}): cookieName → {CookieName}", recentlyViewedProductIds, cookieName);
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //create cookie value
        var productIdsCookie = string.Join(",", recentlyViewedProductIds);
		_logger.LogDebug("AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds}): productIdsCookie → {ProductIdsCookie}", recentlyViewedProductIds, productIdsCookie);

        //create cookie options 
        var cookieExpires = _cookieSettings.RecentlyViewedProductsCookieExpires;
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddHours(cookieExpires),
            HttpOnly = true,
            Secure = _webHelper.IsCurrentConnectionSecured()
        };

        //add cookie
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, productIdsCookie, cookieOptions);

		_logger.LogDebug("<<< Exiting AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds})", recentlyViewedProductIds);
        return Task.CompletedTask;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a "recently viewed products" list
    /// </summary>
    /// <param name="number">Number of products to load</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the "recently viewed products" list
    /// </returns>
    public virtual async Task<IList<Product>> GetRecentlyViewedProductsAsync(int number)
    {
		_logger.LogDebug(">>> Entering GetRecentlyViewedProductsAsync(number={Number})", number);
        //get list of recently viewed product identifiers
        var productIds = GetRecentlyViewedProductsIds(number);
		_logger.LogDebug("GetRecentlyViewedProductsAsync(number={Number}): productIds → {ProductIds}", number, productIds);

        //return list of product
		_logger.LogDebug("<<< Exiting GetRecentlyViewedProductsAsync(number={Number})", number);
        return (await _productService.GetProductsByIdsAsync(productIds.ToArray()))
            .Where(product => product.Published && !product.Deleted).ToList();
    }

    /// <summary>
    /// Adds a product to a recently viewed products list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task AddProductToRecentlyViewedListAsync(int productId)
    {
		_logger.LogDebug(">>> Entering AddProductToRecentlyViewedListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return;

        //whether recently viewed products is enabled
        if (!_catalogSettings.RecentlyViewedProductsEnabled)
            return;

        //get list of recently viewed product identifiers
        var productIds = GetRecentlyViewedProductsIds();
		_logger.LogDebug("AddProductToRecentlyViewedListAsync(productId={ProductId}): productIds → {ProductIds}", productId, productIds);

        //whether product identifier to add already exist
        if (!productIds.Contains(productId))
            productIds.Insert(0, productId);

        //limit list based on the allowed number of the recently viewed products
        productIds = productIds.Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();

        //set cookie
        await AddRecentlyViewedProductsCookieAsync(productIds);
		_logger.LogDebug("<<< Exiting AddProductToRecentlyViewedListAsync(productId={ProductId})", productId);
    }

    #endregion
}