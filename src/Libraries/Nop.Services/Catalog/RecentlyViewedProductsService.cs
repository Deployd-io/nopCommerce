using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Core.Security;

namespace Nop.Services.Catalog;

/// <summary>
/// Recently viewed products service
/// </summary>
public partial class RecentlyViewedProductsService : IRecentlyViewedProductsService
{
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
        _catalogSettings = catalogSettings;
        _cookieSettings = cookieSettings;
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a list of identifier of recently viewed products
    /// </summary>
    /// <returns>List of identifier</returns>
    protected List<int> GetRecentlyViewedProductsIds()
    {
		logger.LogDebug(">>> Entering GetRecentlyViewedProductsIds()");
		logger.LogDebug("<<< Exiting GetRecentlyViewedProductsIds()");
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
		logger.LogDebug("<<< Exiting GetRecentlyViewedProductsIds(number={Number})", number);
        return productIds.Select(int.Parse).Distinct().Take(number).ToList();
    }

    /// <summary>
    /// Add cookie value for the recently viewed products
    /// </summary>
    /// <param name="recentlyViewedProductIds">Collection of the recently viewed products identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual Task AddRecentlyViewedProductsCookieAsync(IEnumerable<int> recentlyViewedProductIds)
    {
		logger.LogDebug(">>> Entering AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds})", recentlyViewedProductIds);
        //delete current cookie if exists
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.RecentlyViewedProductsCookie}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
		logger.LogDebug("AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds}): cookieName → {CookieName}", recentlyViewedProductIds, cookieName);

        //create cookie value
        var productIdsCookie = string.Join(",", recentlyViewedProductIds);

		logger.LogDebug("AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds}): productIdsCookie → {ProductIdsCookie}", recentlyViewedProductIds, productIdsCookie);
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

		logger.LogDebug("<<< Exiting AddRecentlyViewedProductsCookieAsync(recentlyViewedProductIds={RecentlyViewedProductIds})", recentlyViewedProductIds);
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
		logger.LogDebug(">>> Entering GetRecentlyViewedProductsAsync(number={Number})", number);
        //get list of recently viewed product identifiers
        var productIds = GetRecentlyViewedProductsIds(number);

		logger.LogDebug("GetRecentlyViewedProductsAsync(number={Number}): productIds → {ProductIds}", number, productIds);
        //return list of product
		logger.LogDebug("<<< Exiting GetRecentlyViewedProductsAsync(number={Number})", number);
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
		logger.LogDebug(">>> Entering AddProductToRecentlyViewedListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return;

        //whether recently viewed products is enabled
        if (!_catalogSettings.RecentlyViewedProductsEnabled)
            return;

        //get list of recently viewed product identifiers
        var productIds = GetRecentlyViewedProductsIds();

		logger.LogDebug("AddProductToRecentlyViewedListAsync(productId={ProductId}): productIds → {ProductIds}", productId, productIds);
        //whether product identifier to add already exist
        if (!productIds.Contains(productId))
            productIds.Insert(0, productId);

        //limit list based on the allowed number of the recently viewed products
        productIds = productIds.Take(_catalogSettings.RecentlyViewedProductsNumber).ToList();

        //set cookie
        await AddRecentlyViewedProductsCookieAsync(productIds);
		logger.LogDebug("<<< Exiting AddProductToRecentlyViewedListAsync(productId={ProductId})", productId);
    }

    #endregion
}
