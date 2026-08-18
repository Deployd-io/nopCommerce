using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Shipping;

[TestFixture]
public class ShippingServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private IShippingService _shippingService;
    private IProductService _productService;

    #endregion

    #region Setup

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _shippingService = GetService<IShippingService>();
        _productService = GetService<IProductService>();
		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
		_logger.LogDebug("SetUp(): _shippingService → {_shippingService}", _shippingService);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    #endregion
    
    [Test]
    public async Task CanGetShoppingCartTotalWeightWithoutAttributes()
    {
		_logger.LogDebug(">>> Entering CanGetShoppingCartTotalWeightWithoutAttributes()");
        var product1 = await _productService.GetProductBySkuAsync("AS_551_LP");
        var product2 = await _productService.GetProductBySkuAsync("FIRST_PRP");
		_logger.LogDebug("CanGetShoppingCartTotalWeightWithoutAttributes(): product2 → {Product2}", product2);
		_logger.LogDebug("CanGetShoppingCartTotalWeightWithoutAttributes(): product1 → {Product1}", product1);

        var request = new GetShippingOptionRequest
        {
            Items =
            {
                new GetShippingOptionRequest.PackageItem(
                    new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 3, ProductId = product1.Id },
                    product1),
                new GetShippingOptionRequest.PackageItem(
                    new ShoppingCartItem { AttributesXml = string.Empty, Quantity = 4, ProductId = product2.Id },
                    product2)
            }
        };

        var totalWeight = await _shippingService.GetTotalWeightAsync(request);
		_logger.LogDebug("CanGetShoppingCartTotalWeightWithoutAttributes(): totalWeight → {TotalWeight}", totalWeight);
        totalWeight.Should().Be(29M);
		_logger.LogDebug("<<< Exiting CanGetShoppingCartTotalWeightWithoutAttributes()");
    }
}