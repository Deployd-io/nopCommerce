using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class ShoppingCartModelFactoryTests : WebTest
{
    private IShoppingCartModelFactory _shoppingCartModelFactory;
    private IShoppingCartService _shoppingCartService;
    private IWorkContext _workContext;
    private IProductService _producService;
    private ILocalizationService _localizationService;
    private ShoppingCartItem _shoppingCartItem;
    private ShoppingCartItem _wishlistItem;
    private ICustomerService _customerService;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _shoppingCartModelFactory = GetService<IShoppingCartModelFactory>();
        _shoppingCartService = GetService<IShoppingCartService>();
		logger.LogDebug("SetUp(): _shoppingCartModelFactory → {_shoppingCartModelFactory}", _shoppingCartModelFactory);
		logger.LogDebug("SetUp(): _shoppingCartService → {_shoppingCartService}", _shoppingCartService);
        _workContext = GetService<IWorkContext>();
        _producService = GetService<IProductService>();
		logger.LogDebug("SetUp(): _workContext → {_workContext}", _workContext);
		logger.LogDebug("SetUp(): _producService → {_producService}", _producService);
        _localizationService = GetService<ILocalizationService>();
        _customerService = GetService<ICustomerService>();
		logger.LogDebug("SetUp(): _localizationService → {_localizationService}", _localizationService);
		logger.LogDebug("SetUp(): _customerService → {_customerService}", _customerService);

        var store = await GetService<IStoreContext>().GetCurrentStoreAsync();

		logger.LogDebug("SetUp(): store → {Store}", store);
        var customer = await _workContext.GetCurrentCustomerAsync();

		logger.LogDebug("SetUp(): customer → {Customer}", customer);
        _shoppingCartItem = new ShoppingCartItem
        {
            ProductId = 1,
            Quantity = 1,
            CustomerId = customer.Id,
            ShoppingCartType = ShoppingCartType.ShoppingCart,
            StoreId = store.Id
			logger.LogDebug("SetUp(): _shoppingCartItem → {_shoppingCartItem}", _shoppingCartItem);
        };

        _wishlistItem = new ShoppingCartItem
		logger.LogDebug("SetUp(): _wishlistItem → {_wishlistItem}", _wishlistItem);
        {
            ProductId = 2,
            Quantity = 1,
            CustomerId = customer.Id,
            ShoppingCartType = ShoppingCartType.Wishlist
        };

        var shoppingCartRepo = GetService<IRepository<ShoppingCartItem>>();

		logger.LogDebug("SetUp(): shoppingCartRepo → {ShoppingCartRepo}", shoppingCartRepo);
        await shoppingCartRepo.InsertAsync(new List<ShoppingCartItem> { _shoppingCartItem, _wishlistItem });

        customer.HasShoppingCartItems = true;
		logger.LogDebug("SetUp(): customer.HasShoppingCartItems → {Arg0}", customer.HasShoppingCartItems);
        await _customerService.UpdateCustomerAsync(customer);
		logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        await _shoppingCartService.DeleteShoppingCartItemAsync(_shoppingCartItem);
        await _shoppingCartService.DeleteShoppingCartItemAsync(_wishlistItem);

        var customer = await _workContext.GetCurrentCustomerAsync();
        customer.HasShoppingCartItems = false;
		logger.LogDebug("TearDown(): customer → {Customer}", customer);
		logger.LogDebug("TearDown(): customer.HasShoppingCartItems → {Arg0}", customer.HasShoppingCartItems);
        await _customerService.UpdateCustomerAsync(customer);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanPrepareEstimateShippingModel()
    {
		logger.LogDebug(">>> Entering CanPrepareEstimateShippingModel()");
        var model = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync()));

		logger.LogDebug("CanPrepareEstimateShippingModel(): model → {Model}", model);
        model.AvailableCountries.Any().Should().BeTrue();
        model.AvailableStates.Any().Should().BeTrue();
        model.Enabled.Should().BeTrue();
        model.ZipPostalCode.Should().Be("10021");
        model.CountryId.Should().BeNull();
        model.StateProvinceId.Should().BeNull();
		logger.LogDebug("<<< Exiting CanPrepareEstimateShippingModel()");
    }

    [Test]
    public async Task CanPrepareShoppingCartModel()
    {
		logger.LogDebug(">>> Entering CanPrepareShoppingCartModel()");
        var model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(new ShoppingCartModel(),
            new List<ShoppingCartItem> { _shoppingCartItem });

        model.IsEditable.Should().BeTrue();
        model.Items.Any().Should().BeTrue();
        model.Items.Count.Should().Be(1);
        model.Warnings.Count.Should().Be(0);

        model.OrderReviewData.Should().NotBeNull();
        model.OrderReviewData.Display.Should().BeFalse();
        model = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(new ShoppingCartModel(),
            new List<ShoppingCartItem> { _shoppingCartItem }, true, true, true);
        model.OrderReviewData.Should().NotBeNull();
        model.OrderReviewData.Display.Should().BeTrue();
		logger.LogDebug("<<< Exiting CanPrepareShoppingCartModel()");
    }

    [Test]
    public async Task CanPrepareWishlistModel()
    {
		logger.LogDebug(">>> Entering CanPrepareWishlistModel()");
        var model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(new WishlistModel(),
            new List<ShoppingCartItem> { _wishlistItem });

        var customer = await _workContext.GetCurrentCustomerAsync();

		logger.LogDebug("CanPrepareWishlistModel(): customer → {Customer}", customer);
        model.CustomerFullname.Should().Be("John Smith");
        model.CustomerGuid.Should().Be(customer.CustomerGuid);
        model.EmailWishlistEnabled.Should().BeTrue();
        model.IsEditable.Should().BeTrue();
        model.Items.Any().Should().BeTrue();
        model.Items.Count.Should().Be(1);
        model.Warnings.Count.Should().Be(0);
		logger.LogDebug("<<< Exiting CanPrepareWishlistModel()");
    }

    [Test]
    public async Task CanPrepareMiniShoppingCartModel()
    {
		logger.LogDebug(">>> Entering CanPrepareMiniShoppingCartModel()");
        var model = await _shoppingCartModelFactory.PrepareMiniShoppingCartModelAsync();

		logger.LogDebug("CanPrepareMiniShoppingCartModel(): model → {Model}", model);
        model.CurrentCustomerIsGuest.Should().BeFalse();
        model.Items.Any().Should().BeTrue();
        model.Items.Count.Should().Be(1);
        model.TotalProducts.Should().Be(1);
        model.SubTotal.Should().Be("$1,200.00");
		logger.LogDebug("<<< Exiting CanPrepareMiniShoppingCartModel()");
    }

    [Test]
    public async Task CanPrepareOrderTotalsModel()
    {
		logger.LogDebug(">>> Entering CanPrepareOrderTotalsModel()");
        var model = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(new List<ShoppingCartItem> { _shoppingCartItem }, true);

		logger.LogDebug("CanPrepareOrderTotalsModel(): model → {Model}", model);
        model.SubTotal.Should().Be("$1,200.00");
        model.OrderTotal.Should().Be("$1,200.00");

        model.GiftCards.Any().Should().BeFalse();
        model.Shipping.Should().Be("$0.00");
        model.Tax.Should().Be("$0.00");
        model.WillEarnRewardPoints.Should().Be(120);
		logger.LogDebug("<<< Exiting CanPrepareOrderTotalsModel()");
    }

    [Test]
    public async Task CanPrepareEstimateShippingResultModel()
    {
		logger.LogDebug(">>> Entering CanPrepareEstimateShippingResultModel()");
        var model = await _shoppingCartModelFactory.PrepareEstimateShippingResultModelAsync(new List<ShoppingCartItem> { _shoppingCartItem }, new EstimateShippingModel(), true);
        model.Errors.Any().Should().BeFalse();
		logger.LogDebug("<<< Exiting CanPrepareEstimateShippingResultModel()");
		logger.LogDebug("CanPrepareEstimateShippingResultModel(): model → {Model}", model);
    }

    [Test]
    public async Task CanPrepareWishlistEmailAFriendModel()
    {
		logger.LogDebug(">>> Entering CanPrepareWishlistEmailAFriendModel()");
        var model = await _shoppingCartModelFactory.PrepareWishlistEmailAFriendModelAsync(new WishlistEmailAFriendModel(),
            false);

        model.YourEmailAddress.Should().Be(NopTestsDefaults.AdminEmail);
		logger.LogDebug("<<< Exiting CanPrepareWishlistEmailAFriendModel()");
    }

    [Test]
    public async Task CanPrepareCartItemPictureModel()
    {
		logger.LogDebug(">>> Entering CanPrepareCartItemPictureModel()");
        var product = await _producService.GetProductByIdAsync(_shoppingCartItem.ProductId);

		logger.LogDebug("CanPrepareCartItemPictureModel(): product → {Product}", product);
        var model = await _shoppingCartModelFactory.PrepareCartItemPictureModelAsync(_shoppingCartItem, 100, true, await _localizationService.GetLocalizedAsync(product, x => x.Name));

		logger.LogDebug("CanPrepareCartItemPictureModel(): model → {Model}", model);
        model.AlternateText.Should().Be("Picture of Build your own computer");
        model.ImageUrl.Should()
            .Be($"http://{NopTestsDefaults.HostIpAddress}/images/thumbs/0000020_build-your-own-computer_100.jpeg");
        model.Title.Should().Be("Show details for Build your own computer");

        model.FullSizeImageUrl.Should().Be($"http://{NopTestsDefaults.HostIpAddress}/images/thumbs/0000020_build-your-own-computer.jpeg");
        model.ThumbImageUrl.Should().BeNull();
		logger.LogDebug("<<< Exiting CanPrepareCartItemPictureModel()");
    }
}
