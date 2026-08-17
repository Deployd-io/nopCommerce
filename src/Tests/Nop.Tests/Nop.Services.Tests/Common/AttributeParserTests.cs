using System.Xml;
using FluentAssertions;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using NUnit.Framework;
using Microsoft.Extensions.Logging;


namespace Nop.Tests.Nop.Services.Tests.Common;

[TestFixture]
public class AttributeParserTests : BaseNopTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private string _attributesXml;
    private XmlDocument _xmlDoc;
    protected dynamic _parser;
    private IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    private IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;
    private IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    private IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;

    private List<int> _customerAttributeIds = new();
    private List<int> _customerAttributeValuesIds = new();

    private List<int> _vendorAttributeIds = new();
    private List<int> _vendorAttributeValuesIds = new();

    private List<int> _addressAttributeIds = new();
    private List<int> _addressAttributeValuesIds = new();

    private List<int> _checkoutAttributeIds = new();
    private List<int> _checkoutAttributeValuesIds = new();

    protected void PrepareTestData(Type attributeType)
    {
		_logger.LogDebug(">>> Entering PrepareTestData(attributeType={AttributeType})", attributeType);
        _attributesXml = $"<Attributes><{attributeType.Name} ID=\"1\"><{attributeType.Name}Value><Value>1</Value></{attributeType.Name}Value></{attributeType.Name}><{attributeType.Name} ID=\"2\"><{attributeType.Name}Value><Value>2</Value></{attributeType.Name}Value></{attributeType.Name}></Attributes>";

        if (attributeType == typeof(CustomerAttribute))
            _parser = GetService<IAttributeParser<CustomerAttribute, CustomerAttributeValue>>();
			_logger.LogDebug("PrepareTestData(attributeType={AttributeType}): _parser → {_parser}", attributeType, _parser);

        if (attributeType == typeof(VendorAttribute))
            _parser = GetService<IAttributeParser<VendorAttribute, VendorAttributeValue>>();

        if (attributeType == typeof(AddressAttribute))
            _parser = GetService<IAttributeParser<AddressAttribute, AddressAttributeValue>>();

        if (attributeType == typeof(CheckoutAttribute))
            _parser = GetService<IAttributeParser<CheckoutAttribute, CheckoutAttributeValue>>();
			_logger.LogDebug("<<< Exiting PrepareTestData(attributeType={AttributeType})", attributeType);
    }


    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _xmlDoc = new XmlDocument();

		_logger.LogDebug("SetUp(): _xmlDoc → {_xmlDoc}", _xmlDoc);
        _customerAttributeService = GetService<IAttributeService<CustomerAttribute, CustomerAttributeValue>>();
		_logger.LogDebug("SetUp(): _customerAttributeService → {_customerAttributeService}", _customerAttributeService);
        _vendorAttributeService = GetService<IAttributeService<VendorAttribute, VendorAttributeValue>>();
		_logger.LogDebug("SetUp(): _vendorAttributeService → {_vendorAttributeService}", _vendorAttributeService);
        _addressAttributeService = GetService<IAttributeService<AddressAttribute, AddressAttributeValue>>();
		_logger.LogDebug("SetUp(): _addressAttributeService → {_addressAttributeService}", _addressAttributeService);
        _checkoutAttributeService = GetService<IAttributeService<CheckoutAttribute, CheckoutAttributeValue>>();
		_logger.LogDebug("SetUp(): _checkoutAttributeService → {_checkoutAttributeService}", _checkoutAttributeService);

        for (var i = 0; i < 2; i++)
        {
            BaseAttribute attribute = new CustomerAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test"
            };
            await _customerAttributeService.InsertAttributeAsync((CustomerAttribute)attribute);
            _customerAttributeIds.Add(attribute.Id);

            BaseAttributeValue attributeValue = new CustomerAttributeValue { AttributeId = attribute.Id, Name = "test" };
			_logger.LogDebug("SetUp(): attributeValue → {AttributeValue}", attributeValue);
            await _customerAttributeService.InsertAttributeValueAsync((CustomerAttributeValue)attributeValue);
            _customerAttributeValuesIds.Add(attributeValue.Id);

            attributeValue = new CustomerAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _customerAttributeService.InsertAttributeValueAsync((CustomerAttributeValue)attributeValue);
            _customerAttributeValuesIds.Add(attributeValue.Id);

            attribute = new CustomerAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test",
                IsRequired = true
            };
            await _customerAttributeService.InsertAttributeAsync((CustomerAttribute)attribute);
            _customerAttributeIds.Add(attribute.Id);

            attribute = new VendorAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test"
            };
            await _vendorAttributeService.InsertAttributeAsync((VendorAttribute)attribute);
            _vendorAttributeIds.Add(attribute.Id);

            attributeValue = new VendorAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _vendorAttributeService.InsertAttributeValueAsync((VendorAttributeValue)attributeValue);
            _vendorAttributeValuesIds.Add(attributeValue.Id);

            attributeValue = new VendorAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _vendorAttributeService.InsertAttributeValueAsync((VendorAttributeValue)attributeValue);
            _vendorAttributeValuesIds.Add(attributeValue.Id);

            attribute = new VendorAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test",
                IsRequired = true
            };
            await _vendorAttributeService.InsertAttributeAsync((VendorAttribute)attribute);
            _vendorAttributeIds.Add(attribute.Id);

            attribute = new AddressAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test"
            };
            await _addressAttributeService.InsertAttributeAsync((AddressAttribute)attribute);
            _addressAttributeIds.Add(attribute.Id);

            attributeValue = new AddressAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _addressAttributeService.InsertAttributeValueAsync((AddressAttributeValue)attributeValue);
            _addressAttributeValuesIds.Add(attributeValue.Id);

            attributeValue = new AddressAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _addressAttributeService.InsertAttributeValueAsync((AddressAttributeValue)attributeValue);
            _addressAttributeValuesIds.Add(attributeValue.Id);

            attribute = new AddressAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test",
                IsRequired = true
            };
            await _addressAttributeService.InsertAttributeAsync((AddressAttribute)attribute);
            _addressAttributeIds.Add(attribute.Id);

            attribute = new CheckoutAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test"
            };
            await _checkoutAttributeService.InsertAttributeAsync((CheckoutAttribute)attribute);
            _checkoutAttributeIds.Add(attribute.Id);

            attributeValue = new CheckoutAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _checkoutAttributeService.InsertAttributeValueAsync((CheckoutAttributeValue)attributeValue);
            _checkoutAttributeValuesIds.Add(attributeValue.Id);

            attributeValue = new CheckoutAttributeValue { AttributeId = attribute.Id, Name = "test" };
            await _checkoutAttributeService.InsertAttributeValueAsync((CheckoutAttributeValue)attributeValue);
            _checkoutAttributeValuesIds.Add(attributeValue.Id);

            attribute = new CheckoutAttribute
            {
                AttributeControlType = AttributeControlType.DropdownList,
                Name = "test",
                IsRequired = true
            };
            await _checkoutAttributeService.InsertAttributeAsync((CheckoutAttribute)attribute);
            _checkoutAttributeIds.Add(attribute.Id);
        }
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        foreach (var id in _customerAttributeValuesIds)
            await _customerAttributeService.DeleteAttributeValueAsync(new CustomerAttributeValue { Id = id });

        foreach (var id in _vendorAttributeValuesIds)
            await _vendorAttributeService.DeleteAttributeValueAsync(new VendorAttributeValue { Id = id });

        foreach (var id in _addressAttributeValuesIds)
            await _addressAttributeService.DeleteAttributeValueAsync(new AddressAttributeValue { Id = id });

        foreach (var id in _checkoutAttributeValuesIds)
            await _checkoutAttributeService.DeleteAttributeValueAsync(new CheckoutAttributeValue { Id = id });


        foreach (var id in _customerAttributeIds)
            await _customerAttributeService.DeleteAttributeAsync(new CustomerAttribute { Id = id });

        foreach (var id in _vendorAttributeIds)
            await _vendorAttributeService.DeleteAttributeAsync(new VendorAttribute { Id = id });

        foreach (var id in _addressAttributeIds)
            await _addressAttributeService.DeleteAttributeAsync(new AddressAttribute { Id = id });

        foreach (var id in _checkoutAttributeIds)
            await _checkoutAttributeService.DeleteAttributeAsync(new CheckoutAttribute { Id = id });
			_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]

    public void CanRemoveAttribute(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanRemoveAttribute(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);

        var xml = _parser.RemoveAttribute(_attributesXml, 1) as string;
		_logger.LogDebug("CanRemoveAttribute(attributeType={AttributeType}): xml → {Xml}", attributeType, xml);
        _xmlDoc.LoadXml(xml);
        var childNodes = _xmlDoc.SelectNodes($@"//Attributes/{attributeType.Name}");
		_logger.LogDebug("CanRemoveAttribute(attributeType={AttributeType}): childNodes → {ChildNodes}", attributeType, childNodes);

        childNodes?.Count.Should().Be(1);
		_logger.LogDebug("<<< Exiting CanRemoveAttribute(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]

    public async Task CanAddAttribute(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanAddAttribute(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);

        var attribute = (await _parser.ParseAttributesAsync(_attributesXml))[1];
		_logger.LogDebug("CanAddAttribute(attributeType={AttributeType}): attribute → {Attribute}", attributeType, attribute);
        (attribute as BaseAttribute).Should().NotBeNull();

        var values = await _parser.ParseAttributeValuesAsync(_attributesXml);
		_logger.LogDebug("CanAddAttribute(attributeType={AttributeType}): values → {Values}", attributeType, values);
        ((int)values.Count).Should().Be(2);

        var xml = _parser.AddAttribute(_attributesXml, attribute, "3") as string;
		_logger.LogDebug("CanAddAttribute(attributeType={AttributeType}): xml → {Xml}", attributeType, xml);

        values = await _parser.ParseAttributeValuesAsync(xml);
        ((int)values.Count).Should().Be(3);
		_logger.LogDebug("<<< Exiting CanAddAttribute(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void RemoveAttributeShouldReturnEmptyStringWhenAllChildrenIsRemoved(Type attributeType)
    {
		_logger.LogDebug(">>> Entering RemoveAttributeShouldReturnEmptyStringWhenAllChildrenIsRemoved(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);
        var xml = _parser.RemoveAttribute(_attributesXml, 1) as string;
		_logger.LogDebug("RemoveAttributeShouldReturnEmptyStringWhenAllChildrenIsRemoved(attributeType={AttributeType}): xml → {Xml}", attributeType, xml);
        xml = _parser.RemoveAttribute(xml, 2);

        xml.Should().BeEmpty();
		_logger.LogDebug("<<< Exiting RemoveAttributeShouldReturnEmptyStringWhenAllChildrenIsRemoved(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void RemoveAttributeShouldNotChangeXmlIfAttributeValueIdNotExists(Type attributeType)
    {
		_logger.LogDebug(">>> Entering RemoveAttributeShouldNotChangeXmlIfAttributeValueIdNotExists(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);
        var xml = _parser.RemoveAttribute(_attributesXml, 3) as string;
		_logger.LogDebug("RemoveAttributeShouldNotChangeXmlIfAttributeValueIdNotExists(attributeType={AttributeType}): xml → {Xml}", attributeType, xml);

        xml?.Equals(_attributesXml).Should().BeTrue();
		_logger.LogDebug("<<< Exiting RemoveAttributeShouldNotChangeXmlIfAttributeValueIdNotExists(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void CanParseAttributeIds(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanParseAttributeIds(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);
        var ids = _parser.ParseAttributeIds(_attributesXml) as IList<int>;
		_logger.LogDebug("CanParseAttributeIds(attributeType={AttributeType}): ids → {Ids}", attributeType, ids);

        var existsId = new List<int> { 1, 2 };

        ids?.Count.Should().Be(2);
        ids?.All(existsId.Contains).Should().BeTrue();
		_logger.LogDebug("<<< Exiting CanParseAttributeIds(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void CanParseAttributeIdsShouldReturnEmptyListIfXmlIsEmpty(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanParseAttributeIdsShouldReturnEmptyListIfXmlIsEmpty(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);
        var ids = _parser.ParseAttributeIds(string.Empty) as IList<int>;
		_logger.LogDebug("CanParseAttributeIdsShouldReturnEmptyListIfXmlIsEmpty(attributeType={AttributeType}): ids → {Ids}", attributeType, ids);

        ids?.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanParseAttributeIdsShouldReturnEmptyListIfXmlIsEmpty(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void CanParseAttributeIdsShouldReturnEmptyListIfXmlNotContainsRightAttributes(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanParseAttributeIdsShouldReturnEmptyListIfXmlNotContainsRightAttributes(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);
        var ids = _parser.ParseAttributeIds("<Attributes><Item ID=\"1\">Test 1</Item><Item ID=\"2\">Test 2</Item></Attributes>") as IList<int>;
		_logger.LogDebug("CanParseAttributeIdsShouldReturnEmptyListIfXmlNotContainsRightAttributes(attributeType={AttributeType}): ids → {Ids}", attributeType, ids);
		_logger.LogDebug("CanParseAttributeIdsShouldReturnEmptyListIfXmlNotContainsRightAttributes(attributeType={AttributeType}): ID → {ID}", attributeType, ID);

        ids?.Count.Should().Be(0);

        ids = _parser.ParseAttributeIds("<Items><Attribute ID=\"1\">Test 1</Attribute><Attribute ID=\"2\">Test 2</Attribute></Items>") as IList<int>;

        ids?.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanParseAttributeIdsShouldReturnEmptyListIfXmlNotContainsRightAttributes(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]
    public void CanParseAttributeValues(Type attributeType)
    {
		_logger.LogDebug(">>> Entering CanParseAttributeValues(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);

        var values = _parser.ParseAttributeValuesAsync(_attributesXml).Result?.Count as int?;
		_logger.LogDebug("CanParseAttributeValues(attributeType={AttributeType}): values → {Values}", attributeType, values);
        values.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanParseAttributeValues(attributeType={AttributeType})", attributeType);
    }

    [Test]
    [TestCase(typeof(CustomerAttribute))]
    [TestCase(typeof(VendorAttribute))]
    [TestCase(typeof(AddressAttribute))]
    [TestCase(typeof(CheckoutAttribute))]

    public async Task GetAttributeWarnings(Type attributeType)
    {
		_logger.LogDebug(">>> Entering GetAttributeWarnings(attributeType={AttributeType})", attributeType);
        PrepareTestData(attributeType);

        var attribute = (await _parser.ParseAttributesAsync(_attributesXml))[1];
		_logger.LogDebug("GetAttributeWarnings(attributeType={AttributeType}): attribute → {Attribute}", attributeType, attribute);
        (attribute as BaseAttribute).Should().NotBeNull();

        var values = await _parser.GetAttributeWarningsAsync(_attributesXml);
		_logger.LogDebug("GetAttributeWarnings(attributeType={AttributeType}): values → {Values}", attributeType, values);
        ((int)values.Count).Should().BeGreaterOrEqualTo(1);
		_logger.LogDebug("<<< Exiting GetAttributeWarnings(attributeType={AttributeType})", attributeType);
    }
}