using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

[TestFixture]
public class PriceCalculationServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private ICustomerService _customerService;
    private IProductService _productService;
    private IPriceCalculationService _priceCalcService;

    #endregion

    #region SetUp

    [OneTimeSetUp]
    public void SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _customerService = GetService<ICustomerService>();
        _productService = GetService<IProductService>();
		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
		_logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        _priceCalcService = GetService<IPriceCalculationService>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _priceCalcService → {_priceCalcService}", _priceCalcService);
    }

    #endregion

    #region Tests

    [Test]
    public async Task CanGetFinalProductPrice()
    {
		_logger.LogDebug(">>> Entering CanGetFinalProductPrice()");
        var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

		_logger.LogDebug("CanGetFinalProductPrice(): product → {Product}", product);
        var customer = new Customer();
		_logger.LogDebug("CanGetFinalProductPrice(): customer → {Customer}", customer);
        var store = new Store();
		_logger.LogDebug("CanGetFinalProductPrice(): store → {Store}", store);

        var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false);
        finalPrice.Should().Be(79.99M);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);

        (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 2);

        finalPrice.Should().Be(19M);
        finalPriceWithoutDiscounts.Should().Be(finalPriceWithoutDiscounts);
		_logger.LogDebug("<<< Exiting CanGetFinalProductPrice()");
    }

    [Test]
    public async Task CanGetFinalProductPriceWithTierPrices()
    {
		_logger.LogDebug(">>> Entering CanGetFinalProductPriceWithTierPrices()");
        var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

		_logger.LogDebug("CanGetFinalProductPriceWithTierPrices(): product → {Product}", product);
        var customer = new Customer();
		_logger.LogDebug("CanGetFinalProductPriceWithTierPrices(): customer → {Customer}", customer);
        var store = new Store();
		_logger.LogDebug("CanGetFinalProductPriceWithTierPrices(): store → {Store}", store);

        var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false);
        finalPrice.Should().Be(79.99M);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
        (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 2);
        finalPrice.Should().Be(19);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
        (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 3);
        finalPrice.Should().Be(19);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
        (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 5);
        finalPrice.Should().Be(17);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
        (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 7);

        finalPrice.Should().Be(17);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
		_logger.LogDebug("<<< Exiting CanGetFinalProductPriceWithTierPrices()");
    }

    [Test]
    public async Task CanGetFinalProductPriceWithTierPricesByCustomerRole()
    {
		_logger.LogDebug(">>> Entering CanGetFinalProductPriceWithTierPricesByCustomerRole()");
        var product = await _productService.GetProductBySkuAsync("NK_ZSJ_MM");

		_logger.LogDebug("CanGetFinalProductPriceWithTierPricesByCustomerRole(): product → {Product}", product);
        //customer
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		_logger.LogDebug("CanGetFinalProductPriceWithTierPricesByCustomerRole(): customer → {Customer}", customer);
        var store = new Store();
		_logger.LogDebug("CanGetFinalProductPriceWithTierPricesByCustomerRole(): store → {Store}", store);

        var roles = await _customerService.GetAllCustomerRolesAsync();
		_logger.LogDebug("CanGetFinalProductPriceWithTierPricesByCustomerRole(): roles → {Roles}", roles);
        var customerRole = roles.FirstOrDefault();
		_logger.LogDebug("CanGetFinalProductPriceWithTierPricesByCustomerRole(): customerRole → {CustomerRole}", customerRole);

        customerRole.Should().NotBeNull();

        var tierPrices = new List<TierPrice>
		_logger.LogDebug("<<< Exiting CanGetFinalProductPriceWithTierPricesByCustomerRole()");
        {
            new() { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 2, Price = 25 },
            new() { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 5, Price = 20 },
            new() { CustomerRoleId = customerRole.Id, ProductId = product.Id, Quantity = 10, Price = 15 }
        };

        foreach (var tierPrice in tierPrices)
            await _productService.InsertTierPriceAsync(tierPrice);

        var (rezWithoutDiscount1, rez1, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false);
        var (rezWithoutDiscount2, rez2, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 2);
        var (rezWithoutDiscount3, rez3, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 3);
        var (rezWithoutDiscount4, rez4, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 5);
        var (rezWithoutDiscount5, rez5, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 10);
        var (rezWithoutDiscount6, rez6, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 0, false, 15);

        foreach (var tierPrice in tierPrices)
            await _productService.DeleteTierPriceAsync(tierPrice);

        rez1.Should().Be(30M);
        rez2.Should().Be(25);
        rez3.Should().Be(25);
        rez4.Should().Be(20);
        rez5.Should().Be(15);
        rez6.Should().Be(15);

        rez1.Should().Be(rezWithoutDiscount1);
        rez2.Should().Be(rezWithoutDiscount2);
        rez3.Should().Be(rezWithoutDiscount3);
        rez4.Should().Be(rezWithoutDiscount4);
        rez5.Should().Be(rezWithoutDiscount5);
        rez6.Should().Be(rezWithoutDiscount6);
    }

    [Test]
    public async Task CanGetFinalProductPriceWithAdditionalFee()
    {
		_logger.LogDebug(">>> Entering CanGetFinalProductPriceWithAdditionalFee()");
        var product = await _productService.GetProductBySkuAsync("BP_20_WSP");

		_logger.LogDebug("CanGetFinalProductPriceWithAdditionalFee(): product → {Product}", product);
        //customer
        var customer = new Customer();
		_logger.LogDebug("CanGetFinalProductPriceWithAdditionalFee(): customer → {Customer}", customer);
        var store = new Store();
		_logger.LogDebug("CanGetFinalProductPriceWithAdditionalFee(): store → {Store}", store);

        var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store, 5, false);

        finalPrice.Should().Be(84.99M);
        finalPrice.Should().Be(finalPriceWithoutDiscounts);
		_logger.LogDebug("<<< Exiting CanGetFinalProductPriceWithAdditionalFee()");
    }

    [Test]
    public async Task CanGetFinalProductPriceWithDiscount()
    {
		_logger.LogDebug(">>> Entering CanGetFinalProductPriceWithDiscount()");
        var product = await _productService.GetProductBySkuAsync("BP_20_WSP");
        var customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		_logger.LogDebug("CanGetFinalProductPriceWithDiscount(): customer → {Customer}", customer);
        var store = new Store();
		_logger.LogDebug("CanGetFinalProductPriceWithDiscount(): store → {Store}", store);
		_logger.LogDebug("CanGetFinalProductPriceWithDiscount(): product → {Product}", product);

        var mapping = new DiscountProductMapping
        {
            DiscountId = 1,
            EntityId = product.Id
        };

        await _productService.InsertDiscountProductMappingAsync(mapping);
        await _customerService.ApplyDiscountCouponCodeAsync(customer, "123");

        var (finalPriceWithoutDiscounts, finalPrice, _, _) = await _priceCalcService.GetFinalPriceAsync(product, customer, store);

        await _productService.DeleteDiscountProductMappingAsync(mapping);
        await _customerService.RemoveDiscountCouponCodeAsync(customer, "123");

        finalPrice.Should().Be(69.99M);
        finalPriceWithoutDiscounts.Should().Be(79.99M);
		_logger.LogDebug("<<< Exiting CanGetFinalProductPriceWithDiscount()");
    }

    [TestCase(12.366, 12.37, RoundingType.Rounding001)]
    [TestCase(12.363, 12.36, RoundingType.Rounding001)]
    [TestCase(12.000, 12.00, RoundingType.Rounding001)]
    [TestCase(12.001, 12.00, RoundingType.Rounding001)]
    [TestCase(12.34, 12.35, RoundingType.Rounding005Up)]
    [TestCase(12.36, 12.40, RoundingType.Rounding005Up)]
    [TestCase(12.35, 12.35, RoundingType.Rounding005Up)]
    [TestCase(12.00, 12.00, RoundingType.Rounding005Up)]
    [TestCase(12.05, 12.05, RoundingType.Rounding005Up)]
    [TestCase(12.20, 12.20, RoundingType.Rounding005Up)]
    [TestCase(12.001, 12.00, RoundingType.Rounding005Up)]
    [TestCase(12.34, 12.30, RoundingType.Rounding005Down)]
    [TestCase(12.36, 12.35, RoundingType.Rounding005Down)]
    [TestCase(12.00, 12.00, RoundingType.Rounding005Down)]
    [TestCase(12.05, 12.05, RoundingType.Rounding005Down)]
    [TestCase(12.20, 12.20, RoundingType.Rounding005Down)]
    [TestCase(12.35, 12.40, RoundingType.Rounding01Up)]
    [TestCase(12.36, 12.40, RoundingType.Rounding01Up)]
    [TestCase(12.00, 12.00, RoundingType.Rounding01Up)]
    [TestCase(12.10, 12.10, RoundingType.Rounding01Up)]
    [TestCase(12.35, 12.30, RoundingType.Rounding01Down)]
    [TestCase(12.36, 12.40, RoundingType.Rounding01Down)]
    [TestCase(12.00, 12.00, RoundingType.Rounding01Down)]
    [TestCase(12.10, 12.10, RoundingType.Rounding01Down)]
    [TestCase(12.24, 12.00, RoundingType.Rounding05)]
    [TestCase(12.49, 12.50, RoundingType.Rounding05)]
    [TestCase(12.74, 12.50, RoundingType.Rounding05)]
    [TestCase(12.99, 13.00, RoundingType.Rounding05)]
    [TestCase(12.00, 12.00, RoundingType.Rounding05)]
    [TestCase(12.50, 12.50, RoundingType.Rounding05)]
    [TestCase(12.49, 12.00, RoundingType.Rounding1)]
    [TestCase(12.50, 13.00, RoundingType.Rounding1)]
    [TestCase(12.00, 12.00, RoundingType.Rounding1)]
    [TestCase(12.01, 13.00, RoundingType.Rounding1Up)]
    [TestCase(12.99, 13.00, RoundingType.Rounding1Up)]
    [TestCase(12.00, 12.00, RoundingType.Rounding1Up)]
    public void CanRound(decimal valueToRounding, decimal roundedValue, RoundingType roundingType)
    {
		_logger.LogDebug(">>> Entering CanRound(valueToRounding={ValueToRounding},roundedValue={RoundedValue},roundingType={RoundingType})", valueToRounding, roundedValue, roundingType);
        _priceCalcService.Round(valueToRounding, roundingType).Should().Be(roundedValue);
		_logger.LogDebug("<<< Exiting CanRound(valueToRounding={ValueToRounding},roundedValue={RoundedValue},roundingType={RoundingType})", valueToRounding, roundedValue, roundingType);
    }

    #endregion
}