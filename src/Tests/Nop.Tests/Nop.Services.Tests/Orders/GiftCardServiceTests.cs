using FluentAssertions;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Orders;

[TestFixture]
public class GiftCardServiceTests : ServiceTest
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private IGiftCardService _giftCardService;
    private GiftCard _giftCard1;
    private GiftCard _giftCard2;

    [OneTimeSetUp]
    public async Task SetUp()
    {
		_logger.LogDebug(">>> Entering SetUp()");
        _giftCardService = GetService<IGiftCardService>();

		_logger.LogDebug("SetUp(): _giftCardService → {_giftCardService}", _giftCardService);
        _giftCard1 = new GiftCard { Amount = 100, IsGiftCardActivated = true };
		_logger.LogDebug("SetUp(): _giftCard1 → {_giftCard1}", _giftCard1);
        _giftCard2 = new GiftCard { Amount = 100 };
		_logger.LogDebug("SetUp(): _giftCard2 → {_giftCard2}", _giftCard2);

        await _giftCardService.InsertGiftCardAsync(_giftCard1);
        await _giftCardService.InsertGiftCardAsync(_giftCard2);
		_logger.LogDebug("<<< Exiting SetUp()");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
		_logger.LogDebug(">>> Entering TearDown()");
        await _giftCardService.DeleteGiftCardAsync(_giftCard1);
        await _giftCardService.DeleteGiftCardAsync(_giftCard2);
		_logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    public async Task CanValidateGiftCard()
    {
		_logger.LogDebug(">>> Entering CanValidateGiftCard()");
        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 30 });

        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 20 });

        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 5 });

        //valid
        var isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
		_logger.LogDebug("CanValidateGiftCard(): isValid → {IsValid}", isValid);
        isValid.Should().BeTrue();

        //mark as not active
        _giftCard1.IsGiftCardActivated = false;
		_logger.LogDebug("CanValidateGiftCard(): _giftCard1.IsGiftCardActivated → {Arg0}", _giftCard1.IsGiftCardActivated);
        isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
        isValid.Should().BeFalse();

        //again active
        _giftCard1.IsGiftCardActivated = true;
        isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
        isValid.Should().BeTrue();

        //add usage history record
        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard1.Id, UsedWithOrderId = 1, UsedValue = 1000 });

        isValid = await _giftCardService.IsGiftCardValidAsync(_giftCard1);
        isValid.Should().BeFalse();
		_logger.LogDebug("<<< Exiting CanValidateGiftCard()");
    }

    [Test]
    public async Task CanCalculateGiftCardRemainingAmount()
    {
		_logger.LogDebug(">>> Entering CanCalculateGiftCardRemainingAmount()");
        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 30 });

        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 20 });

        await _giftCardService.InsertGiftCardUsageHistoryAsync(
            new GiftCardUsageHistory { GiftCardId = _giftCard2.Id, UsedWithOrderId = 1, UsedValue = 5 });

        var remainingAmount = await _giftCardService.GetGiftCardRemainingAmountAsync(_giftCard2);
		_logger.LogDebug("CanCalculateGiftCardRemainingAmount(): remainingAmount → {RemainingAmount}", remainingAmount);
        remainingAmount.Should().Be(45);
		_logger.LogDebug("<<< Exiting CanCalculateGiftCardRemainingAmount()");
    }
}