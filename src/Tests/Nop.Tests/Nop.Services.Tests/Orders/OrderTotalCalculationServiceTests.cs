using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Tests.Nop.Services.Tests.Payments;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class OrderTotalCalculationServiceTests : ServiceTest
{
    private IOrderTotalCalculationService _orderTotalCalcService;
    private IProductService _productService;
    private ICustomerService _customerService;
    private IDiscountService _discountService;
    private TaxSettings _taxSettings;
    private ISettingService _settingService;
    private IShoppingCartService _shoppingCartService;
    private ShoppingCartSettings _shoppingCartSettings;
    private IStoreService _storeService;
    private RewardPointsSettings _rewardPointsSettings;
    private IGenericAttributeService _genericAttributeService;


    private Discount _discount;
    private Customer _customer;
    private Store _store;
    private string _checkoutAttrXml;

    #region Utilities

    private async Task<ShoppingCartItem> CreateTestShopCartItemAsync(decimal productPrice, int quantity = 1)
    {
		logger.LogDebug(">>> Entering CreateTestShopCartItemAsync(productPrice={ProductPrice},quantity={Quantity})", productPrice, quantity);
        //shopping cart
        var product = new Product
        {
            Name = "Product name 1",
            Price = productPrice,
            CustomerEntersPrice = false,
            Published = true
        };

        await _productService.InsertProductAsync(product);

        var shoppingCartItem = new ShoppingCartItem
        {
            CustomerId = _customer.Id,
            ProductId = product.Id,
            StoreId = _store.Id,
            Quantity = quantity
        };

		logger.LogDebug("<<< Exiting CreateTestShopCartItemAsync(productPrice={ProductPrice},quantity={Quantity})", productPrice, quantity);
        return shoppingCartItem;
    }

    private async Task<List<ShoppingCartItem>> GetShoppingCartAsync()
    {
		logger.LogDebug(">>> Entering GetShoppingCartAsync()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        var sci1 = new ShoppingCartItem
		logger.LogDebug("GetShoppingCartAsync(): product → {Product}", product);
        {
            ProductId = product.Id,
            Quantity = 2
        };
        product = await _productService.GetProductBySkuAsync("FIRST_PRP");
        var sci2 = new ShoppingCartItem
        {
            ProductId = product.Id,
            Quantity = 3
        };

        var cart = new List<ShoppingCartItem> { sci1, sci2 };
        foreach (var sci in cart)
        {
            sci.CustomerId = _customer.Id;
			logger.LogDebug("GetShoppingCartAsync(): sci.CustomerId → {Arg0}", sci.CustomerId);
        }

		logger.LogDebug("<<< Exiting GetShoppingCartAsync()");
        return cart;
    }

    #endregion

    [SetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();

		logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		logger.LogDebug("SetUp(): _taxSettings.ActiveTaxProviderSystemName → {Arg0}", _taxSettings.ActiveTaxProviderSystemName);
		logger.LogDebug("SetUp(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
        var shippingSettings = GetService<ShippingSettings>();
        shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");
		logger.LogDebug("SetUp(): shippingSettings → {ShippingSettings}", shippingSettings);
        _taxSettings = GetService<TaxSettings>();
		logger.LogDebug("SetUp(): _taxSettings → {_taxSettings}", _taxSettings);
        _taxSettings.ActiveTaxProviderSystemName = "FixedTaxRateTest";
        _taxSettings.ShippingIsTaxable = true;
        await _settingService.SaveSettingAsync(shippingSettings);
        await _settingService.SaveSettingAsync(_taxSettings);

        _orderTotalCalcService = GetService<IOrderTotalCalculationService>();
		logger.LogDebug("SetUp(): _orderTotalCalcService → {_orderTotalCalcService}", _orderTotalCalcService);
        _productService = GetService<IProductService>();
        _customerService = GetService<ICustomerService>();
		logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);
        _discountService = GetService<IDiscountService>();
        _shoppingCartService = GetService<IShoppingCartService>();
		logger.LogDebug("SetUp(): _discountService → {_discountService}", _discountService);
		logger.LogDebug("SetUp(): _shoppingCartService → {_shoppingCartService}", _shoppingCartService);
        _shoppingCartSettings = GetService<ShoppingCartSettings>();
        _storeService = GetService<IStoreService>();
		logger.LogDebug("SetUp(): _shoppingCartSettings → {_shoppingCartSettings}", _shoppingCartSettings);
		logger.LogDebug("SetUp(): _storeService → {_storeService}", _storeService);
        _rewardPointsSettings = GetService<RewardPointsSettings>();

		logger.LogDebug("SetUp(): _rewardPointsSettings → {_rewardPointsSettings}", _rewardPointsSettings);
        _genericAttributeService = GetService<IGenericAttributeService>();
        var checkoutAttributeService = GetService<IAttributeService<CheckoutAttribute, CheckoutAttributeValue>>();
		logger.LogDebug("SetUp(): _genericAttributeService → {_genericAttributeService}", _genericAttributeService);
		logger.LogDebug("SetUp(): checkoutAttributeService → {CheckoutAttributeService}", checkoutAttributeService);

        var attr = await checkoutAttributeService.GetAttributeByIdAsync(1);

		logger.LogDebug("SetUp(): attr → {Attr}", attr);
        var values = await checkoutAttributeService.GetAttributeValuesAsync(attr.Id);

		logger.LogDebug("SetUp(): values → {Values}", values);
        var val = values.FirstOrDefault(p => p.Name == "Yes")?.Id.ToString();

		logger.LogDebug("SetUp(): val → {Val}", val);
        _checkoutAttrXml = GetService<IAttributeParser<CheckoutAttribute, CheckoutAttributeValue>>().AddAttribute(string.Empty, attr, val);

		logger.LogDebug("SetUp(): _checkoutAttrXml → {_checkoutAttrXml}", _checkoutAttrXml);
        _discount = new Discount
        {
            IsActive = true,
            Name = "Discount 1",
            DiscountType = DiscountType.AssignedToOrderSubTotal,
            DiscountAmount = 3,
            DiscountLimitation = DiscountLimitationType.Unlimited
			logger.LogDebug("SetUp(): _discount → {_discount}", _discount);
        };

        _customer = await _customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
		logger.LogDebug("SetUp(): _customer → {_customer}", _customer);
        _store = (await _storeService.GetAllStoresAsync()).First();

		logger.LogDebug("SetUp(): _store → {_store}", _store);
        await _genericAttributeService.SaveAttributeAsync(_customer,
            NopCustomerDefaults.SelectedPaymentMethodAttribute, "Payments.TestMethod", 1);
			logger.LogDebug("<<< Exiting SetUp()");
    }

    [TearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        var settingService = GetService<ISettingService>();

		logger.LogDebug("TearDown(): settingService → {SettingService}", settingService);
        var shippingSettings = GetService<ShippingSettings>();
        shippingSettings.ActiveShippingRateComputationMethodSystemNames.Clear();
		logger.LogDebug("TearDown(): shippingSettings → {ShippingSettings}", shippingSettings);

        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
		logger.LogDebug("TearDown(): _taxSettings.PaymentMethodAdditionalFeeIsTaxable → {Arg0}", _taxSettings.PaymentMethodAdditionalFeeIsTaxable);
		logger.LogDebug("TearDown(): _taxSettings.ActiveTaxProviderSystemName → {Arg0}", _taxSettings.ActiveTaxProviderSystemName);
		logger.LogDebug("TearDown(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
        _taxSettings.ActiveTaxProviderSystemName = string.Empty;
        _taxSettings.ShippingIsTaxable = false;
        await settingService.SaveSettingAsync(shippingSettings);
        await settingService.SaveSettingAsync(_taxSettings);

        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 0M;
		logger.LogDebug("TearDown(): product → {Product}", product);
		logger.LogDebug("TearDown(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("TearDown(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        product = await _productService.GetProductBySkuAsync("FIRST_PRP");
        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        await GetService<IGenericAttributeService>().SaveAttributeAsync<string>(_customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, 1);

        foreach (var item in GetService<IRepository<Discount>>().Table.Where(d => d.Name == "Discount 1").ToList())
            await _discountService.DeleteDiscountAsync(item);

        await _productService.DeleteProductsAsync(GetService<IRepository<Product>>().Table.Where(p => p.Name == "Product name 1").ToList());

        await _genericAttributeService.SaveAttributeAsync<string>(_customer, NopCustomerDefaults.CheckoutAttributes, null, _store.Id);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanGetShoppingCartSubTotalExcludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubTotalExcludingTax()");
        //10% - default tax rate
        var (discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), false);
        discountAmount.Should().Be(0);
        appliedDiscounts.Count.Should().Be(0);
        subTotalWithoutDiscount.Should().Be(207M);
        subTotalWithDiscount.Should().Be(207M);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.7M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubTotalExcludingTax()");
    }

    [Test]
    public async Task CanGetShoppingCartSubTotalIncludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubTotalIncludingTax()");
        var (discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), true);
        discountAmount.Should().Be(0);
        appliedDiscounts.Count.Should().Be(0);
        subTotalWithoutDiscount.Should().Be(227.7M);
        subTotalWithDiscount.Should().Be(227.7M);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.7M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubTotalIncludingTax()");
    }

    [Test]
    public async Task CanGetShoppingCartSubTotal()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubTotal()");
        var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

        discountAmountExclTax.Should().Be(0);
        subTotalWithoutDiscountExclTax.Should().Be(207M);
        subTotalWithDiscountExclTax.Should().Be(207M);

        discountAmountInclTax.Should().Be(0);
        subTotalWithoutDiscountInclTax.Should().Be(227.7M);
        subTotalWithDiscountInclTax.Should().Be(227.7M);

        appliedDiscounts.Count.Should().Be(0);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.7M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubTotal()");
    }

    [Test]
    public async Task CanGetShoppingCartSubTotalWithCheckoutAttribute()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubTotalWithCheckoutAttribute()");
        var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

        discountAmountExclTax.Should().Be(0);
        subTotalWithoutDiscountExclTax.Should().Be(207M);
        subTotalWithDiscountExclTax.Should().Be(207M);

        discountAmountInclTax.Should().Be(0);
        subTotalWithoutDiscountInclTax.Should().Be(227.7M);
        subTotalWithDiscountInclTax.Should().Be(227.7M);

        appliedDiscounts.Count.Should().Be(0);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.7M);

        await _genericAttributeService.SaveAttributeAsync(_customer, NopCustomerDefaults.CheckoutAttributes, _checkoutAttrXml, _store.Id);

        (_, _, _, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

        subTotalWithoutDiscountExclTax.Should().Be(217M);
        subTotalWithDiscountExclTax.Should().Be(217M);

        subTotalWithoutDiscountInclTax.Should().Be(238.7M);
        subTotalWithDiscountInclTax.Should().Be(238.7M);

        await _genericAttributeService.SaveAttributeAsync<string>(_customer, NopCustomerDefaults.CheckoutAttributes, null, _store.Id);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubTotalWithCheckoutAttribute()");
    }

    [Test]
    public async Task CanGetShoppingCartSubtotalDiscountExcludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubtotalDiscountExcludingTax()");
        await _discountService.InsertDiscountAsync(_discount);

        //10% - default tax rate
        var (discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), false);

        await _discountService.DeleteDiscountAsync(_discount);

        discountAmount.Should().Be(3);
        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
        subTotalWithoutDiscount.Should().Be(207M);
        subTotalWithDiscount.Should().Be(204M);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.4M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubtotalDiscountExcludingTax()");
    }

    [Test]
    public async Task CanGetShoppingCartSubtotalDiscountIncludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubtotalDiscountIncludingTax()");
        await _discountService.InsertDiscountAsync(_discount);

        var (discountAmount, appliedDiscounts, subTotalWithoutDiscount, subTotalWithDiscount, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalAsync(await GetShoppingCartAsync(), true);

        await _discountService.DeleteDiscountAsync(_discount);

        //The comparison test failed before, because of a very tiny number difference.
        //discountAmount.ShouldEqual(3.3);
        (Math.Round(discountAmount, 10) == 3.3M).Should().BeTrue();
        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
        subTotalWithoutDiscount.Should().Be(227.7M);
        subTotalWithDiscount.Should().Be(224.4M);
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.4M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubtotalDiscountIncludingTax()");
    }

    [Test]
    public async Task CanGetShoppingCartSubtotalDiscountExcludingAndIncludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartSubtotalDiscountExcludingAndIncludingTax()");
        await _discountService.InsertDiscountAsync(_discount);

        //10% - default tax rate
        var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax, subTotalWithoutDiscountExclTax, subTotalWithDiscountInclTax, subTotalWithDiscountExclTax, taxRates) = await _orderTotalCalcService.GetShoppingCartSubTotalsAsync(await GetShoppingCartAsync());

        await _discountService.DeleteDiscountAsync(_discount);

        discountAmountExclTax.Should().Be(3);
        subTotalWithoutDiscountExclTax.Should().Be(207M);
        subTotalWithDiscountExclTax.Should().Be(204M);

        (Math.Round(discountAmountInclTax, 10) == 3.3M).Should().BeTrue();
        subTotalWithoutDiscountInclTax.Should().Be(227.7M);
        subTotalWithDiscountInclTax.Should().Be(224.4M);

        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.4M);
		logger.LogDebug("<<< Exiting CanGetShoppingCartSubtotalDiscountExcludingAndIncludingTax()");
    }

    [Test]
    public async Task ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping()
    {
		logger.LogDebug(">>> Entering ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping()");
        await TearDown();
        await SetUp();

        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = true;
		logger.LogDebug("ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping(): product → {Product}", product);
		logger.LogDebug("ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);

        await _productService.GetProductBySkuAsync("FIRST_PRP");
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
        isFreeShipping.Should().BeTrue();
		logger.LogDebug("<<< Exiting ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping()");
		logger.LogDebug("ShippingShouldBeFreeWhenAllShoppingCartItemsAreMarkedAsFreeShipping(): isFreeShipping → {IsFreeShipping}", isFreeShipping);
    }

    [Test]
    public async Task ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping()
    {
		logger.LogDebug(">>> Entering ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
		logger.LogDebug("ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping(): product → {Product}", product);
		logger.LogDebug("ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);
        var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
        product.IsFreeShipping = true;
		logger.LogDebug("ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping(): isFreeShipping → {IsFreeShipping}", isFreeShipping);
        await _productService.UpdateProductAsync(product);
        isFreeShipping.Should().BeFalse();
		logger.LogDebug("<<< Exiting ShippingShouldNotBeFreeWhenSomeOfShoppingCartItemsAreNotMarkedAsFreeShipping()");
    }

    [Test]
    public async Task ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping()
    {
		logger.LogDebug(">>> Entering ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
		logger.LogDebug("ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping(): product → {Product}", product);
		logger.LogDebug("ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);
        var role = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName);
        role.FreeShipping = true;
		logger.LogDebug("ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping(): role → {Role}", role);
		logger.LogDebug("ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping(): role.FreeShipping → {Arg0}", role.FreeShipping);
        await _customerService.UpdateCustomerRoleAsync(role);
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);
        role.FreeShipping = false;
        await _customerService.UpdateCustomerRoleAsync(role);
        var isFreeShipping = await _orderTotalCalcService.IsFreeShippingAsync(await GetShoppingCartAsync());
        isFreeShipping.Should().BeTrue();
		logger.LogDebug("<<< Exiting ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping()");
		logger.LogDebug("ShippingShouldBeFreeWhenCustomerIsInRoleWithFreeShipping(): isFreeShipping → {IsFreeShipping}", isFreeShipping);
    }

    [Test]
    public async Task CanGetShippingTotalWithFixedShippingRateExcludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShippingTotalWithFixedShippingRateExcludingTax()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 21.25M;
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateExcludingTax(): product → {Product}", product);
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateExcludingTax(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateExcludingTax(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);

        var (shipping, taxRate, appliedDiscounts) =
            await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), false);

        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        shipping.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change
        shipping.Should().Be(52.5M);
        appliedDiscounts.Count.Should().Be(0);
        //10 - default fixed tax rate
        taxRate.Should().Be(10);
		logger.LogDebug("<<< Exiting CanGetShippingTotalWithFixedShippingRateExcludingTax()");
    }

    [Test]
    public async Task CanGetShippingTotalWithFixedShippingRateIncludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShippingTotalWithFixedShippingRateIncludingTax()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 21.25M;
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateIncludingTax(): product → {Product}", product);
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateIncludingTax(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("CanGetShippingTotalWithFixedShippingRateIncludingTax(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);

        var (shipping, taxRate, appliedDiscounts) =
            await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), true);

        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        shipping.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change
        shipping.Should().Be(57.75M);
        appliedDiscounts.Count.Should().Be(0);
        //10 - default fixed tax rate
        taxRate.Should().Be(10);
		logger.LogDebug("<<< Exiting CanGetShippingTotalWithFixedShippingRateIncludingTax()");
    }

    [Test]
    public async Task CanGetShippingTotalsWithFixedShippingRate()
    {
		logger.LogDebug(">>> Entering CanGetShippingTotalsWithFixedShippingRate()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 21.25M;
		logger.LogDebug("CanGetShippingTotalsWithFixedShippingRate(): product → {Product}", product);
		logger.LogDebug("CanGetShippingTotalsWithFixedShippingRate(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("CanGetShippingTotalsWithFixedShippingRate(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);

        var (shippingInclTax, shippingExclTax, taxRate, appliedDiscounts) =
            await _orderTotalCalcService.GetShoppingCartShippingTotalsAsync(await GetShoppingCartAsync());

        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        shippingInclTax.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change
        shippingInclTax.Should().Be(57.75M);
        appliedDiscounts.Count.Should().Be(0);
        //10 - default fixed tax rate
        taxRate.Should().Be(10);

        shippingExclTax.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change
        shippingExclTax.Should().Be(52.5M);
		logger.LogDebug("<<< Exiting CanGetShippingTotalsWithFixedShippingRate()");
    }

    [Test]
    public async Task CanGetShippingTotalDiscountExcludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShippingTotalDiscountExcludingTax()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 21.25M;
		logger.LogDebug("CanGetShippingTotalDiscountExcludingTax(): product → {Product}", product);
		logger.LogDebug("CanGetShippingTotalDiscountExcludingTax(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("CanGetShippingTotalDiscountExcludingTax(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);

        _discount.DiscountType = DiscountType.AssignedToShipping;
		logger.LogDebug("CanGetShippingTotalDiscountExcludingTax(): _discount.DiscountType → {Arg0}", _discount.DiscountType);
        await _discountService.InsertDiscountAsync(_discount);

        var (shipping, taxRate, appliedDiscounts) =
            await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), false);

        await _discountService.DeleteDiscountAsync(_discount);
        _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
        shipping.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
        shipping.Should().Be(49.5M);
        //10 - default fixed tax rate
        taxRate.Should().Be(10);
		logger.LogDebug("<<< Exiting CanGetShippingTotalDiscountExcludingTax()");
    }

    [Test]
    public async Task CanGetShippingTotalDiscountIncludingTax()
    {
		logger.LogDebug(">>> Entering CanGetShippingTotalDiscountIncludingTax()");
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.AdditionalShippingCharge = 21.25M;
		logger.LogDebug("CanGetShippingTotalDiscountIncludingTax(): product → {Product}", product);
		logger.LogDebug("CanGetShippingTotalDiscountIncludingTax(): product.AdditionalShippingCharge → {Arg0}", product.AdditionalShippingCharge);
		logger.LogDebug("CanGetShippingTotalDiscountIncludingTax(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);

        _discount.DiscountType = DiscountType.AssignedToShipping;
		logger.LogDebug("CanGetShippingTotalDiscountIncludingTax(): _discount.DiscountType → {Arg0}", _discount.DiscountType);
        await _discountService.InsertDiscountAsync(_discount);

        var (shipping, taxRate, appliedDiscounts) =
            await _orderTotalCalcService.GetShoppingCartShippingTotalAsync(await GetShoppingCartAsync(), true);

        await _discountService.DeleteDiscountAsync(_discount);
        _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
        product.AdditionalShippingCharge = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
        shipping.Should().NotBeNull();
        //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
        shipping.Should().Be(54.45M);
        //10 - default fixed tax rate
        taxRate.Should().Be(10);
		logger.LogDebug("<<< Exiting CanGetShippingTotalDiscountIncludingTax()");
    }

    [Test]
    public async Task CanGetTaxTotal()
    {
		logger.LogDebug(">>> Entering CanGetTaxTotal()");
        //207 - items, 10 - shipping (fixed), 20 - payment fee

        TestPaymentMethod.AdditionalHandlingFee = 20M;
		logger.LogDebug("CanGetTaxTotal(): TestPaymentMethod.AdditionalHandlingFee → {Arg0}", TestPaymentMethod.AdditionalHandlingFee);
        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
		logger.LogDebug("CanGetTaxTotal(): product → {Product}", product);
		logger.LogDebug("CanGetTaxTotal(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);

        //1. shipping is taxable, payment fee is taxable
        _taxSettings.ShippingIsTaxable = true;
		logger.LogDebug("CanGetTaxTotal(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
		logger.LogDebug("CanGetTaxTotal(): _taxSettings.PaymentMethodAdditionalFeeIsTaxable → {Arg0}", _taxSettings.PaymentMethodAdditionalFeeIsTaxable);
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

        await _settingService.SaveSettingAsync(_taxSettings);

        var (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
        taxTotal.Should().Be(23.7M);
        taxRates.Should().NotBeNull();
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(23.7M);

        //2. shipping is taxable, payment fee is not taxable
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
        await _settingService.SaveSettingAsync(_taxSettings);

        (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
        taxTotal.Should().Be(21.7M);
        taxRates.Should().NotBeNull();
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(21.7M);

        //3. shipping is not taxable, payment fee is taxable
        _taxSettings.ShippingIsTaxable = false;
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
        await _settingService.SaveSettingAsync(_taxSettings);

        (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
        taxTotal.Should().Be(22.7M);
        taxRates.Should().NotBeNull();
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(22.7M);

        //4. shipping is not taxable, payment fee is not taxable
        _taxSettings.ShippingIsTaxable = false;
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
        await _settingService.SaveSettingAsync(_taxSettings);

        (taxTotal, taxRates) = await GetService<IOrderTotalCalculationService>().GetTaxTotalAsync(await GetShoppingCartAsync());
        taxTotal.Should().Be(20.7M);
        taxRates.Should().NotBeNull();
        taxRates.Count.Should().Be(1);
        taxRates.ContainsKey(10).Should().BeTrue();
        taxRates[10].Should().Be(20.7M);

        TestPaymentMethod.AdditionalHandlingFee = 0M;
        product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
        await _productService.UpdateProductAsync(product);
		logger.LogDebug("<<< Exiting CanGetTaxTotal()");
    }

    [Test]
    public async Task CanGetShoppingCartTotalWithoutShippingRequired()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartTotalWithoutShippingRequired()");
        await TearDown();
        await SetUp();

        //shipping is taxable, payment fee is taxable
        _taxSettings.ShippingIsTaxable = true;
		logger.LogDebug("CanGetShoppingCartTotalWithoutShippingRequired(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
		logger.LogDebug("CanGetShoppingCartTotalWithoutShippingRequired(): _taxSettings.PaymentMethodAdditionalFeeIsTaxable → {Arg0}", _taxSettings.PaymentMethodAdditionalFeeIsTaxable);
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

        await _settingService.SaveSettingAsync(_taxSettings);

        TestPaymentMethod.AdditionalHandlingFee = 20M;
		logger.LogDebug("CanGetShoppingCartTotalWithoutShippingRequired(): TestPaymentMethod.AdditionalHandlingFee → {Arg0}", TestPaymentMethod.AdditionalHandlingFee);

        //207 - items, 20 - payment fee, 22.7 - tax
        var (cartTotal, _, _, _, _, _) =
            await _orderTotalCalcService.GetShoppingCartTotalAsync(await GetShoppingCartAsync());
        cartTotal.Should().Be(249.7M);

        TestPaymentMethod.AdditionalHandlingFee = 0M;
		logger.LogDebug("<<< Exiting CanGetShoppingCartTotalWithoutShippingRequired()");
    }

    [Test]
    public async Task CanGetShoppingCartTotalWithShippingRequired()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartTotalWithShippingRequired()");
        //shipping is taxable, payment fee is taxable
        _taxSettings.ShippingIsTaxable = true;
		logger.LogDebug("CanGetShoppingCartTotalWithShippingRequired(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
		logger.LogDebug("CanGetShoppingCartTotalWithShippingRequired(): _taxSettings.PaymentMethodAdditionalFeeIsTaxable → {Arg0}", _taxSettings.PaymentMethodAdditionalFeeIsTaxable);
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

        await _settingService.SaveSettingAsync(_taxSettings);

        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
		logger.LogDebug("CanGetShoppingCartTotalWithShippingRequired(): product → {Product}", product);
		logger.LogDebug("CanGetShoppingCartTotalWithShippingRequired(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);

        TestPaymentMethod.AdditionalHandlingFee = 20M;
		logger.LogDebug("CanGetShoppingCartTotalWithShippingRequired(): TestPaymentMethod.AdditionalHandlingFee → {Arg0}", TestPaymentMethod.AdditionalHandlingFee);

        //207 - items, 10 - shipping (fixed), 20 - payment fee, 23.7 - tax
        var (cartTotal, _, _, _, _, _) =
            await _orderTotalCalcService.GetShoppingCartTotalAsync(await GetShoppingCartAsync());
        cartTotal.Should().Be(260.7M);

        TestPaymentMethod.AdditionalHandlingFee = 0M;
        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);
		logger.LogDebug("<<< Exiting CanGetShoppingCartTotalWithShippingRequired()");
    }

    [Test]
    public async Task CanGetShoppingCartItemUnitPrice()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartItemUnitPrice()");
        var items = await GetShoppingCartAsync();
        var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(items[0], true);
		logger.LogDebug("CanGetShoppingCartItemUnitPrice(): items → {Items}", items);
        unitPrice.Should().Be(new decimal(27.0));
		logger.LogDebug("<<< Exiting CanGetShoppingCartItemUnitPrice()");
    }

    [Test]
    public async Task CanGetShoppingCartItemSubtotal()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartItemSubtotal()");
        var items = await GetShoppingCartAsync();
        var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(items[0], true);
		logger.LogDebug("CanGetShoppingCartItemSubtotal(): items → {Items}", items);
        subTotal.Should().Be(new decimal(54.0));
		logger.LogDebug("<<< Exiting CanGetShoppingCartItemSubtotal()");
    }

    [Test]
    [TestCase(12.00009, 12.00)]
    [TestCase(12.119, 12.12)]
    [TestCase(12.115, 12.12)]
    [TestCase(12.114, 12.11)]
    public async Task TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(decimal inputPrice, decimal expectedPrice)
    {
		logger.LogDebug(">>> Entering TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice})", inputPrice, expectedPrice);
        // arrange
        var shoppingCartItem = await CreateTestShopCartItemAsync(inputPrice);

		logger.LogDebug("TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice}): shoppingCartItem → {ShoppingCartItem}", inputPrice, expectedPrice, shoppingCartItem);
        // act
        _shoppingCartSettings.RoundPricesDuringCalculation = true;
		logger.LogDebug("TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice}): _shoppingCartSettings.RoundPricesDuringCalculation → {Arg2}", inputPrice, expectedPrice, _shoppingCartSettings.RoundPricesDuringCalculation);
        await _settingService.SaveSettingAsync(_shoppingCartSettings);

        var (resultPrice, _, _) = await GetService<IShoppingCartService>().GetUnitPriceAsync(shoppingCartItem, true);

        // assert
        resultPrice.Should().Be(expectedPrice);
		logger.LogDebug("<<< Exiting TestGetUnitPriceWhenRoundPricesDuringCalculationIsTruePriceMustBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice})", inputPrice, expectedPrice);
    }

    [Test]
    [TestCase(12.00009, 12.00009)]
    [TestCase(12.119, 12.119)]
    [TestCase(12.115, 12.115)]
    [TestCase(12.114, 12.114)]
    public async Task TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(decimal inputPrice, decimal expectedPrice)
    {
		logger.LogDebug(">>> Entering TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice})", inputPrice, expectedPrice);
        // arrange            
        var shoppingCartItem = await CreateTestShopCartItemAsync(inputPrice);

		logger.LogDebug("TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice}): shoppingCartItem → {ShoppingCartItem}", inputPrice, expectedPrice, shoppingCartItem);
        // act
        _shoppingCartSettings.RoundPricesDuringCalculation = false;
		logger.LogDebug("TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice}): _shoppingCartSettings.RoundPricesDuringCalculation → {Arg2}", inputPrice, expectedPrice, _shoppingCartSettings.RoundPricesDuringCalculation);
        await _settingService.SaveSettingAsync(_shoppingCartSettings);

        var (resultPrice, _, _) = await GetService<IShoppingCartService>().GetUnitPriceAsync(shoppingCartItem, true);

        // assert
        resultPrice.Should().Be(expectedPrice);
		logger.LogDebug("<<< Exiting TestGetUnitPriceWhenNotRoundPricesDuringCalculationIsFalsePriceMustNotBeRounded(inputPrice={InputPrice},expectedPrice={ExpectedPrice})", inputPrice, expectedPrice);
    }

    [Test]
    public async Task CanGetShoppingCartTotalDiscount()
    {
		logger.LogDebug(">>> Entering CanGetShoppingCartTotalDiscount()");
        _discount.DiscountType = DiscountType.AssignedToOrderTotal;

		logger.LogDebug("CanGetShoppingCartTotalDiscount(): _discount.DiscountType → {Arg0}", _discount.DiscountType);
        await _discountService.InsertDiscountAsync(_discount);

        //shipping is taxable, payment fee is taxable
        _taxSettings.ShippingIsTaxable = true;
		logger.LogDebug("CanGetShoppingCartTotalDiscount(): _taxSettings.ShippingIsTaxable → {Arg0}", _taxSettings.ShippingIsTaxable);
		logger.LogDebug("CanGetShoppingCartTotalDiscount(): _taxSettings.PaymentMethodAdditionalFeeIsTaxable → {Arg0}", _taxSettings.PaymentMethodAdditionalFeeIsTaxable);
        _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

        await _settingService.SaveSettingAsync(_taxSettings);

        TestPaymentMethod.AdditionalHandlingFee = 20M;
		logger.LogDebug("CanGetShoppingCartTotalDiscount(): TestPaymentMethod.AdditionalHandlingFee → {Arg0}", TestPaymentMethod.AdditionalHandlingFee);

        var product = await _productService.GetProductBySkuAsync("FR_451_RB");
        product.IsFreeShipping = false;
		logger.LogDebug("CanGetShoppingCartTotalDiscount(): product → {Product}", product);
		logger.LogDebug("CanGetShoppingCartTotalDiscount(): product.IsFreeShipping → {Arg0}", product.IsFreeShipping);
        await _productService.UpdateProductAsync(product);

        //207 - items, 10 - shipping (fixed), 20 - payment fee, 23.7 - tax, [-3] - discount
        var (scTotal, discountAmount, appliedDiscounts, _, _, _) = await GetService<IOrderTotalCalculationService>().GetShoppingCartTotalAsync(await GetShoppingCartAsync());
        await _discountService.DeleteDiscountAsync(_discount);
        _discount.DiscountType = DiscountType.AssignedToOrderSubTotal;
        TestPaymentMethod.AdditionalHandlingFee = 0M;

        product.IsFreeShipping = true;
        await _productService.UpdateProductAsync(product);

        scTotal.Should().Be(257.7M);
        discountAmount.Should().Be(3);
        appliedDiscounts.Count.Should().Be(1);
        appliedDiscounts.First().Name.Should().Be("Discount 1");
		logger.LogDebug("<<< Exiting CanGetShoppingCartTotalDiscount()");
    }

    [Test]
    public async Task CanConvertRewardPointsToAmount()
    {
		logger.LogDebug(">>> Entering CanConvertRewardPointsToAmount()");
        _rewardPointsSettings.ExchangeRate = 15M;

		logger.LogDebug("CanConvertRewardPointsToAmount(): _rewardPointsSettings.ExchangeRate → {Arg0}", _rewardPointsSettings.ExchangeRate);
        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        var rewardPointsToAmount = await GetService<IOrderTotalCalculationService>().ConvertRewardPointsToAmountAsync(100);

		logger.LogDebug("CanConvertRewardPointsToAmount(): rewardPointsToAmount → {RewardPointsToAmount}", rewardPointsToAmount);
        _rewardPointsSettings.ExchangeRate = 1M;

        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        rewardPointsToAmount.Should().Be(1500);
		logger.LogDebug("<<< Exiting CanConvertRewardPointsToAmount()");
    }

    [Test]
    public async Task CanCheckMinimumRewardPointsToUseRequirement()
    {
		logger.LogDebug(">>> Entering CanCheckMinimumRewardPointsToUseRequirement()");
        _rewardPointsSettings.Enabled = true;
        _rewardPointsSettings.MinimumRewardPointsToUse = 0;
		logger.LogDebug("CanCheckMinimumRewardPointsToUseRequirement(): _rewardPointsSettings.MinimumRewardPointsToUse → {Arg0}", _rewardPointsSettings.MinimumRewardPointsToUse);
		logger.LogDebug("CanCheckMinimumRewardPointsToUseRequirement(): _rewardPointsSettings.Enabled → {Arg0}", _rewardPointsSettings.Enabled);

        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(0).Should().BeTrue();
        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(1).Should().BeTrue();
        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(10).Should().BeTrue();

        _rewardPointsSettings.MinimumRewardPointsToUse = 2;
        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(0).Should().BeFalse();
        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(1).Should().BeFalse();
        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(2).Should().BeTrue();
        GetService<IOrderTotalCalculationService>().CheckMinimumRewardPointsToUseRequirement(10).Should().BeTrue();
		logger.LogDebug("<<< Exiting CanCheckMinimumRewardPointsToUseRequirement()");
    }
}