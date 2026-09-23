using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Orders;

/// <summary>
/// Checkout attribute helper
/// </summary>
public partial class CheckoutAttributeFormatter : ICheckoutAttributeFormatter
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDownloadService _downloadService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CheckoutAttributeFormatter(IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICurrencyService currencyService,
        IDownloadService downloadService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IPriceFormatter priceFormatter,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext})", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext);
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _checkoutAttributeService → {_checkoutAttributeService}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _checkoutAttributeService);
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _checkoutAttributeParser → {_checkoutAttributeParser}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _checkoutAttributeParser);
    _logger.LogWarning("<<< Exiting CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext})", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext);
        _currencyService = currencyService;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _currencyService → {_currencyService}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _currencyService);
        _downloadService = downloadService;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _downloadService → {_downloadService}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _downloadService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _htmlFormatter → {_htmlFormatter}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _htmlFormatter);
        _localizationService = localizationService;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _localizationService → {_localizationService}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _localizationService);
        _priceFormatter = priceFormatter;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _priceFormatter → {_priceFormatter}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _priceFormatter);
        _taxService = taxService;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _taxService → {_taxService}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _taxService);
        _webHelper = webHelper;
		_logger.LogDebug("CheckoutAttributeFormatter(checkoutAttributeParser={CheckoutAttributeParser},checkoutAttributeService={CheckoutAttributeService},currencyService={CurrencyService},downloadService={DownloadService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},priceFormatter={PriceFormatter},taxService={TaxService},webHelper={WebHelper},workContext={WorkContext}): _webHelper → {_webHelper}", checkoutAttributeParser, checkoutAttributeService, currencyService, downloadService, htmlFormatter, localizationService, priceFormatter, taxService, webHelper, workContext, _webHelper);
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customer">Customer</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    /// <param name="renderPrices">A value indicating whether to render prices</param>
    /// <param name="allowHyperlinks">A value indicating whether to HTML hyperlink tags could be rendered (if required)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<string> FormatAttributesAsync(string attributesXml,
        Customer customer,
        string separator = "<br />",
        bool htmlEncode = true,
        bool renderPrices = true,
        bool allowHyperlinks = true)
    {
		_logger.LogError(">>> Entering FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks})", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks);
        var result = new StringBuilder();
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): currentLanguage → {CurrentLanguage}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, currentLanguage);
		_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): result → {Result}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, result);
        var attributes = await _checkoutAttributeParser.ParseAttributesAsync(attributesXml);
        for (var i = 0; i < attributes.Count; i++)
        {
			_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): attributes → {Attributes}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, attributes);
            var attribute = attributes[i];
            var valuesStr = _checkoutAttributeParser.ParseValues(attributesXml, attribute.Id);
			_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): valuesStr → {ValuesStr}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, valuesStr);
            for (var j = 0; j < valuesStr.Count; j++)
            {
                var valueStr = valuesStr[j];
                var formattedAttribute = string.Empty;
				_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): formattedAttribute → {FormattedAttribute}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, formattedAttribute);
                if (!attribute.ShouldHaveValues)
                {
                    //no values
                    if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        //multiline textbox
                        var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
						_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): attributeName → {AttributeName}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, attributeName);
                        //encode (if required)
                        if (htmlEncode)
                            attributeName = WebUtility.HtmlEncode(attributeName);
                        formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                        //we never encode multiline textbox input
                    }
                    else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                    {
                        //file upload
                        _ = Guid.TryParse(valueStr, out var downloadGuid);
						_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): _ → {_}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, _);
                        var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
						_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): download → {Download}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, download);
                        if (download != null)
                        {
                            string attributeText;
                            var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";
							_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): fileName → {FileName}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, fileName);
                            //encode (if required)
                            if (htmlEncode)
                                fileName = WebUtility.HtmlEncode(fileName);
                            if (allowHyperlinks)
                            {
                                //hyperlinks are allowed
                                var downloadLink = $"{_webHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}";
								_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): downloadLink → {DownloadLink}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, downloadLink);
                                attributeText = $"<a href=\"{downloadLink}\" class=\"fileuploadattribute\">{fileName}</a>";
								_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): attributeText → {AttributeText}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, attributeText);
                            }
                            else
                            {
                                //hyperlinks aren't allowed
								_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): attributeText → {AttributeText}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, attributeText);
                                attributeText = fileName;
                            }

                            var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {attributeText}";
                        }
                    }
                    else
                    {
                        //other attributes (textbox, datepicker)
                        formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }
                else
                {
                    if (int.TryParse(valueStr, out var attributeValueId))
                    {
                        var attributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(attributeValueId);
						_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): attributeValue → {AttributeValue}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, attributeValue);

                        if (attributeValue != null)
                        {
                            formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                            if (renderPrices)
                            {
                                var priceAdjustmentBase = (await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, customer)).price;
								_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): priceAdjustmentBase → {PriceAdjustmentBase}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, priceAdjustmentBase);
                                var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());
								_logger.LogError("FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks}): priceAdjustment → {PriceAdjustment}", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks, priceAdjustment);
                                if (priceAdjustmentBase > 0)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        "+", await _priceFormatter.FormatPriceAsync(priceAdjustment), string.Empty);
                                }
                            }
                        }

                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }

                if (string.IsNullOrEmpty(formattedAttribute))
                    continue;

                if (i != 0 || j != 0)
                    result.Append(separator);
                result.Append(formattedAttribute);
            }
        }

		_logger.LogWarning("<<< Exiting FormatAttributesAsync(attributesXml={AttributesXml},customer={Customer},separator={Separator},htmlEncode={HtmlEncode},renderPrices={RenderPrices},allowHyperlinks={AllowHyperlinks})", attributesXml, customer, separator, htmlEncode, renderPrices, allowHyperlinks);
        return result.ToString();
    }

    #endregion
}