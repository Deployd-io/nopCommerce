using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Microsoft.Extensions.Logging;

namespace Nop.Services.Attributes;

/// <summary>
/// Attribute parser
/// </summary>
/// <typeparam name="TAttribute">Type of the attribute (see <see cref="BaseAttribute"/>)</typeparam>
/// <typeparam name="TAttributeValue">Type of the attribute value (see <see cref="BaseAttributeValue"/>)</typeparam>
public partial class AttributeParser<TAttribute, TAttributeValue> : IAttributeParser<TAttribute, TAttributeValue>
    where TAttribute : BaseAttribute
    where TAttributeValue : BaseAttributeValue
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    protected readonly IAttributeService<TAttribute, TAttributeValue> _attributeService;
    protected readonly ILocalizationService _localizationService;

    protected readonly string _attributeName;
    protected readonly string _attributeValueName;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public AttributeParser(IAttributeService<TAttribute, TAttributeValue> attributeService,
        ILocalizationService localizationService)
    {
		_logger.LogError(">>> Entering AttributeParser(attributeService={AttributeService},localizationService={LocalizationService})", attributeService, localizationService);
        _attributeName = typeof(TAttribute).Name;
        _attributeValueName = typeof(TAttributeValue).Name;
		_logger.LogError("AttributeParser(attributeService={AttributeService},localizationService={LocalizationService}): _attributeValueName → {_attributeValueName}", attributeService, localizationService, _attributeValueName);
		_logger.LogDebug("AttributeParser(attributeService={AttributeService},localizationService={LocalizationService}): _attributeName → {_attributeName}", attributeService, localizationService, _attributeName);
		_logger.LogDebug("AttributeParser(attributeService={AttributeService},localizationService={LocalizationService}): _attributeValueName → {_attributeValueName}", attributeService, localizationService, _attributeValueName);

        _attributeService = attributeService;
        _localizationService = localizationService;
		_logger.LogDebug("AttributeParser(attributeService={AttributeService},localizationService={LocalizationService}): _attributeService → {_attributeService}", attributeService, localizationService, _attributeService);
    _logger.LogWarning("<<< Exiting AttributeParser(attributeService={AttributeService},localizationService={LocalizationService})", attributeService, localizationService);
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets attribute values
    /// </summary>
    /// <param name="valuesStr">string value attribute identifiers</param>
    /// <returns>Attribute values</returns>
    protected virtual async IAsyncEnumerable<TAttributeValue> GetValuesAsync(IList<string> valuesStr)
    {
		_logger.LogError(">>> Entering GetValuesAsync(valuesStr={ValuesStr})", valuesStr);
        foreach (var valueStr in valuesStr)
        {
            if (string.IsNullOrEmpty(valueStr))
                continue;

            if (!int.TryParse(valueStr, out var id))
                continue;

            var value = await _attributeService.GetAttributeValueByIdAsync(id);
			_logger.LogError("GetValuesAsync(valuesStr={ValuesStr}): value → {Value}", valuesStr, value);
            if (value != null)
                yield return value;
        }
		_logger.LogWarning("<<< Exiting GetValuesAsync(valuesStr={ValuesStr})", valuesStr);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets selected attribute identifiers
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>Selected attribute identifiers</returns>
    public virtual IEnumerable<int> ParseAttributeIds(string attributesXml)
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

            var nodes = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");
			_logger.LogError("ParseAttributeIds(attributesXml={AttributesXml}): nodes → {Nodes}", attributesXml, nodes);

            if (nodes == null)
                return Enumerable.Empty<int>();

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes?["ID"] == null)
                    continue;

                var str1 = node.Attributes["ID"].InnerText.Trim();
				_logger.LogError("ParseAttributeIds(attributesXml={AttributesXml}): str1 → {Str1}", attributesXml, str1);

                if (int.TryParse(str1, out var id))
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

    /// <summary>
    /// Gets selected attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the selected attributes
    /// </returns>
    public virtual async Task<IList<TAttribute>> ParseAttributesAsync(string attributesXml)
    {
		_logger.LogError(">>> Entering ParseAttributesAsync(attributesXml={AttributesXml})", attributesXml);
        var result = new List<TAttribute>();
        if (string.IsNullOrEmpty(attributesXml))
            return result;

        var ids = ParseAttributeIds(attributesXml);
		_logger.LogError("ParseAttributesAsync(attributesXml={AttributesXml}): ids → {Ids}", attributesXml, ids);
        foreach (var id in ids)
        {
            var attribute = await _attributeService.GetAttributeByIdAsync(id);
			_logger.LogError("ParseAttributesAsync(attributesXml={AttributesXml}): attribute → {Attribute}", attributesXml, attribute);
            if (attribute != null)
                result.Add(attribute);
        }

		_logger.LogWarning("<<< Exiting ParseAttributesAsync(attributesXml={AttributesXml})", attributesXml);
        return result;
    }

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Attribute identifier</param>
    /// <returns>Updated result (XML format)</returns>
    public virtual string RemoveAttribute(string attributesXml, int attributeId)
    {
		_logger.LogError(">>> Entering RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId})", attributesXml, attributeId);
        var result = string.Empty;

		_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): result → {Result}", attributesXml, attributeId, result);
        if (string.IsNullOrEmpty(attributesXml))
            return string.Empty;

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): xmlDoc → {XmlDoc}", attributesXml, attributeId, xmlDoc);

            xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): rootElement → {RootElement}", attributesXml, attributeId, rootElement);

            if (rootElement == null)
                return string.Empty;

            XmlElement attributeElement = null;
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): attributeElement → {AttributeElement}", attributesXml, attributeId, attributeElement);
            //find existing
            var childNodes = xmlDoc.SelectNodes($@"//Attributes/{_attributeName}");
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): childNodes → {ChildNodes}", attributesXml, attributeId, childNodes);

            if (childNodes == null)
                return string.Empty;

            var count = childNodes.Count;
			_logger.LogError("RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): count → {Count}", attributesXml, attributeId, count);

            foreach (XmlElement childNode in childNodes)
            {
                if (!int.TryParse(childNode.Attributes["ID"]?.InnerText.Trim(), out var id))
                    continue;

                if (id != attributeId)
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
			_logger.LogError("Exception in RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId}): {Arg2}", attributesXml, attributeId, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting RemoveAttribute(attributesXml={AttributesXml},attributeId={AttributeId})", attributesXml, attributeId);
        return result;
    }

    /// <summary>
    /// Gets selected attribute value
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Attribute identifier</param>
    /// <returns>Attribute value</returns>
    public virtual IList<string> ParseValues(string attributesXml, int attributeId)
    {
		_logger.LogError(">>> Entering ParseValues(attributesXml={AttributesXml},attributeId={AttributeId})", attributesXml, attributeId);
        var selectedAddressAttributeValues = new List<string>();
        if (string.IsNullOrEmpty(attributesXml))
            return selectedAddressAttributeValues;

        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): xmlDoc → {XmlDoc}", attributesXml, attributeId, xmlDoc);
            xmlDoc.LoadXml(attributesXml);

            var nodeList1 = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");
			_logger.LogError("ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): nodeList1 → {NodeList1}", attributesXml, attributeId, nodeList1);
            if (nodeList1 == null)
                return new List<string>();

            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes?["ID"] == null)
                    continue;

                var str1 = node1.Attributes["ID"].InnerText.Trim();
				_logger.LogError("ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): str1 → {Str1}", attributesXml, attributeId, str1);
                if (!int.TryParse(str1, out var id))
                    continue;

                if (id != attributeId)
                    continue;

                var nodeList2 = node1.SelectNodes(@$"{_attributeValueName}/Value");
				_logger.LogError("ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): nodeList2 → {NodeList2}", attributesXml, attributeId, nodeList2);

                if (nodeList2 == null)
                    continue;

                foreach (XmlNode node2 in nodeList2)
                {
                    var value = node2.InnerText.Trim();
					_logger.LogError("ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): value → {Value}", attributesXml, attributeId, value);
                    selectedAddressAttributeValues.Add(value);
                }
            }
        }
        catch
        {
			_logger.LogError("Exception in ParseValues(attributesXml={AttributesXml},attributeId={AttributeId}): {Arg2}", attributesXml, attributeId, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting ParseValues(attributesXml={AttributesXml},attributeId={AttributeId})", attributesXml, attributeId);
        return selectedAddressAttributeValues;
    }

    /// <summary>
    /// Get attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attribute values
    /// </returns>
    public virtual async Task<IList<TAttributeValue>> ParseAttributeValuesAsync(string attributesXml)
    {
		_logger.LogError(">>> Entering ParseAttributeValuesAsync(attributesXml={AttributesXml})", attributesXml);
        var values = new List<TAttributeValue>();

		_logger.LogError("ParseAttributeValuesAsync(attributesXml={AttributesXml}): values → {Values}", attributesXml, values);
        if (string.IsNullOrEmpty(attributesXml))
            return values;

        var attributes = await ParseAttributesAsync(attributesXml);
		_logger.LogError("ParseAttributeValuesAsync(attributesXml={AttributesXml}): attributes → {Attributes}", attributesXml, attributes);
        foreach (var attribute in attributes)
        {
            if (!attribute.ShouldHaveValues)
                continue;

            var valuesStr = ParseValues(attributesXml, attribute.Id);
			_logger.LogError("ParseAttributeValuesAsync(attributesXml={AttributesXml}): valuesStr → {ValuesStr}", attributesXml, valuesStr);
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

            values.AddRange(await GetValuesAsync(valuesStr).ToArrayAsync());
			_logger.LogError("ParseAttributeValuesAsync(attributesXml)={AttributesXml}: database query executed in {Arg1} ms", attributesXml, __logStart.ElapsedMilliseconds);
        }

		_logger.LogWarning("<<< Exiting ParseAttributeValuesAsync(attributesXml={AttributesXml})", attributesXml);
        return values;
    }

    /// <summary>
    /// Get attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>Attribute values</returns>
    public virtual async IAsyncEnumerable<(TAttribute attribute, IAsyncEnumerable<TAttributeValue> values)> ParseAttributeValues(string attributesXml)
    {
        if (string.IsNullOrEmpty(attributesXml))
            yield break;

        var attributes = await ParseAttributesAsync(attributesXml);

        foreach (var attribute in attributes)
        {
            if (!attribute.ShouldHaveValues)
                continue;

            var valuesStr = ParseValues(attributesXml, attribute.Id);

            yield return (attribute, GetValuesAsync(valuesStr));
        }
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attribute">Attribute</param>
    /// <param name="value">Attribute value</param>
    /// <returns>Attributes</returns>
    public virtual string AddAttribute(string attributesXml, TAttribute attribute, string value)
    {
		_logger.LogError(">>> Entering AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value})", attributesXml, attribute, value);
        var result = string.Empty;
		_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): result → {Result}", attributesXml, attribute, value, result);
        try
        {
            var xmlDoc = new XmlDocument();
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): xmlDoc → {XmlDoc}", attributesXml, attribute, value, xmlDoc);
            if (string.IsNullOrEmpty(attributesXml))
                xmlDoc.AppendChild(xmlDoc.CreateElement("Attributes"));
            else
                xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes")!;
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): rootElement → {RootElement}", attributesXml, attribute, value, rootElement);

            XmlElement attributeElement = null;
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): attributeElement → {AttributeElement}", attributesXml, attribute, value, attributeElement);
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@$"//Attributes/{_attributeName}");
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): nodeList1 → {NodeList1}", attributesXml, attribute, value, nodeList1);

            if (nodeList1 != null)
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["ID"] == null)
                        continue;

                    var str1 = node1.Attributes["ID"].InnerText.Trim();
					_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): str1 → {Str1}", attributesXml, attribute, value, str1);
                    if (!int.TryParse(str1, out var id))
                        continue;

                    if (id != attribute.Id)
                        continue;

                    attributeElement = (XmlElement)node1;
                    break;
                }

            //create new one if not found
            if (attributeElement == null)
            {
                attributeElement = xmlDoc.CreateElement(_attributeName);
                attributeElement.SetAttribute("ID", attribute.Id.ToString());
                rootElement.AppendChild(attributeElement);
            }

            var attributeValueElement = xmlDoc.CreateElement(_attributeValueName);
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): attributeValueElement → {AttributeValueElement}", attributesXml, attribute, value, attributeValueElement);
            attributeElement.AppendChild(attributeValueElement);

            var attributeValueValueElement = xmlDoc.CreateElement("Value");
			_logger.LogError("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): attributeValueValueElement → {AttributeValueValueElement}", attributesXml, attribute, value, attributeValueValueElement);
			_logger.LogDebug("AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): attributeValueValueElement.InnerText → {Arg3}", attributesXml, attribute, value, attributeValueValueElement.InnerText);
            attributeValueValueElement.InnerText = value;
            attributeValueElement.AppendChild(attributeValueValueElement);

            result = xmlDoc.OuterXml;
        }
        catch
        {
			_logger.LogError("Exception in AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value}): {Arg3}", attributesXml, attribute, value, ex.Message);
            //ignore
        }

		_logger.LogWarning("<<< Exiting AddAttribute(attributesXml={AttributesXml},attribute={Attribute},value={Value})", attributesXml, attribute, value);
        return result;
    }

    /// <summary>
    /// Validates attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> GetAttributeWarningsAsync(string attributesXml)
    {
		_logger.LogError(">>> Entering GetAttributeWarningsAsync(attributesXml={AttributesXml})", attributesXml);
        var warnings = new List<string>();

		_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): warnings → {Warnings}", attributesXml, warnings);
        //ensure it's our attributes
        var attributes1 = await ParseAttributesAsync(attributesXml);
		_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): attributes1 → {Attributes1}", attributesXml, attributes1);

        //validate required attributes (whether they're chosen/selected/entered)
        var attributes2 = await _attributeService.GetAllAttributesAsync();
		_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): attributes2 → {Attributes2}", attributesXml, attributes2);

        foreach (var a2 in attributes2)
        {
            if (!a2.IsRequired)
                continue;

            var found = false;
			_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): found → {Found}", attributesXml, found);
            //selected attributes
            foreach (var a1 in attributes1)
            {
                if (a1.Id != a2.Id)
                    continue;

                var valuesStr = ParseValues(attributesXml, a1.Id);
				_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): valuesStr → {ValuesStr}", attributesXml, valuesStr);

                found = valuesStr.Any(str1 => !string.IsNullOrEmpty(str1.Trim()));
            }

            if (found)
                continue;

            //if not found
            var notFoundWarning = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(a2, a => a.Name));
			_logger.LogError("GetAttributeWarningsAsync(attributesXml={AttributesXml}): notFoundWarning → {NotFoundWarning}", attributesXml, notFoundWarning);

            warnings.Add(notFoundWarning);
        }

		_logger.LogWarning("<<< Exiting GetAttributeWarningsAsync(attributesXml={AttributesXml})", attributesXml);
        return warnings;
    }

    /// <summary>
    /// Get custom attributes from the passed form
    /// </summary>
    /// <param name="form">Form values</param>
    /// <param name="attributeControlName">Name of the attribute control</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    public virtual async Task<string> ParseCustomAttributesAsync(IFormCollection form, string attributeControlName)
    {
		_logger.LogError(">>> Entering ParseCustomAttributesAsync(form={Form},attributeControlName={AttributeControlName})", form, attributeControlName);
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;
		_logger.LogError("ParseCustomAttributesAsync(form={Form},attributeControlName={AttributeControlName}): attributesXml → {AttributesXml}", form, attributeControlName, attributesXml);

        foreach (var attribute in await _attributeService.GetAllAttributesAsync())
        {
            var controlId = string.Format(attributeControlName, attribute.Id);
			_logger.LogError("ParseCustomAttributesAsync(form={Form},attributeControlName={AttributeControlName}): controlId → {ControlId}", form, attributeControlName, controlId);
            var attributeValues = form[controlId];
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                        attributesXml = AddAttribute(attributesXml, attribute, value.ToString());
                    break;

                case AttributeControlType.Checkboxes:
                    foreach (var attributeValue in attributeValues.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!int.TryParse(attributeValue, out value) || value <= 0)
                            continue;

                        attributesXml = AddAttribute(attributesXml, attribute, value.ToString());
                    }

                    break;

                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var readOnlyAttributeValues = await _attributeService.GetAttributeValuesAsync(attribute.Id);
					_logger.LogError("ParseCustomAttributesAsync(form={Form},attributeControlName={AttributeControlName}): readOnlyAttributeValues → {ReadOnlyAttributeValues}", form, attributeControlName, readOnlyAttributeValues);
                    foreach (var readOnlyAttributeValue in readOnlyAttributeValues)
                    {
                        if (!readOnlyAttributeValue.IsPreSelected)
                            continue;

                        attributesXml = AddAttribute(attributesXml, attribute, readOnlyAttributeValue.Id.ToString());
                    }

                    break;

                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    if (!StringValues.IsNullOrEmpty(attributeValues))
                        attributesXml = AddAttribute(attributesXml, attribute, attributeValues.ToString().Trim());
                    break;

                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                default:
                    break;
            }
        }

		_logger.LogWarning("<<< Exiting ParseCustomAttributesAsync(form={Form},attributeControlName={AttributeControlName})", form, attributeControlName);
        return attributesXml;
    }

    /// <summary>
    /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
    /// </summary>
    /// <param name="conditionAttributeXml">Condition attributes (XML format)</param>
    /// <param name="selectedAttributesXml">Selected attributes (XML format)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool?> IsConditionMetAsync(string conditionAttributeXml, string selectedAttributesXml)
    {
		_logger.LogError(">>> Entering IsConditionMetAsync(conditionAttributeXml={ConditionAttributeXml},selectedAttributesXml={SelectedAttributesXml})", conditionAttributeXml, selectedAttributesXml);
        if (string.IsNullOrEmpty(conditionAttributeXml))
            //no condition
            return null;
        
        //load an attribute this one depends on
        var dependOnAttribute = (await ParseAttributesAsync(conditionAttributeXml)).FirstOrDefault();
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

		_logger.LogWarning("<<< Exiting IsConditionMetAsync(conditionAttributeXml={ConditionAttributeXml},selectedAttributesXml={SelectedAttributesXml})", conditionAttributeXml, selectedAttributesXml);
        return allFound;
    }

    #endregion
}