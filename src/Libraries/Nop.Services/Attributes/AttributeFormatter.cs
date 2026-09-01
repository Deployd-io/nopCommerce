using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Services.Html;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Attributes;

/// <summary>
/// Attribute formatter
/// </summary>
public partial class
    AttributeFormatter<TAttribute, TAttributeValue> : IAttributeFormatter<TAttribute, TAttributeValue>
    where TAttribute : BaseAttribute
    where TAttributeValue : BaseAttributeValue
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeParser<TAttribute, TAttributeValue> _attributeParser;
    protected readonly IAttributeService<TAttribute, TAttributeValue> _attributeService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AttributeFormatter(IAttributeParser<TAttribute, TAttributeValue> attributeParser,
        IAttributeService<TAttribute, TAttributeValue> attributeService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IWorkContext workContext)
    {
		_logger.LogDebug(">>> Entering AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext})", attributeParser, attributeService, htmlFormatter, localizationService, workContext);
        _attributeParser = attributeParser;
		_logger.LogDebug("AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext}): _attributeParser → {_attributeParser}", attributeParser, attributeService, htmlFormatter, localizationService, workContext, _attributeParser);
        _attributeService = attributeService;
		_logger.LogDebug("AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext}): _attributeService → {_attributeService}", attributeParser, attributeService, htmlFormatter, localizationService, workContext, _attributeService);
        _htmlFormatter = htmlFormatter;
		_logger.LogDebug("AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext}): _htmlFormatter → {_htmlFormatter}", attributeParser, attributeService, htmlFormatter, localizationService, workContext, _htmlFormatter);
        _localizationService = localizationService;
		_logger.LogDebug("AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext}): _localizationService → {_localizationService}", attributeParser, attributeService, htmlFormatter, localizationService, workContext, _localizationService);
        _workContext = workContext;
		_logger.LogDebug("AttributeFormatter(attributeParser={AttributeParser},attributeService={AttributeService},htmlFormatter={HtmlFormatter},localizationService={LocalizationService},workContext={WorkContext}): _workContext → {_workContext}", attributeParser, attributeService, htmlFormatter, localizationService, workContext, _workContext);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes
    /// </returns>
    public virtual async Task<string> FormatAttributesAsync(string attributesXml,
        string separator = "<br />",
        bool htmlEncode = true)
    {
		_logger.LogDebug(">>> Entering FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode})", attributesXml, separator, htmlEncode);
        var result = new StringBuilder();
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
		_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): currentLanguage → {CurrentLanguage}", attributesXml, separator, htmlEncode, currentLanguage);
        var attributes = await _attributeParser.ParseAttributesAsync(attributesXml);
		_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): attributes → {Attributes}", attributesXml, separator, htmlEncode, attributes);
		_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): result → {Result}", attributesXml, separator, htmlEncode, result);

        for (var i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            var valuesStr = _attributeParser.ParseValues(attributesXml, attribute.Id);
			_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): valuesStr → {ValuesStr}", attributesXml, separator, htmlEncode, valuesStr);

            for (var j = 0; j < valuesStr.Count; j++)
            {
                var valueStr = valuesStr[j];
                var formattedAttribute = string.Empty;
				_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): formattedAttribute → {FormattedAttribute}", attributesXml, separator, htmlEncode, formattedAttribute);

                if (!attribute.ShouldHaveValues)
                {
                    //no values
                    if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        //multiline textbox
                        var attributeName =
                            await _localizationService.GetLocalizedAsync(attribute, a => a.Name,
                                currentLanguage.Id);
							_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): attributeName → {AttributeName}", attributesXml, separator, htmlEncode, attributeName);

                        //encode (if required)
                        if (htmlEncode)
                            attributeName = WebUtility.HtmlEncode(attributeName);

                        formattedAttribute =
                            $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                        //we never encode multiline textbox input
                    }
                    else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                    {
                        //file upload
                        //not supported for this attributes
                    }
                    else
                    {
                        //other attributes (textbox, datepicker)
                        formattedAttribute =
                            $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";

                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }
                else
                {
                    if (int.TryParse(valueStr, out var attributeValueId))
                    {
                        var attributeValue = await _attributeService.GetAttributeValueByIdAsync(attributeValueId);
						_logger.LogDebug("FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode}): attributeValue → {AttributeValue}", attributesXml, separator, htmlEncode, attributeValue);

                        if (attributeValue != null)
                            formattedAttribute =
                                $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";

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

		_logger.LogDebug("<<< Exiting FormatAttributesAsync(attributesXml={AttributesXml},separator={Separator},htmlEncode={HtmlEncode})", attributesXml, separator, htmlEncode);
        return result.ToString();
    }

    #endregion
}