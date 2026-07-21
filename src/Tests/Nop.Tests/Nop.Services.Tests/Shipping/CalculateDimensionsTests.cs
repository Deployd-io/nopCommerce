using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class CalculateDimensionsTests : ServiceTest
{
    private IProductService _productService;
    private IShippingService _shippingService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _productService = GetService<IProductService>();
        _shippingService = GetService<IShippingService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
		logger.LogDebug("SetUp(): _shippingService → {_shippingService}", _shippingService);
    }

    [Test]
    public async Task ShouldReturnZeroWithAllZeroDimensions()
    {
		logger.LogDebug(">>> Entering ShouldReturnZeroWithAllZeroDimensions()");
        var product = await _productService.GetProductBySkuAsync("VG_CR_025");
        product.Length = 0;
		logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product → {Product}", product);
		logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Length → {Arg0}", product.Length);
		logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Width → {Arg0}", product.Width);
		logger.LogDebug("ShouldReturnZeroWithAllZeroDimensions(): product.Height → {Arg0}", product.Height);
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
		logger.LogDebug("<<< Exiting ShouldReturnZeroWithAllZeroDimensions()");
    }

    [Test]
    public async Task CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()
    {
		logger.LogDebug(">>> Entering CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 3;
		logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product → {Product}", product);
		logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Length → {Arg0}", product.Length);
		logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Width → {Arg0}", product.Width);
		logger.LogDebug("CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod(): product.Height → {Arg0}", product.Height);
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
		logger.LogDebug("<<< Exiting CanCalculateWithSingleItemAndQty1ShouldIgnoreCubicMethod()");
    }

    [Test]
    public async Task CanCalculateWithSingleItemAndQty2()
    {
		logger.LogDebug(">>> Entering CanCalculateWithSingleItemAndQty2()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 2;
		logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product → {Product}", product);
		logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Length → {Arg0}", product.Length);
		logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Width → {Arg0}", product.Width);
		logger.LogDebug("CanCalculateWithSingleItemAndQty2(): product.Height → {Arg0}", product.Height);
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
		logger.LogDebug("<<< Exiting CanCalculateWithSingleItemAndQty2()");
    }

    [Test]
    public async Task CanCalculateWithCubicItemAndMultipleQty()
    {
		logger.LogDebug(">>> Entering CanCalculateWithCubicItemAndMultipleQty()");
        var product = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product.Length = 2;
		logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product → {Product}", product);
		logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Length → {Arg0}", product.Length);
		logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Width → {Arg0}", product.Width);
		logger.LogDebug("CanCalculateWithCubicItemAndMultipleQty(): product.Height → {Arg0}", product.Height);
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
		logger.LogDebug("<<< Exiting CanCalculateWithCubicItemAndMultipleQty()");
    }

    [Test]
    public async Task CanCalculateWithMultipleItems()
    {
		logger.LogDebug(">>> Entering CanCalculateWithMultipleItems()");
        var product1 = await _productService.GetProductBySkuAsync("AP_MBP_13");
        product1.Length = 2;
		logger.LogDebug("CanCalculateWithMultipleItems(): product1 → {Product1}", product1);
		logger.LogDebug("CanCalculateWithMultipleItems(): product1.Length → {Arg0}", product1.Length);
		logger.LogDebug("CanCalculateWithMultipleItems(): product1.Width → {Arg0}", product1.Width);
		logger.LogDebug("CanCalculateWithMultipleItems(): product1.Height → {Arg0}", product1.Height);
        product1.Width = 2;
        product1.Height = 2;

        var product2 = await _productService.GetProductBySkuAsync("VG_CR_025");
        product2.Length = 3;
		logger.LogDebug("CanCalculateWithMultipleItems(): product2 → {Product2}", product2);
		logger.LogDebug("CanCalculateWithMultipleItems(): product2.Length → {Arg0}", product2.Length);
		logger.LogDebug("CanCalculateWithMultipleItems(): product2.Width → {Arg0}", product2.Width);
		logger.LogDebug("CanCalculateWithMultipleItems(): product2.Height → {Arg0}", product2.Height);
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
            product.Length = 1;
			logger.LogDebug("CanCalculateWithMultipleItems(): product → {Product}", product);
			logger.LogDebug("CanCalculateWithMultipleItems(): product.Length → {Arg0}", product.Length);
			logger.LogDebug("CanCalculateWithMultipleItems(): product.Width → {Arg0}", product.Width);
			logger.LogDebug("CanCalculateWithMultipleItems(): product.Height → {Arg0}", product.Height);
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
		logger.LogDebug("<<< Exiting CanCalculateWithMultipleItems()");
    }
}
