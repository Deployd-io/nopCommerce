using FluentAssertions;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Services.Discounts;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Discounts;

[TestFixture]
public class DiscountServiceTests : ServiceTest
{
    private IDiscountPluginManager _discountPluginManager;
    private IDiscountService _discountService;

    [OneTimeSetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _discountPluginManager = GetService<IDiscountPluginManager>();
        _discountService = GetService<IDiscountService>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _discountPluginManager → {_discountPluginManager}", _discountPluginManager);
		logger.LogDebug("SetUp(): _discountService → {_discountService}", _discountService);
    }

    [Test]
    public async Task CanGetAllDiscount()
    {
		logger.LogDebug(">>> Entering CanGetAllDiscount()");
        var discounts = await _discountService.GetAllDiscountsAsync(isActive: null);
        discounts.Should().NotBeNull();
		logger.LogDebug("CanGetAllDiscount(): discounts → {Discounts}", discounts);
        discounts.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanGetAllDiscount()");
    }

    [Test]
    public async Task CanLoadDiscountRequirementRules()
    {
		logger.LogDebug(">>> Entering CanLoadDiscountRequirementRules()");
        var rules = await _discountPluginManager.LoadAllPluginsAsync();
        rules.Should().NotBeNull();
		logger.LogDebug("CanLoadDiscountRequirementRules(): rules → {Rules}", rules);
        rules.Any().Should().BeTrue();
		logger.LogDebug("<<< Exiting CanLoadDiscountRequirementRules()");
    }

    [Test]
    public async Task CanLoadDiscountRequirementRuleBySystemKeyword()
    {
		logger.LogDebug(">>> Entering CanLoadDiscountRequirementRuleBySystemKeyword()");
        var rule = await _discountPluginManager.LoadPluginBySystemNameAsync("TestDiscountRequirementRule");
        rule.Should().NotBeNull();
		logger.LogDebug("<<< Exiting CanLoadDiscountRequirementRuleBySystemKeyword()");
		logger.LogDebug("CanLoadDiscountRequirementRuleBySystemKeyword(): rule → {Rule}", rule);
    }

    [Test]
    public async Task ShouldAcceptValidDiscountCode()
    {
		logger.LogDebug(">>> Entering ShouldAcceptValidDiscountCode()");
        var discount = CreateDiscount();
        var customer = CreateCustomer();
		logger.LogDebug("ShouldAcceptValidDiscountCode(): discount → {Discount}", discount);
		logger.LogDebug("ShouldAcceptValidDiscountCode(): customer → {Customer}", customer);

        var result = await _discountService.ValidateDiscountAsync(discount, customer, ["CouponCode 1"]);
        result.IsValid.Should().BeTrue();
		logger.LogDebug("<<< Exiting ShouldAcceptValidDiscountCode()");
		logger.LogDebug("ShouldAcceptValidDiscountCode(): result → {Result}", result);
    }

    private static Customer CreateCustomer()
    {
		logger.LogDebug(">>> Entering CreateCustomer()");
		logger.LogDebug("<<< Exiting CreateCustomer()");
        return new Customer
        {
            CustomerGuid = Guid.NewGuid(),
            AdminComment = string.Empty,
            Active = true,
            Deleted = false,
            CreatedOnUtc = new DateTime(2010, 01, 01),
            LastActivityDateUtc = new DateTime(2010, 01, 02)
        };
    }

    private static Discount CreateDiscount()
    {
		logger.LogDebug(">>> Entering CreateDiscount()");
		logger.LogDebug("<<< Exiting CreateDiscount()");
        return new Discount
        {
            IsActive = true,
            DiscountType = DiscountType.AssignedToSkus,
            Name = "Discount 2",
            UsePercentage = false,
            DiscountPercentage = 0,
            DiscountAmount = 5,
            RequiresCouponCode = true,
            CouponCode = "CouponCode 1",
            DiscountLimitation = DiscountLimitationType.Unlimited
        };
    }

    [Test]
    public async Task ShouldNotAcceptWrongDiscountCode()
    {
		logger.LogDebug(">>> Entering ShouldNotAcceptWrongDiscountCode()");
        var discount = CreateDiscount();
        var customer = CreateCustomer();
		logger.LogDebug("ShouldNotAcceptWrongDiscountCode(): discount → {Discount}", discount);
		logger.LogDebug("ShouldNotAcceptWrongDiscountCode(): customer → {Customer}", customer);

        var result = await _discountService.ValidateDiscountAsync(discount, customer, ["CouponCode 2"]);
        result.IsValid.Should().BeFalse();
		logger.LogDebug("<<< Exiting ShouldNotAcceptWrongDiscountCode()");
		logger.LogDebug("ShouldNotAcceptWrongDiscountCode(): result → {Result}", result);
    }

    [Test]
    public async Task CanValidateDiscountDateRange()
    {
		logger.LogDebug(">>> Entering CanValidateDiscountDateRange()");
        var discount = CreateDiscount();
        discount.RequiresCouponCode = false;
		logger.LogDebug("CanValidateDiscountDateRange(): discount → {Discount}", discount);
		logger.LogDebug("CanValidateDiscountDateRange(): discount.RequiresCouponCode → {Arg0}", discount.RequiresCouponCode);

        var customer = CreateCustomer();

		logger.LogDebug("CanValidateDiscountDateRange(): customer → {Customer}", customer);
        var result = await _discountService.ValidateDiscountAsync(discount, customer, null);
        result.IsValid.Should().BeTrue();
		logger.LogDebug("CanValidateDiscountDateRange(): result → {Result}", result);

        discount.StartDateUtc = DateTime.UtcNow.AddDays(1);
        result = await _discountService.ValidateDiscountAsync(discount, customer, null);
        result.IsValid.Should().BeFalse();
		logger.LogDebug("<<< Exiting CanValidateDiscountDateRange()");
    }

    [Test]
    public void CanCalculateDiscountAmountPercentage()
    {
		logger.LogDebug(">>> Entering CanCalculateDiscountAmountPercentage()");
        var discount = new Discount
        {
            UsePercentage = true,
            DiscountPercentage = 30
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(30);

        discount.DiscountPercentage = 60;
		logger.LogDebug("CanCalculateDiscountAmountPercentage(): discount.DiscountPercentage → {Arg0}", discount.DiscountPercentage);
        _discountService.GetDiscountAmount(discount, 200).Should().Be(120);
		logger.LogDebug("<<< Exiting CanCalculateDiscountAmountPercentage()");
    }

    [Test]
    public void CanCalculateDiscountAmountFixed()
    {
		logger.LogDebug(">>> Entering CanCalculateDiscountAmountFixed()");
        var discount = new Discount
        {
            UsePercentage = false,
            DiscountAmount = 10
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(10);

        discount.DiscountAmount = 20;
		logger.LogDebug("CanCalculateDiscountAmountFixed(): discount.DiscountAmount → {Arg0}", discount.DiscountAmount);
        _discountService.GetDiscountAmount(discount, 200).Should().Be(20);
		logger.LogDebug("<<< Exiting CanCalculateDiscountAmountFixed()");
    }

    [Test]
    public void MaximumDiscountAmountIsUsed()
    {
		logger.LogDebug(">>> Entering MaximumDiscountAmountIsUsed()");
        var discount = new Discount
        {
            UsePercentage = true,
            DiscountPercentage = 30,
            MaximumDiscountAmount = 3.4M
        };

        _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

        discount.DiscountPercentage = 60;
		logger.LogDebug("MaximumDiscountAmountIsUsed(): discount.DiscountPercentage → {Arg0}", discount.DiscountPercentage);
        _discountService.GetDiscountAmount(discount, 200).Should().Be(3.4M);
        _discountService.GetDiscountAmount(discount, 100).Should().Be(3.4M);

        discount.DiscountPercentage = 1;
        discount.GetDiscountAmount(200).Should().Be(2);
		logger.LogDebug("<<< Exiting MaximumDiscountAmountIsUsed()");
    }

    [TestCase(RequirementGroupInteractionType.And)]
    [TestCase(RequirementGroupInteractionType.Or)]
    [TestCase(RequirementGroupInteractionType.And, true)]
    [TestCase(RequirementGroupInteractionType.Or, true)]
    [Test]
    public async Task CanCheckRequirements(RequirementGroupInteractionType interactionType, bool checkFalse = false)
    {
		logger.LogDebug(">>> Entering CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse})", interactionType, checkFalse);
        var discount = CreateDiscount();
        discount.RequiresCouponCode = false;
		logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): discount → {Discount}", interactionType, checkFalse, discount);
		logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): discount.RequiresCouponCode → {Arg2}", interactionType, checkFalse, discount.RequiresCouponCode);
        var customer = CreateCustomer();

		logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): customer → {Customer}", interactionType, checkFalse, customer);
        await _discountService.InsertDiscountAsync(discount);

        var groupRequirement = new DiscountRequirement
        {
            DiscountId = discount.Id,
            IsGroup = true,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            InteractionType = interactionType
        };

        await _discountService.InsertDiscountRequirementAsync(groupRequirement);

        var requirement1 = new DiscountRequirement
        {
            DiscountId = discount.Id,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            ParentId = groupRequirement.Id
        };

        var requirement2 = new DiscountRequirement
        {
            DiscountId = discount.Id,
            DiscountRequirementRuleSystemName = "TestDiscountRequirementRule",
            ParentId = groupRequirement.Id
        };

        if (interactionType == RequirementGroupInteractionType.Or || checkFalse)
        {
            requirement1.InteractionType = RequirementGroupInteractionType.And;
			logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): requirement1.InteractionType → {Arg2}", interactionType, checkFalse, requirement1.InteractionType);

            if (interactionType == RequirementGroupInteractionType.Or && checkFalse)
                requirement2.InteractionType = RequirementGroupInteractionType.Or;
				logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): requirement2.InteractionType → {Arg2}", interactionType, checkFalse, requirement2.InteractionType);
        }

        await _discountService.InsertDiscountRequirementAsync(requirement1);
        await _discountService.InsertDiscountRequirementAsync(requirement2);

        try
        {
            var result = await _discountService.ValidateDiscountAsync(discount, customer, null);
            result.IsValid.Should().Be(!checkFalse);
			logger.LogDebug("CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse}): result → {Result}", interactionType, checkFalse, result);

        }
        finally
        {
            await _discountService.DeleteDiscountRequirementAsync(groupRequirement, true);
            await _discountService.DeleteDiscountAsync(discount);
        }
		logger.LogDebug("<<< Exiting CanCheckRequirements(interactionType={InteractionType},checkFalse={CheckFalse})", interactionType, checkFalse);
    }
}

public static class DiscountExtensions
{
    private static readonly DiscountService _discountService;

    static DiscountExtensions()
    {
		logger.LogDebug(">>> Entering DiscountExtensions()");
        _discountService = new DiscountService(null, null,
            null, null, null, null, null, null, null, null, null);
			logger.LogDebug("<<< Exiting DiscountExtensions()");
			logger.LogDebug("DiscountExtensions(): _discountService → {_discountService}", _discountService);
    }

    public static decimal GetDiscountAmount(this Discount discount, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(discount);

        return _discountService.GetDiscountAmount(discount, amount);

    }
}
