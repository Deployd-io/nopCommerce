using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute formatter
/// </summary>
public partial class ProductAttributeFormatter : IProductAttributeFormatter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICurrencyService _currencyService;
    protected readonly IDownloadService _downloadService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IStoreContext _storeContext;
    protected readonly ShoppingCartSettings _shoppingCartSettings;

    #endregion

    #region Ctor

    public ProductAttributeFormatter(ICurrencyService currencyService,
        IDownloadService downloadService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext,
        IStoreContext storeContext,
        ShoppingCartSettings shoppingCartSettings)
    {
		_logger.LogDebug(">>> Entering ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>)", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext);
        _currencyService = currencyService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _currencyService → {_currencyService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _currencyService);
        _downloadService = downloadService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _downloadService → {_downloadService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _downloadService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _htmlFormatter → {_htmlFormatter}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _htmlFormatter);
        _localizationService = localizationService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _localizationService → {_localizationService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _localizationService);
        _priceCalculationService = priceCalculationService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _priceCalculationService → {_priceCalculationService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _priceCalculationService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _priceFormatter → {_priceFormatter}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _priceFormatter);
        _productAttributeParser = productAttributeParser;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _productAttributeParser → {_productAttributeParser}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _productAttributeParser);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _productAttributeService → {_productAttributeService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _productAttributeService);
        _taxService = taxService;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _taxService → {_taxService}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _taxService);
        _webHelper = webHelper;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _webHelper → {_webHelper}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _webHelper);
        _workContext = workContext;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _workContext → {_workContext}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _workContext);
        _storeContext = storeContext;
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _storeContext → {_storeContext}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _storeContext);
        _shoppingCartSettings = shoppingCartSettings;
		_logger.LogDebug("<<< Exiting ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>)", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext);
		_logger.LogDebug("ProductAttributeFormatter(currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceCalculationService={PriceCalculationService},priceFormatter={PriceFormatter},productAttributeParser={ProductAttributeParser},productAttributeService={ProductAttributeService},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext},storeContext={StoreContext},shoppingCartSettings=<redacted>): _shoppingCartSettings → {_shoppingCartSettings}", currencyService, downloadService, htmlFormatter, localizationService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, taxService, webHelper, workContext, storeContext, _shoppingCartSettings);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<string> FormatAttributesAsync(Product product, string attributesXml)
    {
		_logger.LogDebug(">>> Entering FormatAttributesAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        var customer = await _workContext.GetCurrentCustomerAsync();
        var currentStore = await _storeContext.GetCurrentStoreAsync();
		_logger.LogDebug("FormatAttributesAsync(product={Product},attributesXml={AttributesXml}): currentStore → {CurrentStore}", product, attributesXml, currentStore);
		_logger.LogDebug("FormatAttributesAsync(product={Product},attributesXml={AttributesXml}): customer → {Customer}", product, attributesXml, customer);

		_logger.LogDebug("<<< Exiting FormatAttributesAsync(product={Product},attributesXml={AttributesXml})", product, attributesXml);
        return await FormatAttributesAsync(product, attributesXml, customer, currentStore);
    }

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customer">Customer</param>
    /// <param name="store">Store</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    /// <param name="renderPrices">A value indicating whether to render prices</param>
    /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
    /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
    /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<string> FormatAttributesAsync(Product product, string attributesXml,
        Customer customer, Store store, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
        bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
        bool allowHyperlinks = true)
    {
        var result = new StringBuilder();
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        //attributes
        if (renderProductAttributes)
        {
            foreach (var attribute in await _productAttributeParser.ParseProductAttributeMappingsAsync(attributesXml))
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId);
                var attributeName = await _localizationService.GetLocalizedAsync(productAttribute, a => a.Name, currentLanguage.Id);

                //attributes without values
                if (!attribute.ShouldHaveValues())
                {
                    foreach (var value in _productAttributeParser.ParseValues(attributesXml, attribute.Id))
                    {
                        var formattedAttribute = string.Empty;
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);

                            //we never encode multiline textbox input
                            formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(value, false, true, false, false, false, false)}";
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            _ = Guid.TryParse(value, out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";

                                //encode (if required)
                                if (htmlEncode)
                                    fileName = WebUtility.HtmlEncode(fileName);

                                var attributeText = allowHyperlinks ? $"<a href=\"{_webHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}\" class=\"fileuploadattribute\">{fileName}</a>"
                                    : fileName;

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                formattedAttribute = $"{attributeName}: {attributeText}";
                            }
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = $"{attributeName}: {value}";

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }

                        if (string.IsNullOrEmpty(formattedAttribute))
                            continue;

                        if (result.Length > 0)
                            result.Append(separator);
                        result.Append(formattedAttribute);
                    }
                }
                //product attribute values
                else
                {
                    foreach (var attributeValue in await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml, attribute.Id))
                    {
                        var formattedAttribute = $"{attributeName}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";

                        if (renderPrices)
                        {
                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                if (attributeValue.PriceAdjustment > decimal.Zero)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        "+", attributeValue.PriceAdjustment.ToString("G29"), "%");
                                }
                                else if (attributeValue.PriceAdjustment < decimal.Zero)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        string.Empty, attributeValue.PriceAdjustment.ToString("G29"), "%");
                                }
                            }
                            else
                            {
                                var attributeValuePriceAdjustment = await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store);
                                var (priceAdjustmentBase, _) = await _taxService.GetProductPriceAsync(product, attributeValuePriceAdjustment, customer);
                                var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());

                                if (priceAdjustmentBase > decimal.Zero)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        "+", await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false), string.Empty);
                                }
                                else if (priceAdjustmentBase < decimal.Zero)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        "-", await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false), string.Empty);
                                }
                            }
                        }

                        //display quantity
                        if (_shoppingCartSettings.RenderAssociatedAttributeValueQuantity && attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                        {
                            //render only when more than 1
                            if (attributeValue.Quantity > 1)
                                formattedAttribute += string.Format(await _localizationService.GetResourceAsync("ProductAttributes.Quantity"), attributeValue.Quantity);
                        }

                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

                        if (string.IsNullOrEmpty(formattedAttribute))
                            continue;

                        if (result.Length > 0)
                            result.Append(separator);
                        result.Append(formattedAttribute);
                    }
                }
            }
        }

        //gift cards
        if (!renderGiftCardAttributes)
            return result.ToString();

        if (!product.IsGiftCard)
            return result.ToString();

        _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

        //sender
        var giftCardFrom = product.GiftCardType == GiftCardType.Virtual ?
            string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.From.Virtual"), giftCardSenderName, giftCardSenderEmail) :
            string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.From.Physical"), giftCardSenderName);
        //recipient
        var giftCardFor = product.GiftCardType == GiftCardType.Virtual ?
            string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.For.Virtual"), giftCardRecipientName, giftCardRecipientEmail) :
            string.Format(await _localizationService.GetResourceAsync("GiftCardAttribute.For.Physical"), giftCardRecipientName);

        //encode (if required)
        if (htmlEncode)
        {
            giftCardFrom = WebUtility.HtmlEncode(giftCardFrom);
            giftCardFor = WebUtility.HtmlEncode(giftCardFor);
        }

        if (!string.IsNullOrEmpty(result.ToString()))
        {
            result.Append(separator);
        }

        result.Append(giftCardFrom);
        result.Append(separator);
        result.Append(giftCardFor);

        return result.ToString();
    }

    #endregion
}