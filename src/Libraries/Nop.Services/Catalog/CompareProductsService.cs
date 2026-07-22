using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Http;
using Nop.Core.Security;

namespace Nop.Services.Catalog;

/// <summary>
/// Compare products service
/// </summary>
public partial class CompareProductsService : ICompareProductsService
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

    public CompareProductsService(CatalogSettings catalogSettings,
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
    /// Get a list of identifier of compared products
    /// </summary>
    /// <returns>List of identifier</returns>
    protected virtual List<int> GetComparedProductIds()
    {
		logger.LogDebug(">>> Entering GetComparedProductIds()");
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request == null)
            return new List<int>();

        //try to get cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
        if (!httpContext.Request.Cookies.TryGetValue(cookieName, out var productIdsCookie) || string.IsNullOrEmpty(productIdsCookie))
			logger.LogDebug("GetComparedProductIds(): cookieName → {CookieName}", cookieName);
            return new List<int>();

        //get array of string product identifiers from cookie
        var productIds = productIdsCookie.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

		logger.LogDebug("GetComparedProductIds(): productIds → {ProductIds}", productIds);
        //return list of int product identifiers
		logger.LogDebug("<<< Exiting GetComparedProductIds()");
        return productIds.Select(int.Parse).Distinct().ToList();
    }

    /// <summary>
    /// Add cookie value for the compared products
    /// </summary>
    /// <param name="comparedProductIds">Collection of compared products identifiers</param>
    protected virtual void AddCompareProductsCookie(IEnumerable<int> comparedProductIds)
    {
        //delete current cookie if exists
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //create cookie value
        var comparedProductIdsCookie = string.Join(",", comparedProductIds);

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
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clears a "compare products" list
    /// </summary>
    public virtual void ClearCompareProducts()
    {
		logger.LogDebug(">>> Entering ClearCompareProducts()");
        if (_httpContextAccessor.HttpContext?.Response == null)
            return;

        //sets an expired cookie
        var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.ComparedProductsCookie}";
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
		logger.LogDebug("ClearCompareProducts(): cookieName → {CookieName}", cookieName);
		logger.LogDebug("<<< Exiting ClearCompareProducts()");
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
		logger.LogDebug(">>> Entering GetComparedProductsAsync()");
        //get list of compared product identifiers
        var productIds = GetComparedProductIds();

		logger.LogDebug("GetComparedProductsAsync(): productIds → {ProductIds}", productIds);
        //return list of product
		logger.LogDebug("<<< Exiting GetComparedProductsAsync()");
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
		logger.LogDebug(">>> Entering RemoveProductFromCompareListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return Task.CompletedTask;

        //get list of compared product identifiers
        var comparedProductIds = GetComparedProductIds();

		logger.LogDebug("RemoveProductFromCompareListAsync(productId={ProductId}): comparedProductIds → {ComparedProductIds}", productId, comparedProductIds);
        //whether product identifier to remove exists
        if (!comparedProductIds.Contains(productId))
            return Task.CompletedTask;

        //it exists, so remove it from list
        comparedProductIds.Remove(productId);

        //set cookie
        AddCompareProductsCookie(comparedProductIds);

		logger.LogDebug("<<< Exiting RemoveProductFromCompareListAsync(productId={ProductId})", productId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds a product to a "compare products" list
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual Task AddProductToCompareListAsync(int productId)
    {
		logger.LogDebug(">>> Entering AddProductToCompareListAsync(productId={ProductId})", productId);
        if (_httpContextAccessor.HttpContext?.Response == null)
            return Task.CompletedTask;

        //get list of compared product identifiers
        var comparedProductIds = GetComparedProductIds();

		logger.LogDebug("AddProductToCompareListAsync(productId={ProductId}): comparedProductIds → {ComparedProductIds}", productId, comparedProductIds);
        //whether product identifier to add already exist
        if (!comparedProductIds.Contains(productId))
            comparedProductIds.Insert(0, productId);

        //limit list based on the allowed number of products to be compared
        comparedProductIds = comparedProductIds.Take(_catalogSettings.CompareProductsNumber).ToList();

        //set cookie
        AddCompareProductsCookie(comparedProductIds);

		logger.LogDebug("<<< Exiting AddProductToCompareListAsync(productId={ProductId})", productId);
        return Task.CompletedTask;
    }

    #endregion
}
