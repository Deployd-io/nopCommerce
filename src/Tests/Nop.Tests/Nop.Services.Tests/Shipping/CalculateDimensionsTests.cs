using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class CalculateDimensionsTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IProductService _productService;
    private IShippingService _shippingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _productService = GetService<IProductService>();
        _shippingService = GetService<IShippingService>();
		_logger.LogDebug("SetUp(): _shippingService → {_shippingService}", _shippingService);
		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [Test]
    public async Task ShouldReturnZeroWithAllZeroDimensions()
    {
		_logger.LogDebug(">>> Entering ShouldReturnZeroWithAllZeroDimensions()");
        var product = await _productService.GetProductBySkuAsync("VG_CR_025");
        product.Length = 0;
		_logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Length → {Arg0}", product.Length);
		_logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Width → {Arg0}", product.Width);
		_logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Height → {Arg0}", product.Height);
		_logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product → {Product}", product);
        product.Width = 0;
        product.Height = 0;

        var items = new List<GetShippingOptionRequest.PackageItem>
        {
            new(new ShoppingCartItem
            {
                Quantity = 1,
                ProductId = product.Id
            }, product)
        };

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        length.Should().Be(0);
        width.Should().Be(0);
        height.Should().Be(0);

        items =
        [
            new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
        {
            Quantity = 2,
            ProductId = product.Id
        }, product)
            ];

        (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        length.Should().Be(0);
        width.Should().Be(0);
        height.Should().Be(0);
		_logger.LogDebug("<<< Exiting ShouldReturnZeroWithAllZeroDimensions()");
    }

    [Test]
    public async Task CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()
    {
		_logger.LogDebug(">>> Entering CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 3;
		_logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Length → {Arg0}", product.Length);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Width → {Arg0}", product.Width);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Height → {Arg0}", product.Height);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product → {Product}", product);
        product.Width = 2;
        product.Height = 2;

        var items = new List<GetShippingOptionRequest.PackageItem>
        {
            new(new ShoppingCartItem
            {
                Quantity = 1,
                ProductId = product.Id
            }, product)
        };

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        length.Should().Be(3);
        width.Should().Be(2);
        height.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()");
    }

    [Test]
    public async Task CanCalculateWithSingleItemAndQty2()
    {
		_logger.LogDebug(">>> Entering CanCalculateWithSingleItemAndQty2()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 2;
		_logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Length → {Arg0}", product.Length);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Width → {Arg0}", product.Width);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Height → {Arg0}", product.Height);
		_logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product → {Product}", product);
        product.Width = 4;
        product.Height = 4;

        var items = new List<GetShippingOptionRequest.PackageItem>
        {
            new(new ShoppingCartItem
            {
                Quantity = 2,
                ProductId = product.Id
            }, product)
        };

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        length.Should().Be(4);
        width.Should().Be(4);
        height.Should().Be(4);
		_logger.LogDebug("<<< Exiting CanCalculateWithSingleItemAndQty2()");
    }

    [Test]
    public async Task CanCalculateWithCubicItemAndMultipleQty()
    {
		_logger.LogDebug(">>> Entering CanCalculateWithCubicItemAndMultipleQty()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 2;
		_logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Length → {Arg0}", product.Length);
		_logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Width → {Arg0}", product.Width);
		_logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Height → {Arg0}", product.Height);
		_logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product → {Product}", product);
        product.Width = 2;
        product.Height = 2;

        var items = new List<GetShippingOptionRequest.PackageItem>
        {
            new(new ShoppingCartItem
            {
                Quantity = 3,
                ProductId = product.Id
            }, product)
        };

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        Math.Round(length, 2).Should().Be(2.88M);
        Math.Round(width, 2).Should().Be(2.88M);
        Math.Round(height, 2).Should().Be(2.88M);
		_logger.LogDebug("<<< Exiting CanCalculateWithCubicItemAndMultipleQty()");
    }

    [Test]
    public async Task CanCalculateWithMultipleItems()
    {
		_logger.LogDebug(">>> Entering CanCalculateWithMultipleItems()");
        var product1 = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product1.Length = 2;
		_logger.LogDebug("CanCalculateWithMultipleItems(): product1.Length → {Arg0}", product1.Length);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product1.Width → {Arg0}", product1.Width);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product1.Height → {Arg0}", product1.Height);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product1 → {Product1}", product1);
        product1.Width = 2;
        product1.Height = 2;

        var product2 = await _productService.GetProductBySkuAsync("VG_CR_025");
		_logger.LogDebug("CanCalculateWithMultipleItems(): product2 → {Product2}", product2);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product2.Length → {Arg0}", product2.Length);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product2.Width → {Arg0}", product2.Width);
		_logger.LogDebug("CanCalculateWithMultipleItems(): product2.Height → {Arg0}", product2.Height);
        product2.Length = 3;
        product2.Width = 5;
        product2.Height = 2;

        await _productService.InsertProductAsync(product2);

        var items = new List<GetShippingOptionRequest.PackageItem>
        {
            new(new ShoppingCartItem
            {
                Quantity = 3,
                ProductId = product1.Id
            }, product1),
            new(new ShoppingCartItem
            {
                Quantity = 1,
                ProductId = product2.Id
            }, product2)
        };

        var (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        Math.Round(length, 2).Should().Be(3.78M);
        Math.Round(width, 2).Should().Be(5);    //preserve max width
        Math.Round(height, 2).Should().Be(3.78M);

        items.Clear();
        //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
        for (var i = 0; i < 8; i++)
        {
            var product = await _productService.GetProductByIdAsync(i + 1);
			_logger.LogDebug("CanCalculateWithMultipleItems(): product → {Product}", product);
			_logger.LogDebug("CanCalculateWithMultipleItems(): product.Length → {Arg0}", product.Length);
			_logger.LogDebug("CanCalculateWithMultipleItems(): product.Width → {Arg0}", product.Width);
			_logger.LogDebug("CanCalculateWithMultipleItems(): product.Height → {Arg0}", product.Height);
            product.Length = 1;
            product.Width = 1;
            product.Height = 1;

            items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
            {
                Quantity = 1,
                ProductId = product.Id
            }, product));
        }

        (width, length, height) = await _shippingService.GetDimensionsAsync(items);

        Math.Round(length, 2).Should().Be(2);
        Math.Round(width, 2).Should().Be(2);
        Math.Round(height, 2).Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateWithMultipleItems()");
    }
}