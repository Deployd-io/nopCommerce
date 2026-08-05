using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Tests.Nop.Services.Tests;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class CheckoutModelFactoryTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ICheckoutModelFactory _checkoutModelFactory;
    private IShoppingCartService _shoppingCartService;
    private IProductService _productService;
    private IList<ShoppingCartItem> _cart;
    private OrderSettings _orderSettings;
    private ShippingSettings _shippingSettings;
    private RewardPointsSettings _rewardPointsSettings;
    private IAddressService _addressService;
    private Address _address;
    private ISettingService _settingService;
    private PaymentSettings _paymentSettings;
    private IRewardPointService _rewardPointService;
    private Customer _customer;
    private IPaymentMethod _paymentMethod;
    private CommonSettings _commonSettings;
    private IOrderService _orderService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _settingService = GetService<ISettingService>();
        _shippingSettings = GetService<ShippingSettings>();
		_logger.LogDebug("SetUp(): _settingService → {_settingService}", _settingService);
		_logger.LogDebug("SetUp(): _shippingSettings → {_shippingSettings}", _shippingSettings);
        _rewardPointsSettings = GetService<RewardPointsSettings>();
        _commonSettings = GetService<CommonSettings>();
		_logger.LogDebug("SetUp(): _rewardPointsSettings → {_rewardPointsSettings}", _rewardPointsSettings);
		_logger.LogDebug("SetUp(): _commonSettings → {_commonSettings}", _commonSettings);

        _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");
        await _settingService.SaveSettingAsync(_shippingSettings);

        _paymentSettings = GetService<PaymentSettings>();
		_logger.LogDebug("SetUp(): _paymentSettings → {_paymentSettings}", _paymentSettings);
        _paymentSettings.ActivePaymentMethodSystemNames.Add("Payments.TestMethod");
        await _settingService.SaveSettingAsync(_paymentSettings);

        _customer = await GetService<IWorkContext>().GetCurrentCustomerAsync();
		_logger.LogDebug("SetUp(): _customer → {_customer}", _customer);

        _rewardPointService = GetService<IRewardPointService>();
		_logger.LogDebug("SetUp(): _rewardPointService → {_rewardPointService}", _rewardPointService);
        await _rewardPointService.AddRewardPointsHistoryEntryAsync(_customer, 10000, 1);

        _shoppingCartService = GetService<IShoppingCartService>();
		_logger.LogDebug("SetUp(): _shoppingCartService → {_shoppingCartService}", _shoppingCartService);
        _productService = GetService<IProductService>();
		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
        _addressService = GetService<IAddressService>();

		_logger.LogDebug("SetUp(): _addressService → {_addressService}", _addressService);
        _address = new Address();
		_logger.LogDebug("SetUp(): _address → {_address}", _address);

        await _addressService.InsertAddressAsync(_address);
        await GetService<ICustomerService>().InsertCustomerAddressAsync(_customer, _address);

        _orderSettings = GetService<OrderSettings>();
		_logger.LogDebug("SetUp(): _orderSettings → {_orderSettings}", _orderSettings);

        await _shoppingCartService.AddToCartAsync(_customer, await _productService.GetProductByIdAsync(1), ShoppingCartType.ShoppingCart, 1);
        await _shoppingCartService.AddToCartAsync(_customer, await _productService.GetProductByIdAsync(2), ShoppingCartType.ShoppingCart, 1);
        await _shoppingCartService.AddToCartAsync(_customer, await _productService.GetProductByIdAsync(3), ShoppingCartType.ShoppingCart, 1);

        _cart = await _shoppingCartService.GetShoppingCartAsync(_customer, ShoppingCartType.ShoppingCart);
		_logger.LogDebug("SetUp(): _cart → {_cart}", _cart);

        _paymentMethod = (await GetService<IPaymentPluginManager>().LoadActivePluginsAsync(["Payments.TestMethod"])).FirstOrDefault();
		_logger.LogDebug("SetUp(): _paymentMethod → {_paymentMethod}", _paymentMethod);
        _orderService = GetService<IOrderService>();
		_logger.LogDebug("SetUp(): _orderService → {_orderService}", _orderService);

        _checkoutModelFactory = GetService<ICheckoutModelFactory>();
		_logger.LogDebug("<<< Exiting SetUp()");
		_logger.LogDebug("SetUp(): _checkoutModelFactory → {_checkoutModelFactory}", _checkoutModelFactory);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        foreach (var shoppingCartItem in _cart)
            await _shoppingCartService.DeleteShoppingCartItemAsync(shoppingCartItem);

        await _addressService.DeleteAddressAsync(_address);

        _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Clear();
        await _settingService.SaveSettingAsync(_shippingSettings);

        _paymentSettings.ActivePaymentMethodSystemNames.Clear();
        await _settingService.SaveSettingAsync(_paymentSettings);

        _rewardPointsSettings.Enabled = true;
		_logger.LogDebug("TearDown(): _rewardPointsSettings.Enabled → {Arg0}", _rewardPointsSettings.Enabled);
        await _settingService.SaveSettingAsync(_rewardPointsSettings);
        foreach (var history in await _rewardPointService.GetRewardPointsHistoryAsync(_customer.Id))
            await _rewardPointService.DeleteRewardPointsHistoryEntryAsync(history);
			_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareBillingAddressModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareBillingAddressModel()");
        var model = new CheckoutBillingAddressModel();
        await _checkoutModelFactory.PrepareBillingAddressModelAsync(model, _cart);
		_logger.LogDebug("CanPrepareBillingAddressModel(): model → {Model}", model);

        model.ShipToSameAddressAllowed.Should().Be(_shippingSettings.ShipToSameAddress &&
                                                   await _shoppingCartService.ShoppingCartRequiresShippingAsync(_cart));
        model.ShipToSameAddress.Should().Be(!_orderSettings.DisableBillingAddressCheckoutStep);

        model.ExistingAddresses.Any().Should().BeTrue();
        model.ExistingAddresses.Count.Should().Be(1);

        model.InvalidExistingAddresses.Any().Should().BeTrue();
        model.InvalidExistingAddresses.Count.Should().Be(1);

        model.BillingNewAddress.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanPrepareBillingAddressModel()");
    }

    [Test]
    public async Task CanPrepareShippingAddressModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareShippingAddressModel()");
        var model = new CheckoutShippingAddressModel();
        await _checkoutModelFactory.PrepareShippingAddressModelAsync(model, _cart);
		_logger.LogDebug("CanPrepareShippingAddressModel(): model → {Model}", model);

        model.DisplayPickupInStore.Should().Be(!_orderSettings.DisplayPickupInStoreOnShippingMethodPage);

        model.ExistingAddresses.Any().Should().BeTrue();
        model.ExistingAddresses.Count.Should().Be(1);

        model.InvalidExistingAddresses.Any().Should().BeTrue();
        model.InvalidExistingAddresses.Count.Should().Be(1);

        model.ShippingNewAddress.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanPrepareShippingAddressModel()");
    }

    [Test]
    public async Task CanPrepareShippingMethodModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareShippingMethodModel()");
        var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(_cart, await _addressService.GetAddressByIdAsync(1));
        model.DisplayPickupInStore.Should().Be(_orderSettings.DisplayPickupInStoreOnShippingMethodPage);
		_logger.LogDebug("CanPrepareShippingMethodModel(): model → {Model}", model);
        model.Warnings.Any().Should().BeFalse();
        model.ShippingMethods.Any().Should().BeTrue();
        model.ShippingMethods.Count.Should().Be(2);
		_logger.LogDebug("<<< Exiting CanPrepareShippingMethodModel()");
    }

    [Test]
    public async Task CanPreparePaymentMethodModel()
    {
		_logger.LogDebug(">>> Entering CanPreparePaymentMethodModel()");
        var model = await _checkoutModelFactory.PreparePaymentMethodModelAsync(_cart, 0);

		_logger.LogDebug("CanPreparePaymentMethodModel(): model → {Model}", model);
        model.DisplayRewardPoints.Should().BeTrue();
        model.PaymentMethods.Count.Should().Be(1);
        model.RewardPointsToUseAmount.Should().Be("$1,944.90");
        model.RewardPointsToUse.Should().Be(1945);
        model.RewardPointsBalance.Should().Be(10000);
        model.RewardPointsEnoughToPayForOrder.Should().BeTrue();
        model.UseRewardPoints.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanPreparePaymentMethodModel()");
    }

    [Test]
    public async Task PreparePaymentMethodModelShouldDependOnSettings()
    {
		_logger.LogDebug(">>> Entering PreparePaymentMethodModelShouldDependOnSettings()");
        var model = await _checkoutModelFactory.PreparePaymentMethodModelAsync(_cart, 0);
        model.DisplayRewardPoints.Should().BeTrue();
		_logger.LogDebug("PreparePaymentMethodModelShouldDependOnSettings(): model → {Model}", model);
        model.RewardPointsToUse.Should().Be(1945);
        model.RewardPointsBalance.Should().Be(10000);

        _rewardPointsSettings.Enabled = false;
		_logger.LogDebug("PreparePaymentMethodModelShouldDependOnSettings(): _rewardPointsSettings.Enabled → {Arg0}", _rewardPointsSettings.Enabled);
        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        model = await GetService<ICheckoutModelFactory>().PreparePaymentMethodModelAsync(_cart, 0);

        _rewardPointsSettings.Enabled = true;
        await _settingService.SaveSettingAsync(_rewardPointsSettings);

        model.DisplayRewardPoints.Should().BeFalse();
        model.RewardPointsBalance.Should().Be(0);
		_logger.LogDebug("<<< Exiting PreparePaymentMethodModelShouldDependOnSettings()");
    }

    [Test]
    public async Task CanPreparePaymentInfoModel()
    {
		_logger.LogDebug(">>> Entering CanPreparePaymentInfoModel()");
        var model = await _checkoutModelFactory.PreparePaymentInfoModelAsync(_paymentMethod);

		_logger.LogDebug("CanPreparePaymentInfoModel(): model → {Model}", model);
        model.PaymentViewComponent.Should().Be(_paymentMethod.GetPublicViewComponent());
        model.DisplayOrderTotals.Should().Be(_orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab);
		_logger.LogDebug("<<< Exiting CanPreparePaymentInfoModel()");
    }

    [Test]
    public async Task CanPrepareConfirmOrderModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareConfirmOrderModel()");
        var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(_cart);

		_logger.LogDebug("CanPrepareConfirmOrderModel(): model → {Model}", model);
        model.TermsOfServiceOnOrderConfirmPage.Should().Be(_orderSettings.TermsOfServiceOnOrderConfirmPage);
        model.TermsOfServicePopup.Should().Be(_commonSettings.PopupForTermsOfServiceLinks);
        model.MinOrderTotalWarning.Should().BeNullOrEmpty();
        model.Warnings.Any().Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanPrepareConfirmOrderModel()");
    }

    [Test]
    public async Task CanPrepareCheckoutCompletedModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCheckoutCompletedModel()");
        var order = await _orderService.GetOrderByIdAsync(1);
        var model = await _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(order);
		_logger.LogDebug("CanPrepareCheckoutCompletedModel(): order → {Order}", order);
		_logger.LogDebug("CanPrepareCheckoutCompletedModel(): model → {Model}", model);

        model.OrderId.Should().Be(order.Id);
        model.OnePageCheckoutEnabled.Should().Be(_orderSettings.OnePageCheckoutEnabled);
        model.CustomOrderNumber.Should().Be(order.CustomOrderNumber);
		_logger.LogDebug("<<< Exiting CanPrepareCheckoutCompletedModel()");
    }

    [Test]
    public void PrepareCheckoutCompletedModelShouldRaiseExceptionIfOrderIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareCheckoutCompletedModelShouldRaiseExceptionIfOrderIsNull()");
        Assert.Throws<ArgumentNullException>(() =>
            _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(null).Wait());
			_logger.LogDebug("<<< Exiting PrepareCheckoutCompletedModelShouldRaiseExceptionIfOrderIsNull()");
    }

    [Test]
    public async Task CanPrepareCheckoutProgressModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareCheckoutProgressModel()");
        foreach (var step in Enum.GetValues(typeof(CheckoutProgressStep)).Cast<CheckoutProgressStep>())
        {
            var model = await _checkoutModelFactory.PrepareCheckoutProgressModelAsync(step);
            model.CheckoutProgressStep.Should().Be(step);
			_logger.LogDebug("CanPrepareCheckoutProgressModel(): model → {Model}", model);
        }
		_logger.LogDebug("<<< Exiting CanPrepareCheckoutProgressModel()");
    }

    [Test]
    public async Task CanPrepareOnePageCheckoutModel()
    {
		_logger.LogDebug(">>> Entering CanPrepareOnePageCheckoutModel()");
        var model = await _checkoutModelFactory.PrepareOnePageCheckoutModelAsync(_cart);

		_logger.LogDebug("CanPrepareOnePageCheckoutModel(): model → {Model}", model);
        model.ShippingRequired.Should().Be(await _shoppingCartService.ShoppingCartRequiresShippingAsync(_cart));
        model.DisableBillingAddressCheckoutStep.Should().Be(_orderSettings.DisableBillingAddressCheckoutStep);
        model.BillingAddress.Should().NotBeNull();
		_logger.LogDebug("<<< Exiting CanPrepareOnePageCheckoutModel()");
    }

    [Test]
    public void PrepareOnePageCheckoutModelShouldRaiseExceptionIfCartIsNull()
    {
		_logger.LogDebug(">>> Entering PrepareOnePageCheckoutModelShouldRaiseExceptionIfCartIsNull()");
        Assert.Throws<AggregateException>(() =>
            _checkoutModelFactory.PrepareOnePageCheckoutModelAsync(null).Wait());
			_logger.LogDebug("<<< Exiting PrepareOnePageCheckoutModelShouldRaiseExceptionIfCartIsNull()");
    }

}