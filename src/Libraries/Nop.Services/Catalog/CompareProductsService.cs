using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Core.Security;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Compare products service
/// </summary>
public partial class CompareProductsService : ICompareProductsService
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

    public CompareProductsService(CatalogSettings catalogSettings,
        CookieSettings cookieSettings,
        IHttpContextAccessor httpContextAccessor,
        IProductService productService,
        IWebHelper webHelper)
    {
		_logger.LogDebug(">>> Entering CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper})", catalogSettings, httpContextAccessor, productService, webHelper);
        _catalogSettings = catalogSettings;
		_logger.LogDebug("CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _catalogSettings → {_catalogSettings}", catalogSettings, httpContextAccessor, productService, webHelper, _catalogSettings);
        _cookieSettings = cookieSettings;
		_logger.LogDebug("CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _cookieSettings → {_cookieSettings}", catalogSettings, httpContextAccessor, productService, webHelper, _cookieSettings);
        _httpContextAccessor = httpContextAccessor;
		_logger.LogDebug("CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _httpContextAccessor → {_httpContextAccessor}", catalogSettings, httpContextAccessor, productService, webHelper, _httpContextAccessor);
        _productService = productService;
		_logger.LogDebug("CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _productService → {_productService}", catalogSettings, httpContextAccessor, productService, webHelper, _productService);
        _webHelper = webHelper;
		_logger.LogDebug("CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper}): _webHelper → {_webHelper}", catalogSettings, httpContextAccessor, productService, webHelper, _webHelper);
		_logger.LogDebug("<<< Exiting CompareProductsService(catalogSettings={CatalogSettings},cookieSettings=<redacted>,httpContextAccessor={HttpContextAccessor},productService={ProductService},webHelper={WebHelper})", catalogSettings, httpContextAccessor, productService, webHelper);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get a list of identifier of compared products
    /// </summary>
    /// <returns>List of identifier</returns>
    protected virtual List<int> GetComparedProductIds()
    {
		_logger.LogDebug(">>> Entering GetComparedProductIds()");
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request == null)
            return new List<int>();

        //try to get cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
		_logger.LogDebug("GetComparedProductIds(): cookieName → {CookieName}", cookieName);
        if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
            return new List<int>();

        //get array of string product identifiers from cookie
        var productIds = productIdsCookie.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
		_logger.LogDebug("GetComparedProductIds(): productIds → {ProductIds}", productIds);

        //return list of int product identifiers
		_logger.LogDebug("<<< Exiting GetComparedProductIds()");
        return productIds.Select(int.Parse).Distinct().ToList();
    }

    /// <summary>
    /// Add cookie value for the compared products
    /// </summary>
    /// <param name="comparedProductIds">Collection of compared products identifiers</param>
    protected virtual void AddCompareProductsCookie(IEnumerable<int> comparedProductIds)
    {
		_logger.LogDebug(">>> Entering AddCompareProductsCookie(comparedProductIds={ComparedProductIds})", comparedProductIds);
        //delete current cookie if exists
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
		_logger.LogDebug("AddCompareProductsCookie(comparedProductIds={ComparedProductIds}): cookieName → {CookieName}", comparedProductIds, cookieName);
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //create cookie value
        var comparedProductIdsCookie = string.Join(",", comparedProductIds);
		_logger.LogDebug("AddCompareProductsCookie(comparedProductIds={ComparedProductIds}): comparedProductIdsCookie → {ComparedProductIdsCookie}", comparedProductIds, comparedProductIdsCookie);

        //create cookie options 
        var cookieExpires = _cookieSettings.CompareProductsCookieExpires;
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddHours(cookieExpires),
            HttpOnly = true,
            Secure = _webHelper.IsCurrentConnectionSecured()
        };

        //add cookie
        _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, comparedProductIdsCookie, cookieOptions);
		_logger.LogDebug("<<< Exiting AddCompareProductsCookie(comparedProductIds={ComparedProductIds})", comparedProductIds);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clears a "compare products" list
    /// </summary>
    public virtual void ClearCompareProducts()
    {
		_logger.LogDebug(">>> Entering ClearCompareProducts()");
        if (_httpContextAccessor.HttpContext?.Response == null)
            return;

        //sets an expired cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
		_logger.LogDebug("ClearCompareProducts(): cookieName → {CookieName}", cookieName);
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
		_logger.LogDebug("<<< Exiting ClearCompareProducts()");
    }

    /// <summary>
    /// Gets a "compare products" list
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the "Compare products" list
    /// </returns>
    public virtual async Task<IList<Product>> GetComparedProductsAsync()
    {
		_logger.LogDebug(">>> Entering GetComparedProductsAsync()");
        //get list of compared product identifiers
        var productIds = GetComparedProductIds();
		_logger.LogDebug("GetComparedProductsAsync(): productIds → {ProductIds}", productIds);

        //return list of product
		_logger.LogDebug("<<< Exiting GetComparedProductsAsync()");
        return (await _productService.GetProductsByIdsAsync(productIds.ToArray()))
            .Where(product => product.Published && !product.Deleted).ToList();
    }

    /// <summary>
    /// Removes a product from a "compare products" list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task RemoveProductFromCompareListAsync(int productId)
    {
		_logger.LogDebug(">>> Entering RemoveProductFromCompareListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return Task.CompletedTask;

        //get list of compared product identifiers
        var comparedProductIds = GetComparedProductIds();
		_logger.LogDebug("RemoveProductFromCompareListAsync(productId={ProductId}): comparedProductIds → {ComparedProductIds}", productId, comparedProductIds);

        //whether product identifier to remove exists
        if (!comparedProductIds.Contains(productId))
            return Task.CompletedTask;

        //it exists, so remove it from list
        comparedProductIds.Remove(productId);

        //set cookie
        AddCompareProductsCookie(comparedProductIds);

		_logger.LogDebug("<<< Exiting RemoveProductFromCompareListAsync(productId={ProductId})", productId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a product to a "compare products" list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task AddProductToCompareListAsync(int productId)
    {
		_logger.LogDebug(">>> Entering AddProductToCompareListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return Task.CompletedTask;

        //get list of compared product identifiers
        var comparedProductIds = GetComparedProductIds();
		_logger.LogDebug("AddProductToCompareListAsync(productId={ProductId}): comparedProductIds → {ComparedProductIds}", productId, comparedProductIds);

        //whether product identifier to add already exist
        if (!comparedProductIds.Contains(productId))
            comparedProductIds.Insert(0, productId);

        //limit list based on the allowed number of products to be compared
        comparedProductIds = comparedProductIds.Take(_catalogSettings.CompareProductsNumber).ToList();

        //set cookie
        AddCompareProductsCookie(comparedProductIds);

		_logger.LogDebug("<<< Exiting AddProductToCompareListAsync(productId={ProductId})", productId);
        return Task.CompletedTask;
    }

    #endregion
}