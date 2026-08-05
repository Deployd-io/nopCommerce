using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Orders;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class CheckoutAttributeParserAndFormatterTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    private IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    private ICheckoutAttributeFormatter _checkoutAttributeFormatter;

    private CheckoutAttribute _ca1, _ca2, _ca3;
    private CheckoutAttributeValue _cav11, _cav12, _cav21, _cav22;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _checkoutAttributeParser = GetService<IAttributeParser<CheckoutAttribute, CheckoutAttributeValue>>();
        _checkoutAttributeService = GetService<IAttributeService<CheckoutAttribute, CheckoutAttributeValue>>();
		_logger.LogDebug("SetUp(): _checkoutAttributeParser → {_checkoutAttributeParser}", _checkoutAttributeParser);
		_logger.LogDebug("SetUp(): _checkoutAttributeService → {_checkoutAttributeService}", _checkoutAttributeService);
        _checkoutAttributeFormatter = GetService<ICheckoutAttributeFormatter>();

		_logger.LogDebug("SetUp(): _checkoutAttributeFormatter → {_checkoutAttributeFormatter}", _checkoutAttributeFormatter);
        //color (dropdownlist)
        _ca1 = new CheckoutAttribute
		_logger.LogDebug("SetUp(): _ca1 → {_ca1}", _ca1);
        {
            Name = "Color",
            TextPrompt = "Select color:",
            IsRequired = true,
            AttributeControlType = AttributeControlType.DropdownList,
            DisplayOrder = 1
        };

        await _checkoutAttributeService.InsertAttributeAsync(_ca1);

        _cav11 = new CheckoutAttributeValue
		_logger.LogDebug("SetUp(): _cav11 → {_cav11}", _cav11);
        {
            Name = "Green",
            DisplayOrder = 1,
            AttributeId = _ca1.Id
        };
        _cav12 = new CheckoutAttributeValue
		_logger.LogDebug("SetUp(): _cav12 → {_cav12}", _cav12);
        {
            Name = "Red",
            DisplayOrder = 2,
            AttributeId = _ca1.Id
        };

        await _checkoutAttributeService.InsertAttributeValueAsync(_cav11);
        await _checkoutAttributeService.InsertAttributeValueAsync(_cav12);

        //custom option (checkboxes)
        _ca2 = new CheckoutAttribute
		_logger.LogDebug("SetUp(): _ca2 → {_ca2}", _ca2);
        {
            Name = "Custom option",
            TextPrompt = "Select custom option:",
            IsRequired = true,
            AttributeControlType = AttributeControlType.Checkboxes,
            DisplayOrder = 2
        };

        await _checkoutAttributeService.InsertAttributeAsync(_ca2);

        _cav21 = new CheckoutAttributeValue
		_logger.LogDebug("SetUp(): _cav21 → {_cav21}", _cav21);
        {
            Name = "Option 1",
            DisplayOrder = 1,
            AttributeId = _ca2.Id
        };
        _cav22 = new CheckoutAttributeValue
		_logger.LogDebug("SetUp(): _cav22 → {_cav22}", _cav22);
        {
            Name = "Option 2",
            DisplayOrder = 2,
            AttributeId = _ca2.Id
        };

        await _checkoutAttributeService.InsertAttributeValueAsync(_cav21);
        await _checkoutAttributeService.InsertAttributeValueAsync(_cav22);

        //custom text
        _ca3 = new CheckoutAttribute
		_logger.LogDebug("SetUp(): _ca3 → {_ca3}", _ca3);
        {
            Name = "Custom text",
            TextPrompt = "Enter custom text:",
            IsRequired = true,
            AttributeControlType = AttributeControlType.MultilineTextbox,
            DisplayOrder = 3
        };

        await _checkoutAttributeService.InsertAttributeAsync(_ca3);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _checkoutAttributeService.DeleteAttributeValueAsync(_cav11);
        await _checkoutAttributeService.DeleteAttributeValueAsync(_cav12);
        await _checkoutAttributeService.DeleteAttributeAsync(_ca1);

        await _checkoutAttributeService.DeleteAttributeValueAsync(_cav21);
        await _checkoutAttributeService.DeleteAttributeValueAsync(_cav22);
        await _checkoutAttributeService.DeleteAttributeAsync(_ca2);

        await _checkoutAttributeService.DeleteAttributeAsync(_ca3);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanAddAndParseCheckoutAttributes()
    {
		_logger.LogDebug(">>> Entering CanAddAndParseCheckoutAttributes()");
        var attributes = string.Empty;
        //color: green
		_logger.LogDebug("CanAddAndParseCheckoutAttributes(): attributes → {Attributes}", attributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca1, _cav11.Id.ToString());
        //custom option: option 1, option 2
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav21.Id.ToString());
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav22.Id.ToString());
        //custom text
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca3, "Some custom text goes here");

        var parsedAttributeValues = await _checkoutAttributeParser.ParseAttributeValues(attributes).ToListAsync();
        var attributeValues = await parsedAttributeValues.SelectAwait(async x => await x.values.Select(p => p.Id).ToListAsync()).SelectMany(p => p.ToAsyncEnumerable()).ToListAsync();
		_logger.LogDebug("CanAddAndParseCheckoutAttributes(): parsedAttributeValues → {ParsedAttributeValues}", parsedAttributeValues);
		_logger.LogDebug("CanAddAndParseCheckoutAttributes(): attributeValues → {AttributeValues}", attributeValues);
		_logger.LogInformation("CanAddAndParseCheckoutAttributes(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        attributeValues.Contains(_cav11.Id).Should().BeTrue();
        attributeValues.Contains(_cav12.Id).Should().BeFalse();
        attributeValues.Contains(_cav21.Id).Should().BeTrue();
        attributeValues.Contains(_cav22.Id).Should().BeTrue();
        attributeValues.Contains(_cav22.Id).Should().BeTrue();

        var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
        parsedValues.Count.Should().Be(1);
		_logger.LogDebug("CanAddAndParseCheckoutAttributes(): parsedValues → {ParsedValues}", parsedValues);
        parsedValues.Contains("Some custom text goes here").Should().BeTrue();
        parsedValues.Contains("Some other custom text").Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanAddAndParseCheckoutAttributes()");
    }

    [Test]
    public async Task CanAddRenderAttributesWithoutPrices()
    {
		_logger.LogDebug(">>> Entering CanAddRenderAttributesWithoutPrices()");
        var attributes = string.Empty;
        //color: green
		_logger.LogDebug("CanAddRenderAttributesWithoutPrices(): attributes → {Attributes}", attributes);
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca1, _cav11.Id.ToString());
        //custom option: option 1, option 2
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav21.Id.ToString());
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav22.Id.ToString());
        //custom text
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca3, "Some custom text goes here");

        var customer = new Customer();

		_logger.LogDebug("CanAddRenderAttributesWithoutPrices(): customer → {Customer}", customer);
        var formattedAttributes =
            await _checkoutAttributeFormatter.FormatAttributesAsync(attributes, customer, "<br />", false, false);
        formattedAttributes.Should()
            .Be(
                "Color: Green<br />Custom option: Option 1<br />Custom option: Option 2<br />Custom text: Some custom text goes here");
				_logger.LogDebug("<<< Exiting CanAddRenderAttributesWithoutPrices()");
    }

    [Test]
    public async Task CanAddAndRemoveCheckoutAttributes()
    {
		_logger.LogDebug(">>> Entering CanAddAndRemoveCheckoutAttributes()");
        var attributes = string.Empty;
        //color: green
		_logger.LogDebug("CanAddAndRemoveCheckoutAttributes(): attributes → {Attributes}", attributes);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca1, _cav11.Id.ToString());
        //custom option: option 1, option 2
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav21.Id.ToString());
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca2, _cav22.Id.ToString());
        //custom text
        attributes = _checkoutAttributeParser.AddAttribute(attributes, _ca3, "Some custom text goes here");
        //delete some of them
        attributes = _checkoutAttributeParser.RemoveAttribute(attributes, _ca2.Id);
        attributes = _checkoutAttributeParser.RemoveAttribute(attributes, _ca3.Id);

        var parsedAttributeValues = await _checkoutAttributeParser.ParseAttributeValues(attributes).ToListAsync();
        var attributeValues = await parsedAttributeValues.SelectAwait(async x => await x.values.Select(p => p.Id).ToListAsync()).SelectMany(p => p.ToAsyncEnumerable()).ToListAsync();
		_logger.LogDebug("CanAddAndRemoveCheckoutAttributes(): parsedAttributeValues → {ParsedAttributeValues}", parsedAttributeValues);
		_logger.LogDebug("CanAddAndRemoveCheckoutAttributes(): attributeValues → {AttributeValues}", attributeValues);
		_logger.LogInformation("CanAddAndRemoveCheckoutAttributes(): database query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);

        attributeValues.Contains(_cav11.Id).Should().BeTrue();
        attributeValues.Contains(_cav12.Id).Should().BeFalse();
        attributeValues.Contains(_cav21.Id).Should().BeFalse();
        attributeValues.Contains(_cav22.Id).Should().BeFalse();
        attributeValues.Contains(_cav22.Id).Should().BeFalse();

        var parsedValues = _checkoutAttributeParser.ParseValues(attributes, _ca3.Id);
        parsedValues.Count.Should().Be(0);
		_logger.LogDebug("<<< Exiting CanAddAndRemoveCheckoutAttributes()");
		_logger.LogDebug("CanAddAndRemoveCheckoutAttributes(): parsedValues → {ParsedValues}", parsedValues);
    }
}