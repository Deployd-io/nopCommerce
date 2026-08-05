using FluentAssertions;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Catalog;

[TestFixture]
public class ProductServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    #region Fields

    private IProductService _productService;

    #endregion

    #region SetUp

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _productService = GetService<IProductService>();

		_logger.LogDebug("SetUp(): _productService → {_productService}", _productService);
        var product = await _productService.GetProductByIdAsync(1);
		_logger.LogDebug("SetUp(): product → {Product}", product);
		_logger.LogDebug("SetUp(): product.ManageInventoryMethod → {Arg0}", product.ManageInventoryMethod);
		_logger.LogDebug("SetUp(): product.UseMultipleWarehouses → {Arg0}", product.UseMultipleWarehouses);
        product.ManageInventoryMethod = ManageInventoryMethod.ManageStock;
        product.UseMultipleWarehouses = true;

        await _productService.UpdateProductAsync(product);

        await _productService.InsertProductWarehouseInventoryAsync(new ProductWarehouseInventory
        {
            ProductId = product.Id,
            WarehouseId = 1,
            StockQuantity = 8,
            ReservedQuantity = 5
        });

        await _productService.InsertProductWarehouseInventoryAsync(new ProductWarehouseInventory
        {
            ProductId = product.Id,
            WarehouseId = 2,
            StockQuantity = 5
        });
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        var product = await _productService.GetProductByIdAsync(1);
        foreach (var productWarehouseInventory in await _productService.GetAllProductWarehouseInventoryRecordsAsync(1))
            await _productService.DeleteProductWarehouseInventoryAsync(productWarehouseInventory);
			_logger.LogDebug("TearDown(): product → {Product}", product);

        product.ManageInventoryMethod = ManageInventoryMethod.DontManageStock;
		_logger.LogDebug("TearDown(): product.ManageInventoryMethod → {Arg0}", product.ManageInventoryMethod);
		_logger.LogDebug("TearDown(): product.UseMultipleWarehouses → {Arg0}", product.UseMultipleWarehouses);
        product.UseMultipleWarehouses = false;

        await _productService.UpdateProductAsync(product);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    #endregion

    #region Tests

    [Test]
    public void CanParseRequiredProductIds()
    {
		_logger.LogDebug(">>> Entering CanParseRequiredProductIds()");
        var product = new Product
        {
            RequiredProductIds = "1, 4,7 ,a,"
        };

        var ids = _productService.ParseRequiredProductIds(product);
        ids.Length.Should().Be(3);
		_logger.LogDebug("CanParseRequiredProductIds(): ids → {Ids}", ids);
        ids[0].Should().Be(1);
        ids[1].Should().Be(4);
        ids[2].Should().Be(7);
		_logger.LogDebug("<<< Exiting CanParseRequiredProductIds()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsNotSet()");
        var product = new Product
        {
            AvailableStartDateTimeUtc = null
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenStartDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
        var product = new Product
        {
            AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenStartDateIsLessThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
        var product = new Product
        {
            AvailableStartDateTimeUtc = new DateTime(2010, 01, 02)
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenStartDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsNotSet()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsNotSet()");
        var product = new Product
        {
            AvailableEndDateTimeUtc = null
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsNotSet()");
    }

    [Test]
    public void ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
        var product = new Product
        {
            AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 01)).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenEndDateIsGreaterThanSomeDate()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
        var product = new Product
        {
            AvailableEndDateTimeUtc = new DateTime(2010, 01, 02)
        };

        _productService.ProductIsAvailable(product, new DateTime(2010, 01, 03)).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenEndDateIsLessThanSomeDate()");
    }

    [Test]
    public void ShouldBeAvailableWhenCurrentDateIsInRange()
    {
		_logger.LogDebug(">>> Entering ShouldBeAvailableWhenCurrentDateIsInRange()");
        var product = new Product
        {
            AvailableStartDateTimeUtc = DateTime.UtcNow.AddDays(-1),
            AvailableEndDateTimeUtc = DateTime.UtcNow.AddDays(1)
        };

        _productService.ProductIsAvailable(product).Should().BeTrue();
		_logger.LogDebug("<<< Exiting ShouldBeAvailableWhenCurrentDateIsInRange()");
    }

    [Test]
    public void ShouldNotBeAvailableWhenCurrentDateIsNotInRange()
    {
		_logger.LogDebug(">>> Entering ShouldNotBeAvailableWhenCurrentDateIsNotInRange()");
        var product = new Product
        {
            AvailableStartDateTimeUtc = DateTime.UtcNow.AddDays(-2),
            AvailableEndDateTimeUtc = DateTime.UtcNow.AddDays(-1)
        };

        _productService.ProductIsAvailable(product).Should().BeFalse();
		_logger.LogDebug("<<< Exiting ShouldNotBeAvailableWhenCurrentDateIsNotInRange()");
    }

    [Test]
    public void CanParseAllowedQuantities()
    {
		_logger.LogDebug(">>> Entering CanParseAllowedQuantities()");
        var product = new Product
        {
            AllowedQuantities = "1, 5,4,10,sdf"
        };

        var result = _productService.ParseAllowedQuantities(product);
        result.Length.Should().Be(4);
		_logger.LogDebug("CanParseAllowedQuantities(): result → {Result}", result);
        result[0].Should().Be(1);
        result[1].Should().Be(5);
        result[2].Should().Be(4);
        result[3].Should().Be(10);
		_logger.LogDebug("<<< Exiting CanParseAllowedQuantities()");
    }

    [Test]
    public async Task CanCalculateTotalQuantityWhenWeDoNotUseMultipleWarehouses()
    {
		_logger.LogDebug(">>> Entering CanCalculateTotalQuantityWhenWeDoNotUseMultipleWarehouses()");
        var result = await _productService.GetTotalStockQuantityAsync(new Product { StockQuantity = 6, ManageInventoryMethod = ManageInventoryMethod.ManageStock });
        result.Should().Be(6);
		_logger.LogDebug("<<< Exiting CanCalculateTotalQuantityWhenWeDoNotUseMultipleWarehouses()");
		_logger.LogDebug("CanCalculateTotalQuantityWhenWeDoNotUseMultipleWarehouses(): result → {Result}", result);
    }

    [Test]
    public async Task PublicVoidCanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithReserved()
    {
		_logger.LogDebug(">>> Entering PublicVoidCanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithReserved()");
        var result = await _productService.GetTotalStockQuantityAsync(await _productService.GetProductByIdAsync(1));
        result.Should().Be(8);
		_logger.LogDebug("<<< Exiting PublicVoidCanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithReserved()");
		_logger.LogDebug("PublicVoidCanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithReserved(): result → {Result}", result);
    }

    [Test]
    public async Task CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithoutReserved()
    {
		_logger.LogDebug(">>> Entering CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithoutReserved()");
        var result = await _productService.GetTotalStockQuantityAsync(await _productService.GetProductByIdAsync(1), false);
        result.Should().Be(13);
		_logger.LogDebug("<<< Exiting CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithoutReserved()");
		_logger.LogDebug("CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithoutReserved(): result → {Result}", result);
    }

    [Test]
    public async Task CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithWarehouseSpecified()
    {
		_logger.LogDebug(">>> Entering CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithWarehouseSpecified()");
        var result = await _productService.GetTotalStockQuantityAsync(await _productService.GetProductByIdAsync(1), true, 1);
        result.Should().Be(3);
		_logger.LogDebug("<<< Exiting CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithWarehouseSpecified()");
		_logger.LogDebug("CanCalculateTotalQuantityWhenWeDoUseMultipleWarehousesWithWarehouseSpecified(): result → {Result}", result);
    }

    [Test]
    public void CanCalculateRentalPeriodsForDays()
    {
		_logger.LogDebug(">>> Entering CanCalculateRentalPeriodsForDays()");
        var product = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Days,
            //rental period length = 1 day
            RentalPriceLength = 1
        };

        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //1 day
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
        //2 days
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(2);
        //3 days
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(3);

        //rental period length = 2 days
        product.RentalPriceLength = 2;
		_logger.LogDebug("CanCalculateRentalPeriodsForDays(): product.RentalPriceLength → {Arg0}", product.RentalPriceLength);
        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //1 day
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 6)).Should().Be(1);
        //2 days
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 7)).Should().Be(1);
        //3 days
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 8)).Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateRentalPeriodsForDays()");
    }

    [Test]
    public void CanCalculateRentalPeriodsForWeeks()
    {
		_logger.LogDebug(">>> Entering CanCalculateRentalPeriodsForWeeks()");
        var product = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Weeks,
            //rental period length = 1 week
            RentalPriceLength = 1
        };

        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a week
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
        //1 week
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
        //several days but less than two weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(2);
        //2 weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(2);
        //3 weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(3);

        //rental period length = 2 weeks
        product.RentalPriceLength = 2;
		_logger.LogDebug("CanCalculateRentalPeriodsForWeeks(): product.RentalPriceLength → {Arg0}", product.RentalPriceLength);
        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a week
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 3)).Should().Be(1);
        //1 week
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 12)).Should().Be(1);
        //several days but less than two weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 13)).Should().Be(1);
        //2 weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 19)).Should().Be(1);
        //3 weeks
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 26)).Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateRentalPeriodsForWeeks()");
    }

    [Test]
    public void CanCalculateRentalPeriodsForMonths()
    {
		_logger.LogDebug(">>> Entering CanCalculateRentalPeriodsForMonths()");
        var product = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Months,
            //rental period length = 1 month
            RentalPriceLength = 1
        };

        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a month
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
        //1 month
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
        //1 month and 1 day
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 6)).Should().Be(2);
        //several days but less than two months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(2);
        //2 months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(2);
        //3 months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(3);
        //several more unit tests
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 1, 2)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 2), new DateTime(1900, 1, 1)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1900, 2, 1)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 2, 1), new DateTime(1900, 1, 1)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 31), new DateTime(1900, 2, 1)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 9, 30)).Should().Be(1);
        _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1900, 10, 1)).Should().Be(2);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1901, 1, 1)).Should().Be(12);
        _productService.GetRentalPeriods(product, new DateTime(1900, 1, 1), new DateTime(1911, 1, 1)).Should().Be(132);
        _productService.GetRentalPeriods(product, new DateTime(1900, 8, 31), new DateTime(1901, 8, 30)).Should().Be(12);

        //rental period length = 2 months
        product.RentalPriceLength = 2;
		_logger.LogDebug("CanCalculateRentalPeriodsForMonths(): product.RentalPriceLength → {Arg0}", product.RentalPriceLength);
        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a month
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 4)).Should().Be(1);
        //1 month
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 5)).Should().Be(1);
        //several days but less than two months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 4, 13)).Should().Be(1);
        //2 months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 5)).Should().Be(1);
        //3 months
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 5, 8)).Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateRentalPeriodsForMonths()");
    }

    [Test]
    public void CanCalculateRentalPeriodsForYears()
    {
		_logger.LogDebug(">>> Entering CanCalculateRentalPeriodsForYears()");
        var product = new Product
        {
            IsRental = true,
            RentalPricePeriod = RentalPricePeriod.Years,
            //rental period length = 1 years
            RentalPriceLength = 1
        };

        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a year
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
        //more than one year
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(2);

        //rental period length = 2 years
        product.RentalPriceLength = 2;
		_logger.LogDebug("CanCalculateRentalPeriodsForYears(): product.RentalPriceLength → {Arg0}", product.RentalPriceLength);
        //the same date
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2014, 3, 5)).Should().Be(1);
        //several days but less than a year
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 1, 1)).Should().Be(1);
        //more than one year
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2015, 3, 7)).Should().Be(1);
        //more than two year
        _productService.GetRentalPeriods(product, new DateTime(2014, 3, 5), new DateTime(2016, 3, 7)).Should().Be(2);
		_logger.LogDebug("<<< Exiting CanCalculateRentalPeriodsForYears()");
    }

    #endregion
}