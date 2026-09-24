using System.Globalization;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Catalog;

/// <summary>
/// Product attribute parser
/// </summary>
public partial class ProductAttributeParser : IProductAttributeParser
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly ICurrencyService _currencyService;
    protected readonly IDownloadService _downloadService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
    protected readonly IWorkContext _workContext;

    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public ProductAttributeParser(ICurrencyService currencyService,
        IDownloadService downloadService,
        ILocalizationService localizationService,
        IProductAttributeService productAttributeService,
        IRepository<ProductAttributeValue> productAttributeValueRepository,
        IWorkContext workContext)
    {
		_logger.LogError(">>> Entering ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext})", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext);
        _currencyService = currencyService;
        _downloadService = downloadService;
		_logger.LogDebug("ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext}): _downloadService → {_downloadService}", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext, _downloadService);
        _productAttributeService = productAttributeService;
		_logger.LogDebug("ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext}): _productAttributeService → {_productAttributeService}", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext, _productAttributeService);
        _productAttributeValueRepository = productAttributeValueRepository;
		_logger.LogDebug("ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext}): _productAttributeValueRepository → {_productAttributeValueRepository}", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext, _productAttributeValueRepository);
        _workContext = workContext;
		_logger.LogDebug("ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext}): _workContext → {_workContext}", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext, _workContext);
        _localizationService = localizationService;
		_logger.LogDebug("ProductAttributeParser(currencyService={CurrencyService},downloadService={DownloadService},localizationService={LocalizationService},productAttributeService={ProductAttributeService},productAttributeValueRepository={ProductAttributeValueRepository},workContext={WorkContext}): _localizationService → {_localizationService}", currencyService, downloadService, localizationService, productAttributeService, productAttributeValueRepository, workContext, _localizationService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Returns a list which contains all possible combinations of elements
    /// </summary>
    /// <typeparam name="T">Type of element</typeparam>
    /// <param name="elements">Elements to make combinations</param>
    /// <returns>All possible combinations of elements</returns>
    protected virtual IList<IList<T>> CreateCombination<T>(IList<T> elements)
    {
        var rez = new List<IList<T>>();

        for (var i = 1; i < Math.Pow(2, elements.Count); i++)
        {
            var current = new List<T>();
            var index = -1;

            //transform int to binary string
            var binaryMask = Convert.ToString(i, 2).PadLeft(elements.Count, '0');

            foreach (var flag in binaryMask)
            {
                index++;

                if (flag == '0')
                    continue;

                //add element if binary mask in the position of element has 1
                current.Add(elements[index]);
            }

            rez.Add(current);
        }

        return rez;
    }

    /// <summary>
    /// Gets selected product attribute values with the quantity entered by the customer
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
    /// <returns>Collections of pairs of product attribute values and their quantity</returns>
    protected virtual IList<Tuple<string, string>> ParseValuesWithQuantity(string attributesXml, int productAttributeMappingId)
    {
        var selectedValues = new List<Tuple<string, string>>();
        if (string.IsNullOrEmpty(attributesXml))
            return selectedValues;

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(attributesXml);

            foreach (XmlNode attributeNode in xmlDoc.SelectNodes(@"//Attributes/ProductAttribute"))
            {
                if (attributeNode.Attributes?["ID"] == null)
                    continue;

                if (!int.TryParse(attributeNode.Attributes["ID"].InnerText.Trim(), out var attributeId) ||
                    attributeId != productAttributeMappingId)
                    continue;

                foreach (XmlNode attributeValue in attributeNode.SelectNodes("ProductAttributeValue"))
                {
                    var value = attributeValue.SelectSingleNode("Value").InnerText.Trim();
                    var quantityNode = attributeValue.SelectSingleNode("Quantity");
                    selectedValues.Add(new Tuple<string, string>(value, quantityNode != null ? quantityNode.InnerText.Trim() : string.Empty));
                }
            }
        }
        catch
        {
            // ignored
        }

        return selectedValues;
    }

    /// <summary>
    /// Adds gift cards attributes in XML format
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    protected virtual void AddGiftCardsAttributesXml(Product product, IFormCollection form, ref string attributesXml)
    {
		_logger.LogError(">>> Entering AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml})", product, form, attributesXml);
        if (!product.IsGiftCard)
            return;

        var recipientName = "";
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): recipientName → {RecipientName}", product, form, attributesXml, recipientName);
        var recipientEmail = "";
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): recipientEmail → {RecipientEmail}", product, form, attributesXml, recipientEmail);
        var senderName = "";
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): senderName → {SenderName}", product, form, attributesXml, senderName);
        var senderEmail = "";
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): senderEmail → {SenderEmail}", product, form, attributesXml, senderEmail);
        var giftCardMessage = "";
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): giftCardMessage → {GiftCardMessage}", product, form, attributesXml, giftCardMessage);
        foreach (var formKey in form.Keys)
        {
            if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
            {
                recipientName = form[formKey];
                continue;
            }
            if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
            {
                recipientEmail = form[formKey];
                continue;
            }
            if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
            {
                senderName = form[formKey];
                continue;
            }
            if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
            {
                senderEmail = form[formKey];
                continue;
            }
            if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
            {
                giftCardMessage = form[formKey];
            }
        }

        attributesXml = AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
		_logger.LogError("AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml}): attributesXml → {AttributesXml1}", product, form, attributesXml, attributesXml);
		_logger.LogWarning("<<< Exiting AddGiftCardsAttributesXml(product={Product},form={Form},attributesXml={AttributesXml})", product, form, attributesXml);
    }

    /// <summary>
    /// Gets product attributes in XML format
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    protected virtual async Task<string> GetProductAttributesXmlAsync(Product product, IFormCollection form, List<string> errors)
    {
		_logger.LogError(">>> Entering GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors})", product, form, errors);
        var attributesXml = string.Empty;
        var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): attributesXml → {AttributesXml}", product, form, errors, attributesXml);
        foreach (var attribute in productAttributes)
        {
			_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): productAttributes → {ProductAttributes}", product, form, errors, productAttributes);
            var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
			_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): controlId → {ControlId}", product, form, errors, controlId);
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var selectedAttributeId = int.Parse(ctrlAttributes);
						_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): selectedAttributeId → {SelectedAttributeId}", product, form, errors, selectedAttributeId);
                        if (selectedAttributeId > 0)
                        {
                            //get quantity entered by customer
                            var quantity = 1;
                            var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
							_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): quantityStr → {QuantityStr}", product, form, errors, quantityStr);
                            if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                errors.Add(await _localizationService.GetResourceAsync("Products.QuantityShouldBePositive"));

                            attributesXml = AddProductAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                        }
                    }
                }
                    break;
                case AttributeControlType.Checkboxes:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        foreach (var item in ctrlAttributes.ToString()
                                     .Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var selectedAttributeId = int.Parse(item);
                            if (selectedAttributeId > 0)
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{item}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(await _localizationService.GetResourceAsync("Products.QuantityShouldBePositive"));

                                attributesXml = AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //load read-only (already server-side selected) values
                    var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): attributeValues → {AttributeValues}", product, form, errors, attributeValues);
                    foreach (var selectedAttributeId in attributeValues
                                 .Where(v => v.IsPreSelected)
                                 .Select(v => v.Id)
                                 .ToList())
                    {
                        //get quantity entered by customer
                        var quantity = 1;
                        var quantityStr = form[$"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}_{selectedAttributeId}_qty"];
                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                            errors.Add(await _localizationService.GetResourceAsync("Products.QuantityShouldBePositive"));

                        attributesXml = AddProductAttribute(attributesXml,
                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                    }
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    var ctrlAttributes = form[controlId];
                    if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                    {
                        var enteredText = ctrlAttributes.ToString().Trim();
						_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): enteredText → {EnteredText}", product, form, errors, enteredText);
                        attributesXml = AddProductAttribute(attributesXml, attribute, enteredText);
                    }
                }
                    break;
                case AttributeControlType.Datepicker:
                {
                    var day = form[controlId + "_day"];
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): day → {Day}", product, form, errors, day);
                    var month = form[controlId + "_month"];
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): month → {Month}", product, form, errors, month);
                    var year = form[controlId + "_year"];
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): year → {Year}", product, form, errors, year);
                    DateTime? selectedDate = null;
                    try
                    {
                        selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
						_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): selectedDate → {SelectedDate}", product, form, errors, selectedDate);
                    }
                    catch
                    {
						_logger.LogError("Exception in GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): {Arg3}", product, form, errors, ex.Message);
						_logger.LogDebug("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): selectedDate → {SelectedDate}", product, form, errors, selectedDate);
                        // ignored
                    }

                    if (selectedDate.HasValue)
                        attributesXml = AddProductAttribute(attributesXml, attribute, selectedDate.Value.ToString("D"));
                }
                    break;
                case AttributeControlType.FileUpload:
                {
                    _ = Guid.TryParse(form[controlId], out var downloadGuid);
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): _ → {_}", product, form, errors, _);
                    var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
					_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): download → {Download}", product, form, errors, download);
                    if (download != null)
                        attributesXml = AddProductAttribute(attributesXml,
                            attribute, download.DownloadGuid.ToString());
                }
                    break;
                default:
                    break;
            }
        }
        //validate conditional attributes (if specified)
        foreach (var attribute in productAttributes)
        {
            var conditionMet = await IsConditionMetAsync(attribute, attributesXml);
			_logger.LogError("GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors}): conditionMet → {ConditionMet}", product, form, errors, conditionMet);
            if (conditionMet.HasValue && !conditionMet.Value)
            {
                attributesXml = RemoveProductAttribute(attributesXml, attribute);
            }
        }
		_logger.LogWarning("<<< Exiting GetProductAttributesXmlAsync(product={Product},form={Form},errors={Errors})", product, form, errors);
        return attributesXml;
    }

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeValueId">Attribute value id</param>
    /// <returns>Updated result (XML format)</returns>
    protected virtual string RemoveAttribute(string attributesXml, int attributeValueId)
    {
		_logger.LogError(">>> Entering RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId})", attributesXml, attributeValueId);
        var result = string.Empty;

		_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): result → {Result}", attributesXml, attributeValueId, result);
        if (string.IsNullOrEmpty(attributesXml))
            return string.Empty;

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): xmlDoc → {XmlDoc}", attributesXml, attributeValueId, xmlDoc);

            xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): rootElement → {RootElement}", attributesXml, attributeValueId, rootElement);

            if (rootElement == null)
                return string.Empty;

            XmlElement attributeElement = null;
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): attributeElement → {AttributeElement}", attributesXml, attributeValueId, attributeElement);
            //find existing
            var childNodes = xmlDoc.SelectNodes($@"//Attributes/{ChildElementName}");
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): childNodes → {ChildNodes}", attributesXml, attributeValueId, childNodes);

            if (childNodes == null)
                return string.Empty;

            var count = childNodes.Count;
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): count → {Count}", attributesXml, attributeValueId, count);

            foreach (XmlElement childNode in childNodes)
            {
                if (!int.TryParse(childNode.Attributes["ID"]?.InnerText.Trim(), out var id))
                    continue;

                if (id != attributeValueId)
                    continue;

                attributeElement = childNode;
                break;
            }

            //found
            if (attributeElement != null)
            {
                rootElement.RemoveChild(attributeElement);
                count -= 1;
            }

            result = count == 0 ? string.Empty : xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId}): {Arg2}", attributesXml, attributeValueId, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting RemoveAttribute(attributesXml={AttributesXml},attributeValueId={AttributeValueId})", attributesXml, attributeValueId);
        return result;
    }

    /// <summary>
    /// Gets selected attribute identifiers
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>Selected attribute identifiers</returns>
    protected virtual IList<int> ParseAttributeIds(string attributesXml)
    {
		_logger.LogError(">>> Entering ParseAttributeIds(attributesXml={AttributesXml})", attributesXml);
        var ids = new List<int>();
        if (string.IsNullOrEmpty(attributesXml))
            return ids;

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("ParseAttributeIds(attributesXml={AttributesXml}): xmlDoc → {XmlDoc}", attributesXml, xmlDoc);
            xmlDoc.LoadXml(attributesXml);

            var elements = xmlDoc.SelectNodes(@$"//Attributes/{ChildElementName}");
			_logger.LogError("ParseAttributeIds(attributesXml={AttributesXml}): elements → {Elements}", attributesXml, elements);

            if (elements == null)
                return Array.Empty<int>();

            foreach (XmlNode node in elements)
            {
                if (node.Attributes?["ID"] == null)
                    continue;

                var attributeValue = node.Attributes["ID"].InnerText.Trim();
				_logger.LogError("ParseAttributeIds(attributesXml={AttributesXml}): attributeValue → {AttributeValue}", attributesXml, attributeValue);
                if (int.TryParse(attributeValue, out var id))
                    ids.Add(id);
            }
        }
        catch
        {
			_logger.LogError("Exception in ParseAttributeIds(attributesXml={AttributesXml}): {Arg1}", attributesXml, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting ParseAttributeIds(attributesXml={AttributesXml})", attributesXml);
        return ids;
    }

    #endregion

    #region Product attributes

    /// <summary>
    /// Gets selected product attribute mappings
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selected product attribute mappings
    /// </returns>
    public virtual async Task<IList<ProductAttributeMapping>> ParseProductAttributeMappingsAsync(string attributesXml)
    {
		_logger.LogError(">>> Entering ParseProductAttributeMappingsAsync(attributesXml={AttributesXml})", attributesXml);
        var result = new List<ProductAttributeMapping>();
        if (string.IsNullOrEmpty(attributesXml))
            return result;

        var ids = ParseAttributeIds(attributesXml);
		_logger.LogError("ParseProductAttributeMappingsAsync(attributesXml={AttributesXml}): ids → {Ids}", attributesXml, ids);
        foreach (var id in ids)
        {
            var attribute = await _productAttributeService.GetProductAttributeMappingByIdAsync(id);
			_logger.LogError("ParseProductAttributeMappingsAsync(attributesXml={AttributesXml}): attribute → {Attribute}", attributesXml, attribute);
            if (attribute != null)
                result.Add(attribute);
        }

		_logger.LogWarning("<<< Exiting ParseProductAttributeMappingsAsync(attributesXml={AttributesXml})", attributesXml);
        return result;
    }

    /// <summary>
    /// /// Get product attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier; pass 0 to load all values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute values
    /// </returns>
    public virtual async Task<IList<ProductAttributeValue>> ParseProductAttributeValuesAsync(string attributesXml, int productAttributeMappingId = 0)
    {
		_logger.LogError(">>> Entering ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>)", attributesXml);
        var values = new List<ProductAttributeValue>();
        if (string.IsNullOrEmpty(attributesXml))
            return values;

        var attributes = await ParseProductAttributeMappingsAsync(attributesXml);
		_logger.LogError("ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): attributes → {Attributes}", attributesXml, attributes);

        //to load values only for the passed product attribute mapping
        if (productAttributeMappingId > 0)
            attributes = attributes.Where(attribute => attribute.Id == productAttributeMappingId).ToList();

        foreach (var attribute in attributes)
        {
            if (!attribute.ShouldHaveValues())
                continue;

            foreach (var attributeValue in ParseValuesWithQuantity(attributesXml, attribute.Id))
            {
                if (string.IsNullOrEmpty(attributeValue.Item1) || !int.TryParse(attributeValue.Item1, out var attributeValueId))
                    continue;

                var value = await _productAttributeService.GetProductAttributeValueByIdAsync(attributeValueId);
				_logger.LogError("ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): value → {Value}", attributesXml, value);
                if (value == null)
                    continue;

                if (!string.IsNullOrEmpty(attributeValue.Item2) && int.TryParse(attributeValue.Item2, out var quantity) && quantity != value.Quantity)
                {
                    //if customer enters quantity, use new entity with new quantity

					var __logStart = System.Diagnostics.Stopwatch.StartNew();
                    var oldValue = await _productAttributeValueRepository.LoadOriginalCopyAsync(value);

					_logger.LogError("ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): oldValue → {OldValue}", attributesXml, oldValue);
					_logger.LogDebug("ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): oldValue.ProductAttributeMappingId → {Arg2}", attributesXml, oldValue.ProductAttributeMappingId);
					_logger.LogError("ParseProductAttributeValuesAsync(attributesXml,productAttributeMappingId)={AttributesXml},<redacted>: find query executed in {Arg2} ms", attributesXml, __logStart.ElapsedMilliseconds);
					_logger.LogDebug("ParseProductAttributeValuesAsync(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): oldValue.Quantity → {Arg2}", attributesXml, oldValue.Quantity);
                    oldValue.ProductAttributeMappingId = attribute.Id;
                    oldValue.Quantity = quantity;
                    values.Add(oldValue);
                }
                else
                    values.Add(value);
            }
        }

        return values;
    }

    /// <summary>
    /// Gets selected product attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
    /// <returns>Product attribute values</returns>
    public virtual IList<string> ParseValues(string attributesXml, int productAttributeMappingId)
    {
		_logger.LogError(">>> Entering ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>)", attributesXml);
        var selectedValues = new List<string>();
        if (string.IsNullOrEmpty(attributesXml))
            return selectedValues;

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): xmlDoc → {XmlDoc}", attributesXml, xmlDoc);
            xmlDoc.LoadXml(attributesXml);

            var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
			_logger.LogError("ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): nodeList1 → {NodeList1}", attributesXml, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["ID"] == null)
                    continue;

                var str1 = node1.Attributes["ID"].InnerText.Trim();
				_logger.LogError("ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): str1 → {Str1}", attributesXml, str1);
                if (!int.TryParse(str1, out var id))
                    continue;

                if (id != productAttributeMappingId)
                    continue;

                var nodeList2 = node1.SelectNodes(@"ProductAttributeValue/Value");
				_logger.LogError("ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): nodeList2 → {NodeList2}", attributesXml, nodeList2);
                foreach (XmlNode node2 in nodeList2)
                {
                    var value = node2.InnerText.Trim();
					_logger.LogError("ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): value → {Value}", attributesXml, value);
                    selectedValues.Add(value);
                }
            }
        }
        catch
        {
			_logger.LogError("Exception in ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>): {Arg2}", attributesXml, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting ParseValues(attributesXml={AttributesXml},productAttributeMappingId=<redacted>)", attributesXml);
        return selectedValues;
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <param name="value">Value</param>
    /// <param name="quantity">Quantity (used with AttributeValueType.AssociatedToProduct to specify the quantity entered by the customer)</param>
    /// <returns>Updated result (XML format)</returns>
    public virtual string AddProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping, string value, int? quantity = null)
    {
		_logger.LogError(">>> Entering AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity})", attributesXml, value, quantity);
        var result = string.Empty;
		_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): result → {Result}", attributesXml, value, quantity, result);
        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): xmlDoc → {XmlDoc}", attributesXml, value, quantity, xmlDoc);
            if (string.IsNullOrEmpty(attributesXml))
            {
                var element1 = xmlDoc.CreateElement("Attributes");
				_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): element1 → {Element1}", attributesXml, value, quantity, element1);
                xmlDoc.AppendChild(element1);
            }
            else
            {
                xmlDoc.LoadXml(attributesXml);
            }

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): rootElement → {RootElement}", attributesXml, value, quantity, rootElement);

            XmlElement attributeElement = null;
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): attributeElement → {AttributeElement}", attributesXml, value, quantity, attributeElement);
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): nodeList1 → {NodeList1}", attributesXml, value, quantity, nodeList1);
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["ID"] == null)
                    continue;

                var str1 = node1.Attributes["ID"].InnerText.Trim();
				_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): str1 → {Str1}", attributesXml, value, quantity, str1);
                if (!int.TryParse(str1, out var id))
                    continue;

                if (id != productAttributeMapping.Id)
                    continue;

                attributeElement = (XmlElement)node1;
                break;
            }

            //create new one if not found
            if (attributeElement == null)
            {
                attributeElement = xmlDoc.CreateElement("ProductAttribute");
                attributeElement.SetAttribute("ID", productAttributeMapping.Id.ToString());
                rootElement.AppendChild(attributeElement);
            }

            var attributeValueElement = xmlDoc.CreateElement("ProductAttributeValue");
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): attributeValueElement → {AttributeValueElement}", attributesXml, value, quantity, attributeValueElement);
            attributeElement.AppendChild(attributeValueElement);

            var attributeValueValueElement = xmlDoc.CreateElement("Value");
			_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): attributeValueValueElement → {AttributeValueValueElement}", attributesXml, value, quantity, attributeValueValueElement);
			_logger.LogDebug("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): attributeValueValueElement.InnerText → {Arg4}", attributesXml, value, quantity, attributeValueValueElement.InnerText);
            attributeValueValueElement.InnerText = value;
            attributeValueElement.AppendChild(attributeValueValueElement);

            //the quantity entered by the customer
            if (quantity.HasValue)
            {
                var attributeValueQuantity = xmlDoc.CreateElement("Quantity");
				_logger.LogError("AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): attributeValueQuantity → {AttributeValueQuantity}", attributesXml, value, quantity, attributeValueQuantity);
                attributeValueQuantity.InnerText = quantity.ToString();
                attributeValueElement.AppendChild(attributeValueQuantity);
            }

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity}): {Arg4}", attributesXml, value, quantity, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting AddProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>,value={Value},quantity={Quantity})", attributesXml, value, quantity);
        return result;
    }

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>Updated result (XML format)</returns>
    public virtual string RemoveProductAttribute(string attributesXml, ProductAttributeMapping productAttributeMapping)
    {
		_logger.LogError(">>> Entering RemoveProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>)", attributesXml);
        _logger.LogWarning("<<< Exiting RemoveProductAttribute(attributesXml={AttributesXml},productAttributeMapping=<redacted>)", attributesXml);
        return RemoveAttribute(attributesXml, productAttributeMapping.Id);
    }

    /// <summary>
    /// Are attributes equal
    /// </summary>
    /// <param name="attributesXml1">The attributes of the first product</param>
    /// <param name="attributesXml2">The attributes of the second product</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <param name="ignoreQuantity">A value indicating whether we should ignore the quantity of attribute value entered by the customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> AreProductAttributesEqualAsync(string attributesXml1, string attributesXml2, bool ignoreNonCombinableAttributes, bool ignoreQuantity = true)
    {
		_logger.LogError(">>> Entering AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity})", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity);
        var attributes1 = await ParseProductAttributeMappingsAsync(attributesXml1);
        if (ignoreNonCombinableAttributes)
            attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
			_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): attributes1 → {Attributes1}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, attributes1);

        var attributes2 = await ParseProductAttributeMappingsAsync(attributesXml2);
		_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): attributes2 → {Attributes2}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, attributes2);
        if (ignoreNonCombinableAttributes)
            attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();

        if (attributes1.Count != attributes2.Count)
            return false;

        var attributesEqual = true;
		_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): attributesEqual → {AttributesEqual}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, attributesEqual);
        foreach (var a1 in attributes1)
        {
            var hasAttribute = false;
			_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): hasAttribute → {HasAttribute}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, hasAttribute);
            foreach (var a2 in attributes2)
            {
                if (a1.Id != a2.Id)
                    continue;

                hasAttribute = true;
                var values1Str = ParseValuesWithQuantity(attributesXml1, a1.Id);
				_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): values1Str → {Values1Str}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, values1Str);
                var values2Str = ParseValuesWithQuantity(attributesXml2, a2.Id);
				_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): values2Str → {Values2Str}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, values2Str);
                if (values1Str.Count == values2Str.Count)
                {
                    foreach (var str1 in values1Str)
                    {
                        var hasValue = false;
						_logger.LogError("AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity}): hasValue → {HasValue}", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity, hasValue);
                        foreach (var str2 in values2Str)
                        {
                            //case insensitive? 
                            //if (str1.Trim().ToLowerInvariant() == str2.Trim().ToLowerInvariant())
                            if (str1.Item1.Trim() != str2.Item1.Trim())
                                continue;

                            hasValue = ignoreQuantity || str1.Item2.Trim() == str2.Item2.Trim();
                            break;
                        }

                        if (hasValue)
                            continue;

                        attributesEqual = false;
                        break;
                    }
                }
                else
                {
                    attributesEqual = false;
                    break;
                }
            }

            if (hasAttribute)
                continue;

            attributesEqual = false;
            break;
        }

		_logger.LogWarning("<<< Exiting AreProductAttributesEqualAsync(attributesXml1={AttributesXml1},attributesXml2={AttributesXml2},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},ignoreQuantity={IgnoreQuantity})", attributesXml1, attributesXml2, ignoreNonCombinableAttributes, ignoreQuantity);
        return attributesEqual;
    }

    /// <summary>
    /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
    /// </summary>
    /// <param name="pam">Product attribute</param>
    /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool?> IsConditionMetAsync(ProductAttributeMapping pam, string selectedAttributesXml)
    {
		_logger.LogError(">>> Entering IsConditionMetAsync(pam={Pam},selectedAttributesXml={SelectedAttributesXml})", pam, selectedAttributesXml);
        ArgumentNullException.ThrowIfNull(pam);

        var conditionAttributeXml = pam.ConditionAttributeXml;
        if (string.IsNullOrEmpty(conditionAttributeXml))
            //no condition
            return null;

        //load an attribute this one depends on
        var dependOnAttribute = (await ParseProductAttributeMappingsAsync(conditionAttributeXml)).FirstOrDefault();
        if (dependOnAttribute == null)
            return true;

        var valuesThatShouldBeSelected = ParseValues(conditionAttributeXml, dependOnAttribute.Id)
            //a workaround here:
            //ConditionAttributeXml can contain "empty" values (nothing is selected)
            //but in other cases (like below) we do not store empty values
            //that's why we remove empty values here
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
        var selectedValues = ParseValues(selectedAttributesXml, dependOnAttribute.Id);
        if (valuesThatShouldBeSelected.Count != selectedValues.Count)
            return false;

        //compare values
        var allFound = true;
        foreach (var t1 in valuesThatShouldBeSelected)
        {
            var found = false;
            foreach (var t2 in selectedValues)
                if (t1 == t2)
                    found = true;
            if (!found)
                allFound = false;
        }

		_logger.LogWarning("<<< Exiting IsConditionMetAsync(pam={Pam},selectedAttributesXml={SelectedAttributesXml})", pam, selectedAttributesXml);
        return allFound;
    }

    /// <summary>
    /// Finds a product attribute combination by attributes stored in XML 
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found product attribute combination
    /// </returns>
    public virtual async Task<ProductAttributeCombination> FindProductAttributeCombinationAsync(Product product,
        string attributesXml, bool ignoreNonCombinableAttributes = true)
    {
        ArgumentNullException.ThrowIfNull(product);

        //anyway combination cannot contains non combinable attributes
        if (string.IsNullOrEmpty(attributesXml))
            return null;

        var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
        return await combinations.FirstOrDefaultAwaitAsync(async x =>
            await AreProductAttributesEqualAsync(x.AttributesXml, attributesXml, ignoreNonCombinableAttributes));
    }

    /// <summary>
    /// Generate all combinations
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
    /// <param name="allowedAttributeIds">List of allowed attribute identifiers. If null or empty then all attributes would be used.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute combinations in XML format
    /// </returns>
    public virtual async Task<IList<string>> GenerateAllCombinationsAsync(Product product, bool ignoreNonCombinableAttributes = false, IList<int> allowedAttributeIds = null)
    {
		_logger.LogError(">>> Entering GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds})", product, ignoreNonCombinableAttributes, allowedAttributeIds);
        ArgumentNullException.ThrowIfNull(product);

        var allProductAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
		_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): allProductAttributeMappings → {AllProductAttributeMappings}", product, ignoreNonCombinableAttributes, allowedAttributeIds, allProductAttributeMappings);

        if (ignoreNonCombinableAttributes)
            allProductAttributeMappings = allProductAttributeMappings.Where(x => !x.IsNonCombinable()).ToList();

        //get all possible attribute combinations
        var allPossibleAttributeCombinations = CreateCombination(allProductAttributeMappings);
		_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): allPossibleAttributeCombinations → {AllPossibleAttributeCombinations}", product, ignoreNonCombinableAttributes, allowedAttributeIds, allPossibleAttributeCombinations);

        var allAttributesXml = new List<string>();
		_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): allAttributesXml → {AllAttributesXml}", product, ignoreNonCombinableAttributes, allowedAttributeIds, allAttributesXml);

        foreach (var combination in allPossibleAttributeCombinations)
        {
            var attributesXml = new List<string>();
            foreach (var productAttributeMapping in combination)
            {
                if (!productAttributeMapping.ShouldHaveValues())
                    continue;

                //get product attribute values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMapping.Id);
				_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): attributeValues → {AttributeValues}", product, ignoreNonCombinableAttributes, allowedAttributeIds, attributeValues);

                //filter product attribute values
                if (allowedAttributeIds?.Any() ?? false)
                    attributeValues = attributeValues.Where(attributeValue => allowedAttributeIds.Contains(attributeValue.Id)).ToList();

                if (!attributeValues.Any())
                    continue;

                var isCheckbox = productAttributeMapping.AttributeControlType == AttributeControlType.Checkboxes ||
                                 productAttributeMapping.AttributeControlType ==
                                 AttributeControlType.ReadonlyCheckboxes;

                var currentAttributesXml = new List<string>();
				_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): currentAttributesXml → {CurrentAttributesXml}", product, ignoreNonCombinableAttributes, allowedAttributeIds, currentAttributesXml);

                if (isCheckbox)
                {
                    //add several values attribute types (checkboxes)

                    //checkboxes could have several values ticked
                    foreach (var oldXml in attributesXml.Any() ? attributesXml : [string.Empty])
                    {
                        foreach (var checkboxCombination in CreateCombination(attributeValues))
                        {
                            var newXml = oldXml;
							_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): newXml → {NewXml}", product, ignoreNonCombinableAttributes, allowedAttributeIds, newXml);
                            foreach (var checkboxValue in checkboxCombination)
                                newXml = AddProductAttribute(newXml, productAttributeMapping, checkboxValue.Id.ToString());

                            if (!string.IsNullOrEmpty(newXml))
                                currentAttributesXml.Add(newXml);
								_logger.LogWarning("<<< Exiting CreateCombination(elements={Elements})", elements);
                        }
                    }
                }
                else
                {
                    //add one value attribute types (dropdownlist, radiobutton, color squares)

                    foreach (var oldXml in attributesXml.Any() ? attributesXml : [string.Empty])
                    {
                        currentAttributesXml.AddRange(attributeValues.Select(attributeValue =>
                            AddProductAttribute(oldXml, productAttributeMapping, attributeValue.Id.ToString())));
                    }
                }

                attributesXml.Clear();
                attributesXml.AddRange(currentAttributesXml);
            }

            allAttributesXml.AddRange(attributesXml);
        }

        //validate conditional attributes (if specified)
        //minor workaround:
        //once it's done (validation), then we could have some duplicated combinations in result
        //we don't remove them here (for performance optimization) because anyway it'll be done in the "GenerateAllAttributeCombinations" method of ProductController
        for (var i = 0; i < allAttributesXml.Count; i++)
        {
            var attributesXml = allAttributesXml[i];
            foreach (var attribute in allProductAttributeMappings)
            {
                var conditionMet = await IsConditionMetAsync(attribute, attributesXml);
				_logger.LogError("GenerateAllCombinationsAsync(product={Product},ignoreNonCombinableAttributes={IgnoreNonCombinableAttributes},allowedAttributeIds={AllowedAttributeIds}): conditionMet → {ConditionMet}", product, ignoreNonCombinableAttributes, allowedAttributeIds, conditionMet);
                if (conditionMet.HasValue && !conditionMet.Value)
                    allAttributesXml[i] = RemoveProductAttribute(attributesXml, attribute);
            }
        }

        return allAttributesXml;
    }

    /// <summary>
    /// Parse a customer entered price of the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer entered price of the product
    /// </returns>
    public virtual async Task<decimal> ParseCustomerEnteredPriceAsync(Product product, IFormCollection form)
    {
		_logger.LogError(">>> Entering ParseCustomerEnteredPriceAsync(product={Product},form={Form})", product, form);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(form);

        var customerEnteredPriceConverted = decimal.Zero;
		_logger.LogError("ParseCustomerEnteredPriceAsync(product={Product},form={Form}): customerEnteredPriceConverted → {CustomerEnteredPriceConverted}", product, form, customerEnteredPriceConverted);
        if (product.CustomerEntersPrice)
        {
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"addtocart_{product.Id}.CustomerEnteredPrice", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (decimal.TryParse(form[formKey], NumberStyles.Any, CultureInfo.InvariantCulture, out var customerEnteredPrice))
                        customerEnteredPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(customerEnteredPrice, await _workContext.GetWorkingCurrencyAsync());
                    break;
                }
            }
        }

		_logger.LogWarning("<<< Exiting ParseCustomerEnteredPriceAsync(product={Product},form={Form})", product, form);
        return customerEnteredPriceConverted;
    }

    /// <summary>
    /// Parse a entered quantity of the product
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <returns>Customer entered price of the product</returns>
    public virtual int ParseEnteredQuantity(Product product, IFormCollection form)
    {
		_logger.LogError(">>> Entering ParseEnteredQuantity(product={Product},form={Form})", product, form);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(form);

        var quantity = 1;
        foreach (var formKey in form.Keys)
            if (formKey.Equals($"addtocart_{product.Id}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
            {
                _ = int.TryParse(form[formKey], out quantity);
				_logger.LogError("ParseEnteredQuantity(product={Product},form={Form}): _ → {_}", product, form, _);
                break;
            }

		_logger.LogWarning("<<< Exiting ParseEnteredQuantity(product={Product},form={Form})", product, form);
        return quantity;
    }

    /// <summary>
    /// Parse product rental dates on the product details page
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form</param>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    public virtual void ParseRentalDates(Product product, IFormCollection form, out DateTime? startDate, out DateTime? endDate)
    {
		_logger.LogError(">>> Entering ParseRentalDates(product={Product},form={Form})", product, form);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(form);

        startDate = null;
		_logger.LogDebug("ParseRentalDates(product={Product},form={Form}): startDate → {StartDate}", product, form, startDate);
        endDate = null;
		_logger.LogDebug("ParseRentalDates(product={Product},form={Form}): endDate → {EndDate}", product, form, endDate);

        if (product.IsRental)
        {
            var ctrlStartDate = form[$"rental_start_date_{product.Id}"];
			_logger.LogError("ParseRentalDates(product={Product},form={Form}): ctrlStartDate → {CtrlStartDate}", product, form, ctrlStartDate);
            var ctrlEndDate = form[$"rental_end_date_{product.Id}"];
			_logger.LogError("ParseRentalDates(product={Product},form={Form}): ctrlEndDate → {CtrlEndDate}", product, form, ctrlEndDate);

            var dateFormat = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ?
                        CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern :
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

            try
            {
                startDate = DateTime.ParseExact(ctrlStartDate,
                    dateFormat,
                    CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate,
                    dateFormat,
                    CultureInfo.InvariantCulture);
            }
            catch
            {
				_logger.LogError("Exception in ParseRentalDates(product={Product},form={Form}): {Arg2}", product, form, ex.Message);
                // ignored
            }
        }
		_logger.LogWarning("<<< Exiting ParseRentalDates(product={Product},form={Form})", product, form);
    }

    /// <summary>
    /// Get product attributes from the passed form
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="form">Form values</param>
    /// <param name="errors">Errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    public virtual async Task<string> ParseProductAttributesAsync(Product product, IFormCollection form, List<string> errors)
    {
		_logger.LogError(">>> Entering ParseProductAttributesAsync(product={Product},form={Form},errors={Errors})", product, form, errors);
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(form);

        //product attributes
        var attributesXml = await GetProductAttributesXmlAsync(product, form, errors);
		_logger.LogError("ParseProductAttributesAsync(product={Product},form={Form},errors={Errors}): attributesXml → {AttributesXml}", product, form, errors, attributesXml);

        //gift cards
        AddGiftCardsAttributesXml(product, form, ref attributesXml);

		_logger.LogWarning("<<< Exiting ParseProductAttributesAsync(product={Product},form={Form},errors={Errors})", product, form, errors);
        return attributesXml;
    }

    #endregion

    #region Gift card attributes

    /// <summary>
    /// Add gift card attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="recipientName">Recipient name</param>
    /// <param name="recipientEmail">Recipient email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="giftCardMessage">Message</param>
    /// <returns>Attributes</returns>
    public virtual string AddGiftCardAttribute(string attributesXml, string recipientName,
        string recipientEmail, string senderName, string senderEmail, string giftCardMessage)
    {
		_logger.LogError(">>> Entering AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage})", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        var result = string.Empty;
		_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): result → {Result}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, result);
        try
        {
            recipientName = recipientName.Trim();
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientName → {RecipientName1}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientName);
            recipientEmail = recipientEmail.Trim();
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientEmail → {RecipientEmail1}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientEmail);
            senderName = senderName.Trim();
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderName → {SenderName1}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderName);
            senderEmail = senderEmail.Trim();
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderEmail → {SenderEmail1}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderEmail);

            var xmlDoc = new XmlDocument();
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): xmlDoc → {XmlDoc}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, xmlDoc);
            if (string.IsNullOrEmpty(attributesXml))
            {
                var element1 = xmlDoc.CreateElement("Attributes");
				_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): element1 → {Element1}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, element1);
                xmlDoc.AppendChild(element1);
            }
            else
                xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): rootElement → {RootElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, rootElement);

            var giftCardElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): giftCardElement → {GiftCardElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, giftCardElement);
            if (giftCardElement == null)
            {
                giftCardElement = xmlDoc.CreateElement("GiftCardInfo");
                rootElement.AppendChild(giftCardElement);
            }

            var recipientNameElement = xmlDoc.CreateElement("RecipientName");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientNameElement → {RecipientNameElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientNameElement);
			_logger.LogDebug("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientNameElement.InnerText → {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientNameElement.InnerText);
            recipientNameElement.InnerText = recipientName;
            giftCardElement.AppendChild(recipientNameElement);

            var recipientEmailElement = xmlDoc.CreateElement("RecipientEmail");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientEmailElement → {RecipientEmailElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientEmailElement);
			_logger.LogDebug("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): recipientEmailElement.InnerText → {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, recipientEmailElement.InnerText);
            recipientEmailElement.InnerText = recipientEmail;
            giftCardElement.AppendChild(recipientEmailElement);

            var senderNameElement = xmlDoc.CreateElement("SenderName");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderNameElement → {SenderNameElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderNameElement);
			_logger.LogDebug("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderNameElement.InnerText → {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderNameElement.InnerText);
            senderNameElement.InnerText = senderName;
            giftCardElement.AppendChild(senderNameElement);

            var senderEmailElement = xmlDoc.CreateElement("SenderEmail");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderEmailElement → {SenderEmailElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderEmailElement);
			_logger.LogDebug("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): senderEmailElement.InnerText → {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, senderEmailElement.InnerText);
            senderEmailElement.InnerText = senderEmail;
            giftCardElement.AppendChild(senderEmailElement);

            var messageElement = xmlDoc.CreateElement("Message");
			_logger.LogError("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): messageElement → {MessageElement}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, messageElement);
			_logger.LogDebug("AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): messageElement.InnerText → {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, messageElement.InnerText);
            messageElement.InnerText = giftCardMessage;
            giftCardElement.AppendChild(messageElement);

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage}): {Arg6}", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting AddGiftCardAttribute(attributesXml={AttributesXml},recipientName={RecipientName},recipientEmail={RecipientEmail},senderName={SenderName},senderEmail={SenderEmail},giftCardMessage={GiftCardMessage})", attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        return result;
    }

    /// <summary>
    /// Get gift card attributes
    /// </summary>
    /// <param name="attributesXml">Attributes</param>
    /// <param name="recipientName">Recipient name</param>
    /// <param name="recipientEmail">Recipient email</param>
    /// <param name="senderName">Sender name</param>
    /// <param name="senderEmail">Sender email</param>
    /// <param name="giftCardMessage">Message</param>
    public virtual void GetGiftCardAttribute(string attributesXml, out string recipientName,
        out string recipientEmail, out string senderName,
        out string senderEmail, out string giftCardMessage)
    {
		_logger.LogError(">>> Entering GetGiftCardAttribute(attributesXml={AttributesXml})", attributesXml);
        recipientName = string.Empty;
        recipientEmail = string.Empty;
		_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientEmail → {RecipientEmail}", attributesXml, recipientEmail);
		_logger.LogDebug("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientName → {RecipientName}", attributesXml, recipientName);
		_logger.LogDebug("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientEmail → {RecipientEmail}", attributesXml, recipientEmail);
        senderName = string.Empty;
		_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): senderName → {SenderName}", attributesXml, senderName);
        senderEmail = string.Empty;
		_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): senderEmail → {SenderEmail}", attributesXml, senderEmail);
		_logger.LogDebug("GetGiftCardAttribute(attributesXml={AttributesXml}): senderEmail → {SenderEmail}", attributesXml, senderEmail);
        giftCardMessage = string.Empty;

		_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): giftCardMessage → {GiftCardMessage}", attributesXml, giftCardMessage);
        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogDebug("GetGiftCardAttribute(attributesXml={AttributesXml}): senderName → {SenderName}", attributesXml, senderName);
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): xmlDoc → {XmlDoc}", attributesXml, xmlDoc);
            xmlDoc.LoadXml(attributesXml);

            var recipientNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientName");
			_logger.LogDebug("GetGiftCardAttribute(attributesXml={AttributesXml}): giftCardMessage → {GiftCardMessage}", attributesXml, giftCardMessage);
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientNameElement → {RecipientNameElement}", attributesXml, recipientNameElement);
            var recipientEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/RecipientEmail");
            var senderNameElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderName");
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientEmailElement → {RecipientEmailElement}", attributesXml, recipientEmailElement);
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): senderNameElement → {SenderNameElement}", attributesXml, senderNameElement);
            var senderEmailElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/SenderEmail");
            var messageElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes/GiftCardInfo/Message");
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): senderEmailElement → {SenderEmailElement}", attributesXml, senderEmailElement);
			_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): messageElement → {MessageElement}", attributesXml, messageElement);

            if (recipientNameElement != null)
                recipientName = recipientNameElement.InnerText;
				_logger.LogError("GetGiftCardAttribute(attributesXml={AttributesXml}): recipientName → {RecipientName}", attributesXml, recipientName);
            if (recipientEmailElement != null)
                recipientEmail = recipientEmailElement.InnerText;
            if (senderNameElement != null)
                senderName = senderNameElement.InnerText;
            if (senderEmailElement != null)
                senderEmail = senderEmailElement.InnerText;
            if (messageElement != null)
                giftCardMessage = messageElement.InnerText;
        }
        catch
        {
			_logger.LogError("Exception in GetGiftCardAttribute(attributesXml={AttributesXml}): {Arg1}", attributesXml, ex.Message);
            //ignore
		_logger.LogWarning("<<< Exiting GetGiftCardAttribute(attributesXml={AttributesXml})", attributesXml);
        }
    }

    #endregion

    #region Properties

    protected string ChildElementName { get; set; } = "ProductAttribute";
        
    #endregion
}