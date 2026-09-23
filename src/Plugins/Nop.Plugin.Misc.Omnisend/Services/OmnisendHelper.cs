using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Misc.Omnisend.DTO;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Nop.Plugin.Misc.Omnisend.Services;

/// <summary>
/// Represents the plugins helpers class
/// </summary>
public class OmnisendHelper
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private string _primaryStoreCurrencyCode;

    private readonly CurrencySettings _currencySettings;
    private readonly ICurrencyService _currencyService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IPictureService _pictureService;
    private readonly IProductAttributeParser _productAttributeParser;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public OmnisendHelper(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IWebHelper webHelper)
    {
		_logger.LogError(">>> Entering OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper})", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper);
        _currencySettings = currencySettings;
        _currencyService = currencyService;
		_logger.LogDebug("OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper}): _currencyService → {_currencyService}", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper, _currencyService);
		_logger.LogDebug("OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper}): _currencySettings → {_currencySettings}", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper, _currencySettings);
    _logger.LogWarning("<<< Exiting OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper})", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper);
        _nopUrlHelper = nopUrlHelper;
		_logger.LogDebug("OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper}): _nopUrlHelper → {_nopUrlHelper}", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper, _nopUrlHelper);
        _pictureService = pictureService;
		_logger.LogDebug("OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper}): _pictureService → {_pictureService}", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper, _pictureService);
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("OmnisendHelper(currencySettings={CurrencySettings},currencyService={CurrencyService},nopUrlHelper={NopUrlHelper},pictureService={PictureService},productAttributeParser={ProductAttributeParser},webHelper={WebHelper}): _productAttributeParser → {_productAttributeParser}", currencySettings, currencyService, nopUrlHelper, pictureService, productAttributeParser, webHelper, _productAttributeParser);
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the primary store currency code
    /// </summary>
    public async Task<string> GetPrimaryStoreCurrencyCodeAsync()
    {
		_logger.LogError(">>> Entering GetPrimaryStoreCurrencyCodeAsync()");
        if (!string.IsNullOrEmpty(_primaryStoreCurrencyCode))
            return _primaryStoreCurrencyCode;

        var currency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
		_logger.LogError("GetPrimaryStoreCurrencyCodeAsync(): currency → {Currency}", currency);

        _primaryStoreCurrencyCode = currency.CurrencyCode;
		_logger.LogDebug("GetPrimaryStoreCurrencyCodeAsync(): _primaryStoreCurrencyCode → {_primaryStoreCurrencyCode}", _primaryStoreCurrencyCode);

		_logger.LogWarning("<<< Exiting GetPrimaryStoreCurrencyCodeAsync()");
        return _primaryStoreCurrencyCode;
    }

    /// <summary>
    /// Gets the product or product attribute combination SKU and ID
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Product attributes on XML format</param>
    public async Task<(string sku, int variantId)> GetSkuAndVariantIdAsync(Product product, string attributesXml)
    {
        var sku = product.Sku;
        var variantId = product.Id;

        var combination =
            await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);

        if (combination == null)
            return (sku, variantId);

        sku = combination.Sku;
        variantId = combination.Id;

        return (sku, variantId);
    }

    /// <summary>
    /// Gets the product URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<string> GetProductUrlAsync(Product product)
    {
		_logger.LogError(">>> Entering GetProductUrlAsync(product={Product})", product);
        _logger.LogWarning("<<< Exiting GetProductUrlAsync(product={Product})", product);
        return await _nopUrlHelper.RouteGenericUrlAsync(product, _webHelper.GetCurrentRequestProtocol());
    }

    /// <summary>
    /// Gets the product picture URL
    /// </summary>
    /// <param name="product">Product</param>
    public async Task<ProductDto.Image> GetProductPictureUrlAsync(Product product)
    {
		_logger.LogError(">>> Entering GetProductPictureUrlAsync(product={Product})", product);
        var picture = (await _pictureService
            .GetPicturesByProductIdAsync(product.Id, 1)).DefaultIfEmpty(null).FirstOrDefault();

        var (url, _) = await _pictureService.GetPictureUrlAsync(picture);

        var storeLocation = _webHelper.GetStoreLocation();
		_logger.LogError("GetProductPictureUrlAsync(product={Product}): storeLocation → {StoreLocation}", product, storeLocation);

        if (!url.StartsWith(storeLocation))
            url = storeLocation + url;
			_logger.LogInformation("GetProductPictureUrlAsync(product={Product}): url → {Url}", product, url);

		_logger.LogWarning("<<< Exiting GetProductPictureUrlAsync(product={Product})", product);
        return new ProductDto.Image { ImageId = (picture?.Id ?? 0).ToString(), Url = url };
    }

    #endregion
}