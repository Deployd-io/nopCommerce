using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

[TestFixture]
public class ProductAttributeParserTests : ServiceTest
{
    private IProductAttributeParser _productAttributeParser;
    private IProductAttributeFormatter _productAttributeFormatter;
    private IEnumerable<KeyValuePair<ProductAttributeMapping, IList<ProductAttributeValue>>> _productAttributeMappings;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        var productAttributeService = GetService<IProductAttributeService>();

		logger.LogDebug("SetUp(): productAttributeService → {ProductAttributeService}", productAttributeService);
		var __logStart = System.Diagnostics.Stopwatch.StartNew();
        _productAttributeParser = GetService<IProductAttributeParser>();
		logger.LogDebug("SetUp(): _productAttributeParser → {_productAttributeParser}", _productAttributeParser);
        _productAttributeFormatter = GetService<IProductAttributeFormatter>();

		logger.LogDebug("SetUp(): _productAttributeFormatter → {_productAttributeFormatter}", _productAttributeFormatter);
        var product = await GetService<IProductService>()
            .GetProductBySkuAsync("COMP_CUST");
        var mappings = await productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
        _productAttributeMappings = await mappings.SelectAwait(async p => KeyValuePair.Create(p, await productAttributeService.GetProductAttributeValuesAsync(p.Id))).ToListAsync();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): mappings → {Mappings}", mappings);
		logger.LogDebug("SetUp(): _productAttributeMappings → {_productAttributeMappings}", _productAttributeMappings);
		logger.LogInformation("SetUp(): find query executed in {Arg0} ms", __logStart.ElapsedMilliseconds);
    }

    [Test]
    public async Task CanAddAndParseProductAttributes()
    {
		logger.LogDebug(">>> Entering CanAddAndParseProductAttributes()");
        var attributes = string.Empty;

		logger.LogDebug("CanAddAndParseProductAttributes(): attributes → {Attributes}", attributes);
        foreach (var productAttributeMapping in _productAttributeMappings)
        {
            var skip = true;

			logger.LogDebug("CanAddAndParseProductAttributes(): skip → {Skip}", skip);
            foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
            {
                if (skip)
                {
                    skip = false;
                    continue;
                }

                attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());
            }
        }

        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributes);

		logger.LogDebug("CanAddAndParseProductAttributes(): attributeValues → {AttributeValues}", attributeValues);
        var parsedAttributeValues = attributeValues.Select(p => p.Id).ToList();

		logger.LogDebug("CanAddAndParseProductAttributes(): parsedAttributeValues → {ParsedAttributeValues}", parsedAttributeValues);
        foreach (var productAttributeMapping in _productAttributeMappings)
        {
            var skip = true;

            foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
            {
                if (skip)
                {
                    parsedAttributeValues.Contains(productAttributeValue.Id).Should().BeFalse();
                    skip = false;
                    continue;
                }

                parsedAttributeValues.Contains(productAttributeValue.Id).Should().BeTrue();
            }
        }
		logger.LogDebug("<<< Exiting CanAddAndParseProductAttributes()");
    }

    [Test]
    public async Task CanAddAndRemoveProductAttributes()
    {
		logger.LogDebug(">>> Entering CanAddAndRemoveProductAttributes()");
        var attributes = string.Empty;

		logger.LogDebug("CanAddAndRemoveProductAttributes(): attributes → {Attributes}", attributes);
        var delete = false;

		logger.LogDebug("CanAddAndRemoveProductAttributes(): delete → {Delete}", delete);
        foreach (var productAttributeMapping in _productAttributeMappings)
        {
            foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
                attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());

            if (delete)
                attributes = _productAttributeParser.RemoveProductAttribute(attributes, productAttributeMapping.Key);

            delete = !delete;
        }

        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributes);

		logger.LogDebug("CanAddAndRemoveProductAttributes(): attributeValues → {AttributeValues}", attributeValues);
        var parsedAttributeValues = attributeValues.Select(p => p.Id).ToList();

		logger.LogDebug("CanAddAndRemoveProductAttributes(): parsedAttributeValues → {ParsedAttributeValues}", parsedAttributeValues);
        delete = false;

        foreach (var productAttributeMapping in _productAttributeMappings)
        {
            foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
                parsedAttributeValues.Contains(productAttributeValue.Id).Should().Be(!delete);

            delete = !delete;
        }
		logger.LogDebug("<<< Exiting CanAddAndRemoveProductAttributes()");
    }

    [Test]
    public async Task CanRenderAttributesWithoutPrices()
    {
		logger.LogDebug(">>> Entering CanRenderAttributesWithoutPrices()");
        var attributes = string.Empty;

		logger.LogDebug("CanRenderAttributesWithoutPrices(): attributes → {Attributes}", attributes);
        foreach (var productAttributeMapping in _productAttributeMappings)
        foreach (var productAttributeValue in productAttributeMapping.Value.OrderBy(p => p.Id))
            attributes = _productAttributeParser.AddProductAttribute(attributes, productAttributeMapping.Key, productAttributeValue.Id.ToString());

        attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
            "recipientName 1", "recipientEmail@gmail.com",
            "senderName 1", "senderEmail@gmail.com", "custom message");

        var product = new Product { IsGiftCard = true, GiftCardType = GiftCardType.Virtual };
        var customer = new Customer();
        var store = new Store();
		logger.LogDebug("CanRenderAttributesWithoutPrices(): customer → {Customer}", customer);
		logger.LogDebug("CanRenderAttributesWithoutPrices(): store → {Store}", store);

        var formattedAttributes = await _productAttributeFormatter.FormatAttributesAsync(product,
            attributes, customer, store, "<br />", false);

        formattedAttributes.Should().Be(
            "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200 [+$15.00]<br />RAM: 2 GB<br />RAM: 4GB [+$20.00]<br />RAM: 8GB [+$60.00]<br />HDD: 320 GB<br />HDD: 400 GB [+$100.00]<br />OS: Vista Home [+$50.00]<br />OS: Vista Premium [+$60.00]<br />Software: Microsoft Office [+$50.00]<br />Software: Acrobat Reader [+$10.00]<br />Software: Total Commander [+$5.00]<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");

        formattedAttributes = await _productAttributeFormatter.FormatAttributesAsync(product,
            attributes, customer, store, "<br />", false, false);

        formattedAttributes.Should().Be(
            "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200<br />RAM: 2 GB<br />RAM: 4GB<br />RAM: 8GB<br />HDD: 320 GB<br />HDD: 400 GB<br />OS: Vista Home<br />OS: Vista Premium<br />Software: Microsoft Office<br />Software: Acrobat Reader<br />Software: Total Commander<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
			logger.LogDebug("<<< Exiting CanRenderAttributesWithoutPrices()");
    }

    [Test]
    public void CanAddAndParseGiftCardAttributes()
    {
		logger.LogDebug(">>> Entering CanAddAndParseGiftCardAttributes()");
        var attributes = string.Empty;
        attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
            "recipientName 1", "recipientEmail@gmail.com",
            "senderName 1", "senderEmail@gmail.com", "custom message");

        _productAttributeParser.GetGiftCardAttribute(attributes,
            out var recipientName,
            out var recipientEmail,
            out var senderName,
            out var senderEmail,
            out var giftCardMessage);
        recipientName.Should().Be("recipientName 1");
        recipientEmail.Should().Be("recipientEmail@gmail.com");
        senderName.Should().Be("senderName 1");
        senderEmail.Should().Be("senderEmail@gmail.com");
        giftCardMessage.Should().Be("custom message");
		logger.LogDebug("<<< Exiting CanAddAndParseGiftCardAttributes()");
    }

    [Test]
    public async Task CanRenderVirtualGiftCart()
    {
		logger.LogDebug(">>> Entering CanRenderVirtualGiftCart()");
        var attributes = _productAttributeParser.AddGiftCardAttribute(string.Empty,
            "recipientName 1", "recipientEmail@gmail.com",
            "senderName 1", "senderEmail@gmail.com", "custom message");

        var product = new Product
        {
            IsGiftCard = true,
            GiftCardType = GiftCardType.Virtual
        };
        var customer = new Customer();
        var store = new Store();
		logger.LogDebug("CanRenderVirtualGiftCart(): customer → {Customer}", customer);
		logger.LogDebug("CanRenderVirtualGiftCart(): store → {Store}", store);

        var formattedAttributes = await _productAttributeFormatter.FormatAttributesAsync(product,
            attributes, customer, store, "<br />", false, false);
        formattedAttributes.Should().Be("From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
		logger.LogDebug("<<< Exiting CanRenderVirtualGiftCart()");
    }

    [Test]
    public async Task CanRenderPhysicalGiftCart()
    {
		logger.LogDebug(">>> Entering CanRenderPhysicalGiftCart()");
        var attributes = _productAttributeParser.AddGiftCardAttribute(string.Empty,
            "recipientName 1", "recipientEmail@gmail.com",
            "senderName 1", "senderEmail@gmail.com", "custom message");

        var product = new Product
        {
            IsGiftCard = true,
            GiftCardType = GiftCardType.Physical
        };
        var customer = new Customer();
        var store = new Store();
		logger.LogDebug("CanRenderPhysicalGiftCart(): customer → {Customer}", customer);
		logger.LogDebug("CanRenderPhysicalGiftCart(): store → {Store}", store);

        var formattedAttributes = await _productAttributeFormatter.FormatAttributesAsync(product,
            attributes, customer, store, "<br />", false, false);
        formattedAttributes.Should().Be("From: senderName 1<br />For: recipientName 1");
		logger.LogDebug("<<< Exiting CanRenderPhysicalGiftCart()");
    }
}
